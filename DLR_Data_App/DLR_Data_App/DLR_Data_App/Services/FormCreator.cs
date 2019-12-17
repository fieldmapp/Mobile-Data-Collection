using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Services
{
    class FormCreationParams
    {
        public ProjectFormElements Element;
        public Project CurrentProject;
        public Func<string, string, string, Task> DisplayAlertFunc;
        
        public FormCreationParams(ProjectFormElements element, Project currentProject, Func<string, string, string, Task> displayAlertFunc)
        {
            Element = element;
            CurrentProject = currentProject;
            DisplayAlertFunc = displayAlertFunc;
        }
    }
    class FormElement
    {
        public FormElement(Grid grid, ProjectFormElements data)
        {
            Grid = grid;
            Data = data;
            ShouldBeShownExpression = string.IsNullOrWhiteSpace(data.Relevance) ? null : OdkDataExtractor.GetBooleanExpression(data.Relevance);
        }
        
        public Grid Grid { get; }
        public ProjectFormElements Data { get; }
        public event EventHandler ValidContentChange;
        public event EventHandler InvalidContentChange;
        public OdkBooleanExpresion ShouldBeShownExpression { get; }

        public void OnValidContentChange() => ValidContentChange?.Invoke(this, null);
        public void OnInvalidContentChange() => InvalidContentChange?.Invoke(this, null);
    }

    class FormContent
    {
        public FormContent(ContentPage form, IReadOnlyList<FormElement> elements)
        {
            Form = form;
            Elements = elements;
        }

        public ContentPage Form { get; }
        public IReadOnlyList<FormElement> Elements { get; }
    }
    static class FormCreator
    {
        private static Dictionary<string, Func<FormCreationParams, FormElement>> TypeToViewCreator = new Dictionary<string, Func<FormCreationParams, FormElement>>()
        {
            { "inputText", CreateTextInput },
            { "inputSelectOne", CreatePicker },
            { "inputNumeric", CreateNumericInput },
            { "inputLocation", CreateLocationSelector },
            { "inputDate", CreateDateSelector }
        };

        private static Dictionary<string, Func<FormCreationParams, FormElement>> SpecialTypeToViewCreator = new Dictionary<string, Func<FormCreationParams, FormElement>>
        {
            { "propRuler", CreateRuler },
            { "unknown", CreateUnknownChecker }
        };

        private static Grid CreateStandardBaseGrid(FormCreationParams parms)
        {
            var elementNameLabel = new Label { Text = Parser.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages) };
            var grid = new Grid { IsVisible = false };
            grid.Children.Add(elementNameLabel, 0, 0);

            var hintText = Parser.GetCurrentLanguageStringFromJsonList(parms.Element.Hint, parms.CurrentProject.Languages);

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

        private static FormElement CreateUnknownChecker(FormCreationParams parms)
        {
            return new FormElement(new Grid(), parms.Element);
        }

        private static FormElement CreateRuler(FormCreationParams parms)
        {
            return new FormElement(new Grid(), parms.Element);
        }


        private static FormElement CreateDateSelector(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new FormElement(grid, parms.Element);

            //TODO: Save and load dates
            var datePicker = new DatePicker { StyleId = parms.Element.Name };
            datePicker.DateSelected += (a,b) => formElement.OnValidContentChange();
            datePicker.Date = new DateTime(1970,1,1);
            grid.Children.Add(datePicker, 0, 1);
            Grid.SetColumnSpan(datePicker, 2);

            return formElement;
        }

        private static FormElement CreateLocationSelector(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new FormElement(grid, parms.Element);

            var labelLat = new Label { Text = "Latitude" };

            var labelLatData = new Label()
            {
                Text = Sensor.Instance.Gps.Latitude.ToString(CultureInfo.CurrentCulture),
                StyleId = parms.Element.Name + "Lat"
            };

            var labelLong = new Label { Text = "Longitude" };

            var labelLongData = new Label()
            {
                Text = Sensor.Instance.Gps.Longitude.ToString(CultureInfo.CurrentCulture),
                StyleId = parms.Element.Name + "Long"
            };

            var labelMessage = new Label { Text = AppResources.message };

            var labelMessageData = new Label()
            {
                Text = Sensor.Instance.Gps.Message,
                StyleId = parms.Element.Name + "Message"
            };

            var saveButton = new Button { Text = AppResources.save };
            var skipButton = new Button { Text = AppResources.skip };

            var savedLocation = new Label { Text = AppResources.saveddata };
            var savedLocationData = new Label
            {
                Text = "",
                StyleId = parms.Element.Name + "LocationData"
            };

            saveButton.Clicked += (sender, args) => savedLocationData.Text = $"Lat:{labelLongData.Text} Long:{labelLatData.Text}";
            saveButton.Clicked += (a, b) => formElement.OnValidContentChange();

            skipButton.Clicked += (sender, args) => savedLocationData.Text = $"Lat:0 Long:0";
            skipButton.Clicked += (a, b) => formElement.OnValidContentChange();


            grid.Children.Add(labelLat, 0, 1);
            grid.Children.Add(labelLatData, 1, 1);
            grid.Children.Add(labelLong, 0, 2);
            grid.Children.Add(labelLongData, 1, 2);
            grid.Children.Add(labelMessage, 0, 3);
            grid.Children.Add(labelMessageData, 1, 3);
            grid.Children.Add(skipButton, 0, 4);
            grid.Children.Add(saveButton, 1, 4);
            grid.Children.Add(savedLocation, 0, 5);
            grid.Children.Add(savedLocationData, 1, 5);

            return formElement;
        }

        private static FormElement CreateNumericInput(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new FormElement(grid, parms.Element);

            var placeholder = Parser.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);
            if (placeholder == "Unable to parse language from json")
            {
                placeholder = "";
            }

            var entry = new Entry
            {
                Placeholder = placeholder,
                Keyboard = Keyboard.Numeric,
                StyleId = parms.Element.Name
            };
            var range = OdkDataExtractor.GetRangeFromJsonString(parms.Element.Range);

            entry.TextChanged += (a, b) =>
            {
                if (!string.IsNullOrWhiteSpace(b.NewTextValue) && float.TryParse(b.NewTextValue, out var decimalInput) && range.IsValidDecimalInput(decimalInput))
                    formElement.OnValidContentChange();
                else
                    formElement.OnInvalidContentChange();
            };

            grid.Children.Add(entry, 0, 1);
            Grid.SetColumnSpan(entry, 2);

            return formElement;
        }

        private static FormElement CreatePicker(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new FormElement(grid, parms.Element);

            var optionsList = new List<string>();
            var options = Parser.ParseOptionsFromJson(parms.Element.Options);
            var title = Parser.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);
            if (title == "Unable to parse language from json")
            {
                title = AppResources.notitle;
            }
            var currentLanguageCode = Parser.GetCurrentLanguageCodeFromJsonList(parms.CurrentProject.Languages);
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

            picker.SelectedIndexChanged += (a, b) => formElement.OnValidContentChange();

            grid.Children.Add(picker, 0, 1);
            Grid.SetColumnSpan(picker, 2);

            return formElement;
        }

        private static FormElement CreateTextInput(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new FormElement(grid, parms.Element);

            var placeholder = Parser.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);
            if (placeholder == "Unable to parse language from json")
            {
                placeholder = "";
            }

            var entry = new Entry
            {
                Placeholder = placeholder,
                Keyboard = Keyboard.Default,
                StyleId = parms.Element.Name
            };

            entry.TextChanged += (a,b) => formElement.OnValidContentChange();

            grid.Children.Add(entry, 0, 1);
            Grid.SetColumnSpan(entry, 2);

            return formElement;
        }

        public static FormContent GenerateForm(ProjectForm form, Project currentProject, Func<string, string, string, Task> displayAlert)
        {
            var contentPage = new ContentPage();
            var scrollView = new ScrollView();
            var stack = new StackLayout();
            var elements = new List<FormElement>();

            contentPage.Padding = new Thickness(10, 10, 10, 10);

            contentPage.Title = form.Title;
            // walk through list of elements and generate form containing elements
            foreach (var element in form.ElementList)
            {
                IEnumerable<char> findSpecialType(string name) => name.SkipWhile(c => c != '{').Skip(1).TakeWhile(c => c != '}');

                var formCreationParams = new FormCreationParams(element, currentProject, displayAlert);
                FormElement formElement = null;

                if (element.Type == "inputText" && element.Name != null && findSpecialType(element.Name).Any())
                {
                    //Special element
                    var specialElementType = new string(findSpecialType(element.Name).ToArray());
                    if (SpecialTypeToViewCreator.TryGetValue(specialElementType, out var viewCreator))
                    {
                        formElement = viewCreator(formCreationParams);
                    }
                }
                else
                {
                    if (TypeToViewCreator.TryGetValue(element.Type, out var viewCreator))
                    {
                        formElement = viewCreator(formCreationParams);
                    }
                }
                if (formElement != null)
                {
                    elements.Add(formElement);
                    stack.Children.Add(formElement.Grid);
                }
            }

            scrollView.Content = stack;
            contentPage.Content = scrollView;
            return new FormContent(contentPage, elements.AsReadOnly());
        }

    }
}
