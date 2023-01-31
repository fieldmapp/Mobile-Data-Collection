using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.SpeechRecognition.Shared
{
    public interface ISpeechRecognitionPlatformDependencyLoader
    {
        void LoadLibraries();
    }
}
