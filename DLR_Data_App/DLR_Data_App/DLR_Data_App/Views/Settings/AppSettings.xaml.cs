using DLR_Data_App.Views.Login;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.DependencyServices;
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
            InitializeComponent();
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
                    await Shell.Current.DisplayAlert(SharedResources.privacypolicy, SharedResources.privacytext1, SharedResources.okay);
                    break;
                case 1:
                    // Remove Database
                    answer = await Shell.Current.DisplayAlert(SharedResources.removedatabase, SharedResources.removedatabasewarning, SharedResources.accept, SharedResources.cancel);
                    if(answer)
                    {
                        if (Directory.Exists(App.Current.MediaPath))
                            Directory.Delete(App.Current.MediaPath, true);

                        try
                        {
                            // Successful
                            File.Delete(App.DatabaseLocation);
                            await DisplayAlert(SharedResources.removedatabase, SharedResources.successful, SharedResources.okay);
                            App.Current.MainPage = new LoginPage();
                        }
                        catch (Exception)
                        {
                            // Failure
                            await DisplayAlert(SharedResources.removedatabase, SharedResources.failed, SharedResources.cancel);
                        }
                    }
                    break;
                case 2:
                    // Export Database
                    try
                    {
                        await ExportData();
                        await Shell.Current.DisplayAlert(SharedResources.save, SharedResources.exporttorootwithtimestampsuccessful, SharedResources.okay);
                    }
                    catch (Exception exception)
                    {
                        await Shell.Current.DisplayAlert(SharedResources.error, exception.ToString(), SharedResources.ok);
                    }
                    break;
            }
        }


        /// <summary>
        /// Exports the whole sqlite database
        /// </summary>
        public static async Task ExportData()
        {
            var baseTempOutputPath = Path.Combine(App.Current.FolderLocation, "export" + Helpers.GetSafeIdentifier(DateTime.UtcNow));

            var databasePath = App.DatabaseLocation;

            var storageAccessProvider = DependencyService.Get<IStorageAccessProvider>();


            if (Directory.Exists(baseTempOutputPath))
                Directory.Delete(baseTempOutputPath, true);
            
            Directory.CreateDirectory(baseTempOutputPath);
            var mediaTargetPath = Path.Combine(baseTempOutputPath, "media");
            Directory.CreateDirectory(mediaTargetPath);
            var targetDatabasePath = Path.Combine(baseTempOutputPath, "database.sqlite");

            using (var internalFileStream = storageAccessProvider.OpenFileRead(databasePath))
            using (var externalFileStream = storageAccessProvider.OpenFileWrite(targetDatabasePath))
            {
                await internalFileStream.CopyToAsync(externalFileStream);
            }


            Helpers.CopyDirectory(App.Current.MediaPath, mediaTargetPath, true);


            var internalZipFilePath = Path.Combine(App.Current.FolderLocation, "export.zip");
            if (File.Exists(internalZipFilePath))
                File.Delete(internalZipFilePath);
            
            ZipFile.CreateFromDirectory(baseTempOutputPath, internalZipFilePath);

            using (var internalFileStream = storageAccessProvider.OpenFileRead(internalZipFilePath))
            using (var externalFileStream = storageAccessProvider.OpenFileWriteExternal("Fieldmapp_Export_" + Helpers.GetSafeIdentifier(DateTime.UtcNow) + ".zip"))
            {
                await internalFileStream.CopyToAsync(externalFileStream);
            }

            File.Delete(internalZipFilePath);

            Directory.Delete(baseTempOutputPath, true);
        }
    }
}