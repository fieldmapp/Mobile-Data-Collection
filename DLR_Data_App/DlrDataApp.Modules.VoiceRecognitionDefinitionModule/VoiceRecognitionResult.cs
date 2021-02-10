using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.VoiceRecognitionDefinitionModule
{
    public class VoiceRecognitionResult
    {
        public string Result;
        public List<VoiceRecognitionResultPart> Parts;
        public VoiceRecognitionResult(string result, List<VoiceRecognitionResultPart> parts)
        {
            Result = result;
            Parts = parts;
        }
    }
}
