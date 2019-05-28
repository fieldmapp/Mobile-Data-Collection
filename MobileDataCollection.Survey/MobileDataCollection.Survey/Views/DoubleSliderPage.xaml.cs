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

        public ObservableCollection<QuestionDoubleSliderPage> Items = new ObservableCollection<QuestionDoubleSliderPage>()
        {
            new QuestionDoubleSliderPage("Q3G1B1_klein.png", 20, 40),
            new QuestionDoubleSliderPage("Q3G1B2_klein.png", 50, 50),
            new QuestionDoubleSliderPage("Q3G1B3_klein.png", 25,45),
            new QuestionDoubleSliderPage("Q3G1B4_klein.png", 60, 30)
        };
        public int AnswersNeeded { get; set; }
        public QuestionDoubleSliderPage Question { get; set; }
        public DoubleSliderPage ()
		{
            this.AnswersNeeded = Items.Count;
            this.AnswersGiven = 1;
            InitializeComponent();
            this.SetQuestion();
        }
        void SetQuestion()
        {
            this.Question = Items[this.AnswersGiven - 1];

            this.Header = String.Format("Frage: {0}/{1}", this.AnswersGiven, this.AnswersNeeded); 
            this.PictureSource = Question.PictureAddress;
            this.Text = Question.Text;

            QuestionNumber.Text = this.Header;
            QuestionText.Text = this.Text;
            Picture.Source = this.PictureSource;
        }
        void ResetSlider()
        {
            sliderA.Value = 0;
            sliderB.Value = 0;
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
            if (this.AnswersGiven < this.AnswersNeeded) this.AnswersGiven++;
            this.SetQuestion();
            this.ResetSlider();
        }
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
        }
	}
}