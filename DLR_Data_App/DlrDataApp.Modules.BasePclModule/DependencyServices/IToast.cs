using System;
using System.Collections.Generic;
using System.Text;

// see https://stackoverflow.com/a/44126899

namespace DlrDataApp.Modules.Base.Shared.DependencyServices
{
    /// <summary>
    /// Provides methods to display text on the Toast.
    /// TODO: Port to Snackbar.
    /// </summary>
    public interface IToast
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
