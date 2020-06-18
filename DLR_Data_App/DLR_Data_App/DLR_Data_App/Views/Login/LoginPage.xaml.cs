using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using System;
using DLR_Data_App.ViewModels.Login;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage
    {
        private readonly LoginViewModel _viewModel = new LoginViewModel();
        
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = _viewModel;

            // after completing the input of username change focus to login button
            EntryUsername.Completed += (s, e) => BtnSignin.Focus();
        }

        /// <summary>
        /// Gets login data, checks them and on success navigates to project list.
        /// </summary>
        private async void Btn_signin_Clicked(object sender, EventArgs e)
        {
            var checkUsername = EntryUsername.Text;

            if (_viewModel.Check_Information(checkUsername))
            {
                Application.Current.MainPage = new SplashScreenPage();
            }
            else
            {
                await DisplayAlert(AppResources.login, AppResources.nouserfound, AppResources.back);
            }
        }

        /// <summary>
        /// Opens form for creating a new user.
        /// </summary>
        private void Btn_newaccount_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NewProfilePage();
        }
    }
}