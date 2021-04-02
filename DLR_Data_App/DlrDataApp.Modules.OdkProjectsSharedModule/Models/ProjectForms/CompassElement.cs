using DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjectsSharedModule.Services;
using DlrDataApp.Modules.SharedModule.Localization;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectForms
{
    class CompassElement : FormElement
    {
        public CompassElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) { }

        public double CurrentHeadingMagneticNorth;
        public double SavedHeadingMagneticNorth;
        public Label CurrentDataLabel;
        public Label SavedDataLabel;

        protected override bool IsValidElementSpecific => SavedHeadingMagneticNorth != 0;

        protected override void OnReset()
        {
            CurrentHeadingMagneticNorth = 0;
            CurrentDataLabel.Text = string.Empty;
            SavedDataLabel.Text = string.Empty;
        }

        public override string GetRepresentationValue() => SavedHeadingMagneticNorth.ToString();

        public override void LoadFromSavedRepresentation(string representation)
        {
            if (double.TryParse(representation, out double heading))
            {
                SavedHeadingMagneticNorth = heading;
                SavedDataLabel.Text = heading.ToString() + " °";
            }
        }

        public static CompassElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var compassElement = new CompassElement(grid, parms.Element, parms.Type);

            var currentCompassLabel = new Label { Text = AppResources.compass };
            compassElement.CurrentDataLabel = currentCompassLabel;
            var currentCompassDataLabel = new Label();
            OdkProjectsSharedModule.Instance.ModuleHost.App.Sensor.Compass.ReadingChanged += (_, eventArgs) =>
            {
                currentCompassDataLabel.Text = ((int)eventArgs.Reading.HeadingMagneticNorth).ToString() + " °";
                compassElement.CurrentHeadingMagneticNorth = eventArgs.Reading.HeadingMagneticNorth;
            };

            var saveButton = new Button { Text = AppResources.save };

            var savedCompassLabel = new Label { Text = AppResources.saveddata };
            compassElement.SavedDataLabel = savedCompassLabel;
            var savedCompassDataLabel = new Label();

            saveButton.Clicked += (_, b) => Device.BeginInvokeOnMainThread(() =>
            {
                savedCompassDataLabel.Text = currentCompassDataLabel.Text;
                compassElement.SavedHeadingMagneticNorth = compassElement.CurrentHeadingMagneticNorth;
                compassElement.OnContentChange();
            });

            grid.Children.Add(currentCompassLabel, 0, 1);
            grid.Children.Add(currentCompassDataLabel, 1, 1);
            grid.Children.Add(saveButton, 0, 2);
            Grid.SetColumnSpan(saveButton, 2);
            grid.Children.Add(savedCompassLabel, 0, 3);
            grid.Children.Add(savedCompassDataLabel, 1, 3);

            return compassElement;
        }
    }
}
