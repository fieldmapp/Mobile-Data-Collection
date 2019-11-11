using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Services
{
    class FormContent
    {
        public FormContent(ContentPage form, List<View> elementList)
        {
            Form = form;
            ElementList = elementList;
        }

        public ContentPage Form { get; }
        public List<View> ElementList { get; }
    }
    static class FormFactory
    {
        public static FormContent GenerateForm(ProjectForm form, Project currentProject, Func<string, string, string, Task> displayAlert, Sensor sensor)
        {
            var contentPage = new ContentPage();
            var scrollView = new ScrollView();
            var stack = new StackLayout();
            List<View> elementList = new List<View>();

            contentPage.Padding = new Thickness(10, 10, 10, 10);

            // walk through list of elements and generate form containing elements
            foreach (var element in form.ElementList)
            {
                contentPage.Title = form.Title;

                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                // show name of element
                var label = new Label
                {
                    Text = Parser.LanguageJson(element.Label, currentProject.Languages)
                };
                //stack.Children.Add(label);
                grid.Children.Add(label, 0, 0);

                //------------------------
                // Special commands

                // Display help

                var hintText = Parser.LanguageJson(element.Hint, currentProject.Languages);
                if (hintText != "Unable to parse language from json")
                {
                    var helpButton = new Button()
                    {
                        Text = AppResources.help
                    };



                    helpButton.Clicked += async (sender, args) => await displayAlert(AppResources.help, hintText, AppResources.okay);
                    grid.Children.Add(helpButton, 1, 0);

                    // Display a ruler on the side of the screen
                    if (element.Type == "inputText" && element.Name.Contains("propRuler"))
                    {
                        continue;
                    }

                    // Display a checkbox with name "unknown"
                    if (element.Type == "inputText" && element.Name.Contains("unknown"))
                    {
                        continue;
                    }
                }

                //-------------------------
                // Normal fields
                switch (element.Type)
                {
                    // input text
                    case "inputText":
                        {
                            var placeholder = Parser.LanguageJson(element.Label, currentProject.Languages);
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
                            elementList.Add(entry);

                            grid.Children.Add(entry, 0, 1);
                            Grid.SetColumnSpan(entry, 2);
                            break;
                        }

                    // Selecting one item of list
                    case "inputSelectOne":
                        {
                            var optionsList = new List<string>();
                            var options = Parser.ParseOptionsFromJson(element.Options);
                            var title = Parser.LanguageJson(element.Label, currentProject.Languages);
                            if (title == "Unable to parse language from json")
                            {
                                title = AppResources.notitle;
                            }

                            foreach (var option in options)
                            {
                                option.Text.TryGetValue("0", out var value);
                                optionsList.Add(value);
                            }
                            var picker = new Picker
                            {
                                Title = title,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                                StyleId = element.Name,
                                ItemsSource = optionsList
                            };
                            elementList.Add(picker);

                            grid.Children.Add(picker, 0, 1);
                            Grid.SetColumnSpan(picker, 2);
                            break;
                        }

                    // Show an entry with only numeric input
                    // As a walk around for an existing Samsung keyboard bug a normal keyboard layout is used
                    case "inputNumeric":
                        {
                            var placeholder = Parser.LanguageJson(element.Label, currentProject.Languages);
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
                            elementList.Add(entry);

                            grid.Children.Add(entry, 0, 1);
                            Grid.SetColumnSpan(entry, 2);
                            break;
                        }

                    // Show current position
                    case "inputLocation":
                        {
                            var labelLat = new Label()
                            {
                                Text = "Latitude"
                            };

                            var labelLatData = new Label()
                            {
                                StyleId = element.Name + "Lat"
                            };
                            elementList.Add(labelLatData);

                            var labelLong = new Label()
                            {
                                Text = "Longitude"
                            };

                            var labelLongData = new Label()
                            {
                                StyleId = element.Name + "Long"
                            };
                            elementList.Add(labelLongData);

                            var labelMessage = new Label()
                            {
                                Text = AppResources.message
                            };

                            var labelMessageData = new Label()
                            {
                                Text = sensor.Gps.Message,
                                StyleId = element.Name + "Message"
                            };
                            elementList.Add(labelMessageData);

                            grid.Children.Add(labelLat, 0, 1);
                            grid.Children.Add(labelLatData, 1, 1);
                            grid.Children.Add(labelLong, 0, 2);
                            grid.Children.Add(labelLongData, 1, 2);
                            grid.Children.Add(labelMessage, 0, 3);
                            grid.Children.Add(labelMessageData, 1, 3);
                            break;
                        }
                }
                stack.Children.Add(grid);
            }

            scrollView.Content = stack;
            contentPage.Content = scrollView;
            return new FormContent(contentPage, elementList);
        }

    }
}
