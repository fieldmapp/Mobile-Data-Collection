//Main contributors: Maya Koehnen, Max Moebius
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Profiling.Shared.Models
{
	public class QuestionDoubleSliderPage : BindableObject, IQuestionContent
    {
        /// <summary>
        /// Binding of parameters in QuestionItem of DoubleSliderPage
        /// </summary>
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(QuestionDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty DifficultyProperty = BindableProperty.Create(nameof(Difficulty), typeof(int), typeof(QuestionDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty QuestionTextProperty = BindableProperty.Create(nameof(QuestionText), typeof(string), typeof(QuestionDoubleSliderPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty PictureAdressProperty = BindableProperty.Create(nameof(PictureAddress), typeof(string), typeof(QuestionDoubleSliderPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerAProperty = BindableProperty.Create(nameof(CorrectAnswerA), typeof(int), typeof(QuestionDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerBProperty = BindableProperty.Create(nameof(CorrectAnswerB), typeof(int), typeof(QuestionDoubleSliderPage), 0, BindingMode.OneWay);
        // property: Beantwortet


        /// <summary>
        /// Defines the maximum valid value of <see /cref="Level"/>
        /// </summary>
        //const int HighestQuestionDifficulty = 3;

        /// <summary>
        /// Intern Id only for this type Of question
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
        /// Represents the picture URI
        /// </summary>
        public string PictureAddress
        {
            get => (string)GetValue(PictureAdressProperty);
            set
            {
                SetValue(PictureAdressProperty, value);
            }
        }

        /// <summary>
        /// Correct answer for SliderA (Bodenbedeckung)
        /// </summary>
        public int CorrectAnswerA
        {
            get => (int)GetValue(CorrectAnswerAProperty);
            set => SetValue(CorrectAnswerAProperty, value);
        }

        /// <summary>
        /// Correct answer for SliderB (Grüne Pflanzenanteile)
        /// </summary>
        public int CorrectAnswerB
        {
            get => (int)GetValue(CorrectAnswerBProperty);
            set => SetValue(CorrectAnswerBProperty, value);
        }

        /// <summary>
        /// Difficulty of the question. Must be in range 1 to <see /cref="HighestQuestionDifficulty"/> (inclusive)
        /// </summary>
        public int Difficulty
        {
            get => (int)GetValue(DifficultyProperty);
            set => SetValue(DifficultyProperty, value);
        }

        /// <summary>
        /// The constructor of QuestionItem in DoubleSliderPage
        /// </summary>
        public QuestionDoubleSliderPage(int internId, int difficulty, string pictureAddress, int answerA, int answerB)
        {
            InternId = internId;
            QuestionText = "Schätzen Sie den Grad der Bedeckung des Bodens durch Pflanzen (A) und den Anteil grüner Pflanzenbestandteile (B) ein.";
            PictureAddress = pictureAddress;
            CorrectAnswerA = answerA;
            CorrectAnswerB = answerB;
            Difficulty = difficulty;
        }

        public void Translate(Dictionary<string, string> translations)
        {
            QuestionText = Helpers.GetCurrentLanguageTranslation(translations, QuestionText);
        }
    }
}