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
    struct FormCreationParams
    {
        public Grid Grid;
        public ProjectFormElements Element;
        public Project CurrentProject;
        public EventHandler ContentChanged;

        public FormCreationParams(Grid grid, ProjectFormElements element, Project currentProject, EventHandler contentChanged)
        {
            Grid = grid;
            Element = element;
            CurrentProject = currentProject;
            ContentChanged = contentChanged;
        }
    }
    class FormElement
    {
        public FormElement(Grid grid, ProjectFormElements data, EventHandler contentChanged)
        {
            new FormCreationParams();
            Grid = grid;
            Data = data;
            contentChanged += (a,b) => ContentChanged(this, null);
        }

        public Grid Grid { get; }
        public ProjectFormElements Data { get; }
        public event EventHandler ContentChanged;
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
        private static Dictionary<string, Action<FormCreationParams>> TypeToViewCreator = new Dictionary<string, Action<FormCreationParams>>()
        {
            { "inputText", CreateTextInput },
            { "inputSelectOne", CreatePicker },
            { "inputNumeric", CreateNumericInput },
            { "inputLocation", CreateLocationSelector },
            { "inputDate", CreateDateSelector }
        };

        private static Dictionary<string, Action<FormCreationParams>> SpecialTypeToViewCreator = new Dictionary<string, Action<FormCreationParams>>
        {
            { "propRuler", CreateRuler },
            { "unknown", CreateUnknownChecker }
        };

        private static void CreateUnknownChecker(FormCreationParams parms)
        {
            
        }

        private static void CreateRuler(FormCreationParams parms)
        {
            
        }


        private static void CreateDateSelector(FormCreationParams parms)
        {
            //TODO: Save and load dates
            var datePicker = new DatePicker { StyleId = parms.Element.Name };
            datePicker.DateSelected += (a,b) => parms.ContentChanged(null, null);
            parms.Grid.Children.Add(datePicker, 0, 1);
            Grid.SetColumnSpan(datePicker, 2);
        }

        private static void CreateLocationSelector(FormCreationParams parms)
        {
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

            var savedLocation = new Label { Text = AppResources.saveddata };
            var savedLocationData = new Label
            {
                Text = "",
                StyleId = parms.Element.Name + "LocationData"
            };

            saveButton.Clicked += (sender, args) => savedLocationData.Text = $"Lat:{labelLongData.Text} Long:{labelLatData.Text}";
            saveButton.Clicked += (a,b) => parms.ContentChanged(null, null);

            parms.Grid.Children.Add(labelLat, 0, 1);
            parms.Grid.Children.Add(labelLatData, 1, 1);
            parms.Grid.Children.Add(labelLong, 0, 2);
            parms.Grid.Children.Add(labelLongData, 1, 2);
            parms.Grid.Children.Add(labelMessage, 0, 3);
            parms.Grid.Children.Add(labelMessageData, 1, 3);
            parms.Grid.Children.Add(saveButton, 0, 4);
            Grid.SetColumnSpan(saveButton, 2);
            parms.Grid.Children.Add(savedLocation, 0, 5);
            parms.Grid.Children.Add(savedLocationData, 1, 5);
        }

        private static void CreateNumericInput(FormCreationParams parms)
        {
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

            entry.TextChanged += (a, b) => parms.ContentChanged(null, null);

            parms.Grid.Children.Add(entry, 0, 1);
            Grid.SetColumnSpan(entry, 2);
        }

        private static void CreatePicker(FormCreationParams parms)
        {
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

            picker.SelectedIndexChanged += (a, b) => parms.ContentChanged(null, null);

            parms.Grid.Children.Add(picker, 0, 1);
            Grid.SetColumnSpan(picker, 2);
        }

        private static void CreateTextInput(FormCreationParams parms)
        {
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

            entry.TextChanged += (a,b) => parms.ContentChanged(null, null);

            parms.Grid.Children.Add(entry, 0, 1);
            Grid.SetColumnSpan(entry, 2);
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
                var grid = new Grid();
                EventHandler contentChanged = delegate { };
                var elementNameLabel = new Label { Text = Parser.GetCurrentLanguageStringFromJsonList(element.Label, currentProject.Languages) };
                grid.Children.Add(elementNameLabel, 0, 0);
                
                var hintText = Parser.GetCurrentLanguageStringFromJsonList(element.Hint, currentProject.Languages);

                if (hintText != "Unable to parse language from json" && !string.IsNullOrWhiteSpace(hintText))
                {
                    var helpButton = new Button { Text = AppResources.help };
                    
                    helpButton.Clicked += async (sender, args) => await displayAlert(AppResources.help, hintText, AppResources.okay);
                    grid.Children.Add(helpButton, 1, 0);
                }
                else
                {
                    Grid.SetColumnSpan(elementNameLabel, 2);
                }

                IEnumerable<char> findSpecialType(string name) => name.SkipWhile(c => c != '{').Skip(1).TakeWhile(c => c != '}');

                if (element.Type == "inputText" && element.Name != null && findSpecialType(element.Name).Any())
                {
                    //Special element
                    var specialElementType = new string(findSpecialType(element.Name).ToArray());
                    if (SpecialTypeToViewCreator.TryGetValue(specialElementType, out var viewCreator))
                    {
                        viewCreator(new FormCreationParams(grid, element, currentProject, contentChanged));
                    }
                }
                else
                {
                    if (TypeToViewCreator.TryGetValue(element.Type, out var viewCreator))
                    {
                        viewCreator(new FormCreationParams(grid, element, currentProject, contentChanged));
                    }
                }
                elements.Add(new FormElement(grid, element, contentChanged));
                stack.Children.Add(grid);
            }

            scrollView.Content = stack;
            contentPage.Content = scrollView;
            return new FormContent(contentPage, elements.AsReadOnly());
        }

    }
}
