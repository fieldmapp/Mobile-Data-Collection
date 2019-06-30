using MobileDataCollection.Survey.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDataCollection.Survey.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageCheckerPage : ContentPage
    {
        public int AnswersGiven { get; set; }
        //Answers needed to complete Question Category (Muss definiert werden: entweder alle verfügbaren Fragen oder feste Zahl)
        public int AnswersNeeded { get; set; }
        //Binding für Question
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem), typeof(QuestionImageCheckerPage), typeof(ImageCheckerPage), new QuestionImageCheckerPage(1, "Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 0, 1, 0, "Q1_G1_F1_B1_klein.png", "Q1_G1_F1_B2_klein.png", "Q1_G1_F1_B3_klein.png", "Q1_G1_F1_B4_klein.png"), BindingMode.OneWay);
        //Binding für Answer
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem), typeof(AnswerImageCheckerPage), typeof(ImageCheckerPage), new AnswerImageCheckerPage(0, 0, 0, 0, 0), BindingMode.OneWay);
        //Binding für Header
        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(String), typeof(ImageCheckerPage), "demo", BindingMode.OneWay);

        /// <summary>
        /// Item of the given Question
        /// </summary>
        public QuestionImageCheckerPage QuestionItem
        {
            get { return (QuestionImageCheckerPage)GetValue(QuestionItemProperty); }
            set { SetValue(QuestionItemProperty, value); }
        }

        /// <summary>
        /// Item of the Answer
        /// </summary>
        public AnswerImageCheckerPage AnswerItem
        {
            get { return (AnswerImageCheckerPage)GetValue(AnswerItemProperty); }
            set { SetValue(AnswerItemProperty, value); }
        }

        /// <summary>
        /// Item of the Header
        /// </summary>
        public String Header
        {
            get { return (String)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// needed to check if loaded ImageCheckerPage is valid or not
        /// </summary>
        private QuestionImageCheckerPage QuestionItemTest;

        /// <summary>
        /// Object to load questions and save answers
        /// </summary>
        private DatabankCommunication DBCom = new DatabankCommunication();

        /// <summary>
        /// the selected Color for the pictures
        /// </summary>
        Color selectedColor = Color.DarkSeaGreen;

        /// <summary>
        /// the unselected Color for the pictures
        /// </summary>
        Color nonSelectedColor = Color.White;

        /// <summary>
        /// used to measure if a picture should be enlarged or only marked
        /// </summary>
        Stopwatch stopwatch;

        public ImageCheckerPage()
        {
            this.AnswersNeeded = 10;
            this.AnswersGiven = 1;
            InitializeComponent();

            NummerFrage.BindingContext = this;
            PictureA.BindingContext = this;
            PictureB.BindingContext = this;
            PictureC.BindingContext = this;
            PictureD.BindingContext = this;
            Frage.BindingContext = this;
            this.Header = $"Frage {this.AnswersGiven}/{this.AnswersNeeded + 1}";

            QuestionItemTest = DBCom.LoadQuestionImageCheckerPage(1);
            if (QuestionItemTest.InternId != 0)
            {
                QuestionItem = QuestionItemTest;
            }
            else
            {
                // TODO: What do we do if no more Questions are available
            }
            stopwatch = new Stopwatch();
        }

        /// <summary>
        /// marks the background of a picture with color
        /// </summary>
        private void MarkPicture(ImageButton imageButton)
        {
            imageButton.BorderColor = imageButton.BorderColor == nonSelectedColor ? selectedColor : nonSelectedColor;
        }

        /// <summary>
        /// starts a new stopwatch to measure how long the image has been pressed
        /// </summary>
        private void PressPicture(object sender, EventArgs e)
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        /// <summary>
        /// stops the timer and accordingly marks the picture or enlarges the picture
        /// </summary>
        private void ReleasePicture(object sender, EventArgs e)
        {
            ImageButton imageButton = (ImageButton)sender;
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds < 1000)
            {
                MarkPicture(imageButton);
            }
        }
        /// <summary>
        /// saves the answer and loads a new Question, if one is available
        /// </summary>
        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            if (this.AnswersGiven < this.AnswersNeeded)
            {
                this.AnswersGiven++;               
                this.Header = $"Frage {this.AnswersGiven}/{this.AnswersNeeded + 1}";


                AnswerItem.InternId = QuestionItem.InternId;
                AnswerItem.Image1Selected = PictureA.BorderColor == selectedColor ? 1 : 0; //in QuestionItem ist ImageCorrectAnswer, wie verwenden
                AnswerItem.Image2Selected = PictureB.BorderColor == selectedColor ? 1 : 0;
                AnswerItem.Image3Selected = PictureC.BorderColor == selectedColor ? 1 : 0;
                AnswerItem.Image4Selected = PictureD.BorderColor == selectedColor ? 1 : 0;
                PictureA.BorderColor = nonSelectedColor;
                PictureB.BorderColor = nonSelectedColor;
                PictureC.BorderColor = nonSelectedColor;
                PictureD.BorderColor = nonSelectedColor;

                AnswerImageCheckerPage Answer = new AnswerImageCheckerPage(AnswerItem.InternId,AnswerItem.Image1Selected, AnswerItem.Image2Selected, AnswerItem.Image3Selected, AnswerItem.Image4Selected);
                DBCom.AddListAnswerImageCheckerPage(Answer);

                int difficulty = 3;//determineNewDifficulty(QuestionItem.Difficulty, QuestionItem.Image1Correct, AnswerItem.Image1Selected, QuestionItem.Image2Correct, AnswerItem.Image2Selected, QuestionItem.Image3Correct, AnswerItem.Image3Selected, QuestionItem.Image4Correct, AnswerItem.Image4Selected);

                QuestionItemTest = DBCom.LoadQuestionImageCheckerPage(difficulty);
                if(QuestionItemTest.InternId != 0)
                {
                    QuestionItem = QuestionItemTest;
                }
                else
                {
                    // TODO: What do we do if no more Questions are available
                }
            }

        }
        /// <summary>
        /// ???
        /// </summary>
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// calculates the new difficulty based on the answers given
        /// </summary>
        int determineNewDifficulty(int oldDifficulty, int img1Cor, int img1Giv, int img2Cor, int img2Giv, int img3Cor, int img3Giv, int img4Cor, int img4Giv)
        {
            int RightAnswers = 0;
            int WrongAnswers = 0;
            if (img1Cor == img1Giv)
            {
                RightAnswers++;
            }
            else
            {
                WrongAnswers++;
            }
            if (img2Cor == img2Giv)
            {
                RightAnswers++;
            }
            else
            {
                WrongAnswers++;
            }
            if (img3Cor == img3Giv)
            {
                RightAnswers++;
            }
            else
            {
                WrongAnswers++;
            }
            if (img4Cor == img4Giv)
            {
                RightAnswers++;
            }
            else
            {
                WrongAnswers++;
            }

            double PercentageRight = RightAnswers / 4;
            double PercentageWrong = WrongAnswers / 4;
            if(PercentageRight >= 0.75)
            {
                if(oldDifficulty == 3)
                {
                    return oldDifficulty;
                }
                return oldDifficulty + 1;
            }
            if (PercentageRight <= 0.25)
            {
                if(oldDifficulty == 1)
                {
                    return oldDifficulty;
                }
                return oldDifficulty - 1;
            }
            return oldDifficulty;
        }
       
    }
}
