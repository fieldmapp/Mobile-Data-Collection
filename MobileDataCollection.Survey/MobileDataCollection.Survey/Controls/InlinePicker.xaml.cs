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

        public InlinePicker()
        {
            InitializeComponent();
        }

        public void Scroll(object sender, EventArgs e)
        {
        }
        public void Tapped(object sender, SelectedItemChangedEventArgs e)
        {
            ContentButton selected =(ContentButton)e.SelectedItem;
            selected.BackgroundColor = Color.LightGray;
        }

    }
}