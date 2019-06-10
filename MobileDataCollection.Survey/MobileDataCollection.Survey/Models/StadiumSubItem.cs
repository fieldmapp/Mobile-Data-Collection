using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    class StadiumSubItem : BindableObject, IInlinePickerElement
    {
        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(StadiumSubItem), Color.Default, BindingMode.OneWay);

        public string StadiumName { get; set; }
        public string ImageSource { get; set; }
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }
    }
}
