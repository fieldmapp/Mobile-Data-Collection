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
        private readonly ProjectViewModel _viewModel = new ProjectViewModel();
        private Project _workingProject = Database.GetCurrentProject();
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _sensor.Gps.StatusChanged += OnGpsChange;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _sensor.Gps.StatusChanged -= OnGpsChange;
        }

        private void ClearView(View element)
        {
            if (element is Entry entry)
                entry.Text = "";
            else if (element is Picker picker)
                picker.SelectedIndex = -1;
        }

        private void ProjectPage_Appearing(object sender, EventArgs e)
        {
            if (_projectLastCheck?.Id == _workingProject?.Id)
                return;
            _projectLastCheck = _workingProject;

            Children.Clear();
            UpdateView();
            foreach (var page in _pages)
            {
                Children.Add(page);
            }

            if (_pages == null || _pages.Count == 0)
            {
                (Application.Current as App).CurrentPage.DisplayAlert(AppResources.warning, AppResources.noactiveproject, AppResources.okay);
            }
        }

        /// <summary>
        /// Updates shown gps data.
        /// </summary>
        private void OnGpsChange(object sender, GpsEventArgs e)
        {
            foreach (var label in Helpers.WalkElements(_pages).OfType<Label>().Where(l => l.StyleId != null))
            {
                if (label.StyleId.Contains("Lat"))
                    Device.BeginInvokeOnMainThread(() => label.Text = e.Latitude.ToString(CultureInfo.CurrentCulture));

                if (label.StyleId.Contains("Long"))
                    Device.BeginInvokeOnMainThread(() => label.Text = e.Longitude.ToString(CultureInfo.CurrentCulture));

                if (label.StyleId.Contains("Message"))
                    Device.BeginInvokeOnMainThread(() => label.Text = e.Message.ToString(CultureInfo.CurrentCulture));
            }
        }

        /// <summary>
        /// Updates view. Sets _pages and _formElements.
        /// </summary>
        public void UpdateView()
        {
            // Get current project
            _workingProject = Database.GetCurrentProject();
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
                element.Grid.IsVisible = true;
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

        private void FormElement_InvalidContentChange(object sender, EventArgs _)
        {
            var changedElement = (FormElement)sender;

            //if the element was not required to progress, then changing it can't make elements invisible
            if (changedElement.Data.Required)
            {
                foreach (var element in _formElements.SkipWhileIncluding(e => e != changedElement))
                {
                    UnlockedElements.Remove(element);
                    element.Grid.IsVisible = false;
                    foreach (var view in element.Grid.Children)
                    {
                        ClearView(view);
                    }
                }
            }
            RefreshVisibilityOnUnlockedElements();
        }

        private void RefreshVisibilityOnUnlockedElements()
        {
            foreach (var element in UnlockedElements)
            {
                if (element.ShouldBeShownExpression?.Evaluate(GeatherVariables()) ?? true)
                    element.Grid.IsVisible = true;
                else
                    element.Grid.IsVisible = false;
            }
        }

        Dictionary<string,string> GeatherVariables()
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            foreach (var view in Helpers.WalkElements(_pages))
            {
                if (view is Entry entry)
                {
                    variables.Add(entry.StyleId, entry.Text ?? "0");
                }
                else if (view is Picker picker)
                {
                    //TODO: IndexOf does not respect actual backing values from odk, 
                    //will result in problems if a picker is not using integer backing values starting from 0
                    variables.Add(picker.StyleId, picker.Items.IndexOf(picker.SelectedItem as string ?? "").ToString());
                }
                else if (view is Label label && label.StyleId != null && label.StyleId.EndsWith("LocationData"))
                {
                    variables.Add(label.StyleId.Substring(0, label.StyleId.Length - "LocationData".Length), label.Text);
                }
            }
            return variables;
        }

        HashSet<FormElement> UnlockedElements = new HashSet<FormElement>();

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
                        UnlockedElements.Add(element);
                        element.Grid.IsVisible = true;
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
        private async void SaveClicked(object sender, EventArgs e)
        {
            if (await CheckActiveProject())
            {
                var elementNameList = new List<string>();
                var elementValueList = new List<string>();

                foreach (var view in Helpers.WalkElements(_pages))
                {
                    if (view is Entry entry)
                    {
                        elementNameList.Add(entry.StyleId);
                        elementValueList.Add(entry.Text ?? "0");
                    }
                    else if (view is Picker picker)
                    {
                        elementNameList.Add(picker.StyleId);
                        elementValueList.Add(picker.SelectedItem as string ?? "0");
                    }
                    else if (view is Label label && label.StyleId != null && label.StyleId.EndsWith("LocationData"))
                    {
                        elementNameList.Add(label.StyleId.Substring(0, label.StyleId.Length - "LocationData".Length));
                        elementValueList.Add(label.Text);
                    }
                }

                var tableName = _workingProject.GetTableName();
                var status = Database.InsertCustomValues(tableName, elementNameList, elementValueList);

                string message;
                if (status)
                {
                    message = AppResources.successful;
                    Helpers.WalkElements(_pages, ClearView);
                }
                else
                {
                    message = AppResources.failed;
                }

                await DisplayAlert(AppResources.save, message, AppResources.okay);
            }
        }
    }
}