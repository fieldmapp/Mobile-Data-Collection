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
            var preKeywords = new string[] { "Rechts", "Vorn", "Links", "Position", "Start", "Stopp" };
            var keywords = new string[] { "Hang Süd", "Hang Nord", "Vernässung", "Nassstelle", "Trockenstress", "Sandlinse", "Waldrand", "Hanglage", "Kuppe", "Verdichtungen" };
            Keywords.Add("Vorgewende");
            Keywords.AddRange(preKeywords.SelectMany(pre => keywords.Select(k => pre + " " + k)));
            Keywords.AddRange(keywords);
        }

        string SafeString = string.Empty;

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
            SafeString = e.Result + Environment.NewLine + "- " + nearestKeyword + " - " + Environment.NewLine + Environment.NewLine + SafeString;
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