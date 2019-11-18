using System;
using Xamarin.Forms.Xaml;

using Plugin.FilePicker;
using DLR_Data_App.Services;
using DLR_Data_App.Localizations;
using System.IO;
using Plugin.FilePicker.Abstractions;

namespace DLR_Data_App.Views.ProjectList
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProjectPage
    {
        private FileData File { get; set; }
        string _fileCopyPath;

        public NewProjectPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        /**
         * Pick zip file containing project
         */
        private async void btn_filepicker_Clicked(object sender, EventArgs e)
        {
            File = await CrossFilePicker.Current.PickFile();

            if (File != null)
            {
                if (File.FileName.EndsWith(".zip"))
                {
                    LblZipPath.Text = File.FileName;
                    _fileCopyPath = Path.Combine(App.FolderLocation, "Data.zip");
                    var dataArray = File.GetStream();
                    System.IO.File.Delete(_fileCopyPath);

                    using (var fileCopy = System.IO.File.Create(_fileCopyPath))
                    {
                        dataArray.CopyTo(fileCopy);
                    }
                }
                else
                {
                    await DisplayAlert(AppResources.filepicker, AppResources.filetypeerror, AppResources.cancel);
                }
            }
        }

        /**
         * Parse selected file and add project to local database
         */
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
                        await DisplayAlert(AppResources.projects, AppResources.successful, AppResources.okay);
                    }
                    else
                    {
                        await DisplayAlert(AppResources.projects, AppResources.failed, AppResources.okay);
                        await Navigation.PopModalAsync();
                    }
                }
            }
            else
            {
                await DisplayAlert(AppResources.projects, AppResources.failed, AppResources.okay);
            }
        }

        /**
         * Cancel the process to add a new project
         */
        private async void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopModalAsync();
            return true;
        }
    }
}