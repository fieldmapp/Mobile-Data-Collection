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

        protected override bool IsValidElementSpecific => !string.IsNullOrWhiteSpace(Entry.Text) && int.TryParse(Entry.Text, out var decimalInput) && ValidRange.IsValidInput(decimalInput);

        public override string GetRepresentationValue() => Entry.Text ?? string.Empty;

        public override void LoadFromSavedRepresentation(string representation) => Entry.Text = representation;

        protected override void OnReset() => Entry.Text = string.Empty;

        public static NumericInputElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new NumericInputElement(grid, parms.Element, parms.Type);

            var placeholder = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);

            var entry = new Entry
            {
                Placeholder = placeholder,
                Keyboard = Keyboard.Numeric
            };

            formElement.Entry = entry;


            entry.TextChanged += (a, b) => formElement.OnContentChange();

            grid.Children.Add(entry, 0, 1);
            Grid.SetColumnSpan(entry, 2);

            return formElement;
        }
    }
}
