using DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjects.Shared.Services;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectForms
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

        protected override bool IsValidElementSpecific => !string.IsNullOrEmpty(Base64Data);

        public override string GetRepresentationValue() => Base64Data ?? string.Empty;

        public override void LoadFromSavedRepresentation(string representation) => Base64Data = representation;

        protected override void OnReset() => Base64Data = string.Empty;

        public static MediaSelectorElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new MediaSelectorElement(grid, parms.Element, parms.Type, parms.DisplayAlertFunc, parms.CurrentProject);

            var pickFileButton = new Button { Text = SharedResources.select };
            var fileSelectedLabel = new Label { Text = SharedResources.fileselected };
            var fileSelectedDataLabel = new Label { Text = SharedResources.no };

            pickFileButton.Clicked += async (a, b) =>
            {
                var image = await DependencyService.Get<ICameraProvider>().OpenCameraApp();
                if (image != null)
                {
                    formElement.Base64Data = Convert.ToBase64String(image);
                    fileSelectedDataLabel.Text = SharedResources.yes;
                    formElement.OnContentChange();
                }
            };

            grid.Children.Add(pickFileButton, 0, 1);
            Grid.SetColumnSpan(pickFileButton, 2);
            grid.Children.Add(fileSelectedLabel, 0, 2);
            grid.Children.Add(fileSelectedDataLabel, 1, 2);
            return formElement;
        }
    }
}
