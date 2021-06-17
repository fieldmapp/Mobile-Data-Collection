//Main contributors: Maximilian Enderling, Max Moebius
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using Newtonsoft.Json;
using DlrDataApp.Modules.Profiling.Shared.Views;

namespace DlrDataApp.Modules.Profiling.Shared.Models
{
    public class ProfilingMenuItem : BindableObject
    {
        public static readonly BindableProperty IdProperty = BindableProperty.Create(nameof(Id), typeof(string), typeof(ProfilingMenuItem), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty ChapterNameProperty = BindableProperty.Create(nameof(ChapterName), typeof(string), typeof(ProfilingMenuItem), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty AnswersNeededProperty = BindableProperty.Create(nameof(AnswersNeeded), typeof(int), typeof(ProfilingMenuItem), 0, BindingMode.OneWay);
        public static readonly BindableProperty MaximumQuestionNumberProperty = BindableProperty.Create(nameof(MaximumQuestionNumber), typeof(int), typeof(ProfilingMenuItem), 0, BindingMode.OneWay);
        public static readonly BindableProperty AnswersGivenProperty = BindableProperty.Create(nameof(AnswersGiven), typeof(int), typeof(ProfilingMenuItem), 0, BindingMode.OneWay);
        public static readonly BindableProperty ProgressStringProperty = BindableProperty.Create(nameof(ProgressString), typeof(string), typeof(ProfilingMenuItem), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty IntrospectionQuestionProperty = BindableProperty.Create(nameof(IntrospectionQuestion), typeof(List<int>), typeof(ProfilingMenuItem), new List<int>());
        
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

        [JsonIgnore]
        public int MaximumQuestionNumber
        {
            get => (int)GetValue(MaximumQuestionNumberProperty);
            private set
            {
                SetValue(MaximumQuestionNumberProperty, value);
                UpdateProgressString();
            }
        }

        [JsonIgnore]
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

        const int StreakToChangeDifficulty = 1;

        [JsonIgnore]
        public int Streak { get; private set; }

        [JsonIgnore]
        public int CurrentDifficulty { get; set; }

        [JsonIgnore]
        public Type QuestionType { get; }

        [JsonIgnore]
        public Type AnswerType { get; }

        [JsonIgnore]
        public Type ProfilingPageType { get; }
        
        public void ApplyAnswer(IUserAnswer answerItem)
        {
            AnswersGiven++;

            bool answerRight = answerItem.EvaluateScore() > .85f;
            if (answerRight)
                Streak = Streak <= 0 ? 1 : Streak + 1;
            else
                Streak = Streak >= 0 ? -1 : Streak - 1;
            if (Streak <= -StreakToChangeDifficulty)
            {
                CurrentDifficulty = Math.Max(1, CurrentDifficulty - 1);
                Streak = 0;
            }
            else if (Streak >= StreakToChangeDifficulty)
            {
                CurrentDifficulty = Math.Min(3, CurrentDifficulty + 1);
                Streak = 0;
            }
        }

        public void SetQuestions(List<IQuestionContent> questions)
        {
            MaximumQuestionNumber = questions.Count;
        }

        public ProfilingMenuItem(string id, string chapterName, int answersNeeded, List<int> introspectionQuestions)
        {
            Id = id;
            ChapterName = chapterName;
            AnswersNeeded = answersNeeded;
            IntrospectionQuestion = introspectionQuestions;
            CurrentDifficulty = 1;
            PropertyChanged += ProfilingMenuItem_PropertyChanged;

            var nspace = typeof(ProfilingMenuItem).Namespace;
            ProfilingPageType = Type.GetType($"{nspace}.Views.Profiling.{id}Page");
            if (ProfilingPageType == null || ProfilingPageType.IsAssignableFrom(typeof(IProfilingPage)))
                throw new ArgumentException($"You need to provide an IProfilingPage matching the id given (for given id it needs to be called {id}Page and must be located in {nspace}.Views.Profiling");
            QuestionType = Type.GetType($"{nspace}.Models.Profiling.Question{id.ToString()}Page");
            if (QuestionType == null || QuestionType.IsAssignableFrom(typeof(IQuestionContent)))
                throw new ArgumentException($"You need to provide a IQuestionContent matching the id given (for given id it needs to be called Question{id}Page and must be located in {nspace}.Models.Profiling");
            AnswerType = Type.GetType($"{nspace}.Models.Profiling.Question{id.ToString()}Page");
            if (AnswerType == null || AnswerType.IsAssignableFrom(typeof(IUserAnswer)))
                throw new ArgumentException($"You need to provide a ProfilingPage matching the id given (for given id it needs to be called Answer{id}Page and must be located in {nspace}.Models.Profiling");
            if (!ProfilingPageType.GetConstructors().Any(ci =>
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
                throw new ArgumentException($"The class {nspace}.Views.Profiling.{id.ToString()}Page needs to have a constructor with parameters: ({nspace}.Models.Profiling.Question{id.ToString()}Page,int,int)");
        }

        private void ProfilingMenuItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AnswersGiven) || e.PropertyName == nameof(AnswersNeeded))
                UpdateProgressString();
        }
    }
}
