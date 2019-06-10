using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class QuestionImageCheckerPage : BindableObject
    {
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(QuestionImageCheckerPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty NumberOfPossibleAnswersProperty = BindableProperty.Create(nameof(NumberOfPossibleAnswers), typeof(int), typeof(QuestionImageCheckerPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty DifficultyProperty = BindableProperty.Create(nameof(Difficulty), typeof(int), typeof(QuestionImageCheckerPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty QuestionTextProperty = BindableProperty.Create(nameof(QuestionText), typeof(string), typeof(QuestionImageCheckerPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty Image1CorrectProperty = BindableProperty.Create(nameof(Image1Correct), typeof(int), typeof(QuestionImageCheckerPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty Image2CorrectProperty = BindableProperty.Create(nameof(Image2Correct), typeof(int), typeof(QuestionImageCheckerPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty Image3CorrectProperty = BindableProperty.Create(nameof(Image3Correct), typeof(int), typeof(QuestionImageCheckerPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty Image4CorrectProperty = BindableProperty.Create(nameof(Image4Correct), typeof(int), typeof(QuestionImageCheckerPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty Image1SourceProperty = BindableProperty.Create(nameof(Image1Source), typeof(string), typeof(QuestionImageCheckerPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty Image2SourceProperty = BindableProperty.Create(nameof(Image2Source), typeof(string), typeof(QuestionImageCheckerPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty Image3SourceProperty = BindableProperty.Create(nameof(Image3Source), typeof(string), typeof(QuestionImageCheckerPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty Image4SourceProperty = BindableProperty.Create(nameof(Image4Source), typeof(string), typeof(QuestionImageCheckerPage), string.Empty, BindingMode.OneWay);


        /// <summary>
        /// Intern Id only for this Type Of Question
        /// </summary>
        [PrimaryKey,AutoIncrement]
        public int InternId
        {
            get => (int)GetValue(InternIdProperty);
            set => SetValue(InternIdProperty, value);
        }

        /// <summary>
        /// Maximum count of selected answers allowed
        /// </summary>
        public int NumberOfPossibleAnswers //war auf 4 gesetzt
        {
            get => (int)GetValue(NumberOfPossibleAnswersProperty);
            set => SetValue(NumberOfPossibleAnswersProperty, value);
        }

        /// <summary>
        /// The difficulty of the Question
        /// </summary>
        public int Difficulty
        {
            get => (int)GetValue(DifficultyProperty);
            set => SetValue(DifficultyProperty, value);
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
        /// Reflects whether image 1 should be selected in a correct answer.
        /// </summary>
        public int Image1Correct
        {
            get => (int)GetValue(Image1CorrectProperty);
            set => SetValue(Image1CorrectProperty, value);
        }

        /// <summary>
        /// Reflects whether image 2 should be selected in a correct answer.
        /// </summary>
        public int Image2Correct
        {
            get => (int)GetValue(Image2CorrectProperty);
            set => SetValue(Image2CorrectProperty, value);
        }

        /// <summary>
        /// Reflects whether image 3 should be selected in a correct answer.
        /// </summary>
        public int Image3Correct
        {
            get => (int)GetValue(Image3CorrectProperty);
            set => SetValue(Image3CorrectProperty, value);
        }

        /// <summary>
        /// Reflects whether image 4 should be selected in a correct answer.
        /// </summary>
        public int Image4Correct
        {
            get => (int)GetValue(Image4CorrectProperty);
            set => SetValue(Image4CorrectProperty, value);
        }

        /// <summary>
        /// Represents the URI of Image 1
        /// </summary>
        public string Image1Source
        {
            get => (string)GetValue(Image1SourceProperty);
            set => SetValue(Image1SourceProperty, value);
        }

        /// <summary>
        /// Represents the URI of Image 2
        /// </summary>
        public string Image2Source
        {
            get => (string)GetValue(Image2SourceProperty);
            set => SetValue(Image2SourceProperty, value);
        }

        /// <summary>
        /// Represents the URI of Image 3
        /// </summary>
        public string Image3Source
        {
            get => (string)GetValue(Image3SourceProperty);
            set => SetValue(Image3SourceProperty, value);
        }

        /// <summary>
        /// Represents the URI of Image 4
        /// </summary>
        public string Image4Source
        {
            get => (string)GetValue(Image4SourceProperty);
            set => SetValue(Image4SourceProperty, value);
        }

        public QuestionImageCheckerPage(string question, int difficulty, int im1Correct, int im2Correct, int im3Correct, int im4Corect, 
            string im1Source, string im2Source, string im3Source, string im4Source)
        {
            QuestionText = question;
            Difficulty = difficulty;
            Image1Correct = im1Correct;
            Image2Correct = im2Correct;
            Image3Correct = im3Correct;
            Image4Correct = im4Corect;
            Image1Source = im1Source;
            Image2Source = im2Source;
            Image3Source = im3Source;
            Image4Source = im4Source;
            NumberOfPossibleAnswers = 4;
        }
    }
}
