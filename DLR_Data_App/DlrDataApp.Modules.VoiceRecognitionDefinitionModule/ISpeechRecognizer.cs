using DLR_Data_App.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DlrDataApp.Modules.VoiceRecognitionDefinitionModule
{
    public interface ISpeechRecognizer
    {
        event EventHandler<VoiceRecognitionPartialResult> PartialResultRecognized;
        event EventHandler<VoiceRecognitionResult> ResultRecognized;
        void Start();
        void Stop();
        Task LoadTask { get; }
    }
}
