using DlrDataApp.Modules.Base.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static DlrDataApp.Modules.FieldCartographer.Shared.DrivingConfigurationPage;

namespace DlrDataApp.Modules.FieldCartographer.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrivingConfigurationSelectionPage : ContentPage
    {
        public ObservableCollection<DrivingConfigurationDisplay> DisplayedItems { get; set; }

        public DrivingConfigurationSelectionPage()
        {
            var configurations = FieldCartographerModule.Instance.Database.Read<DrivingPageConfigurationDTO>().Select(c => c.DrivingPageConfiguration).ToList();
            if (configurations.Count == 0)
            {
                configurations = new List<DrivingPageConfiguration>() { DrivingPageConfiguration.DefaultConfiguration };
                var dto = new DrivingPageConfigurationDTO(DrivingPageConfiguration.DefaultConfiguration);
                FieldCartographerModule.Instance.Database.InsertOrUpdate(dto);
                DrivingPageConfiguration.DefaultConfiguration.Id = dto.Id;
            }
            DisplayedItems = new ObservableCollection<DrivingConfigurationDisplay>(
                configurations.Select(c => new DrivingConfigurationDisplay { Configuration = c }));
            InitializeComponent();
        }

        private void UseSelectedConfigurationButton_Clicked(object sender, EventArgs e)
        {
            var selectedConfiguration = (ConfigurationPicker.SelectedItem as DrivingConfigurationDisplay)?.Configuration;
            if (selectedConfiguration == null)
                return;

            if (selectedConfiguration.GetCauses().All(c => string.IsNullOrWhiteSpace(c.Id)))
            {
                Shell.Current.DisplayAlert(SharedResources.error, "Die gewählte Konfiguration enthält keine Minderertragsursache. Bitte die KONFIGURATION ANPASSEN durch das Ergänzen mindestens einer Minderertragsursache oder eine andere Konfiguration auswählen.", SharedResources.ok);
                return;
            }

            if (string.IsNullOrWhiteSpace(selectedConfiguration.LaneWidth))
            {
                Shell.Current.DisplayAlert(SharedResources.error, "Die Bearbeitungsbreite wurde noch nicht angegeben. Bitte die KONFIGURATION ANPASSEN durch das Ergänzen der Bearbeitungsbreite oder eine andere Konfiguration auswählen.", SharedResources.ok);
                return;
            }

            if (string.IsNullOrWhiteSpace(selectedConfiguration.GpsAntennaToInputLocationOffset))
            {
                Shell.Current.DisplayAlert(SharedResources.error, "Der Versatz zwischen dem Lotfußpunkt der Antenne und dem Referenzpunkt für die Datenaufnahme wurde noch nicht angegeben. Bitte die KONFIGURATION ANPASSEN durch das Ergänzen des Versatzes oder eine andere Konfiguration auswählen. Sollte es keinen Versatz geben, bitte „0“ eintragen.", SharedResources.ok);
                return;
            }

            var newPage = new DrivingPage(selectedConfiguration);
            Navigation.PushAsync(newPage);
        }

        private void NewConfigurationButton_Clicked(object sender, EventArgs e)
        {
            var newConfiguration = new DrivingPageConfiguration();
            var newPage = new DrivingConfigurationPage(newConfiguration)
            {
                Save = () => DisplayedItems.Add(new DrivingConfigurationDisplay { Configuration = newConfiguration })
            };
            Navigation.PushModalAsync(newPage);
        }

        private void EditSelectedConfiguratioButton_Clicked(object sender, EventArgs e)
        {
            var selectedConfiguration = (ConfigurationPicker.SelectedItem as DrivingConfigurationDisplay)?.Configuration;
            if (selectedConfiguration == null)
                return;

            var newPage = new DrivingConfigurationPage(selectedConfiguration);
            Navigation.PushModalAsync(newPage);
        }
    }
}