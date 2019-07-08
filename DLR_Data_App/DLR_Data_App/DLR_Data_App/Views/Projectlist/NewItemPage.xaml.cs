using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DLR_Data_App.Models;
using Plugin.FilePicker;
using DLR_Data_App.Services;
using DLR_Data_App.Localizations;
using System.IO;
using Plugin.FilePicker.Abstractions;

namespace DLR_Data_App.Views.Projectlist
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class NewItemPage : ContentPage
  {
    private FileData file { get; set; }
    string fileCopyPath;

    public NewItemPage()
    {
      InitializeComponent();

      BindingContext = this;
    }

    /**
     * Pick zip file containing project
     */
    private async void btn_filepicker_Clicked(object sender, EventArgs e)
    {
      //var file = await CrossFilePicker.Current.PickFile();
      file = await CrossFilePicker.Current.PickFile();

      if (file != null)
      {
        if (file.FileName.EndsWith(".zip"))
        {
          lbl_zip_path.Text = file.FileName;
          //lbl_zip_path.Text = file.FilePath;

          fileCopyPath = Path.Combine(App.FolderLocation, "Data.zip");
          Stream dataArray = file.GetStream();

          File.Delete(fileCopyPath);

          using (var filecopy = File.Create(fileCopyPath))
          {
            dataArray.CopyTo(filecopy);
          }
        } else
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
      if (file != null)
      {
        if (file.FileName.EndsWith(".zip"))
        {
          ProjectGenerator projectGenerator = new ProjectGenerator(fileCopyPath);
          bool result = await projectGenerator.GenerateProject();

          if(result)
          {
            await DisplayAlert(AppResources.projects, AppResources.successful, AppResources.okay);
            await Navigation.PopModalAsync();
          }
        }
      }
      await DisplayAlert(AppResources.projects, AppResources.failed, AppResources.okay);
    }

    /**
     * Cancel the process to add a new project
     */
    private async void Btn_cancel_Clicked(object sender, EventArgs e)
    {
      await Navigation.PopModalAsync();
    }
  }
}