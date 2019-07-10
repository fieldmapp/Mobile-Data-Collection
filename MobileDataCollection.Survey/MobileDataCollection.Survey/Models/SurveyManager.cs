//Main contributors: Maximilian Enderling
using MobileDataCollection.Survey.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    /// <summary>
    /// This class is responsible for keeping question data, both
    /// QuestionContent and UserAnswers. Chooses next Question (+type).
    /// Dont keep when switching users.
    /// </summary>
    class SurveyManager
    {
        INavigation Navigation;
        SurveyMenuItem CurrentSurvey;
        MainPage MainPage;

        public SurveyManager(INavigation navigation, MainPage mainPage)
        {
            Navigation = navigation;
            MainPage = mainPage;
        }

        public IQuestionContent GetNextQuestion(SurveyMenuItem surveyType)
        {
            var neededType = surveyType.QuestionType;
            var value = DatabankCommunication.LoadQuestion(surveyType.Id, CurrentSurvey.CurrentDifficulty, false);
            if (value == null)
                return null;
            return value;
        }

        public void StartSurvey(SurveyMenuItem selectedSurvey)
        {
            lock(selectedSurvey)
            {
                if (!selectedSurvey.Unlocked || CurrentSurvey != null)
                    return;
                CurrentSurvey = selectedSurvey;
            }
            Navigation.PushAsync(new LoadingPage(), false);
            if (CurrentSurvey.IntrospectionQuestion.All(q => DatabankCommunication.SearchAnswers("Introspection", q)))
            {
                ShowEvaluationPage();
                return;
            }
            ShowNewSurveyPage();
        }

        private void ShowNewSurveyPage()
        {
            if (CurrentSurvey == null)
                return;
            var question = GetNextQuestion(CurrentSurvey);
            if (question == null || CurrentSurvey.IntrospectionQuestion.Any(q => DatabankCommunication.SearchAnswers("Introspection", q)))
            {
                ShowNextIntrospectionPage();
                return;
            }
            //TODO: Check for existence of correct constructor
            var newPage = (ISurveyPage)Activator.CreateInstance(CurrentSurvey.SurveyPageType, new object[] { question, CurrentSurvey.AnswersGiven, CurrentSurvey.AnswersNeeded });
            newPage.PageFinished += NewPage_PageFinished;
            Navigation.PushAsync(newPage as ContentPage);
        }

        private void NewPage_PageFinished(object sender, PageResult e)
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

            DatabankCommunication.AddAnswer(CurrentSurvey.Id, surveyPage.AnswerItem);
            CurrentSurvey.AnswersGiven++;

            bool answerRight = surveyPage.AnswerItem.EvaluateScore() > .85f;
            if (answerRight)
                CurrentSurvey.Streak = CurrentSurvey.Streak <= 0 ? 1 : CurrentSurvey.Streak + 1;
            else
                CurrentSurvey.Streak = CurrentSurvey.Streak >= 0 ? -1 : CurrentSurvey.Streak - 1;
            if (CurrentSurvey.Streak < -2)
            {
                CurrentSurvey.CurrentDifficulty = Math.Max(1, CurrentSurvey.CurrentDifficulty - 1);
                CurrentSurvey.Streak = 0;
            }
            else if (CurrentSurvey.Streak > 2)
            {
                CurrentSurvey.CurrentDifficulty = Math.Min(3, CurrentSurvey.CurrentDifficulty + 1);
                CurrentSurvey.Streak = 0;
            }
            ShowNewSurveyPage();
        }

        private void ShowNextIntrospectionPage()
        {
            if (CurrentSurvey == null)
                return;
            var newIntrospectionQuestion = (QuestionIntrospectionPage)CurrentSurvey.IntrospectionQuestion.Where(id => !DatabankCommunication.SearchAnswers("Introspection", id))
                .Select(id => DatabankCommunication.LoadQuestionById("Introspection", id)).FirstOrDefault();
            if (newIntrospectionQuestion == null)
            {
                ShowEvaluationPage();
                return;
            }
            var introspectionPage = new IntrospectionPage(newIntrospectionQuestion);
            introspectionPage.PageFinished += IntrospectionPage_PageFinished;
            Navigation.PushAsync(introspectionPage);
        }

        private void IntrospectionPage_PageFinished(object sender, PageResult e)
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
            DatabankCommunication.AddAnswer("Introspection", introspectionPage.AnswerItem);
            ShowNextIntrospectionPage();
        }

        private void ShowEvaluationPage()
        {
            var evalItem = GenerateEvaluationItem(CurrentSurvey);
            var evaluationPage = new EvaluationPage(evalItem);
            evaluationPage.PageFinished += EvaluationPage_PageFinished;
            Navigation.PushAsync(evaluationPage);
            CurrentSurvey = null;
            DatabankCommunication.SaveSurveyMenuItemsTxt(MainPage.GetAllSurveyMenuItem());
            DatabankCommunication.SaveAllAnswersInTxt();
            DatabankCommunication.CreateCSV();
        }

        private void EvaluationPage_PageFinished(object sender, EventArgs e)
        {
            (sender as EvaluationPage).PageFinished -= EvaluationPage_PageFinished;
            Navigation.PopAsync();
            Navigation.PopAsync();
        }

        public EvaluationItem GenerateEvaluationItem(SurveyMenuItem survey)
        {
            var surveyId = survey.Id;
            var answeredQuestions = DatabankCommunication.GetAllQuestions(surveyId)
                .Where(q => DatabankCommunication.SearchAnswers(surveyId, q.InternId));
            var answers = answeredQuestions.Select(q => DatabankCommunication.LoadAnswerById(surveyId, q.InternId));
            var easyAnsers = answeredQuestions.Where(q => q.Difficulty == 1)
                .Select(q => DatabankCommunication.LoadAnswerById(surveyId, q.InternId));
            var mediumAnswers = answeredQuestions.Where(q => q.Difficulty == 2)
                .Select(q => DatabankCommunication.LoadAnswerById(surveyId, q.InternId));
            var hardAnswers = answeredQuestions.Where(q => q.Difficulty == 3)
                .Select(q => DatabankCommunication.LoadAnswerById(surveyId, q.InternId));

            var average = answers.Any() ? (int)answers.Average(a => a.EvaluateScore() * 100) : -1;
            var easyAverage = easyAnsers.Any() ? (int)easyAnsers.Average(a => a.EvaluateScore() * 100) : -1;
            var mediumAverage = mediumAnswers.Any() ? (int)mediumAnswers.Average(a => a.EvaluateScore() * 100) : -1;
            var hardAverage = hardAnswers.Any() ? (int)hardAnswers.Average(a => a.EvaluateScore() * 100) : -1;

            return new EvaluationItem(survey.ChapterName, average, easyAverage, mediumAverage, hardAverage);
        }
    }
}
