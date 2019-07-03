using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class AnswerDoubleSliderPage : BindableObject, IUserAnswer
    {
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(AnswerDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty ResultQuestionAProperty = BindableProperty.Create(nameof(ResultQuestionA), typeof(int), typeof(AnswerDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty ResultQuestionBProperty = BindableProperty.Create(nameof(ResultQuestionB), typeof(int), typeof(AnswerDoubleSliderPage), 0, BindingMode.OneWay);

        /// <summary>
        /// Intern Id for Answers of this Type, corrosponds to same number as in QuestionImageCheckerPage
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
        /// Is a reference to the belonging Question
        /// </summary>
        public AnswerDoubleSliderPage(int internId, int resultA, int resultB)
        {
            InternId = internId;
            ResultQuestionA = resultA;
            ResultQuestionB = resultB;
        }

        public int EvaluateScore()
        {
            throw new NotImplementedException();
        }
    }
}
