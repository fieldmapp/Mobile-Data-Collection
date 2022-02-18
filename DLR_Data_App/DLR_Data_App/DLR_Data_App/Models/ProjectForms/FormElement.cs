using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    /// <summary>
    /// Class representing every part of a displayed element in a form.
    /// </summary>
    abstract class FormElement
    {
        const string ThisOdkElement = "thisodkelement";
        Func<string, string, string, Task> DisplayAlertFunc;
        Project Project;
        public FormElement(Grid grid, ProjectFormElements data, string type, Func<string, string, string, Task> displayAlertFunc, Project project)
        {
            Project = project;
            DisplayAlertFunc = displayAlertFunc;
            Grid = grid;
            Frame = new Frame { CornerRadius = 10, BorderColor = Color.DarkSeaGreen, Content = Grid, IsVisible = false, BackgroundColor=Color.Transparent };
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

        public bool IsVisible
        {
            get => Frame.IsVisible;
            set
            {
                Frame.IsVisible = value;

                // Hack: The following Task.Run tries to prevent following issue:
                // When items become visible and should expand beyond the height of the parent StackLayout, they will only expand to the height.
                // If the hack does not work, the user has to e.g. rotate the screen or try making other elements visible
                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    Device.BeginInvokeOnMainThread(() => (Frame.Parent as StackLayout).InvalidateSize());
                });
            }
        }

        public Frame Frame { get; }
        public Grid Grid { get; }
        /// <summary>
        /// Is true if the user requested to skip the question. This is only requestable if the question is not required. Otherwise the value will always be false.
        /// </summary>
        public bool IsSkipped { get; set; }
        public ProjectFormElements Data { get; }
        public event EventHandler ValidContentChange;
        public event EventHandler InvalidContentChange;
        public OdkBooleanExpresion ShouldBeShownExpression { get; }
        public OdkBooleanExpresion ConstraintExpression { get; }
        public bool IsValid => IsSkipped || (IsConstraintFulfilled && IsValidElementSpecific);
        private bool IsConstraintFulfilled => ConstraintExpression?.Evaluate(new Dictionary<string, string> { { ThisOdkElement, GetRepresentationValue() } }) ?? true;
        protected abstract bool IsValidElementSpecific { get; }
        public readonly string Type;

        /// <summary>
        /// Should be called every time the user did some input on this FormElement. Handles calls to <see cref="ValidContentChange"/> and <see cref="InvalidContentChange"/>.
        /// Also displays the set ODK error message if needed.
        /// </summary>
        public void OnContentChange()
        {
            if (IsValid)
                ValidContentChange?.Invoke(this, null);
            else
            {
                InvalidContentChange?.Invoke(this, null);

                var localizedErrorHint = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(Data.InvalidText, Project.Languages);
                if (!string.IsNullOrWhiteSpace(localizedErrorHint))
                    DisplayAlertFunc(AppResources.error, localizedErrorHint, AppResources.ok);
            }
        }

        /// <summary>
        /// Sets the grids content to the default value
        /// </summary>
        public void Reset()
        {
            IsSkipped = false;
            OnReset();
        }

        protected abstract void OnReset();

        /// <summary>
        /// Retrieves a representation for this element
        /// </summary>
        /// <returns>Pair containing 1. the name and 2. the content of the element.</returns>
        public KeyValuePair<string, string> GetRepresentation()
        {
            return new KeyValuePair<string, string>(Data.Name, GetRepresentationValue());
        }

        /// <summary>
        /// Returns the value in a string representation. Should never return null.
        /// </summary>
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
            var grid = new Grid();
            grid.Children.Add(elementNameLabel, 0, 0);

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
