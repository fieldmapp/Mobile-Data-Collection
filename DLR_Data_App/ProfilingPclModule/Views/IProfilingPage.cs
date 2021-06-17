//Main contributors: Maximilian Enderling
using DlrDataApp.Modules.Profiling.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Profiling.Shared.Views
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
