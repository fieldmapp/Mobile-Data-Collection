using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class Plant : BindableObject
    {
        /// <summary>
        /// Binding of the plant names in DatabankCommunication
        /// </summary>
        public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(Plant), string.Empty, BindingMode.OneWay);

        /// <summary>
        /// Name of the different Plants
        /// </summary>
        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        /// <summary>
        /// The Constructor of the plant names in DatabankCommunication
        /// </summary>
        /// <param name="name"></param>
        public Plant(string name)
        {
            Name = name;
        }
    }
}
