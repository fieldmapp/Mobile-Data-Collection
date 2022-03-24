using DLR_Data_App.Controls;
using DLR_Data_App.Models.Profiling;
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
            DisplayedItems = new ObservableCollection<DrivingConfigurationDisplay>
            {
                new DrivingConfigurationDisplay{ Configuration=DefaultConfiguration }
            };
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

        }

        private void EditSelectedConfiguratioButton_Clicked(object sender, EventArgs e)
        {
            var selectedConfiguration = (ConfigurationPicker.SelectedItem as DrivingConfigurationDisplay)?.Configuration;
            if (selectedConfiguration == null)
                return;

            var newPage = new DrivingConfigurationPage(selectedConfiguration);
            newPage.Save = () => 
                OnPropertyChanged(nameof(DisplayedItems));
            // TODO: reflect change back to displayed items list, above line does not work
            Navigation.PushModalAsync(newPage);
        }
    }
}