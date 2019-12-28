//Main contributors: Maximilian Enderling, Maya Koehnen
using DLR_Data_App.Models.Survey;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Controls
{
    /// <summary>
    /// View which works like a picker, but without using popups. Works best when the children implements the <see cref="IInlinePickerElement"/> interface.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InlinePicker : ContentView
    {
        public DataTemplate ItemTemplate { get => ListView.ItemTemplate; set => ListView.ItemTemplate = value; }
        public IEnumerable ItemSource { get => ListView.ItemsSource; set => ListView.ItemsSource = value; }
        public object SelectedItem => ListView.SelectedItem;
        static readonly Color SelectedColor = Color.DarkSeaGreen;
        const bool IndicateScrollable = true;

        public InlinePicker()
        {
            InitializeComponent();
            ListView.PropertyChanged += ListView_PropertyChanged;
        }

        private void ListView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ListView.ItemsSource) && IndicateScrollable)
                Task.Run(IndicateScroll);
        }

        private async Task IndicateScroll()
        {
            var lastItem = ItemSource.OfType<object>().LastOrDefault();
            if (lastItem == null)
                return;
            ListView.ScrollTo(lastItem, ScrollToPosition.MakeVisible, true);
            await Task.Delay(1500);
            var firstItem = ItemSource.OfType<object>().FirstOrDefault();
            if (firstItem == null)
                return;
            ListView.ScrollTo(firstItem, ScrollToPosition.MakeVisible, true);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Reset();
            if (e.SelectedItem is IInlinePickerElement selected)
                selected.BackgroundColor = SelectedColor;
        }

        public void Reset()
        {
            foreach (var item in ItemSource.OfType<IInlinePickerElement>())
            {
                item.BackgroundColor = Color.Transparent;
            }
        }
    }
}