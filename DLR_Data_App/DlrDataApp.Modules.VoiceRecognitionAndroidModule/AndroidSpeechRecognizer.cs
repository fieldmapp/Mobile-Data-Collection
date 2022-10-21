using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.SpeechRecognition.Definition;
using Org.Kaldi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DlrDataApp.Modules.SpeechRecognition.Android
{
    class AndroidSpeechRecognizer : Java.Lang.Object, IRecognitionListener, ISpeechRecognizer
    {
        private SpeechService SpeechService;

        public AndroidSpeechRecognizer(SpeechService speechService)
        {
            SpeechService = speechService;
        }

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


        public event EventHandler<SpeechRecognitionPartialResult> PartialResultRecognized;
        public event EventHandler<SpeechRecognitionResult> ResultRecognized;

        public void OnError(Java.Lang.Exception p0) { }

        public void OnPartialResult(string p0)
        {
            var partialResult = JsonTranslator.GetFromJson<PartialResult>(p0);
            if (!string.IsNullOrWhiteSpace(partialResult.partial))
                PartialResultRecognized?.Invoke(this, new SpeechRecognitionPartialResult(partialResult.partial));
        }

        public void OnResult(string p0)
        {
            var result = JsonTranslator.GetFromJson<Result>(p0);
            if (!string.IsNullOrWhiteSpace(result.text))
                ResultRecognized?.Invoke(this, new SpeechRecognitionResult(result.text, result.result.Select(r => new SpeechRecognitionResultPart(r.word, r.start, r.end, r.conf)).ToList()));
        }

        public void OnTimeout() { }

        public bool StartListening()
        {
            return SpeechService.StartListening();
        }

        public bool StopListening()
        {
            return SpeechService.Stop();
        }
    }
}