using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using System;
using DLR_Data_App.ViewModels.Login;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/**
 * Login page class 
 */
namespace DLR_Data_App.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage
    {
        private readonly LoginViewModel _viewModel = new LoginViewModel();

        /**
         * View for login page
         */
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = _viewModel;

            LoginIcon.HeightRequest = 120;

            // after completing the input of username change focus to password and after that to login button
            EntryUsername.Completed += (s, e) => EntryPassword.Focus();
            EntryPassword.Completed += (s, e) => BtnSignin.Focus();
        }

        /**
         * Get login data, check them and on success navigate to project list
         */
        private async void Btn_signin_Clicked(object sender, EventArgs e)
        {
            var checkUsername = EntryUsername.Text;
            var checkPassword = Helpers.Encrypt_password(EntryPassword.Text);

            if (_viewModel.Check_Information(checkUsername, checkPassword))
            {
                var answer = await DisplayAlert(AppResources.privacypolicy, AppResources.privacytext1, AppResources.accept, AppResources.decline);

                if (!answer) return;

                await this.PushPage(new MainPage());
            }
            else
            {
                await DisplayAlert(AppResources.login, AppResources.nouserfound, AppResources.back);
            }
        }

        /**
         * Open form for creating a new user
         */
        private async void Btn_newaccount_Clicked(object sender, EventArgs e)
        {
            await this.PushPage(new NewProfilePage());
        }
    }
}