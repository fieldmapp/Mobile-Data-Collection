using DLR_Data_App.Controls;
using DLR_Data_App.Models.Profiling;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static DLR_Data_App.Views.DrivingConfigurationPage;

namespace DLR_Data_App.Views.DrivingView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrivingConfigurationSelectionPage : ContentPage
    {
        public class DrivingConfigurationDisplay : BindableObject, IInlinePickerElement
        {
            public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(DrivingConfigurationPage), Color.Transparent, BindingMode.OneWay);
            public Color BackgroundColor
            {
                get => (Color)GetValue(BackgroundColorProperty);
                set => SetValue(BackgroundColorProperty, value);
            }
            public DrivingPageConfiguration Configuration { get; set; }
        }

        public ObservableCollection<DrivingConfigurationDisplay> DisplayedItems { get; set; }

        public DrivingConfigurationSelectionPage()
        {
            var configurations = Database.ReadAll<DrivingPageConfigurationDTO>().Select(c => c.DrivingPageConfiguration).ToList();
            if (configurations.Count == 0)
            {
                configurations = new List<DrivingPageConfiguration>() { DefaultConfiguration };
                var dto = new DrivingPageConfigurationDTO(DefaultConfiguration);
                Database.Update(ref dto);
                DefaultConfiguration.Id = dto.Id;
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