using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class AnswerDoubleSliderPage : BindableObject, IUserAnswer
    {
        /// <summary>
        /// Binding of parameters in AnswerItem of DoubleSliderPage
        /// </summary>
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(AnswerDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty ResultQuestionAProperty = BindableProperty.Create(nameof(ResultQuestionA), typeof(int), typeof(AnswerDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty ResultQuestionBProperty = BindableProperty.Create(nameof(ResultQuestionB), typeof(int), typeof(AnswerDoubleSliderPage), 0, BindingMode.OneWay);

        /// <summary>
        /// Intern Id for Answers of this Type, corrosponds to same number as in QuestionDoubleSliderPage
        /// </summary>
        public int InternId
        {
            get => (int)GetValue(InternIdProperty);
            set => SetValue(InternIdProperty, value);
        }

        /// <summary>
        /// Answer for Question A given by the user. Check <see /cref="Answered"/> to see if user has submitted the answer first.
        /// </summary>
        public int ResultQuestionA
        {
            get => (int)GetValue(ResultQuestionAProperty);
            set => SetValue(ResultQuestionAProperty, value);
        }

        /// <summary>
        /// Answer for Question B given by the user. Check <see /cref="Answered"/> to see if user has submitted the answer first.
        /// </summary>
        public int ResultQuestionB
        {
            get => (int)GetValue(ResultQuestionBProperty);
            set => SetValue(ResultQuestionBProperty, value);
        }

        /// <summary>
        /// The constructor of AnswerItem in DoubleSliderPage
        /// </summary>
        public AnswerDoubleSliderPage(int internId, int resultA, int resultB)
        {
            InternId = internId;
            ResultQuestionA = resultA;
            ResultQuestionB = resultB;
        }

        public float EvaluateScore()
        {
            float evalSingleSlider(int correctAnswer, int givenAnswer)
            {
                int diff = Math.Abs(correctAnswer - givenAnswer);
                int maxDiff = Math.Max(correctAnswer, 100 - correctAnswer);
                float adjustedDiff = diff * 100f / maxDiff;
                return 1 - adjustedDiff / 100;
            }
            var question = (QuestionDoubleSliderPage)DatabankCommunication.LoadQuestionById("DoubleSlider", InternId);
            return evalSingleSlider(question.CorrectAnswerA, ResultQuestionA) * .5f
                + evalSingleSlider(question.CorrectAnswerB, ResultQuestionB) * .5f;
        }
    }
}
