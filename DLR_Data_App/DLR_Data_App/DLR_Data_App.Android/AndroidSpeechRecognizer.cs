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

[assembly: Dependency(typeof(com.DLR.DLR_Data_App.Droid.AndroidSpeechRecognizer))]
namespace com.DLR.DLR_Data_App.Droid
{
    class AndroidSpeechRecognizer : ISpeechRecognizer
    {
        class KaldiListener : Java.Lang.Object, IRecognitionListener
        {

            public void OnError(Java.Lang.Exception ex) { }

            public void OnPartialResult(string p0)
            {

            }

            public void OnResult(string p0)
            {

            }

            public void OnTimeout()
            {

            }
        }

        const string ModelFolderName = "voskModel";
        const string FinishedFileName = "finished";
        const string VoskModelFileName = "voskModelDe.zip";

        KaldiListener Listener = new KaldiListener();
        Org.Kaldi.SpeechRecognizer KaldiRecognizer;
        bool ShouldBeRunning = false;
        public AndroidSpeechRecognizer()
        {
            Initialize();
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
                KaldiRecognizer = new Org.Kaldi.SpeechRecognizer(model);
                KaldiRecognizer.AddListener(Listener);
                if (ShouldBeRunning)
                {
                    KaldiRecognizer.StartListening();
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
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