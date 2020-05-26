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
        public VoiceRecognitionDemoPage()
        {
            InitializeComponent();
        }

        string SafeString = string.Empty;

        protected override void OnAppearing()
        {
            var speechRecognizer = DependencyService.Get<ISpeechRecognizer>();
            speechRecognizer.PartialResultRecognized += SpeechRecognizer_PartialResultRecognized;
            speechRecognizer.ResultRecognized += SpeechRecognizer_ResultRecognized;
        }

        private void SpeechRecognizer_ResultRecognized(object sender, Models.VoiceRecognitionResult e)
        {
            SafeString += e.Result + Environment.NewLine + Environment.NewLine;
            RecognizedStringLabel.Text = SafeString;
        }

        private void SpeechRecognizer_PartialResultRecognized(object sender, Models.VoiceRecognitionPartialResult e)
        {
            RecognizedStringLabel.Text = SafeString + e.PartialResult;
        }

        protected override void OnDisappearing()
        {
            var speechRecognizer = DependencyService.Get<ISpeechRecognizer>();
            speechRecognizer.PartialResultRecognized -= SpeechRecognizer_PartialResultRecognized;
            speechRecognizer.ResultRecognized -= SpeechRecognizer_ResultRecognized;
        }
    }
}