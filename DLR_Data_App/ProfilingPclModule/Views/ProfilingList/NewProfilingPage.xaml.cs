using System;
using Xamarin.Forms.Xaml;
using DLR_Data_App.Services;
using System.IO;
using Xamarin.Essentials;
using DlrDataApp.Modules.Base.Shared.Localization;
using DlrDataApp.Modules.Profiling.Shared.Localization;

namespace DlrDataApp.Modules.Profiling.Shared.Views.ProfilingList
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

                    _fileCopyPath = Path.Combine(ProfilingModule.Instance.ModuleHost.App.FolderLocation, "Data.zip");
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
                    await DisplayAlert(SharedResources.filepicker, SharedResources.filetypeerror, SharedResources.cancel);
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
                        await DisplayAlert(ProfilingResources.profiling, ProfilingResources.profilingSuccessfullyAdded, SharedResources.okay);
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert(ProfilingResources.profiling, SharedResources.failed, SharedResources.okay);
                    }
                }
            }
            else
            {
                await DisplayAlert(ProfilingResources.profiling, SharedResources.failed, SharedResources.okay);
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