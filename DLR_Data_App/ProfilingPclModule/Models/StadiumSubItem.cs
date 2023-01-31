//Main contributors: Maximilian Enderling, Max Moebius
using DlrDataApp.Modules.Base.Shared.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Profiling.Shared.Models
{
    public class StadiumSubItem : BindableObject, IInlinePickerElement
    {
        /// <summary>
        /// Binding of the plant names in DatabankCommunication
        /// </summary>
        public static readonly BindableProperty StadiumNameProperty = BindableProperty.Create(nameof(StadiumName), typeof(string), typeof(StadiumSubItem), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(string), typeof(StadiumSubItem), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty InternNumberProperty = BindableProperty.Create(nameof(InternNumber), typeof(int), typeof(StadiumSubItem), 0, BindingMode.OneWay);
        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(StadiumSubItem), Color.Default, BindingMode.OneWay);

        /// <summary>
        /// Name of the different stadiums
        /// </summary>
        public string StadiumName
        {
            get => (string)GetValue(StadiumNameProperty);
            set => SetValue(StadiumNameProperty, value);
        }

        [JsonIgnore]
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        /// <summary>
        /// Contains the Images of the stadiums
        /// </summary>
        public string ImageSource
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        /// Internally used Number to save and recognize the object
        /// </summary>
        public int InternNumber
        {
            get => (int)GetValue(InternNumberProperty);
            set => SetValue(InternNumberProperty, value);
        }

        /// <summary>
        /// The Constructor of the plant names in DatabankCommunication
        /// </summary>
        /// <param name="stadiumName"></param>
        /// <param name="imageSource"></param>
        /// <param name="internNumber"></param>
        public StadiumSubItem(string stadiumName, string imageSource, int internNumber)
        {
            StadiumName = stadiumName;
            ImageSource = imageSource;
            InternNumber = internNumber;
        }

        public StadiumSubItem(StadiumSubItem item)
        {
            StadiumName = item.StadiumName;
            ImageSource = item.ImageSource;
            InternNumber = item.InternNumber;
        }

        /// <summary>
        /// Used for serialization only
        /// </summary>
        private StadiumSubItem() { }
    }
}
