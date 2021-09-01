﻿using System;
using Xamarin.Forms.Xaml;
using DlrDataApp.Modules.Profiling.Shared.Models;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Profiling.Shared.Localization;
using DlrDataApp.Modules.Base.Shared.Localization;

namespace DlrDataApp.Modules.Profiling.Shared.Views.ProfilingList
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilingDetailPage
    {
        private readonly ProfilingData InspectedProfiling;

        public ProfilingDetailPage(ProfilingData profiling)
        {
            InitializeComponent();
            InspectedProfiling = profiling;
            AuthorsLabel.Text = Helpers.GetCurrentLanguageTranslation(profiling.Translations, profiling.Authors);
            DescriptionLabel.Text = Helpers.GetCurrentLanguageTranslation(profiling.Translations, profiling.Description);
            TitleLabel.Text = Helpers.GetCurrentLanguageTranslation(profiling.Translations, profiling.Title);
        }

        /// <summary>
        /// Default constructor only to be used by the Xamarin Form Previewer 
        /// </summary>
        public ProfilingDetailPage()
        {
            InitializeComponent();
        }

        Database Database => ProfilingModule.Instance.ModuleHost.App.Database;

        /// <summary>
        /// Selects project as current active project.
        /// </summary>
        private async void Btn_current_profiling_Clicked(object sender, EventArgs e)
        {

            // Set project in database as current project
            Database.SetCurrentProfiling(InspectedProfiling);

            // Navigate to current project
            ProfilingModule.Instance.ModuleHost.NavigateTo(ProfilingModule.Instance.CurrentProfilingPageGuid);
        }

        /// <summary>
        /// Removes project from database.
        /// </summary>
        private async void Btn_remove_profiling_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(ProfilingResources.removeprofiling, ProfilingResources.removeprofilingwarning, SharedResources.okay, SharedResources.cancel);
            if (answer)
            {
                Database.Delete(InspectedProfiling);
                await Navigation.PopAsync();
            }
        }
    }
}