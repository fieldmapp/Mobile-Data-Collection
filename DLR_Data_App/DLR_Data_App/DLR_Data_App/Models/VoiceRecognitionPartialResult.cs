using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models
{
    public class VoiceRecognitionPartialResult
    {
        public string PartialResult;

        public VoiceRecognitionPartialResult(string partialResult)
        {
            PartialResult = partialResult;
        }
    }
}
