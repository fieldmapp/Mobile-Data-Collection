using DlrDataApp.Modules.Base.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared.Controls
{
    /// <summary>
    /// Label which has an additional Data property. The labels text will reflect weather or not the Data is null or empty.
    /// </summary>
    public class DataHolder : Label
    {
        private string _data = string.Empty;

        public string Data
        {
            get => _data;
            set
            {
                _data = value;
                Text = string.IsNullOrEmpty(_data) ? SharedResources.no : SharedResources.yes;
            }
        }
    }

}
