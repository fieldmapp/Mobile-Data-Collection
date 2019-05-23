using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDataCollection.Survey.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DoubleSliderPage : ContentPage
	{
		public DoubleSliderPage ()
		{
			InitializeComponent();
		}
		 void OnSliderAValueChanged(object sender, ValueChangedEventArgs args)
        {
            double value = args.NewValue;
            sliderALabel.Text = String.Format("(A): {0}%", value);
        }
        void OnSliderBValueChanged(object sender, ValueChangedEventArgs args)
        {
            double value = args.NewValue;
            sliderBLabel.Text = String.Format("(B): {0}%", value);
        }
        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
        }
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
        }
	}
}