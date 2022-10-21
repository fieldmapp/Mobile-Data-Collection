using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DlrDataApp.Modules.SpeechRecognition.Definition
{
    public interface ISpeechRecognizerProvider
    {
        ISpeechRecognizer Initialize(List<string> acceptedWords);
        Task LoadTask { get; }
    }
}
