using DLR_Data_App.Controls;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services.Sensors;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Services
{
    /// <summary>
    /// Class containing the parameters passed to a view creator.
    /// </summary>
    class FormCreationParams
    {
        public ProjectFormElements Element;
        public Project CurrentProject;
        public Func<string, string, string, Task> DisplayAlertFunc;
        public string Type;

        public FormCreationParams(string type, ProjectFormElements element, Project currentProject, Func<string, string, string, Task> displayAlertFunc)
        {
            Type = type;
            Element = element;
            CurrentProject = currentProject;
            DisplayAlertFunc = displayAlertFunc;
        }
    }

    /// <summary>
    /// Class representing every part of a displayed element in a form.
    /// </summary>
    class FormElement
    {
        public FormElement(Grid grid, ProjectFormElements data, string type)
        {
            Grid = grid;
            Data = data;
            ShouldBeShownExpression = string.IsNullOrWhiteSpace(data.Relevance) ? null : OdkDataExtractor.GetBooleanExpression(data.Relevance);
            Type = type;
        }

        public Grid Grid { get; }
        public ProjectFormElements Data { get; }
        public event EventHandler ValidContentChange;
        public event EventHandler InvalidContentChange;
        public OdkBooleanExpresion ShouldBeShownExpression { get; }
        public readonly string Type;

        public void OnValidContentChange() => ValidContentChange?.Invoke(this, null);
        public void OnInvalidContentChange() => InvalidContentChange?.Invoke(this, null);

        private static readonly DateTime EmptyDate = new DateTime(1970, 1, 1);
        private static readonly long EmptyDateTicks = EmptyDate.Ticks;
        private IEnumerable<View> Children => Grid.Children;

        /// <summary>
        /// Sets the grids content to the default value
        /// </summary>
        public void Reset()
        {
            foreach (var child in Children)
            {
                if (child is Entry entry)
                    entry.Text = string.Empty;
                else if (child is Picker picker)
                    picker.SelectedIndex = -1;
                else if (child is DatePicker datePicker)
                    datePicker.Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                else if (child is Label label && label.StyleId != null)
                    label.Text = string.Empty;
                else if (child is TimePicker timePicker)
                    timePicker.Time = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Retrieves a representation for this element
        /// </summary>
        /// <returns>Pair containing 1. the name and 2. the content of the element.</returns>
        public KeyValuePair<string, string> GetRepresentation()
        {
            switch (Type)
            {
                case "inputText":
                case "inputNumeric":
                    var textEntry = Children.OfType<Entry>().First();
                    return new KeyValuePair<string, string>(textEntry.StyleId, textEntry.Text ?? string.Empty);
                case "inputSelectOne":
                    var picker = Children.OfType<Picker>().First();
                    return new KeyValuePair<string, string>(picker.StyleId, picker.SelectedIndex.ToString());
                case "inputDate":
                    if (!Grid.IsVisible)
                        return new KeyValuePair<string, string>(Data.Name, EmptyDateTicks.ToString());
                    
                    long ticks = 0;
                    var datePicker = Children.OfType<DatePicker>().FirstOrDefault();
                    ticks += datePicker?.Date.Ticks ?? 0;
                    var timePicker = Children.OfType<TimePicker>().FirstOrDefault();
                    ticks += timePicker?.Time.Ticks ?? 0;
                    
                    return new KeyValuePair<string, string>(datePicker.StyleId, ticks.ToString());
                case "inputLocation":
                case "compass":
                    var dataLabel = Children.OfType<Label>().Where(l => l.StyleId != null).First();
                    return new KeyValuePair<string, string>(dataLabel.StyleId, dataLabel.Text);
                case "inputMedia":
                    var mediaHolder = Children.OfType<DataHolder>().First();
                    return new KeyValuePair<string, string>(mediaHolder.StyleId, mediaHolder.Data);
                default:
                    //throw new NotImplementedException("This FormElement has no identifiable type and thus no representation");
                    return new KeyValuePair<string, string>(App.RandomProvider.Next().ToString(), string.Empty);
            }
        }

        /// <summary>
        /// Sets grids content based on the given projetData
        /// </summary>
        /// <param name="projectData">Dictionary matching an elements name and its content</param>
        /// <returns><see cref="Boolean"/> indicating if this FormElement is set to a value which is making it valid</returns>
        public bool LoadContentFromProjectData(Dictionary<string, string> projectData)
        {
            switch (Type)
            {
                case "inputText":
                case "inputNumeric":
                    var textEntry = Children.OfType<Entry>().First();
                    var savedText = projectData[textEntry.StyleId];
                    textEntry.Text = savedText;
                    return !string.IsNullOrEmpty(savedText);
                case "inputSelectOne":
                    var picker = Children.OfType<Picker>().First();
                    var savedIndex = Convert.ToInt32(projectData[picker.StyleId]);
                    picker.SelectedIndex = savedIndex;
                    return savedIndex >= 0;
                case "inputDate":
                    var datePicker = Children.OfType<DatePicker>().FirstOrDefault();
                    var timePicker = Children.OfType<TimePicker>().FirstOrDefault();
                    var savedData = projectData[Data.Name];
                    if (long.TryParse(savedData, out var ticks))
                    {
                        var newDate = new DateTime(ticks);
                        
                        if (datePicker != null)
                            datePicker.Date = newDate - newDate.TimeOfDay;
                        if (timePicker != null)
                            timePicker.Time = newDate.TimeOfDay;

                        //TODO: Replace future check by using odk constraints
                        return (datePicker == null || (EmptyDateTicks != datePicker?.Date.Ticks && newDate <= DateTime.UtcNow))
                            && (timePicker == null || timePicker.Time != TimeSpan.Zero);
                    }
                    return false;
                case "inputLocation":
                case "compass":
                    var dataLabel = Children.OfType<Label>().Where(l => l.StyleId != null).First();
                    savedText = projectData[dataLabel.StyleId];
                    dataLabel.Text = savedText;
                    return !string.IsNullOrEmpty(savedText);
                case "inputMedia":
                    var dataHolder = Children.OfType<DataHolder>().First();
                    savedText = projectData[dataHolder.StyleId];
                    dataHolder.Data = savedText;
                    return !string.IsNullOrEmpty(savedText);
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Class containing a single form page and its elements
    /// </summary>
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

    /// <summary>
    /// Static class providing functions to create a forms pages and elements.
    /// </summary>
    static class FormCreator
    {
        private static Dictionary<string, Func<FormCreationParams, FormElement>> TypeToViewCreator = new Dictionary<string, Func<FormCreationParams, FormElement>>()
        {
            { "inputText", CreateTextInput },
            { "inputSelectOne", CreatePicker },
            { "inputNumeric", CreateNumericInput },
            { "inputLocation", CreateLocationSelector },
            { "inputDate", CreateDateSelector },
            { "inputMedia", CreateMediaSelector }
        };

        private static Dictionary<string, Func<FormCreationParams, FormElement>> SpecialTypeToViewCreator = new Dictionary<string, Func<FormCreationParams, FormElement>>
        {
            { "compass", CreateCompass }
        };

        private static FormElement CreateCompass(FormCreationParams arg)
        {
            var grid = CreateStandardBaseGrid(arg);
            var formElement = new FormElement(grid, arg.Element, arg.Type);

            var currentCompassLabel = new Label { Text = AppResources.compass };
            var currentCompassDataLabel = new Label();
            Sensor.Instance.Compass.ReadingChanged += (_,eventArgs) => currentCompassDataLabel.Text = ((int)eventArgs.Reading.HeadingMagneticNorth).ToString() + " °";

            var saveButton = new Button { Text = AppResources.save };

            var savedCompassLabel = new Label { Text = AppResources.saveddata };
            var savedCompassDataLabel = new Label { StyleId = arg.Element.Name };

            saveButton.Clicked += (_, b) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    savedCompassDataLabel.Text = currentCompassDataLabel.Text;
                    formElement.OnValidContentChange();
                });
            };

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
            var elementName = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);
            var indexOfOpeningCurlyBrace = elementName.IndexOf('{');
            var indexOfClosingCurlyBrace = elementName.IndexOf('}');
            if (indexOfOpeningCurlyBrace != -1 && indexOfClosingCurlyBrace != -1 && indexOfOpeningCurlyBrace < indexOfClosingCurlyBrace )
            {
                elementName = elementName.Remove(indexOfOpeningCurlyBrace, indexOfClosingCurlyBrace - indexOfOpeningCurlyBrace + 1);
            }

            var elementNameLabel = new Label { 
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


        private static FormElement CreateMediaSelector(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new FormElement(grid, parms.Element, parms.Type);

            var pickFileButton = new Button { Text = AppResources.select };
            var fileSelectedLabel = new Label { Text = AppResources.fileselected };
            //HACK: bad way to store an image. blob would be better
            var dataHolder = new DataHolder { StyleId = parms.Element.Name };
            pickFileButton.Clicked += async (a, b) =>
            {
                var file = await CrossFilePicker.Current.PickFile();
                if (file == null)
                    formElement.OnInvalidContentChange();
                dataHolder.Data = Convert.ToBase64String(file.DataArray);
                var length = dataHolder.Data.Length;
                formElement.OnValidContentChange();
            };

            grid.Children.Add(pickFileButton, 0, 1);
            Grid.SetColumnSpan(pickFileButton, 2);
            grid.Children.Add(fileSelectedLabel, 0, 2);
            grid.Children.Add(dataHolder, 1, 2);
            return formElement;
        }


        private static FormElement CreateDateSelector(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new FormElement(grid, parms.Element, parms.Type);
            
            TimePicker timePicker = null;
            var datePicker = new DatePicker { StyleId = parms.Element.Name };

            void onContentChange()
            {
                if (datePicker.Date > DateTime.UtcNow || (timePicker != null && timePicker.Time == TimeSpan.Zero))
                    formElement.OnInvalidContentChange();
                else
                    formElement.OnValidContentChange();
            }

            datePicker.Unfocused += (a, b) =>
            {
                //TODO: Replace by using odk constraints
                if (datePicker.Date > DateTime.UtcNow)
                    parms.DisplayAlertFunc(AppResources.error, AppResources.selectedDateIsInFuture, AppResources.ok);

                onContentChange();
            };
            datePicker.Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            grid.Children.Add(datePicker, 0, 1);
            Grid.SetColumnSpan(datePicker, 2);

            if (parms.Element.Kind == "Full Date and Time")
            {
                timePicker = new TimePicker { StyleId = parms.Element.Name };
                timePicker.Unfocused += (a, b) =>
                {
                    onContentChange();
                };
                timePicker.Time = TimeSpan.Zero;
                grid.Children.Add(timePicker, 0, 2);
                Grid.SetColumnSpan(timePicker, 2);
            }

            return formElement;
        }

        private static FormElement CreateLocationSelector(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new FormElement(grid, parms.Element, parms.Type);

            var labelLat = new Label { Text = "Latitude" };

            var labelLatData = new Label() { Text = Sensor.Instance.Gps.Latitude.ToString(CultureInfo.CurrentCulture) };

            var labelLong = new Label { Text = "Longitude" };

            var labelLongData = new Label() { Text = Sensor.Instance.Gps.Longitude.ToString(CultureInfo.CurrentCulture) };

            var labelMessage = new Label { Text = AppResources.message };

            var labelMessageData = new Label() { Text = Sensor.Instance.Gps.Message };

            Sensor.Instance.Gps.StatusChanged += (sender, args) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    labelLatData.Text = args.Latitude.ToString();
                    labelLongData.Text = args.Longitude.ToString();
                    labelMessage.Text = args.Message;
                });
            };

            var saveButton = new Button { Text = AppResources.save };
            var skipButton = new Button { Text = AppResources.skip };

            var savedLocation = new Label { Text = AppResources.saveddata };
            var savedLocationData = new Label
            {
                Text = string.Empty,
                StyleId = parms.Element.Name
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
            var formElement = new FormElement(grid, parms.Element, parms.Type);

            var placeholder = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);
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
            var formElement = new FormElement(grid, parms.Element, parms.Type);

            var optionsList = new List<string>();
            var options = ProjectParser.ParseOptionsFromJson(parms.Element.Options);
            var title = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);
            if (title == "Unable to parse language from json")
            {
                title = AppResources.notitle;
            }
            var currentLanguageCode = OdkDataExtractor.GetCurrentLanguageCodeFromJsonList(parms.CurrentProject.Languages);
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
            var formElement = new FormElement(grid, parms.Element, parms.Type);

            var placeholder = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);
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

        /// <summary>
        /// Generates a single form page and its elements.
        /// </summary>
        /// <param name="form">Information of current form</param>
        /// <param name="currentProject">Information of current project</param>
        /// <param name="displayAlert">Async function which will display an alert.</param>
        /// <returns></returns>
        public static FormContent GenerateForm(ProjectForm form, Project currentProject, Func<string, string, string, Task> displayAlert)
        {
            var contentPage = new ContentPage();
            var scrollView = new ScrollView();
            var stack = new StackLayout();
            var elements = new List<FormElement>();

            contentPage.Padding = new Thickness(10, 10, 10, 10);

            contentPage.Title = form.Title;
            //walk through list of elements and generate form containing elements
            foreach (var element in form.ElementList)
            {
                IEnumerable<char> findSpecialType(string name) => name.SkipWhileIncluding(c => c != '{').TakeWhile(c => c != '}');

                FormElement formElement = null;

                bool specialType = false;
                if (element.Label != null)
                {
                    var translatedLabel = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(element.Label, currentProject.Languages);
                    if (translatedLabel != null && findSpecialType(translatedLabel).Any())
                    {
                        //Special element
                        var specialElementType = new string(findSpecialType(translatedLabel).ToArray());
                        if (SpecialTypeToViewCreator.TryGetValue(specialElementType, out var viewCreator))
                        {
                            var formCreationParams = new FormCreationParams(specialElementType, element, currentProject, displayAlert);
                            specialType = true;
                            formElement = viewCreator(formCreationParams);
                        }
                    }
                }
                
                if (!specialType)
                {
                    if (TypeToViewCreator.TryGetValue(element.Type, out var viewCreator))
                    {
                        var formCreationParams = new FormCreationParams(element.Type, element, currentProject, displayAlert);
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
