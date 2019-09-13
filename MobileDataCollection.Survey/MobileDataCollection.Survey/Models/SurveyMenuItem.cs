//Main contributors: Maximilian Enderling, Max Moebius
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using Newtonsoft.Json;

namespace MobileDataCollection.Survey.Models
{
    public class SurveyMenuItem : BindableObject
    {
        public static readonly BindableProperty IdProperty = BindableProperty.Create(nameof(Id), typeof(string), typeof(SurveyMenuItem), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty ChapterNameProperty = BindableProperty.Create(nameof(ChapterName), typeof(string), typeof(SurveyMenuItem), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty AnswersNeededProperty = BindableProperty.Create(nameof(AnswersNeeded), typeof(int), typeof(SurveyMenuItem), 0, BindingMode.OneWay);
        public static readonly BindableProperty MaximumQuestionNumberProperty = BindableProperty.Create(nameof(MaximumQuestionNumber), typeof(int), typeof(SurveyMenuItem), 0, BindingMode.OneWay);
        public static readonly BindableProperty AnswersGivenProperty = BindableProperty.Create(nameof(AnswersGiven), typeof(int), typeof(SurveyMenuItem), 0, BindingMode.OneWay);
        public static readonly BindableProperty ProgressStringProperty = BindableProperty.Create(nameof(ProgressString), typeof(string), typeof(SurveyMenuItem), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty IntrospectionQuestionProperty = BindableProperty.Create(nameof(IntrospectionQuestion), typeof(List<int>), typeof(SurveyMenuItem), new List<int>());
        
        public string Id
        {
            get => (string)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        public List<int> IntrospectionQuestion
        {
            get => (List<int>)GetValue(IntrospectionQuestionProperty);
            set => SetValue(IntrospectionQuestionProperty, value);
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
            private set
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

        [JsonIgnore]
        public string ProgressString
        {
            get => (string)GetValue(ProgressStringProperty);
            set => SetValue(ProgressStringProperty, value);
        }
        private string UpdateProgressString() => ProgressString = $"{AnswersGiven}/{AnswersNeeded}";

        [JsonIgnore]
        public int Streak { get; private set; }

        [JsonIgnore]
        public int CurrentDifficulty { get; set; }

        [JsonIgnore]
        public Type QuestionType { get; }

        [JsonIgnore]
        public Type AnswerType { get; }

        [JsonIgnore]
        public Type SurveyPageType { get; }
        
        public void ApplyAnswer(IUserAnswer answerItem)
        {
            bool answerRight = answerItem.EvaluateScore() > .85f;
            if (answerRight)
                Streak = Streak <= 0 ? 1 : Streak + 1;
            else
                Streak = Streak >= 0 ? -1 : Streak - 1;
            if (Streak < -2)
            {
                CurrentDifficulty = Math.Max(1, CurrentDifficulty - 1);
                Streak = 0;
            }
            else if (Streak > 2)
            {
                CurrentDifficulty = Math.Min(3, CurrentDifficulty + 1);
                Streak = 0;
            }
        }

        public SurveyMenuItem(string id, string chapterName, int answersNeeded, List<int> introspectionQuestions)
        {
            Id = id;
            ChapterName = chapterName;
            AnswersNeeded = answersNeeded;
            IntrospectionQuestion = introspectionQuestions;
            CurrentDifficulty = 1;
            MaximumQuestionNumber = DatabankCommunication.GetAllQuestions(id).Count;
            var answers = DatabankCommunication.GetAllAnswers(id);
            AnswersGiven = answers.Count;
            foreach (var answer in answers)
                ApplyAnswer(answer);

            var nspace = typeof(App).Namespace;
            SurveyPageType = Type.GetType($"{nspace}.Views.{id.ToString()}Page");
            if (SurveyPageType == null || SurveyPageType.IsAssignableFrom(typeof(ISurveyPage)))
                throw new ArgumentException($"You need to provide an ISurveyPage matching the id given (for given id it needs to be called {id.ToString()}Page and must be located in {nspace}.Views");
            QuestionType = Type.GetType($"{nspace}.Models.Question{id.ToString()}Page");
            if (QuestionType == null || QuestionType.IsAssignableFrom(typeof(IQuestionContent)))
                throw new ArgumentException($"You need to provide a IQuestionContent matching the id given (for given id it needs to be called Question{id.ToString()}Page and must be located in {nspace}.Models");
            AnswerType = Type.GetType($"{nspace}.Models.Question{id.ToString()}Page");
            if (AnswerType == null || AnswerType.IsAssignableFrom(typeof(IUserAnswer)))
                throw new ArgumentException($"You need to provide a SurveyPage matching the id given (for given id it needs to be called Answer{id.ToString()}Page and must be located in {nspace}.Models");
            if (!SurveyPageType.GetConstructors().Any(ci =>
            {
                var parms = ci.GetParameters();
                if (parms[0].ParameterType != QuestionType)
                    return false;
                if (parms[1].ParameterType != typeof(int))
                    return false;
                if (parms[2].ParameterType != typeof(int))
                    return false;
                return true;
            }))
                throw new ArgumentException($"The class {nspace}.Views.{id.ToString()}Page needs to have a constructor with parameters: ({nspace}.Models.Question{id.ToString()}Page,int,int)");
        }
    }
}
