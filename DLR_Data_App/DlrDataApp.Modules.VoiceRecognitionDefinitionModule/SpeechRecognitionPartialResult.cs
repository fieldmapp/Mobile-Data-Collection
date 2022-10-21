using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.SpeechRecognition.Definition
{
    public class SpeechRecognitionPartialResult
    {
        public string PartialResult;

        public SpeechRecognitionPartialResult(string partialResult)
        {
            PartialResult = partialResult;
        }
    }
}
