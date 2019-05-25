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

        Color selectedColor = Color.Green;
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
            //TODO
            setPictureBSource("Q1_G1_F1_B2_klein.png");
            setPictureCSource("Q1_G1_F1_B3_klein.png");
            setPictureDSource("Q1_G1_F1_B4_klein.png");

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
                WeiterButton.Text = imageButton.Source.ToString();
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
            //TODO: Ausdrücke vereinfachen 
            if (PictureB.BorderColor == selectedColor)
            {
                gegebeneAntworten[1] = true;
            }
            else
            {
                gegebeneAntworten[1] = false;
            }
            if (PictureC.BorderColor == selectedColor)
            {
                gegebeneAntworten[2] = true;
            }
            else
            {
                gegebeneAntworten[2] = false;
            }
            if (PictureD.BorderColor == selectedColor)
            {
                gegebeneAntworten[3] = true;
            }
            else
            {
                gegebeneAntworten[3] = false;
            }
        }
        private string NummerFrageText { get => NummerFrage.Text; set => NummerFrage.Text = value; }
        //TODO: get und set Methoden mit Properties ersetzen
        private void setFrageText(String text)
        {
            Frage.Text = text;
        }
        private String getFrageText()
        {
            return Frage.Text;
        }
        private string PictureASource { get => PictureA.Source.ToString(); set => PictureA.Source = value; }
        private void setPictureASource(String source)
        {
            PictureA.Source = source;
        }
        private string getPictureASource()
        {
            return PictureA.Source.ToString();
        }
        private void setPictureBSource(String source)
        {
            PictureB.Source = source;
        }
        private String getPictureBSource()
        {
            return PictureB.Source.ToString();
        }
        private void setPictureCSource(String source)
        {
            PictureC.Source = source;
        }
        private String getPictureCSource()
        {
            return PictureC.Source.ToString();
        }
        private void setPictureDSource(String source)
        {
            PictureD.Source = source;
        }
        private String getPictureDSource()
        {
            return PictureD.Source.ToString();
        }
    }
}