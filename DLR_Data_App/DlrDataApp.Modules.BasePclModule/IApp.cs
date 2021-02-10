using DlrDataApp.Modules.SharedModule.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.SharedModule
{
    public interface IApp
    {
        IUser CurrentUser { get; }
        NavigationPage CurrentPage { get; }
        Database Database { get; }
        ThreadSafeRandom RandomProvider { get; }
        Sensor Sensor { get; }
        string FolderLocation { get; }
    }
}
