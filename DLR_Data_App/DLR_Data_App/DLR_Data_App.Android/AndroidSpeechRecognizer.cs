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
using DLR_Data_App.Services;
using Xamarin.Forms;
using System.IO;
using System.Threading.Tasks;
using DLR_Data_App.Models;
using DLR_Data_App.Views;
using DLR_Data_App.Services.VoiceControl;

[assembly: Dependency(typeof(com.DLR.DLR_Data_App.Droid.AndroidSpeechRecognizer))]
namespace com.DLR.DLR_Data_App.Droid
{
    class AndroidSpeechRecognizer : Java.Lang.Object, IRecognitionListener, ISpeechRecognizer
    {
        static readonly List<string> acceptedWords = VoiceCommandCompiler.KeywordStrings;
        class PartialResult
        {
            public string partial;
        }

        class Result
        {
            public string text;
            public List<ResultPart> result;
        }

        class ResultPart
        {
            public float conf;
            public float end;
            public float start;
            public string word;
        }

        const string ModelFolderName = "voskModel";
        const string FinishedFileName = "finished";
        const string VoskModelFileName = "voskModelDe.zip";
        SpeechService KaldiRecognizer;
        bool ShouldBeRunning = false;

        public Task LoadTask { get; }

        public event EventHandler<VoiceRecognitionPartialResult> PartialResultRecognized;
        public event EventHandler<VoiceRecognitionResult> ResultRecognized;

        public AndroidSpeechRecognizer()
        {
            LoadTask = Initialize();
        }

        public async Task Initialize()
        {
            var targetDir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ModelFolderName);
            var finishedFilePath = Path.Combine(targetDir, FinishedFileName);
            if (!File.Exists(finishedFilePath))
            {
                var mainContext = Android.App.Application.Context;
                var assets = mainContext.Assets;
                using (var modelZipStream = assets.Open(VoskModelFileName))
                {
                    await Helpers.UnzipFileAsync(modelZipStream, targetDir);
                    using (File.Create(finishedFilePath)) { }
                }
            }

            try
            {
                var model = new Model(targetDir);
                const int sampleRate = 44100;
                string lang = $"[{'"'}{string.Join(' ', acceptedWords.Where(k => k != "[unk]"))}{'"'}, {'"'}[unk]{'"'}]";
                var kaldiRecognizer = new KaldiRecognizer(model, sampleRate, lang);
                KaldiRecognizer = new SpeechService(kaldiRecognizer, sampleRate);
                KaldiRecognizer.AddListener(this);
                if (ShouldBeRunning)
                {
                    KaldiRecognizer.StartListening();
                }
            }
            catch (System.Exception e)
            {

            }
        }
        public void OnError(Java.Lang.Exception ex) { }

        public void OnPartialResult(string p0)
        {
            var partialResult = JsonTranslator.GetFromJson<PartialResult>(p0);
            if (!string.IsNullOrWhiteSpace(partialResult.partial))
                PartialResultRecognized?.Invoke(this, new VoiceRecognitionPartialResult(partialResult.partial));
        }

        public void OnResult(string p0)
        {
            var result = JsonTranslator.GetFromJson<Result>(p0);
            if (!string.IsNullOrWhiteSpace(result.text))
                ResultRecognized?.Invoke(this, new VoiceRecognitionResult(result.text, result.result.Select(r => new VoiceRecognitionResultPart(r.word, r.start, r.end, r.conf)).ToList()));
        }

        public void OnTimeout()
        {

        }

        public void Start()
        {
            KaldiRecognizer?.StartListening();
            ShouldBeRunning = true;
        }

        public void Stop()
        {
            KaldiRecognizer?.Stop();
            ShouldBeRunning = false;
        }
    }
}