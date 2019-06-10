using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    class Plant : BindableObject, IInlinePickerElement
    {
        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(StadiumSubItem), Color.Default, BindingMode.OneWay);

        public String Name { get; set; }
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }
    }
}
