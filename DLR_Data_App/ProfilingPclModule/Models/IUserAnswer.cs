//Main contributors: Maximilian Enderling
using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.Profiling.Shared.Models
{
    public interface IUserAnswer
    {
        /// <summary>
        /// Calculates the score of this IUserAnswer
        /// </summary>
        /// <returns>Score without general range. Must be between 0 and 1 (inclusive)</returns>
        float EvaluateScore();

        int InternId { get; }
    }
}
