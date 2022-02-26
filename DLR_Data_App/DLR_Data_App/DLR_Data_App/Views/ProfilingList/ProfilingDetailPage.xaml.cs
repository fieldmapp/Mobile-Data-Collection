using System;
using DLR_Data_App.Localizations;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using DLR_Data_App.Models;
using DLR_Data_App.Models.Profiling;

namespace DLR_Data_App.Views.ProfilingList
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

        /// <summary>
        /// Selects project as current active project.
        /// </summary>
        private async void Btn_current_profiling_Clicked(object sender, EventArgs e)
        {
            // Set project in database as current project
            Database.SetCurrentProfiling(InspectedProfiling);

            // Navigate to current project
            await App.CurrentMainPage.NavigateFromMenu(MenuItemType.CurrentProfiling);
        }

        /// <summary>
        /// Removes project from database.
        /// </summary>
        private async void Btn_remove_profiling_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(AppResources.removeprofiling, AppResources.removeprofilingwarning, AppResources.okay, AppResources.cancel);
            if (answer)
            {
                Database.DeleteProfiling(InspectedProfiling);
                await Navigation.PopAsync();
            }
        }
    }
}