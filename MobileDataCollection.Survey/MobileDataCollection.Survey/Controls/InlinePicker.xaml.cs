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
        public object SelectedItem => ListView.SelectedItem;
        static readonly Color SelectedColor = Color.DarkSeaGreen;

        public InlinePicker()
        {
            InitializeComponent();
        }

        public void Scroll(object sender, EventArgs e)
        {
        }
    }
}