using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class LocationDetectorElement : FormElement
    {
        public LocationDetectorElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) { }

        public Label SavedLocationLabel;

        public override bool IsValid => !string.IsNullOrEmpty(SavedLocationLabel.Text) && base.IsValid;

        public override string GetRepresentationValue() => SavedLocationLabel.Text;

        public override void LoadFromSavedRepresentation(string representation) => SavedLocationLabel.Text = representation;

        public override void Reset() => SavedLocationLabel.Text = string.Empty;

        public static LocationDetectorElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new LocationDetectorElement(grid, parms.Element, parms.Type);

            var labelLat = new Label { Text = "Latitude" };

            var labelLatData = new Label() { Text = Sensor.Instance.Gps.Latitude.ToString(CultureInfo.CurrentCulture) };

            var labelLong = new Label { Text = "Longitude" };

            var labelLongData = new Label() { Text = Sensor.Instance.Gps.Longitude.ToString(CultureInfo.CurrentCulture) };

            var labelMessage = new Label { Text = AppResources.message };

            var labelMessageData = new Label() { Text = Sensor.Instance.Gps.Message };

            Sensor.Instance.Gps.StatusChanged += (sender, args) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    labelLatData.Text = args.Latitude.ToString();
                    labelLongData.Text = args.Longitude.ToString();
                    labelMessage.Text = args.Message;
                });
            };

            var saveButton = new Button { Text = AppResources.save };
            var skipButton = new Button { Text = AppResources.skip };

            var savedLocation = new Label { Text = AppResources.saveddata };
            var savedLocationData = new Label
            {
                Text = string.Empty,
                StyleId = parms.Element.Name
            };

            formElement.SavedLocationLabel = savedLocationData;

            saveButton.Clicked += (sender, args) =>
            {
                savedLocationData.Text = $"Lat:{labelLongData.Text} Long:{labelLatData.Text}";
                formElement.OnContentChange();
            };

            skipButton.Clicked += (sender, args) =>
            {
                savedLocationData.Text = $"Lat:0 Long:0";
                formElement.OnContentChange();
            };

            grid.Children.Add(labelLat, 0, 1);
            grid.Children.Add(labelLatData, 1, 1);
            grid.Children.Add(labelLong, 0, 2);
            grid.Children.Add(labelLongData, 1, 2);
            grid.Children.Add(labelMessage, 0, 3);
            grid.Children.Add(labelMessageData, 1, 3);
            grid.Children.Add(skipButton, 0, 4);
            grid.Children.Add(saveButton, 1, 4);
            grid.Children.Add(savedLocation, 0, 5);
            grid.Children.Add(savedLocationData, 1, 5);

            return formElement;
        }
    }
}
