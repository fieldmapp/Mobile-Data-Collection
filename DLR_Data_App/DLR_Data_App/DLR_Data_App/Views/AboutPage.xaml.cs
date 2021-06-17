﻿using DlrDataApp.Modules.Base.Shared;
using System;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }
        
        private async void LicenseButton_OnClicked(object sender, EventArgs e)
        {
            await this.PushPage(new LicensesPage());
        }
    }
}