using DLR_Data_App.Models;
using DLR_Data_App.Views;
using DlrDataApp.Modules.Base.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Services
{
    public class ModuleHostService : IModuleHost
    {
        List<ISharedModule> Modules;
        List<HomeMenuItem> MenuItems = new List<HomeMenuItem>();
        MainPage MainPage;
        MenuPage MenuPage;
        public ModuleHostService(IApp app, MainPage mainPage, MenuPage menuPage, List<ISharedModule> modules)
        {
            Modules = modules;
            MainPage = mainPage;
            MenuPage = menuPage;
            App = app;

            Initialize();
        }

        private void Initialize()
        {
            foreach (var module in Modules)
            {
                module.Initialize(this);
            }
            foreach (var module in Modules)
            {
                module.PostInitialize();
            }
        }

        public IApp App { get; }

        public Guid AddToSidebar(string title, NavigationPage page)
        {
            var guid = Guid.NewGuid();
            MenuPage.MenuItems.Add(new HomeMenuItem { Id = guid, Title = title, NavigationPage = page });
            return guid;
        }

        public List<Guid> AddToSidebar(IEnumerable<KeyValuePair<string, NavigationPage>> items)
        {
            var result = new List<Guid>();
            foreach (var item in items)
            {
                result.Add(AddToSidebar(item.Key, item.Value));
            }
            return result;
        }

        public void NavigateTo(Guid id)
        {
            _ = (Application.Current as App).MainPage.NavigateToPage(id);
        }
    }
}
