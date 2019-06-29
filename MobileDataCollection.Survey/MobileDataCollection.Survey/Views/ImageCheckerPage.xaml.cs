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
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem), 
            typeof(QuestionImageCheckerPage), typeof(ImageCheckerPage), 
            new QuestionImageCheckerPage(1, "Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 0, 1, 0, "Q1_G1_F1_B1_klein.png", "Q1_G1_F1_B2_klein.png", "Q1_G1_F1_B3_klein.png", "Q1_G1_F1_B4_klein.png"), BindingMode.OneWay);
        //Binding für Answer
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem), 
            typeof(AnswerImageCheckerPage), typeof(ImageCheckerPage), new AnswerImageCheckerPage(0, 0, 0, 0, 0), 
            BindingMode.OneWay);
        //Binding für Header
        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header),
            typeof(String), typeof(DoubleSliderPage), "demo", BindingMode.OneWay);
        //Header
        public String Header
        {
            get { return (String)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

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

        private QuestionImageCheckerPage QuestionItemTest;
        private DatabankCommunication DBCom = new DatabankCommunication();

        Color selectedColor = Color.DarkSeaGreen;
        Color nonSelectedColor = Color.White;

        Stopwatch stopwatch;

        /*public int AnswersGiven { get; set; } //von Maya für Frageheader
        public int AnswersNeeded { get; set; } = 8; //von Maya für Frageheader
        public String Header { get; set; }  //von Maya für Frageheader*/

        public ImageCheckerPage()
        {
            this.AnswersNeeded = 10;
            this.AnswersGiven = 1;
            InitializeComponent();

            /*this.AnswersGiven = 1; //von Maya für Frageheader
            this.Header = String.Format("Frage: {0}/{1}", this.AnswersGiven, this.AnswersNeeded);  //von Maya für Frageheader
            NummerFrage.Text = this.Header; //von Maya für Frageheader*/

            NummerFrage.BindingContext = this;
            PictureA.BindingContext = this;
            PictureB.BindingContext = this;
            PictureC.BindingContext = this;
            PictureD.BindingContext = this;
            Frage.BindingContext = this;
            this.Header = $"Frage {this.AnswersGiven}/{this.AnswersNeeded + 1}";

            QuestionItemTest = DBCom.LoadQuestionImageChecker(1);
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

        
        private void MarkPicture(ImageButton imageButton)
        {
            imageButton.BorderColor = imageButton.BorderColor == nonSelectedColor ? selectedColor : nonSelectedColor;
        }

        private void PressPicture(object sender, EventArgs e)
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        private void ReleasePicture(object sender, EventArgs e)
        {
            stopwatch.Stop();
            ImageButton imageButton = (ImageButton)sender;
            if (stopwatch.ElapsedMilliseconds < 1000)
            {
                MarkPicture(imageButton);
            }
        }
        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            if (this.AnswersGiven < this.AnswersNeeded)
            {
                this.AnswersGiven++;
                // Set new Question
               
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

                int difficulty = 3; // TODO: implement Method which checks if Question answered right/wrong

                QuestionItemTest = DBCom.LoadQuestionImageChecker(difficulty);
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
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
            //speichereErgebnisse();
        }
       
    }
}
