using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class AnswerStadiumPage : BindableObject, IUserAnswer
    {
        /// <summary>
        /// Binding of parameters in AnswerItem of StadiumPage
        /// </summary>
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(AnswerStadiumPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty AnswerFruitTypeProperty = BindableProperty.Create(nameof(AnswerFruitType), typeof(string), typeof(AnswerStadiumPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty AnswerStadiumProperty = BindableProperty.Create(nameof(AnswerStadium), typeof(string), typeof(AnswerStadiumPage), string.Empty, BindingMode.OneWay);

        /// <summary>
        /// Intern Id for Answers of this Type, corrosponds to same number as in QuestionStadiumPage
        /// </summary>
        public int InternId
        {
            get => (int)GetValue(InternIdProperty);
            set => SetValue(InternIdProperty, value);
        }

        /// <summary>
        /// Selected fruit type
        /// </summary>
        string AnswerFruitType
        {
            get => (string)GetValue(AnswerFruitTypeProperty);
            set => SetValue(AnswerFruitTypeProperty, value);
        }

        /// <summary>
        /// Selected stadium
        /// </summary>
        string AnswerStadium
        {
            get => (string)GetValue(AnswerStadiumProperty);
            set => SetValue(AnswerStadiumProperty, value);
        }

        /// <summary>
        /// The constructor of AnswerItem in StadiumPage
        /// </summary>

        public AnswerStadiumPage(int internId, string answerFruitType, string answerStadium)
        {
            InternId = internId;
            AnswerFruitType = answerFruitType;
            AnswerStadium = answerStadium;
        }

        public float EvaluateScore()
        {
            var question = DatabankCommunication.LoadQuestionStadiumPageById(InternId);
            float score = 0;
            if (question.CorrectAnswerFruitType == AnswerFruitType)
                score += .5f;
            if (question.CorrectAnswerStadium == AnswerStadium)
                score += .5f;
            return score;
        }
    }
}
