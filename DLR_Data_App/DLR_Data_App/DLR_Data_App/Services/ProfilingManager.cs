//Main contributors: Maximilian Enderling
using DLR_Data_App.Models.Profiling;
using DLR_Data_App.Views;
using DLR_Data_App.Views.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Services
{
    /// <summary>
    /// This class is responsible for keeping question data, both
    /// QuestionContent and UserAnswers. Chooses next Question (+type).
    /// Re-initialize after switching users
    /// </summary>
    static class ProfilingManager
    {
        static NavigationPage Navigation => (Application.Current as App).Navigation;
        static ProfilingMenuItem CurrentProfiling;

        public static void Initialize(string userId)
        {
            ProfilingStorageManager.Initilize(userId);
        }

        public static IQuestionContent GetNextQuestion(ProfilingMenuItem profilingType)
        {
            var neededType = profilingType.QuestionType;
            var value = ProfilingStorageManager.GetQuestion(profilingType.Id, CurrentProfiling.CurrentDifficulty, false);
            if (value == null)
                return null;
            return value;
        }

        public static void StartProfiling(ProfilingMenuItem selectedProfiling)
        {
            lock(selectedProfiling)
            {
                if (CurrentProfiling != null)
                    return;
                CurrentProfiling = selectedProfiling;
            }
            _ = Navigation.PushPage(new LoadingPage(), false);
            if (CurrentProfiling.IntrospectionQuestion.All(q => ProfilingStorageManager.DoesAnswersExists("Introspection", q)))
            {
                ShowEvaluationPage();
                return;
            }
            ShowNewProfilingPage();
        }

        private static void ShowNewProfilingPage()
        {
            if (CurrentProfiling == null)
                return;
            var question = GetNextQuestion(CurrentProfiling);
            if (question == null || CurrentProfiling.IntrospectionQuestion.Any(q => ProfilingStorageManager.DoesAnswersExists("Introspection", q)))
            {
                ShowNextIntrospectionPage();
                return;
            }
            var newPage = (IProfilingPage)Activator.CreateInstance(CurrentProfiling.ProfilingPageType, new object[] { question, CurrentProfiling.AnswersGiven, CurrentProfiling.AnswersNeeded });
            newPage.PageFinished += NewPage_PageFinished;
            _ = Navigation.PushPage(newPage as ContentPage);
        }

        private static void NewPage_PageFinished(object sender, PageResult e)
        {
            if (e == PageResult.Evaluation && CurrentProfiling.AnswersGiven < CurrentProfiling.AnswersNeeded)
                return;
            var profilingPage = sender as IProfilingPage;
            profilingPage.PageFinished -= NewPage_PageFinished;
            Navigation.PopAsync();
            if (e == PageResult.Abort)
            {
                CurrentProfiling = null;
                Navigation.PopAsync();
                return;
            }
            if (e == PageResult.Evaluation)
            {
                ShowNextIntrospectionPage();
                return;
            }

            ProfilingStorageManager.AddAnswer(CurrentProfiling.Id, profilingPage.AnswerItem);
            ProfilingStorageManager.SaveAnswers();
            CurrentProfiling.AnswersGiven++;
            
            CurrentProfiling.ApplyAnswer(profilingPage.AnswerItem);
            ShowNewProfilingPage();
        }

        private static void ShowNextIntrospectionPage()
        {
            if (CurrentProfiling == null)
                return;
            var newIntrospectionQuestion = (QuestionIntrospectionPage)CurrentProfiling.IntrospectionQuestion.Where(id => !ProfilingStorageManager.DoesAnswersExists("Introspection", id))
                .Select(id => ProfilingStorageManager.LoadQuestionById("Introspection", id)).FirstOrDefault();
            if (newIntrospectionQuestion == null)
            {
                if (!ProfilingStorageManager.ProfilingMenuItems.Any(s => s.AnswersNeeded > s.AnswersGiven))
                {
                    ProfilingStorageManager.ProjectsFilledSinceLastProfilingCompletion = 0;
                    ProfilingStorageManager.SaveAnswers();
                }
                ShowEvaluationPage();
                return;
            }
            var introspectionPage = new IntrospectionPage(newIntrospectionQuestion);
            introspectionPage.PageFinished += IntrospectionPage_PageFinished;
            _ = Navigation.PushPage(introspectionPage);
        }

        private static void IntrospectionPage_PageFinished(object sender, PageResult e)
        {
            var introspectionPage = sender as IntrospectionPage;
            introspectionPage.PageFinished -= IntrospectionPage_PageFinished;
            Navigation.PopAsync();
            if (e == PageResult.Abort)
            {
                CurrentProfiling = null;
                Navigation.PopAsync();
                return;
            }
            ProfilingStorageManager.AddAnswer("Introspection", introspectionPage.AnswerItem);
            ProfilingStorageManager.SaveAnswers();
            ShowNextIntrospectionPage();
        }

        private static void ShowEvaluationPage()
        {
            var evalItem = GenerateEvaluationItem(CurrentProfiling);
            var evaluationPage = new EvaluationPage(evalItem);
            evaluationPage.PageFinished += EvaluationPage_PageFinished;
            _ = Navigation.PushPage(evaluationPage);
            CurrentProfiling = null;
            ProfilingStorageManager.SaveAnswers();
            ProfilingStorageManager.CreateCSV();
        }

        private static void EvaluationPage_PageFinished(object sender, EventArgs e)
        {
            (sender as EvaluationPage).PageFinished -= EvaluationPage_PageFinished;
            Navigation.PopAsync();
            Navigation.PopAsync();
        }

        public static EvaluationItem GenerateEvaluationItem(ProfilingMenuItem profiling)
        {
            var profilingId = profiling.Id;
            var answeredQuestions = ProfilingStorageManager.GetAllQuestions(profilingId)
                .Where(q => ProfilingStorageManager.DoesAnswersExists(profilingId, q.InternId));
            var answers = answeredQuestions.Select(q => ProfilingStorageManager.LoadAnswerById(profilingId, q.InternId));
            var easyAnsers = answeredQuestions.Where(q => q.Difficulty == 1)
                .Select(q => ProfilingStorageManager.LoadAnswerById(profilingId, q.InternId));
            var mediumAnswers = answeredQuestions.Where(q => q.Difficulty == 2)
                .Select(q => ProfilingStorageManager.LoadAnswerById(profilingId, q.InternId));
            var hardAnswers = answeredQuestions.Where(q => q.Difficulty == 3)
                .Select(q => ProfilingStorageManager.LoadAnswerById(profilingId, q.InternId));

            var average = answers.Any() ? (int)answers.Average(a => a.EvaluateScore() * 100) : -1;
            var easyAverage = easyAnsers.Any() ? (int)easyAnsers.Average(a => a.EvaluateScore() * 100) : -1;
            var mediumAverage = mediumAnswers.Any() ? (int)mediumAnswers.Average(a => a.EvaluateScore() * 100) : -1;
            var hardAverage = hardAnswers.Any() ? (int)hardAnswers.Average(a => a.EvaluateScore() * 100) : -1;

            return new EvaluationItem(profiling.ChapterName, average, easyAverage, mediumAverage, hardAverage);
        }
    }
}
