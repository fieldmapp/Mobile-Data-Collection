using DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjectsSharedModule.Services;
using DlrDataApp.Modules.SharedModule;
using DlrDataApp.Modules.SharedModule.Localization;
using System;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectForms
{
    class MediaSelectorElement : FormElement
    {
        public MediaSelectorElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) { }

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

        public override bool IsValid => Data != null && base.IsValid;

        public override string GetRepresentationValue() => Base64Data ?? string.Empty;

        public override void LoadFromSavedRepresentation(string representation) => Base64Data = representation;

        public override void Reset() => Base64Data = null;

        public static MediaSelectorElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new MediaSelectorElement(grid, parms.Element, parms.Type);

            var pickFileButton = new Button { Text = AppResources.select };
            var fileSelectedLabel = new Label { Text = AppResources.fileselected };
            //HACK: bad way to store an image. blob would be better
            var dataHolder = new Label { Text = AppResources.no };
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
