using System;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.IO;
using DlrDataApp.Modules.Base.Shared.Localization;
using DlrDataApp.Modules.OdkProjects.Shared.Services;
using DlrDataApp.Modules.OdkProjects.Shared.Localization;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjects.Shared.Views.ProjectList
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProjectPage : ContentPage
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
                    LblZipPath.Text = File.FileName;

                    _fileCopyPath = Path.Combine(OdkProjectsModule.Instance.ModuleHost.App.FolderLocation, "Data.zip");
                    using (var dataArray = await File.OpenReadAsync())
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
                    await DisplayAlert(SharedResources.filepicker, SharedResources.filetypeerror, SharedResources.cancel);
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
                        await DisplayAlert(OdkProjectsResources.projects, OdkProjectsResources.projectSuccessfullyAdded, SharedResources.okay);
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert(OdkProjectsResources.projects, SharedResources.failed, SharedResources.okay);
                    }
                }
            }
            else
            {
                await DisplayAlert(OdkProjectsResources.projects, SharedResources.failed, SharedResources.okay);
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