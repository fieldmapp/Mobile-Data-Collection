﻿using MobileDataCollection.Survey.Models;
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
        public int AnswersGiven { get; set; }
        //Current result
        public int CurrentResult { get; set; }
        //Header
        public String Header { get; set; }
        //Question-Text
        public String Text { get; set; }
        //Source of the displayed picture
        public String PictureSource { get; set; }
        //Answers needed to complete Question Category (Muss definiert werden: entweder alle verfügbaren Fragen oder feste Zahl)
        public int AnswersNeeded { get; set; }
        //Currently displayed question
        public QuestionDoubleSliderPage Question { get; set; }
        //Definition of all the available Questions (Muss ggf woanders hinkommen -> Zugriff auf DB)
        public ObservableCollection<QuestionDoubleSliderPage> Items = new ObservableCollection<QuestionDoubleSliderPage>()
        {
            new QuestionDoubleSliderPage("Q3G1B1_klein.png", 7, 4, 1),
            new QuestionDoubleSliderPage("Q3G1B2_klein.png", 49, 91, 1),
            new QuestionDoubleSliderPage("Q3G1B3_klein.png", 12,3, 1),
            new QuestionDoubleSliderPage("Q3G1B4_klein.png", 64, 94, 1)
        };
        
        public DoubleSliderPage ()
		{
            this.AnswersNeeded = Items.Count;
            this.AnswersGiven = 1;
            InitializeComponent();
            this.SetQuestion();
        }
        //Sets the a new Question
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
            int diffAnsA =100-Math.Abs(answerA - this.Question.CorrectAnswerA);
            int diffAnsB =100-Math.Abs(answerB - this.Question.CorrectAnswerB);
            //TODO: Use AnswerDoubleSliderPage here
            //Save submitted question as answered with result
            /*this.Question.Answered = true;
            this.Question.Result = (diffAnsA + diffAnsB) / 2;
            this.CurrentResult = this.CurrentResult + this.Question.Result;
            DisplayAlert("Hinweis", String.Format("Aktuelles Ergebnis ist {0}%, Gesamtergebnis {1}%.", this.Question.Result, this.CurrentResult/this.AnswersGiven), "OK");*/
            //Count number of already submitted Question 
            if (this.AnswersGiven < this.AnswersNeeded) this.AnswersGiven++;
            //Set new Question
            this.SetQuestion();
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