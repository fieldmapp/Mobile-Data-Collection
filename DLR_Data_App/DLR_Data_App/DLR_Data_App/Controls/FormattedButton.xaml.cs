using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormattedButton : ContentButton
    {
        public static readonly BindableProperty FormattedTextProperty = BindableProperty.Create(nameof(FormattedText), typeof(FormattedString), typeof(FormattedButton), null, BindingMode.TwoWay);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(FormattedButton), null, BindingMode.TwoWay);

        public FormattedButton()
        {
            InitializeComponent();
            PropertyChanged += FormattedButton_PropertyChanged;
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
    }
}