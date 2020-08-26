using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models
{
    public class VoiceRecognitionResultPart
    {
        public float Confidence;
        public float End;
        public float Start;
        public string Word;

        public VoiceRecognitionResultPart(string word, float start, float end, float confidence)
        {
            Confidence = confidence;
            End = end;
            Start = start;
            Word = word;
        }
    }
}
