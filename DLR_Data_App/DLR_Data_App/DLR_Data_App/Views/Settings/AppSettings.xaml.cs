using DLR_Data_App.Views.Login;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Localization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            _elementList = new List<string> {SharedResources.privacypolicy, SharedResources.removedatabase, SharedResources.exportdatabase};
            
            AppSettingsList.ItemsSource = _elementList;
        }

        Database Database => (App.Current as App).Database;

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
                    await DisplayAlert(SharedResources.privacypolicy, SharedResources.privacytext1, SharedResources.okay);
                    break;
                case 1:
                    // Remove Database
                    answer = await DisplayAlert(SharedResources.removedatabase, SharedResources.removedatabasewarning, SharedResources.accept, SharedResources.cancel);
                    if(answer)
                    {
                        if(Database.DeleteDatabase())
                        {
                            // Successful
                            await DisplayAlert(SharedResources.removedatabase, SharedResources.successful, SharedResources.okay);
                            App.Current.MainPage = new LoginPage();
                        }
                        else
                        {
                            // Failure
                            await DisplayAlert(SharedResources.removedatabase, SharedResources.failed, SharedResources.cancel);
                        }
                    }
                    break;
                case 2:
                    // Export Database
                    var exportString = ExportData();
                    ExportDatabase(exportString, DependencyService.Get<IStorageAccessProvider>());
                    await DisplayAlert(SharedResources.save, SharedResources.exporttorootwithtimestampsuccessful, SharedResources.okay);
                    break;
            }
        }

        private void ExportDatabase(string exportString, IStorageAccessProvider storageAccessProvider)
        {
            var filename = "Fieldmapp_Database_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss", CultureInfo.InvariantCulture) + ".json";

            using (var fileStream = storageAccessProvider.OpenFileWriteExternal(filename))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(exportString);
            }
        }

        /// <summary>
        /// Exports data of current project to a json string.
        /// </summary>
        public static string ExportData()
        {
            return @"{""info"":""todo""}";
            // TODO
            // get data of the project from the db
            /*
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
            */
        }
    }
}