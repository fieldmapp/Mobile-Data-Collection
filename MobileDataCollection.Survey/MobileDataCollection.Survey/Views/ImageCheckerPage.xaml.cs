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

        Color selectedColor = Color.DarkSeaGreen;
        Color nonSelectedColor = Color.White;

        int anzahlAntworten;
        int anzahlRichtigerAntworten;
        bool[] vorgegebeneAntworten;
        bool[] gegebeneAntworten;
        bool[] richtigeAntworten;
        Stopwatch stopwatch;


        public ImageCheckerPage()
        {
            //getFragebogen();

            InitializeComponent();

            PictureASource = "Q1_G1_F1_B1_klein.png";
            PictureBSource = "Q1_G1_F1_B2_klein.png";
            PictureCSource = "Q1_G1_F1_B3_klein.png";
            PictureDSource = "Q1_G1_F1_B4_klein.png";

            anzahlAntworten = 4;
            anzahlRichtigerAntworten = 0;
            vorgegebeneAntworten = new bool[anzahlAntworten];
            gegebeneAntworten = new bool[anzahlAntworten];
            richtigeAntworten = new bool[anzahlAntworten];
            for (int i = 0; i < anzahlAntworten; i++)
            {
                vorgegebeneAntworten[i] = true;
                gegebeneAntworten[i] = false;
                richtigeAntworten[i] = false;
            }

            stopwatch = new Stopwatch();
        }
        private void MarkPicture(ImageButton imageButton)
        {
            imageButton.BorderColor = imageButton.BorderColor == nonSelectedColor ? selectedColor : nonSelectedColor;
        }
        private void OpenBigPicture(ImageButton imageButton)
        {
            string source = imageButton.Source.ToString();
            source = source.Substring(6);
            source = source.Substring(0, source.Length - 10);
            source = source + ".png";
            Frage.Text = source;
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
            anzahlRichtigerAntworten = 0;
            speichereErgebnisse();

            for (int i = 0; i < anzahlAntworten; i++)
            {
                if (vorgegebeneAntworten[i] == gegebeneAntworten[i])
                {
                    richtigeAntworten[i] = true;
                    anzahlRichtigerAntworten++;
                }
                else
                {
                    richtigeAntworten[i] = false;
                }
            }

        }
        void OnSpeicherButtonClicked(object sender, EventArgs e)
        {
            speichereErgebnisse();
        }
        void speichereErgebnisse()
        {
            gegebeneAntworten[0] = PictureA.BorderColor == selectedColor;
            gegebeneAntworten[1] = PictureB.BorderColor == selectedColor;
            gegebeneAntworten[2] = PictureC.BorderColor == selectedColor;
            gegebeneAntworten[3] = PictureD.BorderColor == selectedColor;
        }
        private string NummerFrageText { get => NummerFrage.Text; set => NummerFrage.Text = value; }
        private string FrageText { get => Frage.Text; set => Frage.Text = value; }
        private string PictureASource { get => PictureA.Source.ToString(); set => PictureA.Source = value; }
        private string PictureBSource { get => PictureB.Source.ToString(); set => PictureB.Source = value; }
        private string PictureCSource { get => PictureC.Source.ToString(); set => PictureC.Source = value; }
        private string PictureDSource { get => PictureD.Source.ToString(); set => PictureD.Source = value; }
    }
}
