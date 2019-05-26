using MobileDataCollection.Survey.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        QuestionDoubleSliderPage Item = new QuestionDoubleSliderPage()
        {
            Text = "Schätzen Sie den Grad der Bedeckung des Bodens durch Pflanzen (A) und den Anteil grüner Pflanzenbestandteile (B). ein",
            PictureAdresses = new[] { @"C:\Users\PC\source\repos\LayoutSlider\LayoutSlider\LayoutSlider\Bilder\bedeckung1.PNG" },
            RightAnswers = new[] { 1 }
        };
        
		public DoubleSliderPage ()
		{
			InitializeComponent();
		}
		 void OnSliderAValueChanged(object sender, ValueChangedEventArgs args)
        {
            int value = (int)(args.NewValue);
            sliderALabel.Text = String.Format("(A): {0}%", value);
        }
        void OnSliderBValueChanged(object sender, ValueChangedEventArgs args)
        {
            int value = (int)(args.NewValue);
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