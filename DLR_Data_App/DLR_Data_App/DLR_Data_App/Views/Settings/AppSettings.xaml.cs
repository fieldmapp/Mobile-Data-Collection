using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using DLR_Data_App.Views.Login;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppSettings
    {
        private readonly List<string> _elementList;
        
        public AppSettings ()
        {
            InitializeComponent ();
            _elementList = new List<string> {AppResources.privacypolicy, AppResources.removedatabase, AppResources.exportdatabase};
            
            AppSettingsList.ItemsSource = _elementList;
        }

        /// <summary>
        /// List of settings about the app shown
        /// </summary>
        private async void AppSettingsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            bool answer;
            switch (e.ItemIndex)
            {
                case 0:
                    // Show privacy policy
                    await DisplayAlert(AppResources.privacypolicy, AppResources.privacytext1, AppResources.okay);
                    break;
                case 1:
                    // Remove Database
                    answer = await DisplayAlert(AppResources.removedatabase, AppResources.removedatabasewarning, AppResources.accept, AppResources.cancel);
                    if(answer)
                    {
                        if(Database.RemoveDatabase())
                        {
                            // Successful
                            await DisplayAlert(AppResources.removedatabase, AppResources.successful, AppResources.okay);
                            App.Current.MainPage = new LoginPage();
                        }
                        else
                        {
                            // Failure
                            await DisplayAlert(AppResources.removedatabase, AppResources.failed, AppResources.cancel);
                        }
                    }
                    break;
                case 2:
                    // Export Database
                    var exportString = ExportData();
                    (Application.Current as App).StorageProvider.ExportDatabase(exportString);
                    await DisplayAlert(AppResources.save, AppResources.exporttorootwithtimestampsuccessful, AppResources.okay);
                    break;
            }
        }

        /// <summary>
        /// Exports data of current project to a json string.
        /// </summary>
        public static string ExportData()
        {
            // get data of the project from the db
            var workingProject = Database.GetCurrentProject();
            var tableContent = Database.ReadCustomTable(ref workingProject);
            if (tableContent == null)
            {
                return "";
            }

            // get current user
            var user = App.CurrentUser;

            JObject dataObject = new JObject();

            for (var i = 0; i < tableContent.RowNameList.Count; i++)
            {
                JArray dataArray = JArray.FromObject(tableContent.ValueList[i]);
                JProperty name = new JProperty(tableContent.RowNameList[i], dataArray);
                dataObject.Add(name);
            }

            JObject exportObject = new JObject(
              new JProperty("User",
                new JObject(
                    new JProperty("User_Id", user.Id),
                    new JProperty("User_Name", user.Username))),
              new JProperty("Project",
                new JObject(
                    new JProperty("Project_Id", workingProject.Id),
                    new JProperty("Project_Title", workingProject.Title),
                    new JProperty("Project_Authors", workingProject.Authors),
                    new JProperty("Project_Description", workingProject.Description))),
              new JProperty("Data",
                dataObject
              ));

            return exportObject.ToString();
        }
    }
}