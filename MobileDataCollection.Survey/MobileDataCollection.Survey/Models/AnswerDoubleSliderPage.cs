using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class AnswerDoubleSliderPage : BindableObject, IUserAnswer
    {
        public static readonly BindableProperty ResultQuestionAProperty = BindableProperty.Create(nameof(ResultQuestionA), typeof(int), typeof(AnswerDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty ResultQuestionBProperty = BindableProperty.Create(nameof(ResultQuestionB), typeof(int), typeof(AnswerDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty QuestionProperty = BindableProperty.Create(nameof(Question), typeof(QuestionDoubleSliderPage), typeof(AnswerDoubleSliderPage), null, BindingMode.OneWay);

        /// <summary>
        /// Answer for Question A given by the user. Check <see cref="Answered"/> to see if user has submitted the answer first.
        /// </summary>
        public int ResultQuestionA
        {
            get => (int)GetValue(ResultQuestionAProperty);
            set => SetValue(ResultQuestionAProperty, value);
        }

        /// <summary>
        /// Answer for Question B given by the user. Check <see cref="Answered"/> to see if user has submitted the answer first.
        /// </summary>
        public int ResultQuestionB
        {
            get => (int)GetValue(ResultQuestionBProperty);
            set => SetValue(ResultQuestionBProperty, value);
        }

        /// <summary>
        /// Is a reference to the belonging Question
        /// </summary>
        public QuestionDoubleSliderPage Question
        {
            get => (QuestionDoubleSliderPage)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }

        public AnswerDoubleSliderPage(QuestionDoubleSliderPage question, int resultA, int resultB)
        {
            Question = question;
            ResultQuestionA = resultA;
            ResultQuestionB = resultB;
        }
    }
}
