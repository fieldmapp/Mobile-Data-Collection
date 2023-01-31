using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.Base.Shared.DependencyServices
{
    /// <summary>
    /// Provides a method to close the app completely.
    /// </summary>
    public interface IAppCloser
    {
        /// <summary>
        /// Close the entire app and bring the user back into their home screen.
        /// </summary>
        void CloseApp();
    }
}
