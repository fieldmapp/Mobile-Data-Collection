using DLR_Data_App.Models.ProjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class LocationDetectorElement : FormElement
    {
        public LocationDetectorElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) { }

        public Label SavedLocationLabel;

        public override bool IsValid => !string.IsNullOrEmpty(SavedLocationLabel.Text);

        public override string GetRepresentationValue() => SavedLocationLabel.Text;

        public override void LoadFromSavedRepresentation(string representation) => SavedLocationLabel.Text = representation;

        public override void Reset() => SavedLocationLabel.Text = string.Empty;
    }
}
