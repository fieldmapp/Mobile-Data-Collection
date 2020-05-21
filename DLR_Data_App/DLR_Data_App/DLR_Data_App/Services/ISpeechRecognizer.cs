using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Services
{
    public interface ISpeechRecognizer
    {
        void Start();
        void Stop();
    }
}
