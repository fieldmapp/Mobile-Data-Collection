using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class QuestionIntrospectionPage : BindableObject, IQuestionContent
    {
        public static readonly BindableProperty QuestionTextProperty = BindableProperty.Create(nameof(QuestionText), typeof(string), typeof(QuestionIntrospectionPage), string.Empty, BindingMode.OneWay);

        public string QuestionText
        {
            get => (string)GetValue(QuestionTextProperty);
            set => SetValue(QuestionTextProperty, value);
        }

        public QuestionIntrospectionPage(string text)
        {
            QuestionText = text;
        }

        public QuestionIntrospectionPage()
        {

        }
    }
}
