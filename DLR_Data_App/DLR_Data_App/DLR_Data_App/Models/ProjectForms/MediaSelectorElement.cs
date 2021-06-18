using DLR_Data_App.Controls;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class MediaSelectorElement : FormElement
    {
        public const string ImageFolderName = "img_rec";
        public const string DateToFileFormat = "yyyyMMdd_HHmmss";
        public static string MediaPath => Path.Combine(App.FolderLocation, ImageFolderName);
        public MediaSelectorElement(Grid grid, ProjectFormElements data, string type, Func<string, string, string, Task> displayAlertFunc, Project project) : base(grid, data, type, displayAlertFunc, project) { }

        public DataHolder DataHolder;

        protected override bool IsValidElementSpecific => !string.IsNullOrEmpty(DataHolder.Data);

        public override string GetRepresentationValue() => DataHolder.Data ?? string.Empty;

        public override void LoadFromSavedRepresentation(string representation) => DataHolder.Data = representation;

        protected override void OnReset() => DataHolder.Data = string.Empty;

        public static MediaSelectorElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new MediaSelectorElement(grid, parms.Element, parms.Type, parms.DisplayAlertFunc, parms.CurrentProject);

            var pickFileButton = new Button { Text = AppResources.select };
            var fileSelectedLabel = new Label { Text = AppResources.fileselected };
            var dataHolder = new DataHolder();
            formElement.DataHolder = dataHolder;
            pickFileButton.Clicked += async (a, b) =>
            {
                var image = await DependencyService.Get<ICameraProvider>().OpenCameraApp();
                if (image != null)
                {
                    // save image into own folder
                    Directory.CreateDirectory(MediaPath);
                    if (!string.IsNullOrWhiteSpace(dataHolder.Data))
                        File.Delete(dataHolder.Data);

                    var targetFilePath = Path.Combine(MediaPath, DateTime.UtcNow.ToString(DateToFileFormat) + ".jpg");
                    using (var fileStream = DependencyService.Get<IStorageAccessProvider>().OpenFileWrite(targetFilePath))
                        await fileStream.WriteAsync(image, 0, image.Length);

                    dataHolder.Data = targetFilePath;
                    formElement.OnContentChange();
                }
            };

            grid.Children.Add(pickFileButton, 0, 1);
            Grid.SetColumnSpan(pickFileButton, 2);
            grid.Children.Add(fileSelectedLabel, 0, 2);
            grid.Children.Add(dataHolder, 1, 2);
            return formElement;
        }
    }
}
