using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface IModuleHost
    {
        List<ISharedModule> Modules { get; }

        /// <summary>
        /// 
        /// </summary>
        List<Func<string, string>> RedirectionFuncs { get; }

        /// <summary>
        /// Provides access to shared functionality from the app
        /// </summary>
        IApp App { get; }

        /// <summary>
        /// Provides loose access to and sharing of methods between modules
        /// </summary>
        ISharedMethodProvider SharedMethodProvider { get; }

        /// <summary>
        /// Adds a validity check that may prevent shell URL navigation (by changing target) to specific pages
        /// </summary>
        /// <param name="redirectionFunc">
        /// Func which will receive, upon any url-based navigation, the url of the target. </br>
        /// It may either output a new url (for redirection) or null to allow the navigation.</param>
        void AddNavigationValidityCheck(Func<string, string> redirectionFunc);
    }
}
