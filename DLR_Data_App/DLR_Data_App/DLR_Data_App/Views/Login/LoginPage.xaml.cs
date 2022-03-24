using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using System;
using DLR_Data_App.ViewModels.Login;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using DLR_Data_App.Models.Profiling;
using System.Linq;

namespace DLR_Data_App.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage
    {
        public class UserDisplay : BindableObject, IInlinePickerElement
        {
            public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(UserDisplay), Color.Transparent, BindingMode.OneWay);
            public Color BackgroundColor
            {
                get => (Color)GetValue(BackgroundColorProperty);
                set => SetValue(BackgroundColorProperty, value);
            }
            public string Name { get; set; }
        }

        private readonly LoginViewModel _viewModel = new LoginViewModel();
        private List<UserDisplay> UserList;
        
        public LoginPage()
        {
            UserList = Database.ReadUsers().Select(u => new UserDisplay { Name = u.Username }).ToList();
            InitializeComponent();
            UserPicker.ItemsSource = UserList;
            BindingContext = _viewModel;
        }

        /// <summary>
        /// Gets login data, checks them and on success navigates to project list.
        /// </summary>
        private async void Btn_signin_Clicked(object sender, EventArgs e)
        {
            if (UserPicker.SelectedItem == null)
                return;

            var selectedUserName = ((UserDisplay)UserPicker.SelectedItem).Name;

            if (_viewModel.Check_Information(selectedUserName))
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