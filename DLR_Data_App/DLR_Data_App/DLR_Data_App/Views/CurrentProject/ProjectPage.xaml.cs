using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
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
        const int MaxDaysSinceLastSurveyCompletion = 45;
        const int MaxProjectsFilledPerSurvey = 10;

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

        private void ProjectPage_Appearing(object sender, EventArgs e)
        {
            var newProject = Database.GetCurrentProject();

            if (newProject != null)
            {
                CheckForSurveyCompletionNeeded();
            }

            if (_projectLastCheck?.Id == newProject?.Id)
                return;
            _projectLastCheck = newProject;
            
            UnlockedElements = new HashSet<FormElement>();
            Children.Clear();
            LoadPagesAndElements(newProject);
            foreach (var page in _pages)
            {
                Children.Add(page);
            }

            if (_pages == null || _pages.Count == 0)
            {
                (Application.Current as App).CurrentPage.DisplayAlert(AppResources.warning, AppResources.noactiveproject, AppResources.okay);
            }            
        }

        private static void CheckForSurveyCompletionNeeded()
        {
            var lastAnsweredSurveyDate = SurveyStorageManager.GetLastCompletedSurveyDate();
            if ((DateTime.UtcNow - lastAnsweredSurveyDate).TotalDays > MaxDaysSinceLastSurveyCompletion
                || SurveyStorageManager.ProjectsFilledSinceLastSurveyCompletion > MaxProjectsFilledPerSurvey)
            {
                DependencyService.Get<IToast>().LongAlert(AppResources.pleaseCompleteProfiling);

                if (Application.Current.MainPage is MainPage mainPage)
                    _ = mainPage.NavigateFromMenu(MenuItemType.Profiling);
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
                    var content = FormCreator.GenerateForm(projectForm, _workingProject, DisplayAlert);
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
            var lastRequiredElement = formElements.LastOrDefault(e => e.Data.Required);
            if (lastRequiredElement != null)
            {
                lastRequiredElement.ValidContentChange += LastRequiredElement_ValidContentChange;
            }


            _pages = pages;
            _formElements = formElements.AsReadOnly();
        }

        private void UnlockInitialVisibleElements(IEnumerable<FormElement> formElements)
        {
            var initialVisibleElements = formElements.TakeUntilIncluding(f => f.Data.Required);
            foreach (var element in initialVisibleElements)
            {
                UnlockElement(element);
            }
        }

        private void LastRequiredElement_ValidContentChange(object sender, EventArgs _)
        {
            var lastRequiredElement = (FormElement)sender;
            lastRequiredElement.ValidContentChange -= LastRequiredElement_ValidContentChange;
            DependencyService.Get<IToast>().ShortAlert(AppResources.pleaseSaveBeforeQuiting);
        }

        private void UnlockElement(FormElement element)
        {
            UnlockedElements.Add(element);
            element.Grid.IsVisible = true;
        }

        private void LockElement(FormElement element)
        {
            UnlockedElements.Remove(element);
            element.Grid.IsVisible = false;
            element.Reset();
        }

        private void FormElement_InvalidContentChange(object sender, EventArgs _)
        {
            var changedElement = (FormElement)sender;

            //if the element was not required to progress, then changing it can't make elements invisible
            if (changedElement.Data.Required)
            {
                foreach (var element in _formElements.SkipWhileIncluding(e => e != changedElement))
                {
                    LockElement(element);
                }
            }
            RefreshVisibilityOnUnlockedElements();
        }

        private void RefreshVisibilityOnUnlockedElements()
        {
            Dictionary<string,string> variables = null;
            foreach (var element in UnlockedElements.Where(e => e.ShouldBeShownExpression != null))
            {
                if (variables == null)
                    variables = GeatherVariables();
                if (element.ShouldBeShownExpression.Evaluate(variables))
                {
                    element.Grid.IsVisible = true;
                }
                else
                {
                    var wasVisible = element.Grid.IsVisible;
                    if (wasVisible)
                    {
                        element.Grid.IsVisible = false;
                        element.Reset();
                    }
                }
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
            var changedElement = (FormElement)sender;

            //if the element was not required to progress, then changing it can't make a new element visible
            if (changedElement.Data.Required)
            {
                var currentlyRequiredElement = _formElements.LastOrDefault(e => e.Grid.IsVisible && e.Data.Required);
                if (currentlyRequiredElement == changedElement)
                {
                    //Show all questions until (including) the next required one
                    var newlyUnlockedElements = _formElements.SkipWhileIncluding(e => e != changedElement).TakeUntilIncluding(e => e.Data.Required);
                    foreach (var element in newlyUnlockedElements)
                    {
                        UnlockElement(element);
                    }
                }
            }
            RefreshVisibilityOnUnlockedElements();
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

                SurveyStorageManager.ProjectsFilledSinceLastSurveyCompletion++;
                SurveyStorageManager.SaveAnswers();

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
                CheckForSurveyCompletionNeeded();
            }
        }
    }
}