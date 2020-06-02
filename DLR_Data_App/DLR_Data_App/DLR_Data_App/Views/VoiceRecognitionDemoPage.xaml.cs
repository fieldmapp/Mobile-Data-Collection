using DLR_Data_App.Models;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VoiceRecognitionDemoPage : ContentPage
    {
        List<string> Keywords = new List<string>();
        public VoiceRecognitionDemoPage()
        {
            InitializeComponent();
            Keywords.AddRange(new[]
            {
                "Anfang", "Ende", "Abbrechen", "Kuppe", "Verdichtungen", "Hang", "Sandlinse", "Vernässung", "Trockenstress", "Mäusefraß", "Wildschaden", "Vorgewende", "Waldrand",
                "Links fünf Meter", "Links zehn Meter", "Links fünfzehn Meter", "Links zwanzig Meter",
                "Rechts fünf Meter", "Rechts zehn Meter", "Rechts fünfzehn Meter", "Rechts zwanzig Meter"
            });
        }

        string SafeString = string.Empty;

        [OnSplashScreenLoad]
        static void OnSplashScreenLoad()
        {
            DependencyService.Get<ISpeechRecognizer>().Start();
        }

        protected override void OnAppearing()
        {
            SafeString = string.Empty;
            var speechRecognizer = DependencyService.Get<ISpeechRecognizer>();
            speechRecognizer.PartialResultRecognized += SpeechRecognizer_PartialResultRecognized;
            speechRecognizer.ResultRecognized += SpeechRecognizer_ResultRecognized;
        }

        private void SpeechRecognizer_ResultRecognized(object sender, VoiceRecognitionResult e)
        {
            var nearestKeyword = Keywords.MinBy(k => Helpers.LevenshteinDistnace(e.Result, k));
            SafeString = Environment.NewLine + Environment.NewLine + e.Result + Environment.NewLine + "- " + nearestKeyword + " - " +  SafeString;
            RecognizedStringLabel.Text = SafeString;
        }

        private void SpeechRecognizer_PartialResultRecognized(object sender, Models.VoiceRecognitionPartialResult e)
        {
            RecognizedStringLabel.Text =  e.PartialResult + SafeString;
        }

        protected override void OnDisappearing()
        {
            var speechRecognizer = DependencyService.Get<ISpeechRecognizer>();
            speechRecognizer.PartialResultRecognized -= SpeechRecognizer_PartialResultRecognized;
            speechRecognizer.ResultRecognized -= SpeechRecognizer_ResultRecognized;
        }
    }
}