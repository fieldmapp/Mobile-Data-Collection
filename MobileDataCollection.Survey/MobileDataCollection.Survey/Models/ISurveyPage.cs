using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    interface ISurveyPage
    {
        IQuestionContent QuestionItem { get; }
        IUserAnswer AnswerItem { get; }
        event EventHandler PageFinished;
    }
}
