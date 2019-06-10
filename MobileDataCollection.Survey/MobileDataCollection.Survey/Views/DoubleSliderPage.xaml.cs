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
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem), typeof(QuestionDoubleSliderPage), typeof(DoubleSliderPage), new QuestionDoubleSliderPage("Q1_G1_F1_B2_klein", ImageSource.FromResource("Q1_G1_F1_B2_klein"), 0, 0, 1), BindingMode.OneWay);
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem), typeof(AnswerDoubleSliderPage), typeof(DoubleSliderPage), new AnswerDoubleSliderPage(null, 0, 0), BindingMode.OneWay);
        
        
        //Item of the given Question
        public QuestionDoubleSliderPage QuestionItem
        {
            get { return (QuestionDoubleSliderPage)GetValue(QuestionItemProperty); }
            set { SetValue(QuestionItemProperty, value); }
        }
        //Item of the Answer
        public AnswerDoubleSliderPage AnswerItem
        {
            get { return (AnswerDoubleSliderPage)GetValue(AnswerItemProperty); }
            set { SetValue(AnswerItemProperty, value); }
        }
        
        //Zurzeit nicht verwendet
        //Definition of all the available Questions (Muss ggf woanders hinkommen -> Zugriff auf DB)
        /*public ObservableCollection<QuestionDoubleSliderPage> Items = new ObservableCollection<QuestionDoubleSliderPage>()
        {
            new QuestionDoubleSliderPage("Q3G1B1_klein.png", 7, 4, 1),
            new QuestionDoubleSliderPage("Q3G1B2_klein.png", 49, 91, 1),
            new QuestionDoubleSliderPage("Q3G1B3_klein.png", 12,3, 1),
            new QuestionDoubleSliderPage("Q3G1B4_klein.png", 64, 94, 1)
        };*/
        
        public DoubleSliderPage ()
		{
            QuestionItem.AnswersNeeded = 4;
            //this.AnswersGiven = 1;
            InitializeComponent();
            Picture.BindingContext = this;
            QuestionText.BindingContext = this;
            QuestionItem = new QuestionDoubleSliderPage("Q3G1B1_klein.png", ImageSource.FromResource("Q3G1B1_klein.png"), 7, 4, 1);
            //Picture.Source = ImageSource.FromFile("Q3G1B1_klein.png");
        }
        
        //Resets the Sliders
        void ResetSlider()
        {
            sliderA.Value = 0;
            sliderB.Value = 0;
        }
        //Displays the selected Percentage of SliderA in LabelA
		void OnSliderAValueChanged(object sender, ValueChangedEventArgs args)
        {
            int value = (int)(args.NewValue);
            sliderALabel.Text = String.Format("(A): {0}%", value);
        }
        //Displays the selected Percentage of SliderB in LabelB
        void OnSliderBValueChanged(object sender, ValueChangedEventArgs args)
        {
            int value = (int)(args.NewValue);
            sliderBLabel.Text = String.Format("(B): {0}%", value);
        }
        //Called when the Forward-Button is clicked
        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            //Checking the given answers
            int answerA = (int)(sliderA.Value);
            int answerB = (int)(sliderB.Value);
            //Analyse of the given Answer referring to the right answer
            int diffAnsA =100-Math.Abs(answerA - QuestionItem.CorrectAnswerA);
            int diffAnsB =100-Math.Abs(answerB - QuestionItem.CorrectAnswerB);
            //TODO: Use AnswerDoubleSliderPage here
            //Save submitted question as answered with result
            AnswerItem.Question = QuestionItem;
            AnswerItem.ResultQuestionA = diffAnsA;
            AnswerItem.ResultQuestionB = diffAnsB;
            /*this.Question.Answered = true;
            this.Question.Result = (diffAnsA + diffAnsB) / 2;
            this.CurrentResult = this.CurrentResult + this.Question.Result;
            DisplayAlert("Hinweis", String.Format("Aktuelles Ergebnis ist {0}%, Gesamtergebnis {1}%.", this.Question.Result, this.CurrentResult/this.AnswersGiven), "OK");*/
            //Count number of already submitted Question 
            //QuestionItem = new QuestionDoubleSliderPage("Q3G1B2_klein.png", 49, 91, 1);
            QuestionItem = new QuestionDoubleSliderPage("Q3G1B2_klein.png", ImageSource.FromResource("Q3G1B1_klein.png"), 49, 91, 1);
            //fehlt noch einfügen neuer Frage (hier nur 1 mal)
            this.ResetSlider();
        }
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
            //TBD:
            //Warning (?)
            //Return to Homescreen
        }
	}
}