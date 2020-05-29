using DLR_Data_App.Models.ProjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class CompassElement : FormElement
    {
        public CompassElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) { }

        public double CurrentHeadingMagneticNorth;
        public double SavedHeadingMagneticNorth;
        public Label CurrentDataLabel;
        public Label SavedDataLabel;

        public override bool IsValid => SavedHeadingMagneticNorth != 0;

        public override void Reset()
        {
            CurrentHeadingMagneticNorth = 0;
            CurrentDataLabel.Text = string.Empty;
            SavedDataLabel.Text = string.Empty;
        }

        public override string GetRepresentationValue() => SavedHeadingMagneticNorth.ToString();

        public override void LoadFromSavedRepresentation(string representation)
        {
            if (double.TryParse(representation, out double heading))
            {
                SavedHeadingMagneticNorth = heading;
                SavedDataLabel.Text = heading.ToString() + " °";
            }
        }
    }
}
