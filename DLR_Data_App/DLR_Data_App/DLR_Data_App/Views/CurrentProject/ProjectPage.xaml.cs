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
        public List<string> ElementNameList = new List<string>();
        public List<string> ElementValueList = new List<string>();
        public List<View> ElementList = new List<View>();

        private readonly ProjectViewModel _viewModel = new ProjectViewModel();
        private Project _workingProject = Database.GetCurrentProject();
        private List<ContentPage> _pages;

        private readonly Sensor _sensor = new Sensor();

        public ProjectPage()
        {
            InitializeComponent();

            _sensor.Gps.StatusChanged += OnGpsChange;

            BindingContext = _viewModel;

            Appearing += ProjectPage_Appearing;
        }

        private void ClearView(View element)
        {
            if (element is Entry entry)
                entry.Text = "";
            else if (element is Picker picker)
                picker.SelectedIndex = -1;
        }

        private void SaveInfoFromView(View element)
        {
            if (element is Entry entry)
            {
                ElementNameList.Add(entry.StyleId);
                ElementValueList.Add(entry.Text ?? "0");
            }
            else if (element is Picker picker)
            {
                ElementNameList.Add(picker.StyleId);
                ElementValueList.Add(picker.SelectedItem as string ?? "0");
            }
            else if (element is Label label && label.StyleId != null)
            {
                var containedInfo = string.Empty;
                if (label.StyleId.Contains("Lat"))
                    containedInfo = "Lat";
                else if (label.StyleId.Contains("Long"))
                    containedInfo = "Long";
                if (containedInfo != string.Empty)
                {
                    var styleId = label.StyleId.Replace(containedInfo, string.Empty);
                    var index = ElementNameList.IndexOf(styleId);
                    if (index != -1)
                    {
                        ElementValueList[index] += $" {containedInfo}: {label.Text}";
                    }
                    else
                    {
                        ElementNameList.Add(styleId);
                        ElementValueList.Add($"{containedInfo}: {label.Text}");
                    }
                }
            }
        }

        private void ProjectPage_Appearing(object sender, EventArgs e)
        {
            Appearing -= ProjectPage_Appearing;
            foreach (var page in UpdateView())
            {
                Children.Add(page);
            }

            if (_pages == null || _pages.Count == 0)
            {
                (Application.Current as App).CurrentPage.DisplayAlert(AppResources.warning, AppResources.noactiveproject, AppResources.okay);
            }
        }

        /**
         * Update shown gps data
         */
        private void OnGpsChange(object sender, GpsEventArgs e)
        {
            foreach (var label in ElementList.OfType<Label>())
            {
                if (label.StyleId.Contains("Lat"))
                    label.Text = e.Latitude.ToString(CultureInfo.CurrentCulture);

                if (label.StyleId.Contains("Long"))
                    label.Text = e.Longitude.ToString(CultureInfo.CurrentCulture);

                if (label.StyleId.Contains("Message"))
                    label.Text = e.Message.ToString(CultureInfo.CurrentCulture);
            }
        }

        /**
         * Update view
         */
        public IEnumerable<ContentPage> UpdateView()
        {
            // Get current project
            _workingProject = Database.GetCurrentProject();
            _pages = new List<ContentPage>();

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
                    var content = FormFactory.GenerateForm(projectForm, _workingProject, DisplayAlert);
                    ElementList.AddRange(content.ElementList);
                    _pages.Add(content.Form);
                    yield return content.Form;
                }
            }

            yield break;
        }

        /**
         * Check if active project exists
         */
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

        /**
         * Navigate to edit page
         */
        private async void EditClicked(object sender, EventArgs e)
        {
            if (await CheckActiveProject())
            {
                await Navigation.PushAsync(new EditDataPage());
            }
        }

        /**
         * Save data
         */
        private async void SaveClicked(object sender, EventArgs e)
        {
            if (await CheckActiveProject())
            {
                ElementNameList = new List<string>();
                ElementValueList = new List<string>();
                Helpers.WalkElements(_pages, SaveInfoFromView);

                var tableName = Parser.LanguageJsonStandard(_workingProject.Title, _workingProject.Languages) + "_" + _workingProject.Id;
                var status = Database.InsertCustomValues(tableName, ElementNameList, ElementValueList);

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