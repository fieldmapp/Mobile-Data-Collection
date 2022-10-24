using DLR_Data_App.Controls;
using DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjects.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectForms
{
    class TimeSelectorElement : FormElement
    {
        public TimeSelectorElement(Grid grid, ProjectFormElements data, string type, Func<string, string, string, Task> displayAlertFunc, Project project) : base(grid, data, type, displayAlertFunc, project) 
        {
            ValidRange = OdkDataExtractor.GetRangeFromJsonString(data.Range, DateTime.Parse);
        }
        static Color SetColor => (Color)OdkProjectsModule.Instance.App.Resources["TextPrimary"];
        static Color SetColorNight => (Color)OdkProjectsModule.Instance.App.Resources["TextPrimaryNight"];
        static Color UnsetColor = (Color)OdkProjectsModule.Instance.App.Resources["TextSecondary"];
        static Color UnsetColorNight = (Color)OdkProjectsModule.Instance.App.Resources["TextSecondaryNight"];
        public DatePicker DatePicker;
        public TimePicker TimePicker;
        bool _isSet;
        public bool IsSet
        {
            get => _isSet;
            set
            {
                _isSet = value;
                UpdateColor(value);
            }
        }

        void UpdateColor(bool isSet)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var newColor = isSet ? SetColor : UnsetColor;
                if (DatePicker != null)
                    DatePicker.SetAppThemeColor(DatePicker.TextColorProperty, isSet ? SetColor : UnsetColor, isSet ? SetColorNight : UnsetColorNight);
                if (TimePicker != null)
                    TimePicker.SetAppThemeColor(TimePicker.TextColorProperty, isSet ? SetColor : UnsetColor, isSet ? SetColorNight : UnsetColorNight);
            });
        }

        private OdkRange<DateTime> ValidRange;

        protected override bool IsValidElementSpecific => IsSet
            && ValidRange.IsValidInput(GetCombinedDateTime());

        private DateTime GetCombinedDateTime()
        {
            if (!IsSet)
                return new DateTime();

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
            var timeSelectorElement = new TimeSelectorElement(grid, parms.Element, parms.Type, parms.DisplayAlertFunc, parms.CurrentProject);

            TimePicker timePicker = null;
            var datePicker = new OKCancelDatePicker { TextColor = UnsetColor };
            timeSelectorElement.DatePicker = datePicker;

            datePicker.Closed += (a, b) =>
            {
                if (b == OKCancelDatePicker.CloseType.Cancel)
                    return;

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
            timeSelectorElement.UpdateColor(false);

            return timeSelectorElement;
        }
    }
}
