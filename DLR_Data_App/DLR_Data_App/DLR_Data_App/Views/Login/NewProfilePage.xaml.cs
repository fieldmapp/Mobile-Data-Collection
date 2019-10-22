using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Login
{
    /**
     * Creating a new profile, same design like login page
     */
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProfilePage
    {
        private User _user;

        /**
         * Constructor
         */
        public NewProfilePage()
        {
            InitializeComponent();

            // setting icon size
            LoginIcon.HeightRequest = 120;

            // directing user to next entry after inserting text to make it easier to insert informations
            EntryUsername.Completed += (s, e) => EntryPassword.Focus();
            EntryPassword.Completed += (s, e) => BtnAccept.Focus();
        }

        /**
         * Adding user
         */
        private async void Btn_accept_Clicked(object sender, EventArgs e)
        {
            var username = EntryUsername.Text;
            var password = EntryPassword.Text;

            // check if entry is empty
            if (username == null
              || password == null)
            {
                await DisplayAlert(AppResources.newaccount, AppResources.wrongentry, AppResources.okay);
                return;
            }

            _user = new User { Username = username, Password = Helpers.Encrypt_password(password) };

            foreach (var user in Database.ReadUser())
            {
                if (_user.Username != user.Username) continue;

                await DisplayAlert(AppResources.newaccount, AppResources.useralreadyexists, AppResources.okay);
                Cancel();
                return;
            }

            var answer = await DisplayAlert(AppResources.privacypolicy, AppResources.privacytext1, AppResources.accept, AppResources.decline);

            if (!answer)
            {
                // if user dont accept privacy policy cancel process and return to login page
                Cancel();
                return;
            }

            var status = Database.Insert(ref _user);
            if (!status)
            {
                await DisplayAlert(AppResources.newaccount, AppResources.failed, AppResources.okay);
                Cancel();
                return;
            }

            App.CurrentUser = _user;

            Application.Current.MainPage = new MainPage();
        }

        /**
         * Cancel user creation
         */
        private void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            Cancel();
        }

        /**
         * Cancel methode and return to login page
         */
        private void Cancel()
        {
            Application.Current.MainPage = new LoginPage();
        }
    }
}