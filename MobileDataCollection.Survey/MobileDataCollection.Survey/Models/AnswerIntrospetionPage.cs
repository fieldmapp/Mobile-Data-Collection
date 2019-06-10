using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class AnswerIntrospetionPage : BindableObject
    {
        public static readonly BindableProperty SelectedAnswerProperty = BindableProperty.Create(nameof(SelectedAnswer), typeof(int), typeof(AnswerIntrospetionPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty QuestionProperty = BindableProperty.Create(nameof(Question), typeof(QuestionIntrospectionPage), typeof(AnswerIntrospetionPage), null, BindingMode.OneWay);

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
        public QuestionIntrospectionPage Question
        {
            get => (QuestionIntrospectionPage)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }

        public AnswerIntrospetionPage(QuestionIntrospectionPage question, int selectedanswer)
        {
            Question = question;
            SelectedAnswer = selectedanswer;
        }
    }
}
