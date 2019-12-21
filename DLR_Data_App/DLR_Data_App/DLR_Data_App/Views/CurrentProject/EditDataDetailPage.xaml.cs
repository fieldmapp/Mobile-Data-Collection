using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using DLR_Data_App.Services.Sensors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.CurrentProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditDataDetailPage
    {
        //TODO: Merge EditDataDetailPage and ProjectPage

        private readonly Project _workingProject = Database.GetCurrentProject();
        private int _id;
        private readonly Sensor _sensor = Sensor.Instance;
        private List<ContentPage> _pages;
        HashSet<FormElement> UnlockedElements;
        private IReadOnlyList<FormElement> _formElements;

        public EditDataDetailPage(Dictionary<string, string> projectData)
        {
            InitializeComponent();

            if (_workingProject == null)
                throw new Exception();

            var translatedProject = Helpers.TranslateProjectDetails(_workingProject);
            Title = translatedProject.Title;

            _id = Convert.ToInt32(projectData["Id"]);

            UnlockedElements = new HashSet<FormElement>();
            Children.Clear();
            LoadPagesAndElements(_workingProject);
            foreach (var page in _pages)
            {
                Children.Add(page);
            }

            if (_pages == null || _pages.Count == 0)
            {
                (Application.Current as App).CurrentPage.DisplayAlert(AppResources.warning, AppResources.noactiveproject, AppResources.okay);
            }


            FormElement lastSetRequiredElement = null;
            foreach (var element in _formElements)
            {
                var updatedSomething = element.LoadContentFromProjectData(projectData);
                if (updatedSomething && element.Data.Required)
                    lastSetRequiredElement = element;
            }

            foreach (var element in _formElements)
            {
                LockElement(element, false);
            }

            var nextRequiredElement = _formElements.SkipWhileIncluding(e => e != lastSetRequiredElement).FirstOrDefault(e => e.Data.Required);

            foreach (var initialVisibleElement in _formElements.TakeUntilIncluding(e => e == nextRequiredElement))
            {
                UnlockElement(initialVisibleElement);
            }
            RefreshVisibilityOnUnlockedElements();
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopAsync();
            return true;
        }

        /// <summary>
        /// Updates view. Sets _pages and _formElements.
        /// </summary>
        public void LoadPagesAndElements(Project newProject)
        {
            // Get current project
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
            var initialVisibleElements = formElements.TakeUntilIncluding(f => f.Data.Required);
            foreach (var element in initialVisibleElements)
            {
                UnlockElement(element);
            }
            var lastRequiredElement = formElements.LastOrDefault(e => e.Data.Required);
            if (lastRequiredElement != null)
            {
                lastRequiredElement.ValidContentChange += LastRequiredElement_ValidContentChange;
            }


            _pages = pages;
            _formElements = formElements.AsReadOnly();
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

        private void LockElement(FormElement element, bool reset = true)
        {
            UnlockedElements.Remove(element);
            element.Grid.IsVisible = false;
            if (reset)
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

        Dictionary<string, string> GeatherVariables()
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            foreach (var representation in _formElements.Select(e => e.GetRepresentation()))
            {
                variables.Add(representation.Key, representation.Value);
            }
            return variables;
        }

        private async void SaveClicked(object sender, EventArgs _)
        {
            var tableName = _workingProject.GetTableName();

            var elementNameList = new List<string>();
            var elementValueList = new List<string>();

            foreach (var representation in _formElements.Select(e => e.GetRepresentation()))
            {
                elementNameList.Add(representation.Key);
                elementValueList.Add(representation.Value);
            }

            var success = Database.UpdateCustomValuesById(tableName, _id, elementNameList, elementValueList);

            string message = success ? AppResources.successful : AppResources.failed;
            await DisplayAlert(AppResources.save, message, AppResources.okay);
        }
    }
}