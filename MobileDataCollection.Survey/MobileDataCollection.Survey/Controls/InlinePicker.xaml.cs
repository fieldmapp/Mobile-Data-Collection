using MobileDataCollection.Survey.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDataCollection.Survey.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InlinePicker : ContentView
    {
        public DataTemplate ItemTemplate { get => ListView.ItemTemplate; set => ListView.ItemTemplate = value; }
        public IEnumerable ItemSource { get => ListView.ItemsSource; set => ListView.ItemsSource = value; }
        static readonly Color SelectedColor = Color.Gray;

        public InlinePicker()
        {
            InitializeComponent();
        }

        public void Scroll(object sender, EventArgs e)
        {
        }
        public void ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            foreach (var item in ItemSource.Cast<IInlinePickerElement>())
            {
                item.BackgroundColor = Color.Transparent;
            }
            var selected = (IInlinePickerElement)e.SelectedItem;
            selected.BackgroundColor = SelectedColor;
        }

    }
}