using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    interface IUserAnswer
    {
        /// <summary>
        /// Calculates the score of this IUserAnswer
        /// </summary>
        /// <returns>Score without general range. Must be between 0 and 1 (inclusive)</returns>
        float EvaluateScore();
    }
}
