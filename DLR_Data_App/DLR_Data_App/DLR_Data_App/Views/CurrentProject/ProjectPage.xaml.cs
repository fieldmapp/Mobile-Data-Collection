using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using DLR_Data_App.Services.Sensors;
using DLR_Data_App.ViewModels.CurrentProject;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.CurrentProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectPage
    {
        public List<string> ElementNameList = new List<string>();
        public List<string> ElementValueList = new List<string>();
        public List<object> ElementList = new List<object>();

        private readonly ProjectViewModel _viewModel = new ProjectViewModel();
        private Project _workingProject = Database.GetCurrentProject();
        private List<ContentPage> _pages;

        private readonly Sensor _sensor = new Sensor();

        public ProjectPage()
        {
            InitializeComponent();

            if (_pages != null)
            {
                WalkElements(false);
            }

            _sensor.Gps.StatusChanged += OnGpsChange;

            BindingContext = _viewModel;
        }

        /**
         * Generating forms from parsed information
         * @param form Elements that should be generated
         */
        public ContentPage GenerateForm(ProjectForm form)
        {
            var contentPage = new ContentPage();
            var scrollView = new ScrollView();
            var stack = new StackLayout();

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
                    Text = Parser.LanguageJson(element.Label, _workingProject.Languages)
                };
                //stack.Children.Add(label);
                grid.Children.Add(label, 0, 0);

                //------------------------
                // Special commands

                // Display help
                var helpButton = new Button()
                {
                    Text = AppResources.help
                };

                var hintText = Parser.LanguageJson(element.Hint, _workingProject.Languages);
                if (hintText == "Unable to parse language from json")
                {
                    hintText = AppResources.nohint;
                }

                helpButton.Clicked += async (sender, args) => await DisplayAlert(AppResources.help, hintText, AppResources.okay);
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

                //-------------------------
                // Normal fields
                switch (element.Type)
                {
                    // input text
                    case "inputText":
                        {
                            var placeholder = Parser.LanguageJson(element.Label, _workingProject.Languages);
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
                            ElementList.Add(entry);

                            grid.Children.Add(entry, 0, 1);
                            Grid.SetColumnSpan(entry, 2);
                            break;
                        }

                    // Selecting one item of list
                    case "inputSelectOne":
                        {
                            var optionsList = new List<string>();
                            var options = Parser.ParseOptionsFromJson(element.Options);
                            var title = Parser.LanguageJson(element.Label, _workingProject.Languages);
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
                            ElementList.Add(picker);

                            grid.Children.Add(picker, 0, 1);
                            Grid.SetColumnSpan(picker, 2);
                            break;
                        }

                    // Show an entry with only numeric input
                    // As a walk around for an existing Samsung keyboard bug a normal keyboard layout is used
                    case "inputNumeric":
                        {
                            var placeholder = Parser.LanguageJson(element.Label, _workingProject.Languages);
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
                            ElementList.Add(entry);

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
                                Text = _sensor.Gps.Latitude.ToString(CultureInfo.CurrentCulture),
                                StyleId = element.Name + "Lat"
                            };
                            ElementList.Add(labelLatData);

                            var labelLong = new Label()
                            {
                                Text = "Longitude"
                            };

                            var labelLongData = new Label()
                            {
                                Text = _sensor.Gps.Longitude.ToString(CultureInfo.CurrentCulture),
                                StyleId = element.Name + "Long"
                            };
                            ElementList.Add(labelLongData);

                            var labelMessage = new Label()
                            {
                                Text = AppResources.message
                            };

                            var labelMessageData = new Label()
                            {
                                Text = _sensor.Gps.Message,
                                StyleId = element.Name + "Message"
                            };
                            ElementList.Add(labelMessageData);

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
            return contentPage;
        }

        /**
         * Override hardware back button on Android devices to return to project list
         */
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();
                if (Application.Current.MainPage is MainPage mainPage)
                    await mainPage.NavigateFromMenu(MenuItemType.Projects);
            });

            return true;
        }

        /**
         * Update shown gps data
         */
        private void OnGpsChange(object sender, GpsEventArgs e)
        {
            foreach (var element in ElementList)
            {
                if (element is Label label)
                {
                    if (label.StyleId.Contains("Lat"))
                    {
                        label.Text = e.Latitude.ToString(CultureInfo.CurrentCulture);
                    }

                    if (label.StyleId.Contains("Long"))
                    {
                        label.Text = e.Longitude.ToString(CultureInfo.CurrentCulture);
                    }

                    if (label.StyleId.Contains("Message"))
                    {
                        label.Text = e.Message.ToString(CultureInfo.CurrentCulture);
                    }
                }
            }
        }

        /**
         * Refresh view
         */
        protected override void OnAppearing()
        {
            foreach (var page in UpdateView())
            {
                Children.Add(page);
            }

            if (_pages == null || _pages.Count == 0)
            {
                (Application.Current as App).CurrentPage.DisplayAlert(AppResources.warning, AppResources.noactiveproject, AppResources.okay);
            }

            base.OnAppearing();
        }

        /**
         * Update view
         */
        public List<ContentPage> UpdateView()
        {
            // Get current project
            _workingProject = Database.GetCurrentProject();
            _pages = new List<ContentPage>();

            // Check if current project is set
            if (_workingProject == null)
            {
                Title = AppResources.currentproject;
            }
            else
            {
                var translatedProject = Helpers.TranslateProjectDetails(_workingProject);
                Title = translatedProject.Title;

                foreach (var projectForm in _workingProject.FormList)
                {
                    var content = GenerateForm(projectForm);
                    _pages.Add(content);
                }
            }

            return _pages;
        }

        /**
         * Check if active project exists
         */
        private async Task<bool> CheckActiveProject()
        {
            if (_pages == null || _pages.Count == 0)
            {
                await DisplayAlert(AppResources.warning, AppResources.noactiveproject, AppResources.okay);
                return false;
            }
            else
            {
                return true;
            }
        }

        /**
         * Navigate to edit page
         */
        private async void EditClicked(object sender, EventArgs e)
        {
            if (await CheckActiveProject())
            {
                await Navigation.PushAsync(new EditDataPage());
            }
        }

        /**
         * Save data
         */
        private async void SaveClicked(object sender, EventArgs e)
        {
            if (await CheckActiveProject())
            {
                WalkElements(false);

                var tableName = Parser.LanguageJsonStandard(_workingProject.Title, _workingProject.Languages) + "_" + _workingProject.Id;
                var status = Database.InsertCustomValues(tableName, ElementNameList, ElementValueList);

                string message;
                if (status)
                {
                    message = AppResources.successful;
                    WalkElements(true);
                }
                else
                {
                    message = AppResources.failed;
                }

                await DisplayAlert(AppResources.save, message, AppResources.okay);
            }

        }

        /**
         * Walk through all elements, get values and store them in name list and value list
         *
         * @param bool if true resets content of element
         */
        private void WalkElements(bool clean)
        {
            var currentPosition = "";

            // walk trough all pages
            foreach (var page in _pages)
            {
                // walk trough all elements in content page
                // https://forums.xamarin.com/discussion/21032/how-to-get-the-children-of-a-contentpage-or-in-a-view
                foreach (var child in page.Content.LogicalChildren.Where(x => true))
                {
                    // https://forums.xamarin.com/discussion/72424/how-to-add-entry-control-dynamically-and-retrieve-its-value
                    if (!(child is StackLayout stack)) continue;

                    foreach (var view in stack.Children)
                    {
                        if (!(view is Grid grid)) continue;

                        foreach (var element in grid.Children)
                        {
                            // Store information from element in list, access element by different type
                            if (element is Entry entry)
                            {
                                if (clean)
                                {
                                    entry.Text = "";
                                }
                                else
                                {
                                    ElementNameList.Add(entry.StyleId);
                                    // check if entry is NULL, if true set default value
                                    ElementValueList.Add(entry.Text ?? "0");
                                }
                            }

                            if (element is Picker picker)
                            {
                                if (clean)
                                {
                                    picker.SelectedIndex = 0;
                                }
                                else
                                {
                                    ElementNameList.Add(picker.StyleId);
                                    // check if entry is NULL, if true set default value
                                    ElementValueList.Add(picker.SelectedItem as string ?? "0");
                                }
                            }

                            if (element is Label label)
                            {
                                if (label.StyleId != null)
                                {
                                    if (label.StyleId.Contains("Lat"))
                                    {
                                        currentPosition += label.Text;
                                    }
                                    else if (label.StyleId.Contains("Long"))
                                    {
                                        var elementName = label.StyleId.Substring(0, label.StyleId.Length - 4);
                                        currentPosition += " " + label.Text;
                                        ElementNameList.Add(elementName);
                                        ElementValueList.Add(currentPosition);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}