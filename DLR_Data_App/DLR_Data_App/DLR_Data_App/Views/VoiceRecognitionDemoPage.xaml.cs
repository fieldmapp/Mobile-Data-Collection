using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VoiceRecognitionDemoPage : ContentPage
    {
        enum ModelConfiguration
        {
            Hang,
            Hanglage,
            Gefaelle
        }
        ModelConfiguration CurrentConfiguration;
        List<string> HangKeywords = new List<string>
        {
            "Anfang", "Ende", "Abbrechen", "Kuppe", "Verdichtungen", "Hang", "Sandlinse", "Vernässung", "Trockenstress", "Mäusefraß", "Wildschaden", "Vorgewende", "Waldrand",
            "Links fünf Meter", "Links zehn Meter", "Links fünfzehn Meter", "Links zwanzig Meter",
            "Rechts fünf Meter", "Rechts zehn Meter", "Rechts fünfzehn Meter", "Rechts zwanzig Meter"
        };
        List<string> HanglageKeywords = new List<string>
        {
            "Anfang", "Ende", "Abbrechen", "Kuppe", "Verdichtungen", "Hanglage", "Sandlinse", "Vernässung", "Trockenstress", "Mäusefraß", "Wildschaden", "Vorgewende", "Waldrand",
            "Links fünf Meter", "Links zehn Meter", "Links fünfzehn Meter", "Links zwanzig Meter",
            "Rechts fünf Meter", "Rechts zehn Meter", "Rechts fünfzehn Meter", "Rechts zwanzig Meter"
        };
        List<string> GefaelleKeywords = new List<string>
        {
            "Anfang", "Ende", "Abbrechen", "Kuppe", "Verdichtungen", "Gefälle", "Sandlinse", "Vernässung", "Trockenstress", "Mäusefraß", "Wildschaden", "Vorgewende", "Waldrand",
            "Links fünf Meter", "Links zehn Meter", "Links fünfzehn Meter", "Links zwanzig Meter",
            "Rechts fünf Meter", "Rechts zehn Meter", "Rechts fünfzehn Meter", "Rechts zwanzig Meter"
        };
        List<string> AcceptedKeywords =>
            CurrentConfiguration == ModelConfiguration.Hang ? HangKeywords : (
            CurrentConfiguration == ModelConfiguration.Gefaelle ? GefaelleKeywords : (
            CurrentConfiguration == ModelConfiguration.Hanglage ? HanglageKeywords :
            throw new NotImplementedException()));

        public VoiceRecognitionDemoPage()
        {
            InitializeComponent();
            ChangeConfiguration(ModelConfiguration.Hang);
        }

        string SafeString = string.Empty;

        [OnSplashScreenLoad]
        static void OnSplashScreenLoad()
        {
            DependencyService.Get<ISpeechRecognizer>().Start();
        }

        CancellationTokenSource ShowReadyCancelationTokenSource;
        async Task DisplayReady(CancellationToken cancelationToken)
        {
            await DependencyService.Get<ISpeechRecognizer>().LoadTask;
            if (cancelationToken.IsCancellationRequested)
                return;
            RecognizedStringLabel.Text = SafeString = Environment.NewLine + Environment.NewLine + AppResources.ready;
        }

        Task ShowReadyTask;
        protected override void OnAppearing()
        {
            ShowReadyCancelationTokenSource = new CancellationTokenSource();
            ShowReadyTask = DisplayReady(ShowReadyCancelationTokenSource.Token);
            SafeString = string.Empty;
            var speechRecognizer = DependencyService.Get<ISpeechRecognizer>();
            speechRecognizer.PartialResultRecognized += SpeechRecognizer_PartialResultRecognized;
            speechRecognizer.ResultRecognized += SpeechRecognizer_ResultRecognized;
        }

        private void SpeechRecognizer_ResultRecognized(object sender, VoiceRecognitionResult e)
        {
            var nearestKeyword = AcceptedKeywords.MinBy(k => Helpers.LevenshteinDistnace(e.Result, k));
            var distance = Helpers.LevenshteinDistnace(e.Result, nearestKeyword);
            var normalizedDistance = distance / (float)e.Result.Length;
            SafeString = $"{Environment.NewLine}{Environment.NewLine}{e.Result}{Environment.NewLine}- {nearestKeyword} : (abs {distance}; norm{normalizedDistance}) -{SafeString}";
            RecognizedStringLabel.Text = SafeString;
        }

        private void SpeechRecognizer_PartialResultRecognized(object sender, Models.VoiceRecognitionPartialResult e)
        {
            RecognizedStringLabel.Text =  e.PartialResult + SafeString;
        }

        protected override void OnDisappearing()
        {
            ShowReadyCancelationTokenSource.Cancel();
            var speechRecognizer = DependencyService.Get<ISpeechRecognizer>();
            speechRecognizer.PartialResultRecognized -= SpeechRecognizer_PartialResultRecognized;
            speechRecognizer.ResultRecognized -= SpeechRecognizer_ResultRecognized;
        }

        private void ChangeConfiguration(ModelConfiguration configuration)
        {
            CurrentConfiguration = configuration;
            HangSetupButton.BackgroundColor = Color.LightGray;
            HanglageSetupButton.BackgroundColor = Color.LightGray;
            GefaelleSetupButton.BackgroundColor = Color.LightGray;
            var selectedButton = CurrentConfiguration == ModelConfiguration.Hang ? HangSetupButton : (
                CurrentConfiguration == ModelConfiguration.Gefaelle ? GefaelleSetupButton : (
                CurrentConfiguration == ModelConfiguration.Hanglage ? HanglageSetupButton :
                throw new NotImplementedException()));
            selectedButton.BackgroundColor = Color.DarkSeaGreen;
        }

        private void HangSetupButton_Clicked(object sender, EventArgs e)
        {
            ChangeConfiguration(ModelConfiguration.Hang);
        }

        private void HanglageSetupButton_Clicked(object sender, EventArgs e)
        {
            ChangeConfiguration(ModelConfiguration.Hanglage);
        }

        private void GefaelleSetupButton_Clicked(object sender, EventArgs e)
        {
            ChangeConfiguration(ModelConfiguration.Gefaelle);
        }
    }
}