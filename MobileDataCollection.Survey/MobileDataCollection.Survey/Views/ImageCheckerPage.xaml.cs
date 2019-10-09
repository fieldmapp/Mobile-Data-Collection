//Main contributors: Max Moebius, Henning Woydt
using MobileDataCollection.Survey.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDataCollection.Survey.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageCheckerPage : ContentPage, ISurveyPage
    {
        /// <summary>
        /// Bindings of QuestionItem, AnswerItem and Header
        /// </summary>
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem), typeof(QuestionImageCheckerPage), typeof(ImageCheckerPage), new QuestionImageCheckerPage(1, "Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 0, 1, 0, "Q1_G1_F1_B1_klein.png", "Q1_G1_F1_B2_klein.png", "Q1_G1_F1_B3_klein.png", "Q1_G1_F1_B4_klein.png"), BindingMode.OneWay);
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem), typeof(AnswerImageCheckerPage), typeof(ImageCheckerPage), null);
        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(string), typeof(ImageCheckerPage), "demo", BindingMode.OneWay);

        /// <summary>
        /// Item of the given Question
        /// </summary>
        public QuestionImageCheckerPage QuestionItem
        {
            get { return (QuestionImageCheckerPage)GetValue(QuestionItemProperty); }
            set { SetValue(QuestionItemProperty, value); }
        }

        /// <summary>
        /// Item of the corresponding answer of the question
        /// </summary>
        public AnswerImageCheckerPage AnswerItem
        {
            get { return (AnswerImageCheckerPage)GetValue(AnswerItemProperty); }
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


        IQuestionContent ISurveyPage.QuestionItem => QuestionItem;

        IUserAnswer ISurveyPage.AnswerItem => AnswerItem;

        /// <summary>
        /// the selected Color for the pictures
        /// </summary>
        Color selectedColor = Color.DarkSeaGreen;

        /// <summary>
        /// the unselected Color for the pictures
        /// </summary>
        Color nonSelectedColor = Color.White;

        public event EventHandler<PageResult> PageFinished;

        public ImageCheckerPage(QuestionImageCheckerPage question, int answersGiven, int answersNeeded)
        {
            InitializeComponent();

            HeaderText.BindingContext = this;
            PictureA.BindingContext = this;
            PictureB.BindingContext = this;
            PictureC.BindingContext = this;
            PictureD.BindingContext = this;
            Frage.BindingContext = this;

            Header = $"Frage {answersGiven + 1}/{answersNeeded} Id {question.InternId}";
            QuestionItem = question;
            EvalButton.BindingContext = this;
            if (answersGiven >= answersNeeded) EvaluationTextColor = Color.Gray;
            EvaluationTextColor = Color.LightGray;
        }

        private void Picture_ShortPress(object sender, EventArgs e)
        {
            MarkPicture((ImageButton)sender);
        }

        /// <summary>
        /// marks the background of a picture with color
        /// </summary>
        private void MarkPicture(ImageButton imageButton)
        {
            imageButton.BorderColor = imageButton.BorderColor == nonSelectedColor ? selectedColor : nonSelectedColor;
        }

        /// <summary>
        /// saves the answer and loads a new Question, if one is available
        /// </summary>
        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            var id = QuestionItem.InternId;
            var im1Sel = PictureA.BorderColor == selectedColor ? 1 : 0;
            var im2Sel = PictureB.BorderColor == selectedColor ? 1 : 0;
            var im3Sel = PictureC.BorderColor == selectedColor ? 1 : 0;
            var im4Sel = PictureD.BorderColor == selectedColor ? 1 : 0;

            if(PictureA.BorderColor == nonSelectedColor && PictureB.BorderColor == nonSelectedColor 
                && PictureC.BorderColor == nonSelectedColor && PictureD.BorderColor == nonSelectedColor)
            {
                DisplayAlert("Hinweis", "Bitte vervollständigen Sie Ihre Auswahl um fortzufahren.", "OK");
                return;
            }

            AnswerItem = new AnswerImageCheckerPage(id, im1Sel, im2Sel, im3Sel, im4Sel);
            PageFinished?.Invoke(this, PageResult.Continue);
        }
        /// <summary>
        /// ???
        /// </summary>
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
