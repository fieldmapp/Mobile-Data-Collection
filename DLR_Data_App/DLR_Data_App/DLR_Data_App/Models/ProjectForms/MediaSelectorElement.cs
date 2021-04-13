using DLR_Data_App.Controls;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class MediaSelectorElement : FormElement
    {
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
            //HACK: bad way to store an image. blob would be better
            var dataHolder = new DataHolder();
            formElement.DataHolder = dataHolder;
            pickFileButton.Clicked += async (a, b) =>
            {
                var image = await DependencyService.Get<ICameraProvider>().OpenCameraApp();
                if (image != null)
                {
                    dataHolder.Data = Convert.ToBase64String(image);
                    var length = dataHolder.Data.Length;
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
