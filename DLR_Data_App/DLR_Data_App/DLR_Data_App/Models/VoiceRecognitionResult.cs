using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models
{
    public class VoiceRecognitionResult
    {
        public string Result;

        public VoiceRecognitionResult(string result)
        {
            Result = result;
        }
    }
}
