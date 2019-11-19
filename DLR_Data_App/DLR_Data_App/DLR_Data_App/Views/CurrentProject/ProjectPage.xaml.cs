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
            else if (element is Label label && label.StyleId != null && label.StyleId.EndsWith("LocationData"))
            {
                ElementNameList.Add(label.StyleId.Substring(0,label.StyleId.Length - "LocationData".Length));
                ElementValueList.Add(label.Text);
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

        /// <summary>
        /// Updates shown gps data.
        /// </summary>
        private void OnGpsChange(object sender, GpsEventArgs e)
        {
            foreach (var label in ElementList.OfType<Label>())
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
        /// Updates view.
        /// </summary>
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
                    var content = FormCreator.GenerateForm(projectForm, _workingProject, DisplayAlert, _sensor);
                    ElementList.AddRange(content.ElementList);
                    _pages.Add(content.Form);
                    yield return content.Form;
                }
            }

            yield break;
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
                ElementNameList = new List<string>();
                ElementValueList = new List<string>();
                Helpers.WalkElements(_pages, SaveInfoFromView);

                var tableName = _workingProject.GetTableName();
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