using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared.Controls
{
    /// <summary>
    /// <see cref="DatePicker"/> which may be closed with either "ok" or "cancel"
    /// </summary>
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
