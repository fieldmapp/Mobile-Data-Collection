using DLR_Data_App.Localizations;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Controls
{
    /// <summary>
    /// Label which has an additional Data property. The labels text will reflect weather or not the Data is null or empty.
    /// </summary>
    class DataHolder : Label
    {
        private string _data = string.Empty;

        public string Data
        {
            get => _data;
            set
            {
                _data = value;
                Text = string.IsNullOrEmpty(_data) ? AppResources.no : AppResources.yes;
            }
        }
    }
}
