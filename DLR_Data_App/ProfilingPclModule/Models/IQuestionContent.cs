//Main contributors: Maximilian Enderling
using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.ProfilingSharedModule.Models
{
    public interface IQuestionContent
    {
        int Difficulty { get; }
        int InternId { get; }

        void Translate(Dictionary<string, string> translations);
    }
}
