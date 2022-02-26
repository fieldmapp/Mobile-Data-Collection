using System;
using Xamarin.Forms.Xaml;

using DLR_Data_App.Services;
using DLR_Data_App.Localizations;
using System.IO;
using Xamarin.Essentials;

namespace DLR_Data_App.Views.ProfilingList
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProfilingPage
    {
        private FileResult SelectedFile { get; set; }
        string _fileCopyPath;

        public NewProfilingPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        /// <summary>
        /// Pick zip file containing project
        /// </summary>
        private async void btn_filepicker_Clicked(object sender, EventArgs e)
        {
            SelectedFile = await FilePicker.PickAsync();

            if (SelectedFile != null)
            {
                if (SelectedFile.FileName.EndsWith(".zip"))
                {
                    LblZipPath.Text = SelectedFile.FileName;
                    _fileCopyPath = Path.Combine(App.FolderLocation, "Data.zip");
                    using (var dataArray = await SelectedFile.OpenReadAsync())
                    {
                        File.Delete(_fileCopyPath);

                        using (var fileCopy = File.Create(_fileCopyPath))
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
            if (SelectedFile != null)
            {
                if (SelectedFile.FileName.EndsWith(".zip"))
                {
                    var success = await ProfilingGenerator.GenerateProfiling(_fileCopyPath);

                    if (success)
                    {
                        await DisplayAlert(AppResources.projects, AppResources.projectSuccessfullyAdded, AppResources.okay);
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert(AppResources.projects, AppResources.failed, AppResources.okay);
                    }
                }
            }
            else
            {
                await DisplayAlert(AppResources.projects, AppResources.failed, AppResources.okay);
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