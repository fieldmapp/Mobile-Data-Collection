using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DLR_Data_App.Services;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Localization;
using DlrDataApp.Modules.Base.Shared.Services;
using DlrDataApp.Modules.OdkProjects.Shared.Localization;
using DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectForms;
using DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjects.Shared.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using static DlrDataApp.Modules.OdkProjects.Shared.Services.Helpers;

namespace DlrDataApp.Modules.OdkProjects.Shared.Views.CurrentProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectPage
    {
        const int MaxDaysSinceLastProfilingCompletion = 45;
        const int MaxProjectsFilledPerProfiling = 10;

        private Project _workingProject;
        private List<ContentPage> _pages;
        private Project _projectLastCheck;
        private IReadOnlyList<FormElement> _formElements;

        private readonly Sensor _sensor = OdkProjectsModule.Instance.Sensor;

        public ProjectPage()
        {
            InitializeComponent();

            Appearing += ProjectPage_Appearing;
        }

        private async void ProjectPage_Appearing(object sender, EventArgs e)
        {
            var newProject = OdkProjectsModule.Instance.Database.GetActiveElement<Project, ActiveProjectInfo>();

            if (newProject == null)
            {
                DependencyService.Get<IToast>().LongAlert(OdkProjectsResources.noactiveproject);

                _ = Shell.Current.GoToAsync("//projects");
                return;
            }


            await CheckForProfilingCompletionNeeded(newProject);

            if (_projectLastCheck?.Id == newProject.Id)
                return;
            _projectLastCheck = newProject;
            
            UnlockedElements = new HashSet<FormElement>();
            Children.Clear();
            LoadPagesAndElements(newProject);
            foreach (var page in _pages)
            {
                Children.Add(page);
            }
        }

        private async Task CheckForProfilingCompletionNeeded(Project project)
        {
            if (string.IsNullOrWhiteSpace(project.ProfilingId))
                return;

            bool isModuleLoaded = OdkProjectsModule.Instance.CallSharedMethod<string, bool>("Profiling", 
                "IsProfilingLoaded", project.ProfilingId);
            if (!isModuleLoaded)
            {
                await Shell.Current.GoToAsync("//profilings");

                _ = Shell.Current.DisplayAlert(SharedResources.error,
                    string.Format(OdkProjectsResources.profilingModuleMissing, project.ProfilingId),
                    SharedResources.ok);
                return;
            }

            bool profilingRecentlyFinished = OdkProjectsModule.Instance.CallSharedMethod<string, bool>("Profiling",
                "RecentProfilingFinished", project.ProfilingId);

            if (!profilingRecentlyFinished)
            {
                DependencyService.Get<IToast>().LongAlert(OdkProjectsResources.pleaseCompleteProfiling);

                await Shell.Current.GoToAsync("//profilings");
            }
        }

        /// <summary>
        /// Updates view. Sets _pages and _formElements.
        /// </summary>
        public void LoadPagesAndElements(Project newProject)
        {
            // Get current project
            _workingProject = newProject;
            var pages = new List<ContentPage>();
            var formElements = new List<FormElement>();

            // Check if current project is set
            if (_workingProject == null)
            {
                Title = OdkProjectsResources.currentproject;
            }
            else
            {
                var translatedProject = TranslateProjectDetails(_workingProject);
                Title = translatedProject.Title;

                foreach (var projectForm in _workingProject.FormList)
                {
                    var content = FormCreator.GenerateForm(projectForm, _workingProject, DisplayAlert, true);
                    formElements.AddRange(content.Elements);
                    foreach (var formElement in content.Elements)
                    {
                        formElement.ValidContentChange += FormElement_ValidContentChange;
                        formElement.InvalidContentChange += FormElement_InvalidContentChange;
                    }
                    pages.Add(content.Form);
                }
            }
            UnlockInitialVisibleElements(formElements);
            
            var lastRequiredElement = formElements.LastOrDefault();
            if (lastRequiredElement != null)
                lastRequiredElement.ValidContentChange += LastElement_ValidContentChange;

            _pages = pages;
            _formElements = formElements.AsReadOnly();
        }

        private void UnlockInitialVisibleElements(IEnumerable<FormElement> formElements)
        {
            var firstElement = formElements.FirstOrDefault();
            if (firstElement != null)
                UnlockElement(firstElement);
        }

        private void LastElement_ValidContentChange(object sender, EventArgs _)
        {
            var lastRequiredElement = (FormElement)sender;
            lastRequiredElement.ValidContentChange -= LastElement_ValidContentChange;
            DependencyService.Get<IToast>().ShortAlert(SharedResources.pleaseSaveBeforeQuiting);
        }

        private void UnlockElement(FormElement element)
        {
            UnlockedElements.Add(element);
            element.IsVisible = true;
        }

        private void LockElement(FormElement element)
        {
            UnlockedElements.Remove(element);
            element.IsVisible = false;
            element.Reset();
        }

        private void FormElement_InvalidContentChange(object sender, EventArgs _)
        {
            RefreshVisibilityOnUnlockedElements();
        }

        private IEnumerable<FormElement> SelectRelevantElements(IEnumerable<FormElement> allElements)
        {
            Dictionary<string, string> variables = null;
            foreach (var element in allElements)
            {
                if (element.ShouldBeShownExpression == null)
                {
                    yield return element;
                }
                else
                {
                    if (variables == null)
                        variables = GeatherVariables();

                    if (element.ShouldBeShownExpression.Evaluate(variables))
                        yield return element;
                }
            }
        }

        private void RefreshVisibilityOnUnlockedElements()
        {
            // after each step all we want to see are all elements which are both valid and relevant + the next relevant (but invalid) item

            List<FormElement> prevRelevantElements;
            List<FormElement> currRelevantElements = SelectRelevantElements(_formElements).ToList();
            List<FormElement> initialRelevantValidElements = currRelevantElements.Where(e => e.IsValid).ToList();
            
            // we need to loop the locking of newly irrelevant elements because a chain like this may happen (a,b,c are elements):
            // a is invalidated -> locking a -> resetting a -> next iteration -> invalidating b -> locking b -> reseting b -> next iteration -> invalidating c -> .. 
            // and just looping until nothing changes is the lazy but always correct solution
            do
            {
                prevRelevantElements = currRelevantElements;
                var validRelevantElements = prevRelevantElements.Where(e => e.IsValid).ToList();
                foreach (var element in UnlockedElements.ToList().Except(validRelevantElements))
                {
                    LockElement(element);
                }
                currRelevantElements = SelectRelevantElements(_formElements).ToList();
            }
            while (!currRelevantElements.SequenceEqual(prevRelevantElements));

            var nextRelevantInvalidElement = currRelevantElements.Except(initialRelevantValidElements).FirstOrDefault();
            if (nextRelevantInvalidElement != null)
            {
                UnlockElement(nextRelevantInvalidElement);
            }
        }

        Dictionary<string,string> GeatherVariables()
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            foreach (var representation in _formElements.Select(e => e.GetRepresentation()))
            {
                variables.Add(representation.Key, representation.Value);
            }
            return variables;
        }

        HashSet<FormElement> UnlockedElements;

        private void FormElement_ValidContentChange(object sender, EventArgs _)
        {
            RefreshVisibilityOnUnlockedElements();
        }

        /// <summary>
        /// Checks if active project exists.
        /// </summary>
        private async Task<bool> CheckActiveProject()
        {
            if (_pages == null || _pages.Count == 0)
            {
                await DisplayAlert(SharedResources.warning, OdkProjectsResources.noactiveproject, SharedResources.okay);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Navigates to <see cref="EditDataPage"/>.
        /// </summary>
        private async void EditClicked(object sender, EventArgs e)
        {
            if (await CheckActiveProject())
            {
                _ = Shell.Current.Navigation.PushPage(new EditDataPage());
            }
        }

        /// <summary>
        /// Saves the user-supplied data.
        /// </summary>
        private async void SaveClicked(object sender, EventArgs _)
        {
            if (await CheckActiveProject())
            {
                var elementNameList = new List<string>();
                var elementValueList = new List<string>();

                foreach (var representation in _formElements.Select(e => e.GetRepresentation()))
                {
                    elementNameList.Add(representation.Key);
                    elementValueList.Add(representation.Value);
                }

                var tableName = _workingProject.GetTableName();
                var status = Database.InsertCustomValues(tableName, elementNameList, elementValueList);

                if (!string.IsNullOrWhiteSpace(_workingProject.ProfilingId))
                {
                    // ProfilingStorageManager.ProjectsFilledSinceLastProfilingCompletion++;
                    // TODO: next line currently crashes the app. disabling it will disable saving the ProjectsFilledSinceLastProfilingCompletion though
                    // ProfilingStorageManager.SaveCurrentAnswer();
                }

                string message;
                if (status)
                {
                    message = SharedResources.successful;
                    foreach (var element in _formElements)
                    {
                        element.Reset();
                        LockElement(element);
                    }
                    UnlockInitialVisibleElements(_formElements);
                }
                else
                {
                    message = SharedResources.failed;
                }

                await DisplayAlert(SharedResources.save, message, SharedResources.okay);
                await CheckForProfilingCompletionNeeded(_workingProject);
            }
        }
    }
}