using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class TextInputElement : FormElement
    {
        public TextInputElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) 
        {
            LengthRange = OdkDataExtractor.GetRangeFromJsonString(data.Length, true, true);
        }

        public OdkRange LengthRange;
        public Entry Entry;

        public override bool IsValid => !string.IsNullOrEmpty(Entry.Text) && LengthRange.IsValidIntegerInput(Entry.Text.Length) && base.IsValid;

        public override string GetRepresentationValue() => Entry.Text;

        public override void LoadFromSavedRepresentation(string representation) => Entry.Text = representation;

        public override void Reset() => Entry.Text = string.Empty;
    }
}
