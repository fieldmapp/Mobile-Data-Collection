using System;
using Xamarin.Forms.Xaml;

using Plugin.FilePicker;
using DLR_Data_App.Services;
using Xamarin.Essentials;
using DlrDataApp.Modules.SharedModule.Localization;
using Xamarin.Essentials;

namespace DlrDataApp.Modules.OdkProjectsSharedModule.Views.ProjectList
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProjectPage
    {
        private FileResult File { get; set; }
        string _fileCopyPath;

        public NewProjectPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        /// <summary>
        /// Pick zip file containing project
        /// </summary>
        private async void btn_filepicker_Clicked(object sender, EventArgs e)
        {
            File = await FilePicker.PickAsync();

            if (File != null)
            {
                if (File.FileName.EndsWith(".zip"))
                {
                    _fileCopyPath = Path.Combine(OdkProjectsSharedModule.Instance.ModuleHost.App.FolderLocation, "Data.zip");
                    using (var dataArray = await File.OpenReadAsync())
                    using (var dataArray = File.GetStream())
                    {
                        System.IO.File.Delete(_fileCopyPath);

                        using (var fileCopy = System.IO.File.Create(_fileCopyPath))
                        {
                            dataArray.CopyTo(fileCopy);
                        }
                    }
                }
                else
                {
                    await DisplayAlert(AppResources.filepicker, AppResources.filetypeerror, AppResources.cancel);
                }
            }
        }

        /// <summary>
        /// Parse selected file and add project to local database
        /// </summary>
        private async void Btn_save_Clicked(object sender, EventArgs e)
        {
            if (File != null)
            {
                if (File.FileName.EndsWith(".zip"))
                {
                    var projectGenerator = new ProjectGenerator(_fileCopyPath);
                    var result = await projectGenerator.GenerateProject();

                    if (result)
                    {
                        await DisplayAlert(Localization.AppResources.projects, Localization.AppResources.projectSuccessfullyAdded, AppResources.okay);
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert(Localization.AppResources.projects, AppResources.failed, AppResources.okay);
                    }
                }
            }
            else
            {
                await DisplayAlert(Localization.AppResources.projects, AppResources.failed, AppResources.okay);
            }
        }

        /// <summary>
        /// Cancel the process to add a new project
        /// </summary>
        private async void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}