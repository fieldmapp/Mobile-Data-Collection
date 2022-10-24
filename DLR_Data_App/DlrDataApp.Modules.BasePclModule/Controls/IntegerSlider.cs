using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Controls
{
    public class IntegerSlider : Slider
    {
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
