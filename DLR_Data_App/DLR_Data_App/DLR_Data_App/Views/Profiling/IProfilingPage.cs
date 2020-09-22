//Main contributors: Maximilian Enderling
using DLR_Data_App.Models.Profiling;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Views.Profiling
{
    public enum PageResult
    {
        Continue,
        Abort,
        Evaluation
    }
    interface IProfilingPage
    {
        IQuestionContent QuestionItem { get; }
        IUserAnswer AnswerItem { get; }
        event EventHandler<PageResult> PageFinished;
    }
}
