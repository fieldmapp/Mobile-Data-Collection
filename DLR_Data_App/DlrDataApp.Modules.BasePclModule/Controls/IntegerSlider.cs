using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared.Controls
{
    /// <summary>
    /// A <see cref="Slider"/> which steps are integers in a predefined range
    /// </summary>
    public class IntegerSlider : Slider
    {
        /// <summary>
        /// Currently selected value
        /// </summary>
        public int IntegerValue
        {
            get { return (int)GetValue(IntegerValueProperty); }
            set 
            { 
                SetValue(IntegerValueProperty, value);
                SetValue(ValueProperty, value);
            }
        }
        public static readonly BindableProperty IntegerValueProperty = BindableProperty.Create(nameof(IntegerValue), typeof(int), typeof(IntegerSlider), 0, BindingMode.TwoWay);
        public IntegerSlider()
        {
            ValueChanged += OnSliderValueChanged;
        }
        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            var roundedValue = (int)Math.Round(e.NewValue);

            Value = roundedValue;
            IntegerValue = roundedValue;
        }
    }
}
