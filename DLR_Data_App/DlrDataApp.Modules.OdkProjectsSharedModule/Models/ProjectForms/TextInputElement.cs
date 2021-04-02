﻿using DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjectsSharedModule.Services;
using System;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectForms
{
    class TextInputElement : FormElement
    {
        public TextInputElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) 
        {
            LengthRange = OdkDataExtractor.GetRangeFromJsonString(data.Length, Convert.ToInt32, true, true);
        }

        public OdkRange<int> LengthRange;
        public Entry Entry;

        protected override bool IsValidElementSpecific => !string.IsNullOrEmpty(Entry.Text) && LengthRange.IsValidInput(Entry.Text.Length);

        public override string GetRepresentationValue() => Entry.Text ?? string.Empty;

        public override void LoadFromSavedRepresentation(string representation) => Entry.Text = representation;

        protected override void OnReset() => Entry.Text = string.Empty;

        public static TextInputElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new TextInputElement(grid, parms.Element, parms.Type);

            var placeholder = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);

            var entry = new Entry
            {
                Placeholder = placeholder,
                Keyboard = Keyboard.Default
            };
            formElement.Entry = entry;

            entry.TextChanged += (a, b) => formElement.OnContentChange();

            grid.Children.Add(entry, 0, 1);
            Grid.SetColumnSpan(entry, 2);

            return formElement;
        }
    }
}
