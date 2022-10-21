using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.SpeechRecognition.Definition
{
    public class SpeechRecognitionResult
    {
        public string Result;
        public List<SpeechRecognitionResultPart> Parts;
        public SpeechRecognitionResult(string result, List<SpeechRecognitionResultPart> parts)
        {
            Result = result;
            Parts = parts;
        }
    }
}
