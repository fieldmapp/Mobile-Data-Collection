using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class StadiumSubItem : BindableObject
    {
        public static readonly BindableProperty StadiumNameProperty = BindableProperty.Create(nameof(StadiumName), typeof(string), typeof(StadiumSubItem), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(string), typeof(StadiumSubItem), string.Empty, BindingMode.OneWay);

        public string StadiumName
        {
            get => (string)GetValue(StadiumNameProperty);
            set => SetValue(StadiumNameProperty, value);
        }

        public string ImageSource
        {
            get => (string)GetValue(StadiumNameProperty);
            set => SetValue(StadiumNameProperty, value);
        }

        public StadiumSubItem(string stadiumName, string imageSource)
        {
            StadiumName = stadiumName;
            ImageSource = imageSource;
        }
    }
}
