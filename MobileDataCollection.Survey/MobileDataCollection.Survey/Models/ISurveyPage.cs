//Main contributors: Maximilian Enderling
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public enum PageResult
    {
        Continue,
        Abort,
        Evaluation
    }
    interface ISurveyPage
    {
        IQuestionContent QuestionItem { get; }
        IUserAnswer AnswerItem { get; }
        event EventHandler<PageResult> PageFinished;
    }
}
