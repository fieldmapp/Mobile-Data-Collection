using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.VoiceRecognitionDefinitionModule
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
