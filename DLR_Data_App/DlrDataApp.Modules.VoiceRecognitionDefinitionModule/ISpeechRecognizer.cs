using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.SpeechRecognition.Definition
{
    public interface ISpeechRecognizer
    {
        event EventHandler<SpeechRecognitionPartialResult> PartialResultRecognized;
        event EventHandler<SpeechRecognitionResult> ResultRecognized;

        bool StartListening();
        bool StopListening();
    }
}
