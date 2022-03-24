using DLR_Data_App.Models.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Controls
{
    public class InlinePicker : ListView
    {
        static readonly Color SelectedColor = Color.DarkSeaGreen;
        const bool IndicateScrollable = true;

        public InlinePicker()
        {
            PropertyChanged += ListView_PropertyChanged;
            RowHeight = 40;
            ItemSelected += ListView_ItemSelected;
            SelectionMode = ListViewSelectionMode.Single;
        }

        private void ListView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ListView.ItemsSource) && IndicateScrollable)
                Task.Run(IndicateScroll);
        }

        private async Task IndicateScroll()
        {
            var lastItem = ItemsSource.OfType<object>().LastOrDefault();
            if (lastItem == null)
                return;
            ScrollTo(lastItem, ScrollToPosition.MakeVisible, true);
            await Task.Delay(1500);
            var firstItem = ItemsSource.OfType<object>().FirstOrDefault();
            if (firstItem == null)
                return;
            ScrollTo(firstItem, ScrollToPosition.MakeVisible, true);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Reset();
            if (e.SelectedItem is IInlinePickerElement selected)
                selected.BackgroundColor = SelectedColor;
        }

        public void Reset()
        {
            foreach (var item in ItemsSource.OfType<IInlinePickerElement>())
            {
                item.BackgroundColor = Color.Transparent;
            }
        }
    }
}