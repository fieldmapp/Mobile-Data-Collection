using DlrDataApp.Modules.Base.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface IApp
    {
        IModuleHost ModuleHost { get; }
        IUser CurrentUser { get; }
        NavigationPage NavigationPage { get; }
        Page CurrentPage { get; }
        Database Database { get; }
        ThreadSafeRandom RandomProvider { get; }
        Sensor Sensor { get; }
        string FolderLocation { get; }
    }
}
