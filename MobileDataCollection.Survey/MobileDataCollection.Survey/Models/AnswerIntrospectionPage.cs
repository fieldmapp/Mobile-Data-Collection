using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class AnswerIntrospectionPage : BindableObject, IUserAnswer
    {
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(AnswerIntrospectionPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty SelectedAnswerProperty = BindableProperty.Create(nameof(SelectedAnswer), typeof(int), typeof(AnswerIntrospectionPage), 0, BindingMode.OneWay);

        /// <summary>
        /// Intern Id for Answers of this Type, corrosponds to same number as in QuestionImageCheckerPage
        /// </summary>
        public int InternId
        {
            get => (int)GetValue(InternIdProperty);
            set => SetValue(InternIdProperty, value);
        }

        /// <summary>
        /// Represents the index of the selected Answer (in inclusive range 1 - 5)
        /// </summary>
        public int SelectedAnswer
        {
            get => (int)GetValue(SelectedAnswerProperty);
            set => SetValue(SelectedAnswerProperty, value);
        }

        /// <summary>
        /// Is a reference to the belonging Question
        /// </summary>

        public AnswerIntrospectionPage(int internId, int selectedanswer)
        {
            InternId = internId;
            SelectedAnswer = selectedanswer;
        }

        public int EvaluateScore()
        {
            throw new NotSupportedException();
        }
    }
}
