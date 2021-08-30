using DLR_Data_App.Views.Login;
using DlrDataApp.Modules.Base.Shared.Localization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
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
            _elementList = new List<string> { SharedResources.privacypolicy, SharedResources.removedatabase, SharedResources.exportdatabase};
            
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
                    await DisplayAlert(SharedResources.privacypolicy, SharedResources.privacytext1, SharedResources.okay);
                    break;
                case 1:
                    // Remove Database
                    answer = await DisplayAlert(SharedResources.removedatabase, SharedResources.removedatabasewarning, SharedResources.accept, SharedResources.cancel);
                    if(answer)
                    {
                        // TODO
                        /*if (Directory.Exists(MediaSelectorElement.MediaPath))
                            Directory.Delete(MediaSelectorElement.MediaPath, true);

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
                        }*/
                    }
                    break;
                case 2:
                    // Export Database
                    try
                    {
                        await ExportData();
                        await DisplayAlert(SharedResources.save, SharedResources.exporttorootwithtimestampsuccessful, SharedResources.okay);
                    }
                    catch (Exception exception)
                    {
                        await DisplayAlert(SharedResources.error, exception.ToString(), SharedResources.ok);
                    }
                    break;
            }
        }

        public static void WriteDatabaseContentToFile(string content, string baseOutputPath)
        {
            // TODO
            /*
            var filename = "Fieldmapp_Database_" + DateTime.UtcNow.ToString(MediaSelectorElement.DateToFileFormat, CultureInfo.InvariantCulture) + ".json";

            using (var fileStream = DependencyService.Get<IStorageAccessProvider>().OpenFileWrite(Path.Combine(baseOutputPath, filename)))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(content);
            }
            */
        }


        /// <summary>
        /// Exports data of current project to a json string.
        /// </summary>
        public static async Task ExportData()
        {
            // TODO
            /*
            // get data of the project from the db
            var workingProject = Database.GetCurrentProject();
            var tableContent = Database.ReadCustomTable(ref workingProject);

            if (tableContent == null)
                return;

            // get current user
            var user = App.CurrentUser;

            JObject dataObject = new JObject();

            var mediaPath = MediaSelectorElement.MediaPath;
            var storageAccessProvider = DependencyService.Get<IStorageAccessProvider>();


            var baseTempOutputPath = Path.Combine(App.FolderLocation, "export" + DateTime.UtcNow.ToString(MediaSelectorElement.DateToFileFormat, CultureInfo.InvariantCulture));
            if (Directory.Exists(baseTempOutputPath))
                Directory.Delete(baseTempOutputPath, true);
            
            Directory.CreateDirectory(baseTempOutputPath);
            Directory.CreateDirectory(Path.Combine(baseTempOutputPath, MediaSelectorElement.ImageFolderName));

            for (var i = 0; i < tableContent.RowNameList.Count; i++)
            {
                var dataValues = tableContent.ValueList[i];
                JArray dataArray = JArray.FromObject(dataValues);
                for (int j = 0; j < dataValues.Count; j++)
                {
                    var item = dataValues[j];
                    if (item.StartsWith(mediaPath))
                    {
                        // copy file into output directory
                        var fileName = Path.GetFileName(item);
                        dataValues[j] = fileName;
                        if (File.Exists(item))
                            File.Copy(item, Path.Combine(baseTempOutputPath, MediaSelectorElement.ImageFolderName, fileName));
                    }
                }
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

            WriteDatabaseContentToFile(exportObject.ToString(), baseTempOutputPath);

            var internalZipFilePath = Path.Combine(App.FolderLocation, "export.zip");
            if (File.Exists(internalZipFilePath))
                File.Delete(internalZipFilePath);
            
            ZipFile.CreateFromDirectory(baseTempOutputPath, internalZipFilePath);

            using (var internalFileStream = storageAccessProvider.OpenFileRead(internalZipFilePath))
            using (var externalFileStream = storageAccessProvider.OpenFileWriteExternal("Fieldmapp_Export_" + DateTime.UtcNow.ToString(MediaSelectorElement.DateToFileFormat, CultureInfo.InvariantCulture) + ".zip"))
            {
                await internalFileStream.CopyToAsync(externalFileStream);
            }

            File.Delete(internalZipFilePath);

            Directory.Delete(baseTempOutputPath, true);
            */
        }
    }
}