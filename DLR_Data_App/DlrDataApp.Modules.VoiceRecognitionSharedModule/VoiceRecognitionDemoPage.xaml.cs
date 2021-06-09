using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DlrDataApp.Modules.VoiceRecognitionSharedModule
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VoiceRecognitionDemoPage : ContentPage
    {

        const int MaxKeywordInspectionLength = 40;
        const int LaneCount = DrivingPage.MaxTotalLaneCount;

        public VoiceRecognitionDemoPage()
        {
            InitializeComponent();
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
            var command = VoiceCommandCompiler.Compile(e.Parts.Select(p => p.Word).ToList());
            var avgConf = e.Parts.Average(p => p.Confidence);
            SafeString = $"{Environment.NewLine}{Environment.NewLine}{avgConf}:{e.Result}{Environment.NewLine} - {command} -{SafeString}";
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
    }
}