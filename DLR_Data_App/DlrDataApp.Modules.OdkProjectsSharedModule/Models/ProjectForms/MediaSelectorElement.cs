using DLR_Data_App.Controls;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjectsSharedModule.Services;
using DlrDataApp.Modules.SharedModule;
using DlrDataApp.Modules.SharedModule.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectForms
{
    class MediaSelectorElement : FormElement
    {
        public MediaSelectorElement(Grid grid, ProjectFormElements data, string type, Func<string, string, string, Task> displayAlertFunc, Project project) : base(grid, data, type, displayAlertFunc, project) { }

        string _base64Data;
        public string Base64Data 
        { 
            get => _base64Data;
            set
            {
                if (string.IsNullOrWhiteSpace(Base64Data))
                {
                    _base64Data = null;
                }
                else
                {
                    _base64Data = value;
                }
            }
        }

        protected override bool IsValidElementSpecific => !string.IsNullOrEmpty(DataHolder.Data);

        public override string GetRepresentationValue() => Base64Data ?? string.Empty;

        public override void LoadFromSavedRepresentation(string representation) => Base64Data = representation;

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
                    formElement.Base64Data = Convert.ToBase64String(image);
                    dataHolder.Text = AppResources.yes;
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
