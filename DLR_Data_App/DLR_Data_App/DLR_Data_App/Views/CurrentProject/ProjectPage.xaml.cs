using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Models.ProjectForms;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using DLR_Data_App.Services.Sensors;
using DLR_Data_App.ViewModels.CurrentProject;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.CurrentProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectPage
    {
        const int MaxDaysSinceLastProfilingCompletion = 45;
        const int MaxProjectsFilledPerProfiling = 10;

        private readonly ProjectViewModel _viewModel = new ProjectViewModel();
        private Project _workingProject;
        private List<ContentPage> _pages;
        private Project _projectLastCheck;
        private IReadOnlyList<FormElement> _formElements;

        private readonly Sensor _sensor = Sensor.Instance;

        public ProjectPage()
        {
            InitializeComponent();

            BindingContext = _viewModel;

            Appearing += ProjectPage_Appearing;
        }

        private async void ProjectPage_Appearing(object sender, EventArgs e)
        {
            var newProject = Database.GetCurrentProject();

            if (newProject == null)
            {
                DependencyService.Get<IToast>().LongAlert(AppResources.noactiveproject);

                await App.CurrentMainPage.NavigateFromMenu(MenuItemType.Projects);
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

            if (!ProfilingStorageManager.IsProfilingModuleLoaded(project.ProfilingId))
            {
                var profilingList = await App.CurrentMainPage.NavigateFromMenu(MenuItemType.ProfilingList);

                await profilingList.DisplayAlert(AppResources.error,
                    string.Format(AppResources.profilingModuleMissing, project.ProfilingId),
                    AppResources.ok);
                return;
            }

            var lastAnsweredProfilingDate = ProfilingStorageManager.GetLastCompletedProfilingDate(project.ProfilingId);
            if ((DateTime.UtcNow - lastAnsweredProfilingDate).TotalDays > MaxDaysSinceLastProfilingCompletion
                || ProfilingStorageManager.ProjectsFilledSinceLastProfilingCompletion > MaxProjectsFilledPerProfiling)
            {
                DependencyService.Get<IToast>().LongAlert(AppResources.pleaseCompleteProfiling);

                await App.CurrentMainPage.NavigateFromMenu(MenuItemType.ProfilingList);
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
                Title = AppResources.currentproject;
            }
            else
            {
                var translatedProject = Helpers.TranslateProjectDetails(_workingProject);
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
            DependencyService.Get<IToast>().ShortAlert(AppResources.pleaseSaveBeforeQuiting);
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
            //var changedElement = (FormElement)sender;
            //
            //foreach (var element in _formElements.SkipWhileIncluding(e => e != changedElement))
            //{
            //    LockElement(element);
            //}
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

            var relevantElements = SelectRelevantElements(_formElements).ToList();
            var validRelevantElements = relevantElements.Where(e => e.IsValid).ToList();
            var lastValidRelevantElement = validRelevantElements.LastOrDefault();

            var wantedUnlockedElements = validRelevantElements;
            if (lastValidRelevantElement != null)
            {
                var nextRelevantInvalidItem = relevantElements.SkipWhileIncluding(e => e != lastValidRelevantElement).FirstOrDefault();
                if (nextRelevantInvalidItem != null)
                    wantedUnlockedElements.Add(nextRelevantInvalidItem);
            }

            foreach (var element in UnlockedElements.ToList().Except(wantedUnlockedElements))
            {
                LockElement(element);
            }
            foreach (var element in wantedUnlockedElements.ToList().Except(UnlockedElements))
            {
                UnlockElement(element);
            }
            
            //{
            //    if (variables == null)
            //        variables = GeatherVariables();
            //    if (element.ShouldBeShownExpression.Evaluate(variables))
            //    {
            //        element.Frame.IsVisible = true;
            //    }
            //    else
            //    {
            //        var wasVisible = element.Frame.IsVisible;
            //        if (wasVisible)
            //        {
            //            element.Frame.IsVisible = false;
            //            element.Reset();
            //        }
            //    }
            //}
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

            //var changedElement = (FormElement)sender;
            //
            //var currentlyRequiredElement = _formElements.LastOrDefault(e => e.Frame.IsVisible);
            //if (currentlyRequiredElement == changedElement)
            //{
            //    var nextUnlockedElement = SelectRelevantElements(_formElements).SkipWhileIncluding(e => e != changedElement).FirstOrDefault();
            //    UnlockElement(nextUnlockedElement);
            //}
        }

        /// <summary>
        /// Checks if active project exists.
        /// </summary>
        private async Task<bool> CheckActiveProject()
        {
            if (_pages == null || _pages.Count == 0)
            {
                await DisplayAlert(AppResources.warning, AppResources.noactiveproject, AppResources.okay);
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
                await this.PushPage(new EditDataPage());
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
                    ProfilingStorageManager.ProjectsFilledSinceLastProfilingCompletion++;
                    // TODO: next line currently crashes the app. disabling it will disable saving the ProjectsFilledSinceLastProfilingCompletion though
                    // ProfilingStorageManager.SaveCurrentAnswer();
                }

                string message;
                if (status)
                {
                    message = AppResources.successful;
                    foreach (var element in _formElements)
                    {
                        element.Reset();
                        LockElement(element);
                    }
                    UnlockInitialVisibleElements(_formElements);
                }
                else
                {
                    message = AppResources.failed;
                }

                await DisplayAlert(AppResources.save, message, AppResources.okay);
                await CheckForProfilingCompletionNeeded(_workingProject);
            }
        }
    }
}