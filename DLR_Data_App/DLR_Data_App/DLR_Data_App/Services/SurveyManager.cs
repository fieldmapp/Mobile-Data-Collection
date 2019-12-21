//Main contributors: Maximilian Enderling
using DLR_Data_App.Models.Survey;
using DLR_Data_App.Views;
using DLR_Data_App.Views.Survey;
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
    static class SurveyManager
    {
        static NavigationPage Navigation => (Application.Current as App).Navigation;
        static SurveyMenuItem CurrentSurvey;

        public static void Initialize(string userId)
        {
            SurveyStorageManager.Initilize(userId);
        }

        public static IQuestionContent GetNextQuestion(SurveyMenuItem surveyType)
        {
            var neededType = surveyType.QuestionType;
            var value = SurveyStorageManager.GetQuestion(surveyType.Id, CurrentSurvey.CurrentDifficulty, false);
            if (value == null)
                return null;
            return value;
        }

        public static void StartSurvey(SurveyMenuItem selectedSurvey)
        {
            lock(selectedSurvey)
            {
                if (CurrentSurvey != null)
                    return;
                CurrentSurvey = selectedSurvey;
            }
            _ = Navigation.PushPage(new LoadingPage(), false);
            if (CurrentSurvey.IntrospectionQuestion.All(q => SurveyStorageManager.DoesAnswersExists("Introspection", q)))
            {
                ShowEvaluationPage();
                return;
            }
            ShowNewSurveyPage();
        }

        private static void ShowNewSurveyPage()
        {
            if (CurrentSurvey == null)
                return;
            var question = GetNextQuestion(CurrentSurvey);
            if (question == null || CurrentSurvey.IntrospectionQuestion.Any(q => SurveyStorageManager.DoesAnswersExists("Introspection", q)))
            {
                ShowNextIntrospectionPage();
                return;
            }
            var newPage = (ISurveyPage)Activator.CreateInstance(CurrentSurvey.SurveyPageType, new object[] { question, CurrentSurvey.AnswersGiven, CurrentSurvey.AnswersNeeded });
            newPage.PageFinished += NewPage_PageFinished;
            _ = Navigation.PushPage(newPage as ContentPage);
        }

        private static void NewPage_PageFinished(object sender, PageResult e)
        {
            if (e == PageResult.Evaluation && CurrentSurvey.AnswersGiven < CurrentSurvey.AnswersNeeded)
                return;
            var surveyPage = sender as ISurveyPage;
            surveyPage.PageFinished -= NewPage_PageFinished;
            Navigation.PopAsync();
            if (e == PageResult.Abort)
            {
                CurrentSurvey = null;
                Navigation.PopAsync();
                return;
            }
            if (e == PageResult.Evaluation)
            {
                ShowNextIntrospectionPage();
                return;
            }

            SurveyStorageManager.AddAnswer(CurrentSurvey.Id, surveyPage.AnswerItem);
            SurveyStorageManager.SaveAnswers();
            CurrentSurvey.AnswersGiven++;
            
            CurrentSurvey.ApplyAnswer(surveyPage.AnswerItem);
            ShowNewSurveyPage();
        }

        private static void ShowNextIntrospectionPage()
        {
            if (CurrentSurvey == null)
                return;
            var newIntrospectionQuestion = (QuestionIntrospectionPage)CurrentSurvey.IntrospectionQuestion.Where(id => !SurveyStorageManager.DoesAnswersExists("Introspection", id))
                .Select(id => SurveyStorageManager.LoadQuestionById("Introspection", id)).FirstOrDefault();
            if (newIntrospectionQuestion == null)
            {
                if (!SurveyStorageManager.SurveyMenuItems.Any(s => s.AnswersNeeded > s.AnswersGiven))
                {
                    SurveyStorageManager.ProjectsFilledSinceLastSurveyCompletion = 0;
                    SurveyStorageManager.SaveAnswers();
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
                CurrentSurvey = null;
                Navigation.PopAsync();
                return;
            }
            SurveyStorageManager.AddAnswer("Introspection", introspectionPage.AnswerItem);
            SurveyStorageManager.SaveAnswers();
            ShowNextIntrospectionPage();
        }

        private static void ShowEvaluationPage()
        {
            var evalItem = GenerateEvaluationItem(CurrentSurvey);
            var evaluationPage = new EvaluationPage(evalItem);
            evaluationPage.PageFinished += EvaluationPage_PageFinished;
            _ = Navigation.PushPage(evaluationPage);
            CurrentSurvey = null;
            SurveyStorageManager.SaveAnswers();
            SurveyStorageManager.CreateCSV();
        }

        private static void EvaluationPage_PageFinished(object sender, EventArgs e)
        {
            (sender as EvaluationPage).PageFinished -= EvaluationPage_PageFinished;
            Navigation.PopAsync();
            Navigation.PopAsync();
        }

        public static EvaluationItem GenerateEvaluationItem(SurveyMenuItem survey)
        {
            var surveyId = survey.Id;
            var answeredQuestions = SurveyStorageManager.GetAllQuestions(surveyId)
                .Where(q => SurveyStorageManager.DoesAnswersExists(surveyId, q.InternId));
            var answers = answeredQuestions.Select(q => SurveyStorageManager.LoadAnswerById(surveyId, q.InternId));
            var easyAnsers = answeredQuestions.Where(q => q.Difficulty == 1)
                .Select(q => SurveyStorageManager.LoadAnswerById(surveyId, q.InternId));
            var mediumAnswers = answeredQuestions.Where(q => q.Difficulty == 2)
                .Select(q => SurveyStorageManager.LoadAnswerById(surveyId, q.InternId));
            var hardAnswers = answeredQuestions.Where(q => q.Difficulty == 3)
                .Select(q => SurveyStorageManager.LoadAnswerById(surveyId, q.InternId));

            var average = answers.Any() ? (int)answers.Average(a => a.EvaluateScore() * 100) : -1;
            var easyAverage = easyAnsers.Any() ? (int)easyAnsers.Average(a => a.EvaluateScore() * 100) : -1;
            var mediumAverage = mediumAnswers.Any() ? (int)mediumAnswers.Average(a => a.EvaluateScore() * 100) : -1;
            var hardAverage = hardAnswers.Any() ? (int)hardAnswers.Average(a => a.EvaluateScore() * 100) : -1;

            return new EvaluationItem(survey.ChapterName, average, easyAverage, mediumAverage, hardAverage);
        }
    }
}
