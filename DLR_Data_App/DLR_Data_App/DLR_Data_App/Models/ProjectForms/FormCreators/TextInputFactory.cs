using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms.FormCreators
{
    class TextInputFactory : ElementFactory
    {
        public override FormElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new TextInputElement(grid, parms.Element, parms.Type);

            var placeholder = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);
            if (placeholder == "Unable to parse language from json")
            {
                placeholder = string.Empty;
            }

            var entry = new Entry
            {
                Placeholder = placeholder,
                Keyboard = Keyboard.Default,
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
