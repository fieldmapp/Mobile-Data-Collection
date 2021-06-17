using DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjectsSharedModule.Services;
using DlrDataApp.Modules.SharedModule.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectForms
{
    class PickerElement : FormElement
    {
        public PickerElement(Grid grid, ProjectFormElements data, string type, Func<string, string, string, Task> displayAlertFunc, Project project) : base(grid, data, type, displayAlertFunc, project) { }

        public Picker Picker;

        protected override bool IsValidElementSpecific => Picker.SelectedIndex >= 0;

        public override string GetRepresentationValue() => Picker.SelectedIndex.ToString();

        public override void LoadFromSavedRepresentation(string representation)
        {
            if (int.TryParse(representation, out int value))
                Picker.SelectedIndex = value;
        }

        protected override void OnReset() => Picker.SelectedIndex = -1;

        public static PickerElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new PickerElement(grid, parms.Element, parms.Type, parms.DisplayAlertFunc, parms.CurrentProject);

            var optionsList = new List<string>();
            var options = ProjectParser.ParseOptionsFromJson(parms.Element.Options);
            var title = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(parms.Element.Label, parms.CurrentProject.Languages);
            if (string.IsNullOrWhiteSpace(title))
            {
                title = SharedResources.notitle;
            }
            var currentLanguageCode = OdkDataExtractor.GetCurrentLanguageCodeFromJsonList(parms.CurrentProject.Languages);
            foreach (var option in options)
            {
                option.Text.TryGetValue(currentLanguageCode, out var value);
                optionsList.Add(value);
            }
            optionsList.Add(SharedResources.unknown);

            var picker = new Picker
            {
                Title = title,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                ItemsSource = optionsList
            };

            formElement.Picker = picker;

            picker.SelectedIndexChanged += (a, b) => formElement.OnContentChange();

            grid.Children.Add(picker, 0, 1);
            Grid.SetColumnSpan(picker, 2);

            return formElement;
        }
    }
}
