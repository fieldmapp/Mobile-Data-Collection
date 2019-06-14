using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
	public class QuestionDoubleSliderPage : BindableObject, IQuestionContent
    {
        public static readonly BindableProperty AnswersNeededProperty = BindableProperty.Create(nameof(AnswersNeeded), typeof(int), typeof(QuestionDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(QuestionDoubleSliderPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty PictureAdressProperty = BindableProperty.Create(nameof(PictureAddress), typeof(string), typeof(QuestionDoubleSliderPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerAProperty = BindableProperty.Create(nameof(CorrectAnswerA), typeof(int), typeof(QuestionDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerBProperty = BindableProperty.Create(nameof(CorrectAnswerB), typeof(int), typeof(QuestionDoubleSliderPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty PictureSourceProperty = BindableProperty.Create(nameof(PictureSource), typeof(ImageSource), typeof(QuestionDoubleSliderPage), null, BindingMode.OneWay);
        public static readonly BindableProperty LevelProperty = BindableProperty.Create(nameof(Level), typeof(int), typeof(QuestionDoubleSliderPage), 0, BindingMode.OneWay);

        /// <summary>
        /// Defines the maximum valid value of <see cref="Level"/>
        /// </summary>
        const int HighestQuestionDifficulty = 3;

        /// <summary>
        /// Number of available questions (Remove?)
        /// </summary>
        public int AnswersNeeded
        {
            get => (int)GetValue(AnswersNeededProperty);
            set => SetValue(AnswersNeededProperty, value);
        }

        /// <summary>
        /// Question text
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ImageSource PictureSource
        {
            get => (ImageSource)GetValue(PictureSourceProperty);
            private set => SetValue(PictureSourceProperty, value);
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
                PictureSource = ImageSource.FromFile(value);
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
        /// Level of the question. Must be in range 1 to <see cref="HighestQuestionDifficulty"/> (inclusive)
        /// </summary>
        public int Level
        {
            get => (int)GetValue(LevelProperty);
            set
            {
                if (value > HighestQuestionDifficulty)
                    throw new NotImplementedException($"{nameof(value)} must be at most {nameof(HighestQuestionDifficulty)}={HighestQuestionDifficulty}");
                if (value < 1)
                    throw new NotImplementedException($"{nameof(value)} must be at least 1");
                SetValue(LevelProperty, value);
            }
        }
        
        //public QuestionDoubleSliderPage(string pictureAddress, ImageSource pictureSource, int answerA, int answerB, int level)
        public QuestionDoubleSliderPage(string pictureAddress, int answerA, int answerB, int level)
        {
            Text = "Schätzen Sie den Grad der Bedeckung des Bodens durch Pflanzen (A) und den Anteil grüner Pflanzenbestandteile (B) ein.";
            PictureAddress = pictureAddress;
            CorrectAnswerA = answerA;
            CorrectAnswerB = answerB;
            Level = level;
        }

        // Constructor is needed for the database
        public QuestionDoubleSliderPage()
        {

        }

    }
}