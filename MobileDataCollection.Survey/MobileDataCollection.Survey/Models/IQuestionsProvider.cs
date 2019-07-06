using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    public interface IQuestionsProvider
    {
        String LoadQuestionsForImageCheckerFromTXT();
    }
}
