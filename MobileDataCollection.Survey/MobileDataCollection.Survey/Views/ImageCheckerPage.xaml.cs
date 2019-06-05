using MobileDataCollection.Survey.ModelForDatabank;
using MobileDataCollection.Survey.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDataCollection.Survey.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageCheckerPage : ContentPage
    {
        private SQLiteConnection conn;
        private QuestionImageCheckerPage QICPOriginal;

        Color selectedColor = Color.DarkSeaGreen;
        Color nonSelectedColor = Color.White;

        int anzahlAntworten;
        int[] vorgegebeneAntworten;
        int[] gegebeneAntworten;
        Stopwatch stopwatch;

        public int AnswersGiven { get; set; } //von Maya für Frageheader
        public int AnswersNeeded { get; set; } = 8; //von Maya für Frageheader
        public String Header { get; set; }  //von Maya für Frageheader

        public ImageCheckerPage()
        {
            //getFragebogen();

            InitializeComponent();

            createConnectionToDb();
            createTable();
            loadQuestion();

            this.AnswersGiven = 1; //von Maya für Frageheader
            this.Header = String.Format("Frage: {0}/{1}", this.AnswersGiven, this.AnswersNeeded);  //von Maya für Frageheader
            NummerFrage.Text = this.Header; //von Maya für Frageheader

            Frage.Text = QICPOriginal.QuestionText;

            PictureASource = QICPOriginal.Image1Source;
            PictureBSource = QICPOriginal.Image2Source;
            PictureCSource = QICPOriginal.Image3Source;
            PictureDSource = QICPOriginal.Image4Source;

            anzahlAntworten = QICPOriginal.NumberOfPossibleAnswers;
            vorgegebeneAntworten = new int[anzahlAntworten];
            gegebeneAntworten = new int[anzahlAntworten];

            vorgegebeneAntworten[0] = QICPOriginal.Image1Correct;
            vorgegebeneAntworten[1] = QICPOriginal.Image2Correct;
            vorgegebeneAntworten[2] = QICPOriginal.Image3Correct;
            vorgegebeneAntworten[3] = QICPOriginal.Image4Correct;


            stopwatch = new Stopwatch();
        }
        private void MarkPicture(ImageButton imageButton)
        {
            imageButton.BorderColor = imageButton.BorderColor == nonSelectedColor ? selectedColor : nonSelectedColor;
        }
        private void OpenBigPicture(ImageButton imageButton)
        {
            string source = imageButton.Source.ToString();
            if (Frage.Text == "1")
            {
                Frage.Text = QICPOriginal.NumberOfPossibleAnswers.ToString() + " " + QICPOriginal.Image1Source;
            }
            else
            {
                Frage.Text = QICPOriginal.Image3Correct.ToString();
            }
            ImageDetailPage image = new ImageDetailPage(source);
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
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                OpenBigPicture(imageButton);
            }
            else
            {
                MarkPicture(imageButton);
            }
        }
        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            speichereErgebnisse();

            createAnswer();

            if (this.AnswersGiven < this.AnswersNeeded) this.AnswersGiven++; //von Maya für Frageheader
            this.Header = String.Format("Frage: {0}/{1}", this.AnswersGiven, this.AnswersNeeded);  //von Maya für Frageheader
            NummerFrage.Text = this.Header; //von Maya für Frageheader
        }
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
            speichereErgebnisse();
        }
        void speichereErgebnisse()
        {
            gegebeneAntworten[0] = 0;
            gegebeneAntworten[1] = 0;
            gegebeneAntworten[2] = 0;
            gegebeneAntworten[3] = 0;
            gegebeneAntworten[0] = PictureA.BorderColor == selectedColor ? 1 : 0;
            gegebeneAntworten[1] = PictureB.BorderColor == selectedColor ? 1 : 0;
            gegebeneAntworten[2] = PictureC.BorderColor == selectedColor ? 1 : 0;
            gegebeneAntworten[3] = PictureD.BorderColor == selectedColor ? 1 : 0;
        }

        public void createConnectionToDb()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
        }
        private void createTable()
        {
            System.Console.WriteLine("Hello");

            conn.DropTable<QuestionImageCheckerPage>();
            conn.DropTable<AnswerImageCheckerPage>();

            conn.CreateTable<QuestionImageCheckerPage>();
            conn.CreateTable<AnswerImageCheckerPage>();

            if (conn.Table<QuestionImageCheckerPage>().Count() == 0)
            { 
                createQuestions();
            }
        }
        public void createQuestions()
        {
            QuestionImageCheckerPage QICP = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 0, 0, 1, 0, "Q1_G1_F1_B1_klein.png", "Q1_G1_F1_B2_klein.png", "Q1_G1_F1_B3_klein.png", "Q1_G1_F1_B4_klein.png");
            conn.Insert(QICP);
            QICP = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 0, 1, 1, 0, "Q1_G1_F2_B1_klein.png", "Q1_G1_F2_B2_klein.png", "Q1_G1_F2_B3_klein.png", "Q1_G1_F2_B4_klein.png");
            conn.Insert(QICP);
        }
        int i = 0;
        public void loadQuestion()
        {
            QICPOriginal = (from q in conn.Table<QuestionImageCheckerPage>() select q).ElementAt(i);
        }
        public void createAnswer()
        {
            AnswerImageCheckerPage AICP = new AnswerImageCheckerPage(QICPOriginal.internId, gegebeneAntworten[0], gegebeneAntworten[1], gegebeneAntworten[2], gegebeneAntworten[3]);
            conn.Insert(AICP);
            AICP = (from q in conn.Table<AnswerImageCheckerPage>() select q).ElementAt(i);
            i++;
            if (i > 1)
            {
                i = 0;
            }
            System.Console.WriteLine("Element :" + i);
            System.Console.WriteLine(AICP.Image1Selected + " " + gegebeneAntworten[0]);
            System.Console.WriteLine(AICP.Image2Selected + " " + gegebeneAntworten[1]);
            System.Console.WriteLine(AICP.Image3Selected + " " + gegebeneAntworten[2]);
            System.Console.WriteLine(AICP.Image4Selected + " " + gegebeneAntworten[3]);
            System.Console.WriteLine(AICP.internId);

            loadQuestion();
        }
        private string NummerFrageText { get => NummerFrage.Text; set => NummerFrage.Text = value; }
        private string FrageText { get => Frage.Text; set => Frage.Text = value; }
        private string PictureASource { get => PictureA.Source.ToString(); set => PictureA.Source = value; }
        private string PictureBSource { get => PictureB.Source.ToString(); set => PictureB.Source = value; }
        private string PictureCSource { get => PictureC.Source.ToString(); set => PictureC.Source = value; }
        private string PictureDSource { get => PictureD.Source.ToString(); set => PictureD.Source = value; }
    }
}
