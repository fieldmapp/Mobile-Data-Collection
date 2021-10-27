using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class LocationDetectorElement : FormElement
    {
        public LocationDetectorElement(Grid grid, ProjectFormElements data, string type, Func<string, string, string, Task> displayAlertFunc, Project project) : base(grid, data, type, displayAlertFunc, project) { }

        public Label SavedLocationLabel;

        protected override bool IsValidElementSpecific => !string.IsNullOrEmpty(SavedLocationLabel.Text);

        public override string GetRepresentationValue() => SavedLocationLabel.Text;

        public override void LoadFromSavedRepresentation(string representation) => SavedLocationLabel.Text = representation;

        protected override void OnReset() => SavedLocationLabel.Text = string.Empty;

        public static LocationDetectorElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new LocationDetectorElement(grid, parms.Element, parms.Type, parms.DisplayAlertFunc, parms.CurrentProject);

            var labelLat = new Label { Text = AppResources.latitude };
            var labelLatData = new Label() { Text = Sensor.Instance.Gps.Latitude.ToString(CultureInfo.CurrentCulture) };

            var labelLong = new Label { Text = AppResources.longitude };
            var labelLongData = new Label() { Text = Sensor.Instance.Gps.Longitude.ToString(CultureInfo.CurrentCulture) };

            var labelAltitude = new Label { Text = AppResources.altitude };
            var labelAltitudeData = new Label { Text = Sensor.Instance.Gps.Altitude.ToString(CultureInfo.CurrentCulture) };

            var labelAccuracy = new Label { Text = AppResources.accuracy };
            var labelAccuracyData = new Label { Text = Sensor.Instance.Gps.Accuracy.ToString(CultureInfo.CurrentCulture) };

            var labelMessage = new Label { Text = AppResources.message };
            var labelMessageData = new Label() { Text = Sensor.Instance.Gps.Message };

            Sensor.Instance.Gps.StatusChanged += (sender, args) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    labelLatData.Text = args.Latitude.ToString();
                    labelLongData.Text = args.Longitude.ToString();
                    labelAltitudeData.Text = args.Altitude.ToString();
                    labelAccuracyData.Text = args.Accuracy.ToString();
                    labelMessageData.Text = args.Message;
                });
            };

            var saveButton = new Button { Text = AppResources.save };
            var skipButton = new Button { Text = AppResources.skip };

            var savedLocation = new Label { Text = AppResources.saveddata };
            var savedLocationData = new Label { Text = string.Empty };

            formElement.SavedLocationLabel = savedLocationData;

            saveButton.Clicked += (sender, args) =>
            {
                savedLocationData.Text = $"Lat:{labelLongData.Text} Long:{labelLatData.Text} Alt:{labelAltitudeData.Text} Acc:{labelAccuracyData.Text} Utc:{DateTime.UtcNow.Ticks}";
                formElement.OnContentChange();
            };

            skipButton.Clicked += (sender, args) =>
            {
                savedLocationData.Text = $"Lat:0 Long:0 Alt:0 Acc:-1 Utc:0";
                formElement.OnContentChange();
            };

            grid.Children.Add(labelLat, 0, 1);
            grid.Children.Add(labelLatData, 1, 1);

            grid.Children.Add(labelLong, 0, 2);
            grid.Children.Add(labelLongData, 1, 2);

            grid.Children.Add(labelAltitude, 0, 3);
            grid.Children.Add(labelAltitudeData, 1, 3);

            grid.Children.Add(labelAccuracy, 0, 4);
            grid.Children.Add(labelAccuracyData, 1, 4);

            grid.Children.Add(labelMessage, 0, 5);
            grid.Children.Add(labelMessageData, 1, 5);

            grid.Children.Add(skipButton, 0, 6);
            grid.Children.Add(saveButton, 1, 6);
            
            grid.Children.Add(savedLocation, 0, 7);
            grid.Children.Add(savedLocationData, 1, 7);

            return formElement;
        }
    }
}
