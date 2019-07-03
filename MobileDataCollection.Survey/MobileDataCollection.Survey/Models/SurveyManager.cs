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
        SurveyMenuItemType? CurrentSurvey;

        public SurveyManager(INavigation navigation)
        {
            Navigation = navigation;
            var demoDoubleSlider = new QuestionDoubleSliderPage(1,1,"Q3G1B1_klein.png", 7, 4);
            var dbCon = new DatabankCommunication();
            Questions = dbCon.GetAllQuestions();
        }

        Dictionary<SurveyMenuItemType, Func<IQuestionContent, int, int, ISurveyPage>> SurveyPageConstructor = new Dictionary<SurveyMenuItemType, Func<IQuestionContent, int, int, ISurveyPage>>
        {
            { SurveyMenuItemType.DoubleSlider, (question, answered, needed) => new DoubleSliderPage((QuestionDoubleSliderPage)question, answered, needed)},
            { SurveyMenuItemType.ImageChecker, (question, answered, needed) => new ImageCheckerPage((QuestionImageCheckerPage)question, answered, needed)},
            { SurveyMenuItemType.Stadium, (question, answered, needed) => new StadiumPage((QuestionStadiumPage)question, answered, needed)}
        };

        Dictionary<SurveyMenuItemType, Type> SurveyMenuItemTypeToQuestionType = new Dictionary<SurveyMenuItemType, Type>
        {
            { SurveyMenuItemType.DoubleSlider, typeof(QuestionDoubleSliderPage) },
            { SurveyMenuItemType.ImageChecker, typeof(QuestionImageCheckerPage) },
            { SurveyMenuItemType.Stadium, typeof(QuestionStadiumPage) }
        };

        public IQuestionContent GetFirstQuestion(SurveyMenuItemType type)
        {
            var neededType = SurveyMenuItemTypeToQuestionType[type];
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
                CurrentSurvey = selectedSurvey.Id;
            }

            var question = GetFirstQuestion(selectedSurvey.Id);
            var newPage = SurveyPageConstructor[selectedSurvey.Id](question, selectedSurvey.AnswersGiven, selectedSurvey.AnswersNeeded);
            newPage.PageFinished += NewPage_PageFinished;
            Navigation.PushAsync(newPage as ContentPage);
        }

        private void NewPage_PageFinished(object sender, EventArgs e)
        {
            Navigation.PopAsync();
            CurrentSurvey = null;
        }
    }
}
