using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    /// <summary>
    /// Class representing every part of a displayed element in a form.
    /// </summary>
    abstract class FormElement
    {
        const string ThisOdkElement = "thisodkelement";
        public FormElement(Grid grid, ProjectFormElements data, string type)
        {
            Grid = grid;
            Data = data;
            ShouldBeShownExpression = string.IsNullOrWhiteSpace(data.Relevance) ? null : OdkDataExtractor.GetBooleanExpression(data.Relevance);
            if (!string.IsNullOrWhiteSpace(data.Constraint))
            {
                StringBuilder modifiedConstraintString = new StringBuilder();
                bool lastCharWasDigit = false;
                for (int i = 0; i < data.Constraint.Length; i++)
                {
                    if (!lastCharWasDigit && data.Constraint[i] == '.')
                    {
                        modifiedConstraintString.Append(ThisOdkElement);
                    }
                    else
                    {
                        modifiedConstraintString.Append(data.Constraint[i]);
                    }
                    lastCharWasDigit = char.IsDigit(data.Constraint, i);
                }
                ConstraintExpression = OdkDataExtractor.GetBooleanExpression(modifiedConstraintString.ToString());
            }
            Type = type;
        }

        public Grid Grid { get; }
        public ProjectFormElements Data { get; }
        public event EventHandler ValidContentChange;
        public event EventHandler InvalidContentChange;
        public OdkBooleanExpresion ShouldBeShownExpression { get; }
        public OdkBooleanExpresion ConstraintExpression { get; }
        public virtual bool IsValid { get =>
                ConstraintExpression == null ||
                ConstraintExpression.Evaluate(new Dictionary<string, string> { { ThisOdkElement, GetRepresentationValue() } }); 
        }
        public readonly string Type;

        public void OnContentChange()
        {
            if (IsValid)
                ValidContentChange?.Invoke(this, null);
            else
                InvalidContentChange?.Invoke(this, null);
        }

        /// <summary>
        /// Sets the grids content to the default value
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Retrieves a representation for this element
        /// </summary>
        /// <returns>Pair containing 1. the name and 2. the content of the element.</returns>
        public KeyValuePair<string, string> GetRepresentation()
        {
            return new KeyValuePair<string, string>(Data.Name, GetRepresentationValue());
        }

        public abstract string GetRepresentationValue();

        /// <summary>
        /// Sets grids content based on the given projetData
        /// </summary>
        /// <param name="projectData">Dictionary matching an elements name and its content</param>
        /// <returns><see cref="Boolean"/> indicating if this FormElement is set to a value which is making it valid</returns>
        public bool LoadContentFromProjectData(Dictionary<string, string> projectData)
        {
            if (projectData.TryGetValue(Data.Name, out string representation))
            {
                LoadFromSavedRepresentation(representation);
                return IsValid;
            }
            return false;
        }

        /// <returns><see cref="Boolean"/> indicating if this FormElement is set to a value which is making it valid</returns>
        public abstract void LoadFromSavedRepresentation(string representation);

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

            if (!string.IsNullOrWhiteSpace(hintText))
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
    }
}
