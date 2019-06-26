using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class AnswerStadiumPage : BindableObject, IUserAnswer
    {
        public static readonly BindableProperty AnswerFruitTypeProperty = BindableProperty.Create(nameof(AnswerFruitType), typeof(string), typeof(AnswerStadiumPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty AnswerStadiumProperty = BindableProperty.Create(nameof(AnswerStadium), typeof(string), typeof(AnswerStadiumPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty QuestionProperty = BindableProperty.Create(nameof(Question), typeof(QuestionIntrospectionPage), typeof(AnswerStadiumPage), null, BindingMode.OneWay);

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
        /// Is a reference to the belonging Question
        /// </summary>
        QuestionStadiumPage Question
        {
            get => (QuestionStadiumPage)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }

        public AnswerStadiumPage(string answerFruitType, string answerStadium, QuestionStadiumPage question)
        {
            AnswerFruitType = answerFruitType;
            AnswerStadium = answerStadium;
            Question = question;
        }
    }
}
