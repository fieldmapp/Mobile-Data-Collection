using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.SharedModule
{
    public interface IModuleHost
    {
        void AddSidebarItems(params KeyValuePair<string, NavigationPage>[] items);
        void AddSidebarItems(IEnumerable<KeyValuePair<string, NavigationPage>> items);
        IApp App { get; }
    }
}
