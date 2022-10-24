using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DlrDataApp.Modules.SpeechRecognition.Android;
using DlrDataApp.Modules.SpeechRecognition.Shared;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidSpeechRecognitionPlatformDependencyLoader))]
namespace DlrDataApp.Modules.SpeechRecognition.Android
{
    public class AndroidSpeechRecognitionPlatformDependencyLoader : ISpeechRecognitionPlatformDependencyLoader
    {
        public void LoadLibraries()
        {
            JavaSystem.LoadLibrary("vosk_jni");
        }
    }
}