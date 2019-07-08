using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    public interface IQuestionContent
    {
        int Difficulty { get; }
        int InternId { get; }
    }
}
