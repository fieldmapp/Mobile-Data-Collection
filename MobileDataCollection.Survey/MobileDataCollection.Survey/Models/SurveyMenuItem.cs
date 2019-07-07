using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace MobileDataCollection.Survey.Models
{
    public enum SurveyMenuItemType
    {
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
        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(SurveyMenuItem), Color.White, BindingMode.OneWay);
        public static readonly BindableProperty ProgressStringProperty = BindableProperty.Create(nameof(ProgressString), typeof(string), typeof(SurveyMenuItem), string.Empty, BindingMode.OneWay);

        public SurveyMenuItemType Id
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
            set
            {
                SetValue(AnswersNeededProperty, value);
                UpdateProgressString();
            }
        }

        public int MaximumQuestionNumber
        {
            get => (int)GetValue(MaximumQuestionNumberProperty);
            set
            {
                SetValue(MaximumQuestionNumberProperty, value);
                UpdateProgressString();
            }
        }

        public int AnswersGiven
        {
            get => (int)GetValue(AnswersGivenProperty);
            set
            {
                SetValue(AnswersGivenProperty, value);
                UpdateProgressString();
            }
        }

        public bool Unlocked
        {
            get => (bool)GetValue(UnlockedProperty);
            set => SetValue(UnlockedProperty, value);
        }

        public string ProgressString
        {
            get => (string)GetValue(ProgressStringProperty);
            set => SetValue(ProgressStringProperty, value);
        }
        private string UpdateProgressString() => ProgressString = $"{AnswersGiven}/{AnswersNeeded} ({MaximumQuestionNumber})";
        
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public Type QuestionType { get; }

        public Type AnswerType { get; }

        public Type SurveyPageType { get; }

        public SurveyMenuItem(SurveyMenuItemType id, string chapterName, int answersNeeded, int maximumQuestionNumber, int answersGiven, bool unlocked, Color backgroundColor)
        {
            Id = id;
            ChapterName = chapterName;
            AnswersNeeded = AnswersNeeded;
            MaximumQuestionNumber = maximumQuestionNumber;
            AnswersGiven = answersGiven;
            Unlocked = unlocked;
            BackgroundColor = backgroundColor;

            var nspace = typeof(App).Namespace;
            SurveyPageType = Type.GetType($"{nspace}.Views.{id.ToString()}Page");
            if (SurveyPageType == null || SurveyPageType.IsAssignableFrom(typeof(ISurveyPage)))
                throw new ArgumentException($"You need to provide an ISurveyPage matching the id given (for given id it needs to be called {id.ToString()}Page and must be located in {nspace}.Views");
            QuestionType = Type.GetType($"{nspace}.Models.Question{id.ToString()}Page");
            if (QuestionType == null || QuestionType.IsAssignableFrom(typeof(IQuestionContent)))
                throw new ArgumentException($"You need to provide a IQuestionContent matching the id given (for given id it needs to be called Question{id.ToString()}Page and must be located in {nspace}.Models"); ;
            AnswerType = Type.GetType($"{nspace}.Models.Question{id.ToString()}Page");
            if (AnswerType == null || AnswerType.IsAssignableFrom(typeof(IUserAnswer)))
                throw new ArgumentException($"You need to provide a SurveyPage matching the id given (for given id it needs to be called Answer{id.ToString()}Page and must be located in {nspace}.Models"); ;
        }
    }
}
