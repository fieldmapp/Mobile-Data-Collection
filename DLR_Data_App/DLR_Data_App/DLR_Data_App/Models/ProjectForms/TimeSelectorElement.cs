using DLR_Data_App.Models.ProjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class TimeSelectorElement : FormElement
    {
        private static readonly DateTime EmptyDate = new DateTime(1970, 1, 1);
        private static readonly long EmptyDateTicks = EmptyDate.Ticks;
        public TimeSelectorElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) { }

        public DatePicker DatePicker;
        public TimePicker TimePicker;

        public override bool IsValid => (DatePicker == null || (EmptyDateTicks != DatePicker.Date.Ticks))
                                     && (TimePicker == null || TimePicker.Time != TimeSpan.Zero) && base.IsValid;

        public override string GetRepresentationValue()
        {
            long ticks = 0;
            if (DatePicker != null)
                ticks += DatePicker.Date.Ticks;
            if (TimePicker != null)
                ticks += TimePicker.Time.Ticks;
            return ticks.ToString();
        }

        public override void LoadFromSavedRepresentation(string representation)
        {
            if (long.TryParse(representation, out long ticks))
            {
                var savedDateTime = new DateTime(ticks);
                if (DatePicker != null)
                    DatePicker.Date = savedDateTime - savedDateTime.TimeOfDay;
                if (TimePicker != null)
                    TimePicker.Time = savedDateTime.TimeOfDay;
            }
        }

        public override void Reset()
        {
            if (TimePicker != null)
                TimePicker.Time = TimeSpan.Zero;
            if (DatePicker != null)
                DatePicker.Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }
    }
}
