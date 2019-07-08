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
        List<IQuestionContent> Questions;
        INavigation Navigation;
        SurveyMenuItem CurrentSurvey;

        public SurveyManager(INavigation navigation)
        {
            Navigation = navigation;
        }

        public IQuestionContent GetNextQuestion(SurveyMenuItem surveyType)
        {
            //TODO: Switch difficulty
            var neededType = surveyType.QuestionType;
            var value = DatabankCommunication.LoadQuestion(surveyType.Id.ToString(), 1);
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
            if (CurrentSurvey.AnswersGiven >= CurrentSurvey.AnswersNeeded)
            {
                //Display Summary
                //var scoreSum = CurrentSurvey
            }
            ShowNewSurveyPage();
        }

        private void ShowNewSurveyPage()
        {
            if (CurrentSurvey == null)
                return;
            if (CurrentSurvey.AnswersGiven >= CurrentSurvey.AnswersNeeded)
            {
                ShowNextIntrospectionPage();
                return;
            }
            var question = GetNextQuestion(CurrentSurvey);
            //TODO: Check for existence of correct constructor
            var newPage = (ISurveyPage)Activator.CreateInstance(CurrentSurvey.SurveyPageType, new object[] { question, CurrentSurvey.AnswersGiven, CurrentSurvey.AnswersNeeded });
            newPage.PageFinished += NewPage_PageFinished;
            Navigation.PushAsync(newPage as ContentPage);
        }

        private void NewPage_PageFinished(object sender, EventArgs e)
        {
            var surveyPage = sender as ISurveyPage;
            surveyPage.PageFinished -= NewPage_PageFinished;
            bool aborted = surveyPage.AnswerItem == null;
            if (aborted)
            {
                CurrentSurvey = null;
                return;
            }
            CurrentSurvey.AnswersGiven++;
            DatabankCommunication.AddAnswer(CurrentSurvey.Id.ToString(), surveyPage.AnswerItem);
            Navigation.PopAsync();
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

        private void IntrospectionPage_PageFinished(object sender, EventArgs e)
        {
            var introspectionPage = sender as IntrospectionPage;
            introspectionPage.PageFinished -= IntrospectionPage_PageFinished;
            bool aborted = introspectionPage.AnswerItem == null;
            if (aborted)
            {
                CurrentSurvey = null;
                return;
            }
            DatabankCommunication.AddAnswer("Introspection", introspectionPage.AnswerItem);
            Navigation.PopAsync();
            ShowNextIntrospectionPage();
        }

        private void ShowEvaluationPage()
        {
            var evalItem = GenerateEvaluationItem(CurrentSurvey.Id.ToString());
            var evaluationPage = new EvaluationPage(evalItem);
            Navigation.PushAsync(evaluationPage);
            //TODO: Unlock and advance next question
        }

        private EvaluationItem GenerateEvaluationItem(string surveyId)
        {
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

            return new EvaluationItem(CurrentSurvey.ChapterName, average, easyAverage, mediumAverage, hardAverage);
        }
    }
}
