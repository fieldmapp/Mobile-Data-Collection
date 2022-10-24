using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DlrDataApp.Modules.Base.Shared.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormattedButton : ContentButton
    {
        public static readonly BindableProperty FormattedTextProperty = BindableProperty.Create(nameof(FormattedText), typeof(FormattedString), typeof(FormattedButton), null, BindingMode.TwoWay);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(FormattedButton), null, BindingMode.TwoWay);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(FormattedButton), Color.Default, BindingMode.TwoWay);

        public FormattedButton()
        {
            InitializeComponent();
            PropertyChanged += FormattedButton_PropertyChanged;
            this.SetAppThemeColor(TextColorProperty, (Color)Application.Current.Resources["TextPrimary"], (Color)Application.Current.Resources["TextPrimaryNight"]);
            this.SetAppThemeColor(BackgroundColorProperty, (Color)Application.Current.Resources["Background"], (Color)Application.Current.Resources["BackgroundNight"]);
            this.SetAppThemeColor(BorderColorProperty, (Color)Application.Current.Resources["ButtonBorder"], (Color)Application.Current.Resources["ButtonBorderNight"]);
        }

        private void FormattedButton_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FormattedText))
            {
                ButtonTextLabel.FormattedText = FormattedText;
            }
            else if (e.PropertyName == nameof(FontSize))
            {
                ButtonTextLabel.FontSize = FontSize;
            }
            else if (e.PropertyName == nameof(TextColor))
            {
                ButtonTextLabel.TextColor = TextColor;
            }
        }

        public FormattedString FormattedText
        {
            get => (FormattedString)GetValue(FormattedTextProperty);
            set => SetValue(FormattedTextProperty, value);
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }
    }
}