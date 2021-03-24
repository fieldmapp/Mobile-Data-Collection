using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class TimeSelectorElement : FormElement
    {
        public TimeSelectorElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) 
        {
            ValidRange = OdkDataExtractor.GetRangeFromJsonString(data.Range, DateTime.Parse);
        }
        static readonly Color SetColor = Color.Black;
        static readonly Color UnsetColor = Color.LightGray;
        public DatePicker DatePicker;
        public TimePicker TimePicker;
        bool _isSet;
        public bool IsSet
        {
            get => _isSet;
            set
            {
                _isSet = value;
                Device.BeginInvokeOnMainThread(() =>
                {
                    var newColor = value ? SetColor : UnsetColor;
                    if (DatePicker != null)
                        DatePicker.TextColor = newColor;
                    if (TimePicker != null)
                        TimePicker.TextColor = newColor;
                });
            }
        }
        private OdkRange<DateTime> ValidRange;

        protected override bool IsValidElementSpecific => IsSet
            && ValidRange.IsValidInput(GetCombinedDateTime());

        private DateTime GetCombinedDateTime()
        {
            var result = DatePicker.Date;
            if (TimePicker != null)
                result += TimePicker.Time;
            return result;
        }

        public override string GetRepresentationValue() => GetCombinedDateTime().Ticks.ToString();

        public override void LoadFromSavedRepresentation(string representation)
        {
            if (long.TryParse(representation, out long ticks))
            {
                var savedDateTime = new DateTime(ticks);
                DatePicker.Date = savedDateTime - savedDateTime.TimeOfDay;
                if (TimePicker != null)
                    TimePicker.Time = savedDateTime.TimeOfDay;
                IsSet = true;
            }
        }

        protected override void OnReset()
        {
            if (TimePicker != null)
                TimePicker.Time = TimeSpan.Zero;
            DatePicker.Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            IsSet = false;
        }

        public static TimeSelectorElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var timeSelectorElement = new TimeSelectorElement(grid, parms.Element, parms.Type);

            TimePicker timePicker = null;
            var datePicker = new DatePicker { TextColor = UnsetColor };
            timeSelectorElement.DatePicker = datePicker;

            datePicker.Unfocused += (a, b) =>
            {
                timeSelectorElement.IsSet = true;
                timeSelectorElement.OnContentChange();
            };
            datePicker.Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            grid.Children.Add(datePicker, 0, 1);
            Grid.SetColumnSpan(datePicker, 2);

            if (parms.Element.Kind == "Full Date and Time")
            {
                timePicker = new TimePicker { TextColor = UnsetColor };
                timeSelectorElement.TimePicker = timePicker;
                timePicker.Unfocused += (a, b) =>
                {
                    timeSelectorElement.IsSet = true;
                    timeSelectorElement.OnContentChange();
                };
                timePicker.Time = TimeSpan.Zero;
                grid.Children.Add(timePicker, 0, 2);
                Grid.SetColumnSpan(timePicker, 2);
            }

            return timeSelectorElement;
        }
    }
}
