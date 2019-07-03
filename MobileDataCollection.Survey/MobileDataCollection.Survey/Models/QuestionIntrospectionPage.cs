using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{

    public class QuestionIntrospectionPage : BindableObject, IQuestionContent
    {
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(QuestionIntrospectionPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty QuestionTextProperty = BindableProperty.Create(nameof(QuestionText), typeof(string), typeof(QuestionIntrospectionPage), string.Empty, BindingMode.OneWay);

        /// <summary>
        /// Intern Id for Answers of this Type, corrosponds to same number as in QuestionImageCheckerPage
        /// </summary>
        public int InternId
        {
            get => (int)GetValue(InternIdProperty);
            set => SetValue(InternIdProperty, value);
        }

        public string QuestionText
        {
            get => (string)GetValue(QuestionTextProperty);
            set => SetValue(QuestionTextProperty, value);
        }

        public int Difficulty => -1;

        public QuestionIntrospectionPage(int internId, string text)
        {
            InternId = internId;
            QuestionText = text;
        }
    }
}
