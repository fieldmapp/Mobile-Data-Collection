using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface IModuleHost
    {
        Guid AddToSidebar(string title, NavigationPage page);
        List<Guid> AddToSidebar(IEnumerable<KeyValuePair<string, NavigationPage>> items);
        IApp App { get; }
        void NavigateTo(Guid id);
    }
}
