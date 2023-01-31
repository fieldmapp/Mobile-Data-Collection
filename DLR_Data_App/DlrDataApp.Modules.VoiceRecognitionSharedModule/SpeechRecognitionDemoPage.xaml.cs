using DlrDataApp.Modules.Base.Shared.Localization;
using DlrDataApp.Modules.SpeechRecognition.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DlrDataApp.Modules.SpeechRecognition.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpeechRecognitionDemoPage : ContentPage
    {

        const int MaxKeywordInspectionLength = 40;
        const int LaneCount = 3;
        ISpeechRecognizer SpeechRecognizer;

        public SpeechRecognitionDemoPage()
        {
            InitializeComponent();
            SpeechRecognizer = DependencyService.Get<ISpeechRecognizerProvider>().Initialize(null);
            SpeechRecognizer.PartialResultRecognized += SpeechRecognizer_PartialResultRecognized;
            SpeechRecognizer.ResultRecognized += SpeechRecognizer_ResultRecognized;
        }

        string SafeString = string.Empty;

        CancellationTokenSource ShowReadyCancelationTokenSource;
        Task DisplayReady(CancellationToken cancelationToken)
        {
            if (cancelationToken.IsCancellationRequested)
                return Task.CompletedTask;
            RecognizedStringLabel.Text = SafeString = Environment.NewLine + Environment.NewLine + SharedResources.ready;
            return Task.CompletedTask;
        }

        Task ShowReadyTask;
        protected override void OnAppearing()
        {
            ShowReadyCancelationTokenSource = new CancellationTokenSource();
            ShowReadyTask = DisplayReady(ShowReadyCancelationTokenSource.Token);
            SafeString = string.Empty;
            SpeechRecognizer.StartListening();
        }

        private void SpeechRecognizer_ResultRecognized(object sender, SpeechRecognitionResult e)
        {
            //var command = VoiceCommandCompiler.Compile(e.Parts.Select(p => p.Word).ToList());
            var avgConf = e.Parts.Average(p => p.Confidence);
            SafeString = $"{Environment.NewLine}{Environment.NewLine}{avgConf}:{e.Result}{Environment.NewLine} - {/*command*/""} -{SafeString}";
            RecognizedStringLabel.Text = SafeString;
        }

        private void SpeechRecognizer_PartialResultRecognized(object sender, SpeechRecognitionPartialResult e)
        {
            RecognizedStringLabel.Text = e.PartialResult + SafeString;
        }

        protected override void OnDisappearing()
        {
            ShowReadyCancelationTokenSource.Cancel();
            SpeechRecognizer.StopListening();
        }
    }
}