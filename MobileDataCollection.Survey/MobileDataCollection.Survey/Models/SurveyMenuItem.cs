using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public enum SurveyMenuItemType
    {
        Introspection,
        ImageChecker,
        DoubleSlider,
        Stadium
    }
    public class SurveyMenuItem : BindableObject
    {
        public static readonly BindableProperty IdProperty = BindableProperty.Create(nameof(Id), typeof(SurveyMenuItemType), typeof(SurveyMenuItem), null, BindingMode.OneWay);
        public static readonly BindableProperty ChapterNameProperty = BindableProperty.Create(nameof(ChapterName), typeof(string), typeof(SurveyMenuItem), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty AnswersNeededProperty = BindableProperty.Create(nameof(AnswersNeeded), typeof(int), typeof(SurveyMenuItem), 0, BindingMode.OneWay);
        public static readonly BindableProperty MaximumQuestionNumberProperty = BindableProperty.Create(nameof(MaximumQuestionNumber), typeof(int), typeof(SurveyMenuItem), 0, BindingMode.OneWay);
        public static readonly BindableProperty AnswersGivenProperty = BindableProperty.Create(nameof(AnswersGiven), typeof(int), typeof(SurveyMenuItem), 0, BindingMode.OneWay);
        public static readonly BindableProperty UnlockedProperty = BindableProperty.Create(nameof(Unlocked), typeof(bool), typeof(SurveyMenuItem), false, BindingMode.OneWay);
        public static readonly BindableProperty ProgressStringProperty = BindableProperty.Create(nameof(ProgressString), typeof(string), typeof(SurveyMenuItem), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(SurveyMenuItem), Color.White, BindingMode.OneWay);

        public SurveyMenuItemType Id //machen alle Variablen noch Sinn, Namen ändern?, fällt was raus?
        {
            get => (SurveyMenuItemType)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        public string ChapterName
        {
            get => (string)GetValue(ChapterNameProperty);
            set => SetValue(ChapterNameProperty, value);
        }

        public int AnswersNeeded
        {
            get => (int)GetValue(AnswersNeededProperty);
            set => SetValue(AnswersNeededProperty, value);
        }

        public int MaximumQuestionNumber
        {
            get => (int)GetValue(MaximumQuestionNumberProperty);
            set => SetValue(MaximumQuestionNumberProperty, value);
        }

        public int AnswersGiven
        {
            get => (int)GetValue(AnswersGivenProperty);
            set => SetValue(AnswersGivenProperty, value);
        }

        public bool Unlocked
        {
            get => (bool)GetValue(UnlockedProperty);
            set => SetValue(UnlockedProperty, value);
        }

        public string ProgressString //=> $"{AnswersGiven}/{AnswersNeeded} ({MaximumQuestionNumber})"; hab das auf MainPage eingefügt ohne $
        {
            get => (string)GetValue(ProgressStringProperty);
            set => SetValue(ProgressStringProperty, value);
        }

        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public SurveyMenuItem(SurveyMenuItemType id, string chapterName, int answersNeeded, int maximumQuestionNumber, int answersGiven, bool unlocked, string progressString, Color backgroundColor)
        {
            Id = id;
            ChapterName = chapterName;
            AnswersNeeded = AnswersNeeded;
            MaximumQuestionNumber = maximumQuestionNumber;
            AnswersGiven = answersGiven;
            Unlocked = unlocked;
            ProgressString = progressString;
            BackgroundColor = backgroundColor;
        }
    }
}
