using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    class Question
    {
        public Type QuestionContentType, UserAnswerType;
    }
    class Question<T> : Question where T:IQuestionContent
    {
        public T QuestionContent { get; set; }

        public Question(T questionContent)
        {
            QuestionContent = questionContent;
            QuestionContentType = typeof(T);
        }
    }
    class Question<T,U> : Question<T> where T:IQuestionContent where U:IUserAnswer
    {
        public U UserAnswer { get; set; }

        public Question(T questionContent, U userAnswer) : base(questionContent)
        {
            UserAnswerType = typeof(U);
            UserAnswer = userAnswer;
        }
    }
}
