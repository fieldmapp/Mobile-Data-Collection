using DLR_Data_App.Models;
using DLR_Data_App.Views;
using DlrDataApp.Modules.Base.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Services
{
    class ModuleHostService : IModuleHost
    {
        List<ISharedModule> Modules;
        List<HomeMenuItem> MenuItems = new List<HomeMenuItem>();
        public ModuleHostService(IApp app, MainPage mainPage, MenuPage menuPage, List<ISharedModule> modules)
        {
            Modules = modules;

            App = app;
        }

        public IApp App { get; }

        public Guid AddSidebarItems(params KeyValuePair<string, NavigationPage>[] items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> AddSidebarItems(IEnumerable<KeyValuePair<string, NavigationPage>> items)
        {
            foreach (var item in items)
            {
                yield return AddSidebarItems(item);
            }
        }
    }
}
