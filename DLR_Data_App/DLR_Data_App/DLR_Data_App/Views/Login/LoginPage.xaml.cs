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

            // after completing the input of username change focus to login button
            EntryUsername.Completed += (s, e) => BtnSignin.Focus();
        }

        /**
         * Get login data, check them and on success navigate to project list
         */
        private async void Btn_signin_Clicked(object sender, EventArgs e)
        {
            var checkUsername = EntryUsername.Text;

            if (_viewModel.Check_Information(checkUsername))
            {
                Application.Current.MainPage = new SplashScreenPage(true);
            }
            else
            {
                await DisplayAlert(AppResources.login, AppResources.nouserfound, AppResources.back);
            }
        }

        /**
         * Open form for creating a new user
         */
        private void Btn_newaccount_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NewProfilePage();
        }
    }
}