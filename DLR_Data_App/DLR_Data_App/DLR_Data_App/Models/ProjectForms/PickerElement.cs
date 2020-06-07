﻿using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
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

        public override bool IsValid => Picker.SelectedIndex >= 0 && base.IsValid;

        public override string GetRepresentationValue() => Picker.SelectedIndex.ToString();

        public override void LoadFromSavedRepresentation(string representation)
        {
            if (int.TryParse(representation, out int value))
                Picker.SelectedIndex = value;
        }

        public override void Reset() => Picker.SelectedIndex = -1;

        public static PickerElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new PickerElement(grid, parms.Element, parms.Type);

            var optionsList = new List<string>();
            var options = ProjectParser.ParseOptionsFromJson(parms.Element.Options);
            var title = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);
            if (string.IsNullOrWhiteSpace(title))
            {
                title = AppResources.notitle;
            }
            var currentLanguageCode = OdkDataExtractor.GetCurrentLanguageCodeFromJsonList(parms.CurrentProject.Languages);
            foreach (var option in options)
            {
                option.Text.TryGetValue(currentLanguageCode, out var value);
                optionsList.Add(value);
            }
            optionsList.Add(AppResources.unknown);

            var picker = new Picker
            {
                Title = title,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                StyleId = parms.Element.Name,
                ItemsSource = optionsList
            };

            formElement.Picker = picker;

            picker.SelectedIndexChanged += (a, b) => formElement.OnContentChange();

            grid.Children.Add(picker, 0, 1);
            Grid.SetColumnSpan(picker, 2);

            return formElement;
        }
    }
}
