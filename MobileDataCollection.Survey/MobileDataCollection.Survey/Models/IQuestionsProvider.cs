//Main contributors: Henning Woydt
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    /// <summary>
    /// An interface which is used in the Android-Project to read txt files
    /// </summary>
    public interface IQuestionsProvider
    {
        String LoadTextFromTxt(String sourceTxt);
    }
}
