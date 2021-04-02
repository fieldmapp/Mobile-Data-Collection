using System;
using Xamarin.Forms.Xaml;
using DlrDataApp.Modules.ProfilingSharedModule.Models;
using DlrDataApp.Modules.SharedModule;
using DlrDataApp.Modules.ProfilingSharedModule.Localization;

namespace DlrDataApp.Modules.ProfilingSharedModule.Views.ProfilingList
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

        Database Database => ProfilingSharedModule.Instance.ModuleHost.App.Database;

        /// <summary>
        /// Selects project as current active project.
        /// </summary>
        private async void Btn_current_profiling_Clicked(object sender, EventArgs e)
        {

            // Set project in database as current project
            Database.SetCurrentProfiling(InspectedProfiling);

            // Navigate to current project
            // TODO
            //await App.CurrentMainPage.NavigateFromMenu(MenuItemType.CurrentProfiling);
        }

        /// <summary>
        /// Removes project from database.
        /// </summary>
        private async void Btn_remove_profiling_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(AppResources.removeprofiling, AppResources.removeprofilingwarning, SharedModule.Localization.AppResources.okay, SharedModule.Localization.AppResources.cancel);
            if (answer)
            {
                Database.Delete(InspectedProfiling);
                await Navigation.PopAsync();
            }
        }
    }
}