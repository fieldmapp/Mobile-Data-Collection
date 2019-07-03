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
    public partial class DoubleSliderPage : ContentPage, ISurveyPage
    {
        //Binding für Fragen
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem), typeof(QuestionDoubleSliderPage), typeof(DoubleSliderPage), new QuestionDoubleSliderPage(1, 1, "DoubleSlider_one_question1.png", 7, 4), BindingMode.OneWay);
        //Binding für Antwort
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem),  typeof(AnswerDoubleSliderPage), typeof(DoubleSliderPage), new AnswerDoubleSliderPage(0, 0, 0), BindingMode.OneWay);
        //Binding für Header
        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(string), typeof(DoubleSliderPage), "demo", BindingMode.OneWay);

        public event EventHandler PageFinished;

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
        
        //Header
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        IQuestionContent ISurveyPage.QuestionItem => QuestionItem;

        IUserAnswer ISurveyPage.AnswerItem => AnswerItem;

        public DoubleSliderPage(QuestionDoubleSliderPage question, int answersGiven, int answersNeeded)
		{
            InitializeComponent();
            QuestionItem = question;

            Picture.BindingContext = this;
            QuestionText.BindingContext = this;
            HeaderText.BindingContext = this;
            Header = $"Frage {answersGiven}/{answersNeeded + 1}";
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
            int value = (int)args.NewValue;
            sliderALabel.Text = $"(A): {value}%";
        }
        //Displays the selected Percentage of SliderB in LabelB
        void OnSliderBValueChanged(object sender, ValueChangedEventArgs args)
        {
            int value = (int)args.NewValue;
            sliderBLabel.Text = $"(B): {value}%";
        }
        //Called when the Forward-Button is clicked
        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            int answerA = (int)sliderA.Value;
            int answerB = (int)sliderB.Value;
            AnswerItem = new AnswerDoubleSliderPage(QuestionItem.InternId, answerA, answerB);
            PageFinished?.Invoke(this, null);
        }
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
            PageFinished?.Invoke(this, null);
        }

        protected override bool OnBackButtonPressed()
        {
            PageFinished?.Invoke(this, null);
            return true;
        }
    }
}