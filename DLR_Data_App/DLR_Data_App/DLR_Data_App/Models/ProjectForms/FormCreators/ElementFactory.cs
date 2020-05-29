using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms.FormCreators
{
    abstract class ElementFactory
    {
        protected static Grid CreateStandardBaseGrid(FormCreationParams parms)
        {
            var elementName = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);
            var indexOfOpeningCurlyBrace = elementName.IndexOf('{');
            var indexOfClosingCurlyBrace = elementName.IndexOf('}');
            if (indexOfOpeningCurlyBrace != -1 && indexOfClosingCurlyBrace != -1 && indexOfOpeningCurlyBrace < indexOfClosingCurlyBrace)
            {
                elementName = elementName.Remove(indexOfOpeningCurlyBrace, indexOfClosingCurlyBrace - indexOfOpeningCurlyBrace + 1);
            }

            var elementNameLabel = new Label
            {
                Text = elementName,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            var grid = new Grid { IsVisible = false };
            grid.Children.Add(elementNameLabel, 0, 0);
            Grid.SetRowSpan(elementNameLabel, 2);

            var hintText = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Hint, parms.CurrentProject.Languages);

            if (hintText != "Unable to parse language from json" && !string.IsNullOrWhiteSpace(hintText))
            {
                var helpButton = new Button { Text = AppResources.help };

                helpButton.Clicked += async (sender, args) => await parms.DisplayAlertFunc(AppResources.help, hintText, AppResources.okay);
                grid.Children.Add(helpButton, 1, 0);
            }
            else
            {
                Grid.SetColumnSpan(elementNameLabel, 2);
            }
            return grid;
        }
        public abstract FormElement CreateForm(FormCreationParams parms);

    }
}
