using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface IModuleHost
    {
        Guid AddSidebarItems(params KeyValuePair<string, NavigationPage>[] items);
        IEnumerable<Guid> AddSidebarItems(IEnumerable<KeyValuePair<string, NavigationPage>> items);
        IApp App { get; }
    }
}
