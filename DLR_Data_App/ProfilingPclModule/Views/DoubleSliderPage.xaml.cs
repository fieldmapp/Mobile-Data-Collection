//Main contributors: Maya Koehnen
using DlrDataApp.Modules.Profiling.Shared.Localization;
using DlrDataApp.Modules.Profiling.Shared.Models;
using DlrDataApp.Modules.Base.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DlrDataApp.Modules.Profiling.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DoubleSliderPage : ContentPage, IProfilingPage
    {
        /// <summary>
        /// Bindings of QuestionItem, AnswerItem and Header
        /// </summary>
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem), typeof(QuestionDoubleSliderPage), typeof(DoubleSliderPage), new QuestionDoubleSliderPage(1, 1, string.Empty, 7, 4), BindingMode.OneWay);
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem),  typeof(AnswerDoubleSliderPage), typeof(DoubleSliderPage), null, BindingMode.OneWay);
        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(string), typeof(DoubleSliderPage), string.Empty, BindingMode.OneWay);

        public event EventHandler<PageResult> PageFinished;

        /// <summary>
        /// Item of the given Question
        /// </summary>
        public QuestionDoubleSliderPage QuestionItem
        {
            get { return (QuestionDoubleSliderPage)GetValue(QuestionItemProperty); }
            set { SetValue(QuestionItemProperty, value); }
        }
        
        /// <summary>
        /// Item of the corresponding answer of the question
        /// </summary>
        public AnswerDoubleSliderPage AnswerItem
        {
            get { return (AnswerDoubleSliderPage)GetValue(AnswerItemProperty); }
            set { SetValue(AnswerItemProperty, value); }
        }

        /// <summary>
        /// Item of the Header (given answers and number of answers that are missing)
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly BindableProperty EvaluationTextColorProperty = BindableProperty.Create(nameof(EvaluationTextColor), typeof(Color), typeof(StadiumPage), Color.Transparent, BindingMode.OneWay);

        /// <summary>
        /// Color for the Evaluation Button
        /// </summary>
        public Color EvaluationTextColor
        {
            get { return (Color)GetValue(EvaluationTextColorProperty); }
            set { SetValue(EvaluationTextColorProperty, value); }
        }

        IQuestionContent IProfilingPage.QuestionItem => QuestionItem;

        IUserAnswer IProfilingPage.AnswerItem => AnswerItem;

        public DoubleSliderPage(QuestionDoubleSliderPage question, int answersGiven, int answersNeeded)
		{
            InitializeComponent();
            QuestionItem = question;
            
            Picture.BindingContext = this;
            QuestionText.BindingContext = this;
            HeaderText.BindingContext = this;
            Header = string.Format(ProfilingResources.questionEntryFormat, answersGiven + 1, answersNeeded, question.InternId);
            EvalButton.BindingContext = this;
            EvaluationTextColor = answersGiven >= answersNeeded ? Color.Green : Color.LightGray;
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
        private Boolean hintNoticed = false;
        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            int answerA = (int)sliderA.Value;
            int answerB = (int)sliderB.Value;

            if(sliderA.Value == 0 && sliderB.Value == 0 && !hintNoticed)
            {
                DisplayAlert(ProfilingResources.hint, ProfilingResources.selectionCorrectQuestion, SharedResources.ok);
                hintNoticed = true;
                return;
            }

            AnswerItem = new AnswerDoubleSliderPage(QuestionItem.InternId, answerA, answerB);
            PageFinished?.Invoke(this, PageResult.Continue);
        }
        void OnAuswertungButtonClicked(object sender, EventArgs e)
        {
            PageFinished?.Invoke(this, PageResult.Evaluation);
        }

        protected override bool OnBackButtonPressed()
        {
            PageFinished?.Invoke(this, PageResult.Abort);
            return true;
        }
    }
}