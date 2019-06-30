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
        //Attributes of the DoubleSlider Layouts
        //Given Answers
        //public SurveyMenuItem survey {get ; set; }
        public int AnswersGiven { get; set; }
        //Answers needed to complete Question Category (Muss definiert werden: entweder alle verfügbaren Fragen oder feste Zahl)
        public int AnswersNeeded { get; set; }
        //Current result
        public int CurrentResult { get; set; }
        //Binding für Fragen
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem), 
            typeof(QuestionDoubleSliderPage), typeof(DoubleSliderPage), 
            new QuestionDoubleSliderPage(1, 1, "DoubleSlider_one_question1.png", 7, 4), BindingMode.OneWay);
        //Binding für Antwort
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem), 
            typeof(AnswerDoubleSliderPage), typeof(DoubleSliderPage), new AnswerDoubleSliderPage(0, 0, 0), BindingMode.OneWay);
        //Binding für Header
        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header),
            typeof(String), typeof(DoubleSliderPage), "demo", BindingMode.OneWay);

        //Currently displayed question
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

        //IntrospectionPage set at the end of answering the category
        /*
        public QuestionIntrospectionPage IntroQuestion
        {
            get; set;
        }
        */
        //Header
        public String Header
        {
            get { return (String)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private QuestionDoubleSliderPage QuestionItemTest;
        private DatabankCommunication DBCom = new DatabankCommunication();

        public DoubleSliderPage ()
		{
            this.AnswersNeeded = 4;
            this.AnswersGiven = 1;
            InitializeComponent();


            //this.survey = new SurveyMenuItem();
            //this.survey.Id=SurveyMenuItemType.DoubleSlider;
            Picture.BindingContext = this;
            QuestionText.BindingContext = this;
            QuestionNumber.BindingContext = this;
            this.Header = $"Frage {this.AnswersGiven}/{this.AnswersNeeded + 1}";

            QuestionItemTest = DBCom.LoadQuestionDoubleSliderPage(1);
            if (QuestionItemTest.InternId != 0)
            {
                QuestionItem = QuestionItemTest;
            }
            else
            {
                // TODO: What do we do if no more Questions are available
            }
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
            AnswerItem.ResultQuestionA = answerA;
            AnswerItem.ResultQuestionB = answerB;
            /*this.Question.Answered = true;
            this.Question.Result = (diffAnsA + diffAnsB) / 2;
            this.CurrentResult = this.CurrentResult + this.Question.Result;
            DisplayAlert("Hinweis", String.Format("Aktuelles Ergebnis ist {0}%, Gesamtergebnis {1}%.", this.Question.Result, this.CurrentResult/this.AnswersGiven), "OK");*/
            //Count number of already submitted Question 
            if (this.AnswersGiven < this.AnswersNeeded)
            {
                this.AnswersGiven++;
                this.Header = $"Frage {this.AnswersGiven}/{this.AnswersNeeded + 1}";
                this.ResetSlider();
                // Set new Question

                AnswerDoubleSliderPage Answer = new AnswerDoubleSliderPage(AnswerItem.InternId, AnswerItem.ResultQuestionA, AnswerItem.ResultQuestionB);
                DBCom.AddListAnswerDoubleSliderPage(Answer);

                int difficulty = 3; // TODO: implement Method which checks if Question is answered right/wrong

                QuestionItemTest = DBCom.LoadQuestionDoubleSliderPage(difficulty);
                if (QuestionItemTest.InternId != 0)
                {
                    QuestionItem = QuestionItemTest;
                }
                else
                {
                    // TODO: What do we do if no more Questions are available
                }
            }
            else if (this.AnswersGiven == this.AnswersNeeded)
            {
                //TBD: Setze Header der IntrospectionPage auf 5/5 etc. und übergebe Parameter WELCHE IntrospectionPage --> braucht eigenschaft
                //Navigation.PushAsync(new IntrospectionPage(this.survey));
                Navigation.PushAsync(new EvaluationPage((this.AnswerItem.ResultQuestionA+this.AnswerItem.ResultQuestionB)/2));
            }
            
        }
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
            //TBD:
            //Warning (?)
            //Return to Homescreen
        }
	}
}