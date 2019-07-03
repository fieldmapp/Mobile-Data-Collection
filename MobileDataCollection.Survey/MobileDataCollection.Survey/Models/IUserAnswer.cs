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
        /// <returns>Score without general range. Range should be defined in inheriting classes</returns>
        int EvaluateScore();
    }
}
