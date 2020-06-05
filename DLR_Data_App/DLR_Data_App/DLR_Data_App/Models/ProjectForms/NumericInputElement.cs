using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class NumericInputElement : FormElement
    {
        public NumericInputElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type)
        {
            ValidRange = OdkDataExtractor.GetRangeFromJsonString(Data.Range, Convert.ToInt32);
        }

        public Entry Entry;

        private readonly OdkRange<int> ValidRange;

        public override bool IsValid => !string.IsNullOrWhiteSpace(Entry.Text) && int.TryParse(Entry.Text, out var decimalInput) && ValidRange.IsValidInput(decimalInput) && base.IsValid;

        public override string GetRepresentationValue() => Entry.Text;

        public override void LoadFromSavedRepresentation(string representation) => Entry.Text = representation;

        public override void Reset() => Entry.Text = string.Empty;
    }
}
