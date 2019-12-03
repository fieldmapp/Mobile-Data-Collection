//Main contributors: Max Moebius, Maya Koehnen
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.Survey
{
    public class QuestionIntrospectionPage : BindableObject, IQuestionContent
    {
        /// <summary>
        /// Binding of parameters in QuestionItem of QuestionIntrospectionPage
        /// </summary>
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(QuestionIntrospectionPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty QuestionTextProperty = BindableProperty.Create(nameof(QuestionText), typeof(string), typeof(QuestionIntrospectionPage), string.Empty, BindingMode.OneWay);

        /// <summary>
        /// Intern Id only for this type Of question(IntrospectionPage)
        /// </summary>
        public int InternId
        {
            get => (int)GetValue(InternIdProperty);
            set => SetValue(InternIdProperty, value);
        }

        /// <summary>
        /// Question which will be shown to the user
        /// </summary>
        public string QuestionText
        {
            get => (string)GetValue(QuestionTextProperty);
            set => SetValue(QuestionTextProperty, value);
        }

        /// <summary>
        /// No specific difficultylevel for this type of question
        /// </summary>
        public int Difficulty => -1;

        /// <summary>
        /// The constructor of QuestionItem in IntrospectionPage
        /// </summary>
        /// <param name="internId"></param>
        /// <param name="text"></param>
        public QuestionIntrospectionPage(int internId, string text)
        {
            InternId = internId;
            QuestionText = text;
        }

        public void Translate(Dictionary<string, string> translations)
        {
            QuestionText = Helpers.GetCurrentLanguageTranslation(translations, QuestionText);
        }
    }
}
