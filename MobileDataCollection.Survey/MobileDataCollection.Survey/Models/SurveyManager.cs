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

        public IQuestionContent GetFirstQuestion(SurveyMenuItem surveyType)
        {
            var neededType = surveyType.QuestionType;
            var value = Questions.FirstOrDefault(q => neededType == q.GetType());
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
            var question = GetFirstQuestion(CurrentSurvey);
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
                return;
            CurrentSurvey.AnswersGiven++;
            if (CurrentSurvey.AnswersGiven >= CurrentSurvey.AnswersNeeded)
                SurveyTypeFinished();
            var score = surveyPage.AnswerItem.EvaluateScore();
            Navigation.PopAsync();
            CurrentSurvey = null;
        }

        private void SurveyTypeFinished()
        {
            throw new NotImplementedException();
        }
    }
}
