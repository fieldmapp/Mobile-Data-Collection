using DLR_Data_App.Models.ProjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class PickerElement : FormElement
    {
        public PickerElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) { }

        public Picker Picker;

        public override bool IsValid => Picker.SelectedIndex >= 0;

        public override string GetRepresentationValue() => Picker.SelectedIndex.ToString();

        public override void LoadFromSavedRepresentation(string representation)
        {
            if (int.TryParse(representation, out int value))
                Picker.SelectedIndex = value;
        }

        public override void Reset() => Picker.SelectedIndex = -1;
    }
}
