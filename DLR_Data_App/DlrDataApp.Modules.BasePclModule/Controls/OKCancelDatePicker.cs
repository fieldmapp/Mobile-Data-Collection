using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Controls
{
    public class OKCancelDatePicker : DatePicker
    {
        public enum CloseType
        {
            Ok,
            Cancel
        }

        public event EventHandler<CloseType> Closed;

        public void CallClosed(CloseType closeType)
        {
            Closed?.Invoke(this, closeType);
        }
    }
}
