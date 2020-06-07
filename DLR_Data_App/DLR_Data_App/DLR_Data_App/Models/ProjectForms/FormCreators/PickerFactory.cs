using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms.FormCreators
{
    class PickerFactory : ElementFactory
    {
        public override FormElement CreateForm(FormCreationParams parms)
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
