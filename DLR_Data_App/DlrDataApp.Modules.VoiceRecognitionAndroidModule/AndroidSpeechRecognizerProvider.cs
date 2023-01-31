using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Org.Kaldi;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using DlrDataApp.Modules.SpeechRecognition.Definition;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.SpeechRecognition.Android;
using DlrDataApp.Modules.Base.Android;

[assembly: Dependency(typeof(AndroidSpeechRecognizerProvider))]
namespace DlrDataApp.Modules.SpeechRecognition.Android
{
    class AndroidSpeechRecognizerProvider : ISpeechRecognizerProvider
    {
        const string ModelFolderName = "voskModel";
        const string FinishedIndicatorFileName = "finished";
        const string VoskModelFileName = "voskModelDe.zip";
        private Model Model;

        public Task LoadTask { get; }

        public AndroidSpeechRecognizerProvider()
        {
            LoadTask = Initialize();
        }

        public async Task Initialize()
        {
            var targetDir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ModelFolderName);
            var finishedIndicatorFilePath = Path.Combine(targetDir, FinishedIndicatorFileName);
            if (!File.Exists(finishedIndicatorFilePath))
            {
                var mainContext = DependencyService.Get<IMainActivityProvider>().MainActivity;
                var assets = mainContext.Assets;
                using (var modelZipStream = assets.Open(VoskModelFileName))
                {
                    await Helpers.UnzipFileAsync(modelZipStream, targetDir);
                    using (File.Create(finishedIndicatorFilePath)) { }
                }
            }
            Model = new Model(targetDir);
        }

        public ISpeechRecognizer Initialize(List<string> acceptedWords)
        {
            const int sampleRate = 44100;
            KaldiRecognizer kaldiRecognizer;
            if (acceptedWords == null)
            {
                kaldiRecognizer = new KaldiRecognizer(Model, sampleRate);
            }
            else
            {
                string lang = $"[{'"'}{string.Join(' ', acceptedWords.Where(k => k != "[unk]"))}{'"'}, {'"'}[unk]{'"'}]";
                kaldiRecognizer = new KaldiRecognizer(Model, sampleRate, lang);
            }
            var speechService = new SpeechService(kaldiRecognizer, sampleRate);
            var speechRecognizer = new AndroidSpeechRecognizer(speechService);
            speechService.AddListener(speechRecognizer);
            return speechRecognizer;
        }
    }
}