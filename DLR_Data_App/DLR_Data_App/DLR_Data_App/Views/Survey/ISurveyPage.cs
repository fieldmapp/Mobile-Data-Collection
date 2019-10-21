//Main contributors: Maximilian Enderling
using DLR_Data_App.Models.Survey;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Views.Survey
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
