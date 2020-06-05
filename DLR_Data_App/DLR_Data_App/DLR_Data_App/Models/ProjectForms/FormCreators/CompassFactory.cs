using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using DLR_Data_App.Services.Sensors;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms.FormCreators
{
    class CompassFactory : ElementFactory
    {
        public override FormElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var compassElement = new CompassElement(grid, parms.Element, parms.Type);

            var currentCompassLabel = new Label { Text = AppResources.compass };
            compassElement.CurrentDataLabel = currentCompassLabel;
            var currentCompassDataLabel = new Label();
            Sensor.Instance.Compass.ReadingChanged += (_, eventArgs) =>
            {
                currentCompassDataLabel.Text = ((int)eventArgs.Reading.HeadingMagneticNorth).ToString() + " °";
                compassElement.CurrentHeadingMagneticNorth = eventArgs.Reading.HeadingMagneticNorth;
            };

            var saveButton = new Button { Text = AppResources.save };

            var savedCompassLabel = new Label { Text = AppResources.saveddata };
            compassElement.SavedDataLabel = savedCompassLabel;
            var savedCompassDataLabel = new Label { StyleId = parms.Element.Name };

            saveButton.Clicked += (_, b) => Device.BeginInvokeOnMainThread(() =>
                 {
                     savedCompassDataLabel.Text = currentCompassDataLabel.Text;
                     compassElement.SavedHeadingMagneticNorth = compassElement.CurrentHeadingMagneticNorth;
                     compassElement.OnContentChange();
                 });

            grid.Children.Add(currentCompassLabel, 0, 1);
            grid.Children.Add(currentCompassDataLabel, 1, 1);
            grid.Children.Add(saveButton, 0, 2);
            Grid.SetColumnSpan(saveButton, 2);
            grid.Children.Add(savedCompassLabel, 0, 3);
            grid.Children.Add(savedCompassDataLabel, 1, 3);

            

            return compassElement;
        }
    }
}
