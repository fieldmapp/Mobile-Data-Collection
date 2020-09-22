//Main contributors: Maximilian Enderling
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models.Profiling
{
    public interface IQuestionContent
    {
        int Difficulty { get; }
        int InternId { get; }

        void Translate(Dictionary<string, string> translations);
    }
}
