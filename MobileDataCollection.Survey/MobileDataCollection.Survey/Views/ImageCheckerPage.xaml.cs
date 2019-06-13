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
            new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 0, 1, 0, "Q1_G1_F1_B1_klein.png", "Q1_G1_F1_B2_klein.png", "Q1_G1_F1_B3_klein.png", "Q1_G1_F1_B4_klein.png"), BindingMode.OneWay);
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

        private SQLiteConnection conn;
        private QuestionImageCheckerPage QICP;

        Color selectedColor = Color.DarkSeaGreen;
        Color nonSelectedColor = Color.White;

        int zaehler = 0;

        int anzahlAntworten;
        int[] vorgegebeneAntworten;
        int[] gegebeneAntworten;
        Stopwatch stopwatch;

        /*public int AnswersGiven { get; set; } //von Maya für Frageheader
        public int AnswersNeeded { get; set; } = 8; //von Maya für Frageheader
        public String Header { get; set; }  //von Maya für Frageheader*/

        public ImageCheckerPage()
        {
            this.AnswersNeeded = 2;
            this.AnswersGiven = 1;
            InitializeComponent();
            //CreateDatabank();
            //QICP = LoadQuestion(1,zaehler);

            /*this.AnswersGiven = 1; //von Maya für Frageheader
            this.Header = String.Format("Frage: {0}/{1}", this.AnswersGiven, this.AnswersNeeded);  //von Maya für Frageheader
            NummerFrage.Text = this.Header; //von Maya für Frageheader*/

            //UpdatePage(QICP);
            NummerFrage.BindingContext = this;
            PictureA.BindingContext = this;
            PictureB.BindingContext = this;
            PictureC.BindingContext = this;
            PictureD.BindingContext = this;
            Frage.BindingContext = this;
            this.Header = $"Frage {this.AnswersGiven}/{this.AnswersNeeded + 1}";
            QuestionItem = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 0, 1, 0, "Q1_G1_F1_B1_klein.png", "Q1_G1_F1_B2_klein.png", "Q1_G1_F1_B3_klein.png", "Q1_G1_F1_B4_klein.png");
            stopwatch = new Stopwatch();
        }

        /*public ImageCheckerPage(QuestionImageCheckerPage QIPC)
        {
            InitializeComponent();
            CreateDatabank();
            QICP = LoadQuestion(1, zaehler);

            this.AnswersGiven = 1; //von Maya für Frageheader
            this.Header = String.Format("Frage: {0}/{1}", this.AnswersGiven, this.AnswersNeeded);  //von Maya für Frageheader
            NummerFrage.Text = this.Header; //von Maya für Frageheader

            UpdatePage(QICP);

            stopwatch = new Stopwatch();
        }*/

        /*public void UpdatePage(QuestionImageCheckerPage QICP)
        {
            UpdateQuestion(QICP.QuestionText);
            UpdatePictures(QICP.Image1Source, QICP.Image2Source, QICP.Image3Source, QICP.Image4Source);
            UpdateAnswersForQuestion(QICP.NumberOfPossibleAnswers, QICP.Image1Correct, QICP.Image2Correct, QICP.Image3Correct, QICP.Image4Correct);
        }*/

        /*public void UpdateAnswersForQuestion(int numberOfQuestions, int answer1, int answer2, int answer3, int answer4)
        {
            anzahlAntworten = numberOfQuestions;
            vorgegebeneAntworten = new int[numberOfQuestions];
            vorgegebeneAntworten[0] = answer1;
            vorgegebeneAntworten[1] = answer2;
            vorgegebeneAntworten[2] = answer3;
            vorgegebeneAntworten[3] = answer4;
            vorgegebeneAntworten = new int[numberOfQuestions];
            gegebeneAntworten = new int[numberOfQuestions];
        }*/

        /*public void UpdateQuestion(string question)
        {
            Frage.Text = question;
        }*/

        /*public void UpdatePictures(string img1, string img2, string img3, string img4)
        {
            PictureA.Source = img1;
            PictureB.Source = img2;
            PictureC.Source = img3;
            PictureD.Source = img4;
        }*/
        private void MarkPicture(ImageButton imageButton)
        {
            imageButton.BorderColor = imageButton.BorderColor == nonSelectedColor ? selectedColor : nonSelectedColor;
        }
        /*private void OpenBigPicture(ImageButton imageButton)
        {
            string source = imageButton.Source.ToString();
            if (Frage.Text == "1")
            {
                Frage.Text = QICP.NumberOfPossibleAnswers.ToString() + " " + QICP.Image1Source;
            }
            else
            {
                Frage.Text = QICP.Image3Correct.ToString();
            }
            ImageDetailPage image = new ImageDetailPage(source);
        }*/
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
            /*speichereErgebnisse();

            //CreateAnswer();

            if (this.AnswersGiven < this.AnswersNeeded) this.AnswersGiven++; //von Maya für Frageheader
            this.Header = String.Format("Frage: {0}/{1}", this.AnswersGiven, this.AnswersNeeded);  //von Maya für Frageheader
            NummerFrage.Text = this.Header; //von Maya für Frageheader

            QICP = LoadQuestion(1,zaehler);
            UpdatePage(QICP);*/
            if (this.AnswersGiven < this.AnswersNeeded)
            {
                this.AnswersGiven++;
                // Set new Question
               
                this.Header = $"Frage {this.AnswersGiven}/{this.AnswersNeeded + 1}";

                AnswerItem.Image1Selected = PictureA.BorderColor == selectedColor ? 1 : 0; //in QuestionItem ist ImageCorrectAnswer, wie verwenden
                AnswerItem.Image2Selected = PictureB.BorderColor == selectedColor ? 1 : 0;
                AnswerItem.Image3Selected = PictureC.BorderColor == selectedColor ? 1 : 0;
                AnswerItem.Image4Selected = PictureD.BorderColor == selectedColor ? 1 : 0;
                PictureA.BorderColor = nonSelectedColor;
                PictureB.BorderColor = nonSelectedColor;
                PictureC.BorderColor = nonSelectedColor;
                PictureD.BorderColor = nonSelectedColor;

                QuestionItem = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 1, 0, 1, 1, 0, "Q1_G1_F2_B1_klein.png", "Q1_G1_F2_B2_klein.png", "Q1_G1_F2_B3_klein.png", "Q1_G1_F2_B4_klein.png");
            }

        }
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
            //speichereErgebnisse();
        }
        /*void speichereErgebnisse()
        {
    
            gegebeneAntworten[0] = PictureA.BorderColor == selectedColor ? 1 : 0;
            gegebeneAntworten[1] = PictureB.BorderColor == selectedColor ? 1 : 0;
            gegebeneAntworten[2] = PictureC.BorderColor == selectedColor ? 1 : 0;
            gegebeneAntworten[3] = PictureD.BorderColor == selectedColor ? 1 : 0;
        }*/
        /*public void CreateDatabank()
        {
            CreateConnectionToDb();
            CreateTable();
            CreateQuestions();
        }*/

        /*public void CreateConnectionToDb()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
        }*/
        /*private void CreateTable()
        {
            conn.DropTable<QuestionImageCheckerPage>();
            conn.DropTable<AnswerImageCheckerPage>();

            conn.CreateTable<QuestionImageCheckerPage>();
            conn.CreateTable<AnswerImageCheckerPage>();
        }*/
        public void CreateQuestions()
        {
            QuestionImageCheckerPage question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 0, 1, 0, "Q1_G1_F1_B1_klein.png", "Q1_G1_F1_B2_klein.png", "Q1_G1_F1_B3_klein.png", "Q1_G1_F1_B4_klein.png");
            conn.Insert(question);
            question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 1, 0, 1, 1, 0, "Q1_G1_F2_B1_klein.png", "Q1_G1_F2_B2_klein.png", "Q1_G1_F2_B3_klein.png", "Q1_G1_F2_B4_klein.png");
            conn.Insert(question);
            question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 2, 1, 1, 1, 0, "Q1_G2_F1_B1_klein.png", "Q1_G2_F1_B2_klein.png", "Q1_G2_F1_B3_klein.png", "Q1_G2_F1_B4_klein.png");
            conn.Insert(question);
            question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 2, 0, 0, 1, 0, "Q1_G2_F2_B1_klein.png", "Q1_G2_F2_B2_klein.png", "Q1_G2_F2_B3_klein.png", "Q1_G2_F2_B4_klein.png");
            conn.Insert(question);
            question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Kartoffel abgebildet?", 3, 1, 0, 1, 0, "Q1_G3_F1_B1_klein.png", "Q1_G3_F1_B2_klein.png", "Q1_G3_F1_B3_klein.png", "Q1_G3_F1_B4_klein.png");
            conn.Insert(question);
            question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Gerste abgebildet?", 3, 0, 0, 1, 0, "Q1_G3_F2_B1_klein.png", "Q1_G3_F2_B2_klein.png", "Q1_G3_F2_B3_klein.png", "Q1_G3_F2_B4_klein.png");
            conn.Insert(question);
        }
        /*public QuestionImageCheckerPage LoadQuestion(int difficulty,int zaehler)
        {
            string query = String.Format("SELECT * FROM QuestionImageCheckerPage LEFT OUTER JOIN AnswerImageCheckerPage ON QuestionImageCheckerPage.InternId = AnswerImageCheckerPage.InternId WHERE Difficulty = {0}", difficulty);
            IEnumerable<QuestionImageCheckerPage> foo = conn.Query<QuestionImageCheckerPage>(query);
            List<QuestionImageCheckerPage> listOfResults = foo.ToList<QuestionImageCheckerPage>();

            System.Console.WriteLine("Hello {0}", zaehler);
            QuestionImageCheckerPage temp = listOfResults.ElementAt(zaehler);
            if(zaehler == 1)
            {
                this.zaehler = 0;
            }
            else
            {
                this.zaehler++;
            }
            return temp;
        }*/
        /*public void CreateAnswer()
        {
            AnswerImageCheckerPage AICP = new AnswerImageCheckerPage(QICP.InternId, gegebeneAntworten[0], gegebeneAntworten[1], gegebeneAntworten[2], gegebeneAntworten[3]);
            conn.Insert(AICP);
            string query = String.Format("SELECT * FROM AnswerImageCheckerPage WHERE InternID = {0}", QICP.InternId);
            IEnumerable<AnswerImageCheckerPage> foo = conn.Query<AnswerImageCheckerPage>(query);
            List<AnswerImageCheckerPage> liste = foo.ToList<AnswerImageCheckerPage>();
            AICP = liste.First();
            //AICP = (from q in conn.Table<AnswerImageCheckerPage>() select q).Where(AICP.internId = QICP.internId);
        }*/
    }
}
