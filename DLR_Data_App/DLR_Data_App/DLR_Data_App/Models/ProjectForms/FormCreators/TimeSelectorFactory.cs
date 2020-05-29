using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms.FormCreators
{
    class TimeSelectorFactory : ElementFactory
    {
        public override FormElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var timeSelectorElement = new TimeSelectorElement(grid, parms.Element, parms.Type);

            TimePicker timePicker = null;
            var datePicker = new DatePicker { StyleId = parms.Element.Name };
            timeSelectorElement.DatePicker = datePicker;

            datePicker.Unfocused += (a, b) =>
            {
                //TODO: Replace by using odk constraints
                if (datePicker.Date > DateTime.UtcNow)
                    parms.DisplayAlertFunc(AppResources.error, AppResources.selectedDateIsInFuture, AppResources.ok);

                timeSelectorElement.OnContentChange();
            };
            datePicker.Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            grid.Children.Add(datePicker, 0, 1);
            Grid.SetColumnSpan(datePicker, 2);

            if (parms.Element.Kind == "Full Date and Time")
            {
                timePicker = new TimePicker { StyleId = parms.Element.Name };
                timeSelectorElement.TimePicker = timePicker;
                timePicker.Unfocused += (a, b) => timeSelectorElement.OnContentChange();
                timePicker.Time = TimeSpan.Zero;
                grid.Children.Add(timePicker, 0, 2);
                Grid.SetColumnSpan(timePicker, 2);
            }

            return timeSelectorElement;
        }
    }
}
