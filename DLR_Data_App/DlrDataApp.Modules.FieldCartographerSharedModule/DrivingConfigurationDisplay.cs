using DlrDataApp.Modules.Base.Shared.Controls;

using Xamarin.Forms;
using static DlrDataApp.Modules.FieldCartographer.Shared.DrivingConfigurationPage;

namespace DlrDataApp.Modules.FieldCartographer.Shared
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
}