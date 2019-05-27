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
        public int AnswersGiven { get; set; }
        public String Header { get; set; }
        public String Text { get; set; }
        public String PictureSource { get; set; }

        QuestionDoubleSliderPage Item = new QuestionDoubleSliderPage()
        {
            Text = "Schätzen Sie den Grad der Bedeckung des Bodens durch Pflanzen (A) und den Anteil grüner Pflanzenbestandteile (B) ein.",
            PictureAdresses = new[] { "Q3G1B1_klein.png", "Q3G1B2_klein.png", "Q3G1B3_klein.png", "Q3G1B4_klein.png" },
            RightAnswers = new[] { 1 }
        };
        public DoubleSliderPage ()
		{
            this.AnswersGiven = 1;
            this.Header = String.Format("Frage: {0}/{1}", this.AnswersGiven, Item.AnswersNeeded); ;
            this.PictureSource= Item.PictureAdresses[this.AnswersGiven - 1];
            this.Text = Item.Text;
            InitializeComponent();
            QuestionNumber.Text = this.Header;
            QuestionText.Text = this.Text;
            Picture.Source = this.PictureSource;
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
            if(this.AnswersGiven<Item.AnswersNeeded)this.AnswersGiven++;
            this.Header = String.Format("Frage: {0}/{1}", this.AnswersGiven, Item.AnswersNeeded); ;
            this.PictureSource = Item.PictureAdresses[this.AnswersGiven - 1];
            QuestionNumber.Text = this.Header;
            QuestionText.Text = this.Text;
            Picture.Source = this.PictureSource;
        }
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
        }
	}
}