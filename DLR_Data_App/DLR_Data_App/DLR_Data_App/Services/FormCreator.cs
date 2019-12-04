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
    class FormContent
    {
        public FormContent(ContentPage form)
        {
            Form = form;
        }

        public ContentPage Form { get; }
    }
    static class FormCreator
    {
        private static Dictionary<string, Action<Grid, ProjectFormElements, Project>> TypeToViewCreator = new Dictionary<string, Action<Grid, ProjectFormElements, Project>>()
        {
            { "inputText", CreateTextInput },
            { "inputSelectOne", CreatePicker },
            { "inputNumeric", CreateNumericInput },
            { "inputLocation", CreateLocationSelector },
            { "inputDate", CreateDateSelector }
        };

        private static Dictionary<string, Action<Grid, ProjectFormElements, Project>> SpecialTypeToViewCreator = new Dictionary<string, Action<Grid, ProjectFormElements, Project>>
        {
            { "propRuler", CreateRuler },
            { "unknown", CreateUnknownChecker }
        };

        private static void CreateUnknownChecker(Grid grid, ProjectFormElements element, Project currentProject)
        {
            
        }

        private static void CreateRuler(Grid grid, ProjectFormElements element, Project currentProject)
        {
            
        }


        private static void CreateDateSelector(Grid grid, ProjectFormElements element, Project currentProject)
        {
            var datePicker = new DatePicker { StyleId = element.Name };
            grid.Children.Add(datePicker, 0, 1);
            Grid.SetColumnSpan(datePicker, 2);
        }

        private static void CreateLocationSelector(Grid grid, ProjectFormElements element, Project currentProject)
        {
            var labelLat = new Label { Text = "Latitude" };

            var labelLatData = new Label()
            {
                Text = Sensor.Instance.Gps.Latitude.ToString(CultureInfo.CurrentCulture),
                StyleId = element.Name + "Lat"
            };

            var labelLong = new Label { Text = "Longitude" };

            var labelLongData = new Label()
            {
                Text = Sensor.Instance.Gps.Longitude.ToString(CultureInfo.CurrentCulture),
                StyleId = element.Name + "Long"
            };

            var labelMessage = new Label { Text = AppResources.message };

            var labelMessageData = new Label()
            {
                Text = Sensor.Instance.Gps.Message,
                StyleId = element.Name + "Message"
            };

            var saveButton = new Button { Text = AppResources.save };

            var savedLocation = new Label { Text = AppResources.saveddata };
            var savedLocationData = new Label
            {
                Text = "",
                StyleId = element.Name + "LocationData"
            };

            saveButton.Clicked += (sender, args) => savedLocationData.Text = $"Lat:{labelLongData.Text} Long:{labelLatData.Text}";

            grid.Children.Add(labelLat, 0, 1);
            grid.Children.Add(labelLatData, 1, 1);
            grid.Children.Add(labelLong, 0, 2);
            grid.Children.Add(labelLongData, 1, 2);
            grid.Children.Add(labelMessage, 0, 3);
            grid.Children.Add(labelMessageData, 1, 3);
            grid.Children.Add(saveButton, 0, 4);
            Grid.SetColumnSpan(saveButton, 2);
            grid.Children.Add(savedLocation, 0, 5);
            grid.Children.Add(savedLocationData, 1, 5);
        }

        private static void CreateNumericInput(Grid grid, ProjectFormElements element, Project currentProject)
        {
            var placeholder = Parser.GetCurrentLanguageStringFromJsonList(element.Label, currentProject.Languages);
            if (placeholder == "Unable to parse language from json")
            {
                placeholder = "";
            }

            var entry = new Entry
            {
                Placeholder = placeholder,
                Keyboard = Keyboard.Numeric,
                StyleId = element.Name
            };

            grid.Children.Add(entry, 0, 1);
            Grid.SetColumnSpan(entry, 2);
        }

        private static void CreatePicker(Grid grid, ProjectFormElements element, Project currentProject)
        {
            var optionsList = new List<string>();
            var options = Parser.ParseOptionsFromJson(element.Options);
            var title = Parser.GetCurrentLanguageStringFromJsonList(element.Label, currentProject.Languages);
            if (title == "Unable to parse language from json")
            {
                title = AppResources.notitle;
            }
            var currentLanguageCode = Parser.GetCurrentLanguageCodeFromJsonList(currentProject.Languages);
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
                StyleId = element.Name,
                ItemsSource = optionsList
            };

            grid.Children.Add(picker, 0, 1);
            Grid.SetColumnSpan(picker, 2);
        }

        private static void CreateTextInput(Grid grid, ProjectFormElements element, Project currentProject)
        {
            var placeholder = Parser.GetCurrentLanguageStringFromJsonList(element.Label, currentProject.Languages);
            if (placeholder == "Unable to parse language from json")
            {
                placeholder = "";
            }

            var entry = new Entry
            {
                Placeholder = placeholder,
                Keyboard = Keyboard.Default,
                StyleId = element.Name
            };

            grid.Children.Add(entry, 0, 1);
            Grid.SetColumnSpan(entry, 2);
        }

        public static FormContent GenerateForm(ProjectForm form, Project currentProject, Func<string, string, string, Task> displayAlert)
        {
            var contentPage = new ContentPage();
            var scrollView = new ScrollView();
            var stack = new StackLayout();

            contentPage.Padding = new Thickness(10, 10, 10, 10);

            contentPage.Title = form.Title;

            // walk through list of elements and generate form containing elements
            foreach (var element in form.ElementList)
            {
                var grid = new Grid();
                
                var elementNameLabel = new Label { Text = Parser.GetCurrentLanguageStringFromJsonList(element.Label, currentProject.Languages) };
                grid.Children.Add(elementNameLabel, 0, 0);
                
                var hintText = Parser.GetCurrentLanguageStringFromJsonList(element.Hint, currentProject.Languages);

                if (hintText != "Unable to parse language from json")
                {
                    var helpButton = new Button { Text = AppResources.help };
                    
                    helpButton.Clicked += async (sender, args) => await displayAlert(AppResources.help, hintText, AppResources.okay);
                    grid.Children.Add(helpButton, 1, 0);
                }

                IEnumerable<char> findSpecialType(string name) => name.SkipWhile(c => c != '{').Skip(1).TakeWhile(c => c != '}');

                if (element.Type == "inputText" && element.Name != null && findSpecialType(element.Name).Count() > 0)
                {
                    //Special element
                    var specialElementType = new string(findSpecialType(element.Name).ToArray());
                    if (SpecialTypeToViewCreator.TryGetValue(specialElementType, out var viewCreator))
                    {
                        viewCreator(grid, element, currentProject);
                    }
                }
                else
                {
                    if (TypeToViewCreator.TryGetValue(element.Type, out var viewCreator))
                    {
                        viewCreator(grid, element, currentProject);
                    }
                }

                stack.Children.Add(grid);
            }

            scrollView.Content = stack;
            contentPage.Content = scrollView;
            return new FormContent(contentPage);
        }

    }
}
