//Main contributors: Maximilian Enderling, Max Moebius
using DlrDataApp.Modules.SharedModule.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.ProfilingSharedModule.Models
{
    public class Plant : BindableObject, IInlinePickerElement
    {
        /// <summary>
        /// Binding of the plant names in DatabankCommunication
        /// </summary>
        public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(Plant), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty InternLetterProperty = BindableProperty.Create(nameof(InternLetter), typeof(string), typeof(Plant), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(Plant), Color.Default, BindingMode.OneWay);

        /// <summary>
        /// Name of the different Plants
        /// </summary>
        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        /// <summary>
        /// Internally used letter to save and recognize this object
        /// </summary>
        public string InternLetter
        {
            get => (string)GetValue(InternLetterProperty);
            set => SetValue(InternLetterProperty, value);
        }

        [JsonIgnore]
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        /// <summary>
        /// The Constructor of the plant names in DatabankCommunication
        /// </summary>
        /// <param name="name"></param>
        public Plant(string name, string internLetter)
        {
            Name = name;
            InternLetter = internLetter;
        }

        public Plant(Plant plant)
        {
            Name = plant.Name;
            InternLetter = plant.InternLetter;
        }

        /// <summary>
        /// Used for serialization only
        /// </summary>
        public Plant() { }
    }
}
