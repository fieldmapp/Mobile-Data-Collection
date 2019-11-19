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
        private readonly Project _workingProject = Database.GetCurrentProject();
        private int _id;
        private readonly Sensor _sensor = Sensor.Instance;
        private List<ContentPage> _pages;

        public EditDataDetailPage(Dictionary<string, string> projectData)
        {
            void WriteInfoToView(View element)
            {
                if (element is Entry entry)
                {
                    entry.Text = projectData[entry.StyleId];
                }
                else if (element is Picker picker)
                {
                    picker.SelectedIndex = picker.Items.IndexOf(projectData[picker.StyleId]);
                }
                else if (element is Label label && label.StyleId != null && label.StyleId.EndsWith("LocationData"))
                {
                    label.Text = projectData[label.StyleId.Substring(0, label.StyleId.Length - "LocationData".Length)];
                }
            }

            InitializeComponent();

            if (_workingProject == null)
                throw new Exception();

            var translatedProject = Helpers.TranslateProjectDetails(_workingProject);
            Title = translatedProject.Title;

            _id = Convert.ToInt32(projectData["Id"]);

            _pages = UpdateView();
            foreach (var contentPage in _pages)
            {
                Children.Add(contentPage);
            }

            Helpers.WalkElements(_pages, WriteInfoToView);
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

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopAsync();
            return true;
        }

        public List<ContentPage> UpdateView()
        {
            var pages = new List<ContentPage>();

            if (_workingProject == null)
                return pages;

            foreach (var projectForm in _workingProject.FormList)
            {
                var content = FormCreator.GenerateForm(projectForm, _workingProject, DisplayAlert, _sensor);
                pages.Add(content.Form);
            }

            return pages;
        }

        /// <summary>
        /// Updates shown gps data.
        /// </summary>
        private void OnGpsChange(object sender, GpsEventArgs e)
        {
            foreach (var label in Helpers.WalkElements(_pages).OfType<Label>())
            {
                if (label.StyleId.Contains("Lat"))
                    Device.BeginInvokeOnMainThread(() => label.Text = e.Latitude.ToString(CultureInfo.CurrentCulture));

                if (label.StyleId.Contains("Long"))
                    Device.BeginInvokeOnMainThread(() => label.Text = e.Longitude.ToString(CultureInfo.CurrentCulture));

                if (label.StyleId.Contains("Message"))
                    Device.BeginInvokeOnMainThread(() => label.Text = e.Message.ToString(CultureInfo.CurrentCulture));
            }
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            var tableName = _workingProject.GetTableName();

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

            var success = Database.UpdateCustomValuesById(tableName, _id, elementNameList, elementValueList);

            string message = success ? AppResources.successful : AppResources.failed;
            await DisplayAlert(AppResources.save, message, AppResources.okay);
        }
    }
}