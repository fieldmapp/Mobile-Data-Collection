using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services.Sensors;
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

        private static readonly DateTime EmptyDate = new DateTime(1970, 1, 1);
        private static readonly long EmptyDateTicks = EmptyDate.Ticks;
        private IEnumerable<View> Children => Grid.Children;

        public void Reset()
        {
            foreach (var child in Children)
            {
                if (child is Entry entry)
                    entry.Text = "";
                else if (child is Picker picker)
                    picker.SelectedIndex = -1;
                else if (child is DatePicker datePicker)
                    datePicker.Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                else if (child is Label label && label.StyleId != null && label.StyleId.EndsWith("LocationData"))
                    label.Text = string.Empty;
            }
        }

        public void OnGpsChange(GpsEventArgs e)
        {
            foreach (var label in Children.OfType<Label>().Where(l => l.StyleId != null))
            {
                if (label.StyleId.Contains("Lat"))
                    Device.BeginInvokeOnMainThread(() => label.Text = e.Latitude.ToString(CultureInfo.CurrentCulture));

                if (label.StyleId.Contains("Long"))
                    Device.BeginInvokeOnMainThread(() => label.Text = e.Longitude.ToString(CultureInfo.CurrentCulture));

                if (label.StyleId.Contains("Message"))
                    Device.BeginInvokeOnMainThread(() => label.Text = e.Message.ToString(CultureInfo.CurrentCulture));
            }
        }

        public KeyValuePair<string, string> GetRepresentation()
        {
            foreach (var child in Children)
            {
                if (child is Entry entry)
                {
                    return new KeyValuePair<string, string>(entry.StyleId, entry.Text ?? string.Empty);
                }
                else if (child is Picker picker)
                {
                    //TODO: IndexOf does not respect actual backing values from odk, 
                    //will result in problems if a picker is not using integer backing values starting from 0
                    return new KeyValuePair<string, string>(picker.StyleId, picker.Items.IndexOf(picker.SelectedItem as string ?? "").ToString());
                }
                else if (child is Label label && label.StyleId != null && label.StyleId.EndsWith("LocationData"))
                {
                    return new KeyValuePair<string, string>(label.StyleId.Substring(0, label.StyleId.Length - "LocationData".Length), label.Text);
                }
                else if (child is DatePicker datePicker)
                {
                    return new KeyValuePair<string, string>(datePicker.StyleId, Grid.IsVisible ? datePicker.Date.Ticks.ToString() : EmptyDateTicks.ToString());
                }
            }
            return new KeyValuePair<string, string>(App.RandomProvider.Next().ToString(), string.Empty);
            //throw new NotImplementedException("This FormElement has no identifiable type and thus no representation");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectData"></param>
        /// <returns>Boolean indicating if this FormElement set something non initial to one of its views</returns>
        public bool LoadContentFromProjectData(Dictionary<string,string> projectData)
        {
            foreach (var child in Children)
            {
                if (child is Entry entry)
                {
                    var savedData = projectData[entry.StyleId];
                    entry.Text = savedData;
                    return !string.IsNullOrWhiteSpace(savedData);
                }
                else if (child is Picker picker)
                {
                    var savedData = Convert.ToInt32(projectData[picker.StyleId]);
                    picker.SelectedIndex = savedData;
                    return savedData >= 0;
                }
                else if (child is Label label && label.StyleId != null && label.StyleId.EndsWith("LocationData"))
                {
                    var savedData = projectData[label.StyleId.Substring(0, label.StyleId.Length - "LocationData".Length)];
                    label.Text = savedData;
                    return !string.IsNullOrEmpty(savedData);
                }
                else if (child is DatePicker datePicker)
                {
                    var savedData = projectData[datePicker.StyleId];
                    if (long.TryParse(savedData, out var ticks))
                    {
                        var newDate = new DateTime(ticks);
                        datePicker.Date = newDate;
                        //TODO: Replace future check by using odk constraints
                        return EmptyDateTicks != ticks && newDate <= DateTime.UtcNow;
                    }
                    return false;
                }
            }
            return false;
        }
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
            { "unknown", CreateUnknownChecker },
            { "compass", CreateCompass }
        };

        private static FormElement CreateCompass(FormCreationParams arg)
        {
            //TODO: save and load (conflicts with location?)
            var grid = CreateStandardBaseGrid(arg);
            var formElement = new FormElement(grid, arg.Element);

            var currentCompassLabel = new Label { Text = AppResources.compass };
            var currentCompassDataLabel = new Label();
            Sensor.Instance.Compass.ReadingChanged += (_,eventArgs) => currentCompassDataLabel.Text = ((int)eventArgs.Reading.HeadingMagneticNorth).ToString();

            var saveButton = new Button { Text = AppResources.save };

            var savedCompassLabel = new Label { Text = AppResources.saveddata };
            var savedCompassDataLabel = new Label();

            saveButton.Clicked += (_, b) => savedCompassDataLabel.Text = currentCompassDataLabel.Text;

            grid.Children.Add(currentCompassLabel, 0, 1);
            grid.Children.Add(currentCompassDataLabel, 1, 1);
            grid.Children.Add(saveButton, 0, 2);
            Grid.SetColumnSpan(saveButton, 2);
            grid.Children.Add(savedCompassLabel, 0, 3);
            grid.Children.Add(savedCompassDataLabel, 1, 3);

            return formElement;
        }

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
            
            var datePicker = new DatePicker { StyleId = parms.Element.Name };
            
            datePicker.Unfocused += (a, b) =>
            {
                //TODO: Replace by using odk constraints
                if (datePicker.Date > DateTime.UtcNow)
                {
                    formElement.OnInvalidContentChange();
                    parms.DisplayAlertFunc(AppResources.error, AppResources.selectedDateIsInFuture, AppResources.ok);
                }
                else
                    formElement.OnValidContentChange();
            };
            datePicker.Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
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

            entry.TextChanged += (a, b) =>
            {
                if (string.IsNullOrEmpty(b.NewTextValue))
                    formElement.OnInvalidContentChange();
                else
                    formElement.OnValidContentChange();
            };

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
                IEnumerable<char> findSpecialType(string name) => name.SkipWhileIncluding(c => c != '{').TakeWhile(c => c != '}');

                var formCreationParams = new FormCreationParams(element, currentProject, displayAlert);
                FormElement formElement = null;

                bool specialType = false;
                if (element.Label != null)
                {
                    var translatedLabel = Parser.GetCurrentLanguageStringFromJsonList(element.Label, currentProject.Languages);
                    if (translatedLabel != null && findSpecialType(translatedLabel).Any())
                    {
                        //Special element
                        var specialElementType = new string(findSpecialType(translatedLabel).ToArray());
                        if (SpecialTypeToViewCreator.TryGetValue(specialElementType, out var viewCreator))
                        {
                            specialType = true;
                            formElement = viewCreator(formCreationParams);
                        }
                    }
                }
                
                if (!specialType)
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
