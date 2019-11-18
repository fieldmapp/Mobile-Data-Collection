using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProfilePage
    {
        private User _user;

        public NewProfilePage()
        {
            InitializeComponent();

            // directing user to next entry after inserting text to make it easier to insert informations
            EntryUsername.Completed += (s, e) => BtnAccept.Focus();
        }

        /// <summary>
        /// Adds user
        /// </summary>
        private async void Btn_accept_Clicked(object sender, EventArgs e)
        {
            var username = EntryUsername.Text;

            // check if entry is empty
            if (username == null)
            {
                await DisplayAlert(AppResources.newaccount, AppResources.wrongentry, AppResources.okay);
                return;
            }

            _user = new User { Username = username };

            var existingUser = Database.ReadUsers().FirstOrDefault(u => u.Username == _user.Username);
            if (existingUser != null)
            {
                await DisplayAlert(AppResources.newaccount, AppResources.useralreadyexists, AppResources.okay);
                Application.Current.MainPage = new LoginPage();
                return;
            }

            var answer = await DisplayAlert(AppResources.privacypolicy, AppResources.privacytext1, AppResources.accept, AppResources.decline);

            if (!answer)
            {
                // if user dont accept privacy policy cancel process and return to login page
                Application.Current.MainPage = new LoginPage();
                return;
            }

            var status = Database.Insert(ref _user);

            if (!status)
            {
                await DisplayAlert(AppResources.newaccount, AppResources.failed, AppResources.okay);
                Application.Current.MainPage = new LoginPage();
                return;
            }

            App.CurrentUser = _user;

            Application.Current.MainPage = new SplashScreenPage(false);
        }

        /// <summary>
        /// Cancels user creation
        /// </summary>
        private void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new LoginPage();
        }
    }
}