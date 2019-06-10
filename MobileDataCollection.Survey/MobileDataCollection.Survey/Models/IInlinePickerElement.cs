using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    interface IInlinePickerElement : INotifyPropertyChanged
    {
        Color BackgroundColor { get; set; }
    }
}
