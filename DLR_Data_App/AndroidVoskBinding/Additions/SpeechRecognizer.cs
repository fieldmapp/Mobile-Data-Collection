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
using Org.Kaldi;

namespace Org.Kaldi
{
    public partial class SpeechRecognizer
    {
        string Grammar;
        Model Model;
        public SpeechRecognizer(Model model, string grammar) : this(model)
        {
            Model = model;
            Grammar = grammar;
            var recognizerField = Class.GetDeclaredField("recognizer");
            recognizerField.Accessible = true;
            RecognizerHolder = new KaldiRecognizer(model, 16000, grammar);
            recognizerField.Set(this, RecognizerHolder);
            recognizerField.Accessible = false;
        }

        KaldiRecognizer RecognizerHolder;
    }
}