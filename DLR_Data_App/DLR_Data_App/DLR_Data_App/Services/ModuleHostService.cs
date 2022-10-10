﻿using DLR_Data_App.Models;
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
        public ModuleHostService(IApp app, List<ISharedModule> modules)
        {
            Modules = modules;
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
    }
}
