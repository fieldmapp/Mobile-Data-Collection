using DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjectsSharedModule.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectForms
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

        public static NumericInputElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new NumericInputElement(grid, parms.Element, parms.Type);

            var placeholder = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);

            var entry = new Entry
            {
                Placeholder = placeholder,
                Keyboard = Keyboard.Numeric,
                StyleId = parms.Element.Name
            };

            formElement.Entry = entry;


            entry.TextChanged += (a, b) => formElement.OnContentChange();

            grid.Children.Add(entry, 0, 1);
            Grid.SetColumnSpan(entry, 2);

            return formElement;
        }
    }
}
