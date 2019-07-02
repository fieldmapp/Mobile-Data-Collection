using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    /// <summary>
    /// This class is responsible for keeping question data, both
    /// QuestionContent and UserAnswers. Chooses next Question (+type).
    /// Dont keep when switching users.
    /// </summary>
    class SurveyManager
    {
        public SurveyManager()
        {
            var demoDoubleSlider = new QuestionDoubleSliderPage(1,1,"Q3G1B1_klein.png", 7, 4);

            Questions = new List<Question>()
            {
                new Question<QuestionDoubleSliderPage, AnswerDoubleSliderPage>(demoDoubleSlider, new AnswerDoubleSliderPage(demoDoubleSlider.InternId, 10, 20))
            };
        }

        public Question<T> GetFirstQuestion<T>() where T:IQuestionContent
        {
            var value = Questions.Find(q => q is Question<T>);
            return value as Question<T>;
        }


        List<Question> Questions;
    }
}
