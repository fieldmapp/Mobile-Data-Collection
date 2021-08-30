using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface IModuleHost
    {
        Guid AddSidebarItems(KeyValuePair<string, NavigationPage> items);
        List<Guid> AddSidebarItems(IEnumerable<KeyValuePair<string, NavigationPage>> items);
        IApp App { get; }
    }
}
