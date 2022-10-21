using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.SpeechRecognition.Definition
{
    public class SpeechRecognitionResultPart
    {
        public float Confidence;
        public float End;
        public float Start;
        public string Word;

        public SpeechRecognitionResultPart(string word, float start, float end, float confidence)
        {
            Confidence = confidence;
            End = end;
            Start = start;
            Word = word;
        }
    }
}
