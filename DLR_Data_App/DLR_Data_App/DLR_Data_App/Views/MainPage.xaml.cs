using DLR_Data_App.Models;
using DLR_Data_App.Services;
using DLR_Data_App.Views.CurrentProject;
using DLR_Data_App.Views.Login;
using DLR_Data_App.Views.ProjectList;
using DLR_Data_App.Views.Settings;
using DLR_Data_App.Views.Survey;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    /**
     * Main page which handles the menu and sets up the master page
     */
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage
    {
        private readonly Dictionary<MenuItemType, NavigationPage> _menuPages;

        /**
         * Constructor for MainPage
         */
        public MainPage()
        {
            InitializeComponent();
            _menuPages = new Dictionary<MenuItemType, NavigationPage>
            {
                { MenuItemType.Projects, (NavigationPage)Detail },
                { MenuItemType.CurrentProject, new NavigationPage(new ProjectPage()) },
                { MenuItemType.Projects, new NavigationPage(new ProjectListPage()) },
                { MenuItemType.Sensortest, new NavigationPage(new SensorTestPage()) },
                { MenuItemType.Settings, new NavigationPage(new SettingsPage()) },
                { MenuItemType.About, new NavigationPage(new AboutPage()) },
                { MenuItemType.Survey, new NavigationPage(new SurveyListPage()) }
            };

            MasterBehavior = MasterBehavior.Popover;
        }

        /**
         * Navigate to selected page
         * @param id int Selected page
         */
        public async Task NavigateFromMenu(MenuItemType id)
        {
            if (id == MenuItemType.Logout)
            {
                Application.Current.MainPage = new NavigationPage(new LoginPage());
                return;
            }

            var newPage = _menuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}