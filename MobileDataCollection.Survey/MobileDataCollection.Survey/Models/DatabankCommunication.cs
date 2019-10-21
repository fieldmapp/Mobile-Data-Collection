//Main contributors: Henning Woydt
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;

namespace MobileDataCollection.Survey.Models
{
    public static class DatabankCommunication
    {
        /// <summary>
        /// Used to create a randum number
        /// </summary>
        private static Random RandomNumber = new Random();

        /// <summary>
        /// Dictionary containng all lists to save the questions
        /// </summary>
        private static Dictionary<string, List<IQuestionContent>> Questions = new Dictionary<string, List<IQuestionContent>>();

        /// <summary>
        /// Dictionary containing all lists to save the answers. The order of each List is crucial to determine streak and should not be changed ever
        /// </summary>
        private static Dictionary<string, List<IUserAnswer>> Answers = new Dictionary<string, List<IUserAnswer>>();

        /// <summary>
        /// List containing all SurveyMenuItems, which contain all relevant information over the different survey types
        /// </summary>
        public static List<SurveyMenuItem> SurveyMenuItems { get; private set; } = new List<SurveyMenuItem>();

        /// <summary>
        /// Used to load questions
        /// </summary>
        private static IStorageProvider StorageProvider;

        /// <summary>
        /// Initializes the DatabankCommunication
        /// </summary>
        public static void Initilize(IStorageProvider provider)
        {
            StorageProvider = provider;
            Questions = StorageProvider.LoadQuestions();
            Answers = StorageProvider.LoadAnswers();
            SurveyMenuItems = StorageProvider.LoadSurveyMenuItems();
        }

        /// <summary>
        /// Saves all Answers
        /// This will override old saved data!
        /// </summary>
        public static void SaveAnswers()
        {
            StorageProvider.SaveAnswers(Answers);
        }

        /// <summary>
        /// Resets all Answer Txt
        /// </summary>
        public static void ResetSavedAnswers()
        {
            Answers = new Dictionary<string, List<IUserAnswer>>();
            SurveyMenuItems = StorageProvider.LoadSurveyMenuItems();
            SaveAnswers();
        }

        /// <summary>
        /// Creates Answers.csv with all Answers in one line 
        /// </summary>
        public static void CreateCSV()
        {
            string explanation = "UserCode,Date"; /// contains the explanation for the data
            string data = "," + DateTime.Now.ToString("yyyy MM dd"); /// contains the data

            /// write all Introspection Answers in one line
            foreach (QuestionIntrospectionPage question in Questions["Introspection"].OrderBy(o => o.InternId))
            {
                explanation += ",SelectedAnswerQuestion" + question.InternId;
                if (DoesAnswersExists("Introspection", question.InternId))
                {
                    AnswerIntrospectionPage answer = (AnswerIntrospectionPage)LoadAnswerById("Introspection", question.InternId);
                    data += "," + answer.SelectedAnswer;
                }
                else
                {
                    /// no data is available for the question, therefor nothing is written in the field
                    data += ",";
                }
            }
            /// write all ImageChecker Answers in one line
            foreach (QuestionImageCheckerPage question in Questions["ImageChecker"].OrderBy(o => o.InternId))
            {
                explanation += ",DifficultyQuestion" + question.InternId + ",Img1SelQuestion" + question.InternId + ",Img2SelQuestion" + question.InternId + ",Img3SelQuestion" + question.InternId + ",Img4SelQuestion" + question.InternId;
                if (DoesAnswersExists("ImageChecker", question.InternId))
                {
                    AnswerImageCheckerPage answer = (AnswerImageCheckerPage) LoadAnswerById("ImageChecker", question.InternId);
                    data += "," + question.Difficulty + "," + answer.Image1Selected + "," + answer.Image2Selected + "," + answer.Image3Selected + "," + answer.Image4Selected;
                }
                else
                {
                    /// no data is available for the question, therefor nothing is written in the fields
                    data += "," + question.Difficulty + ",,,,";
                }
            }
            /// write all Stadium Answers in one line
            foreach (QuestionStadiumPage question in Questions["Stadium"].OrderBy(o => o.InternId))
            {
                explanation += ",DifficultyQuestion" + question.InternId + ",StadiumQuestion" + question.InternId + ",FruitTypeQuestion" + question.InternId;
                if (DoesAnswersExists("Stadium", question.InternId))
                {
                    AnswerStadiumPage answer = (AnswerStadiumPage)LoadAnswerById("Stadium", question.InternId);
                    data += "," + question.Difficulty + "," + answer.AnswerStadium + "," + answer.AnswerFruitType;
                }
                else
                {
                    /// no data is available for the question, therefor nothig is written in the fields
                    data += "," + question.Difficulty + ",,";
                }
            }
            /// write all DoubleSlider Answers in one line
            foreach (QuestionDoubleSliderPage question in Questions["DoubleSlider"].OrderBy(o => o.InternId))
            {
                explanation += ",DifficultyQuestion" + question.InternId + ",ResAQuestion" + question.InternId + ",ResBQuestion" + question.InternId;
                if (DoesAnswersExists("DoubleSlider", question.InternId))
                {
                    AnswerDoubleSliderPage answer = (AnswerDoubleSliderPage)LoadAnswerById("DoubleSlider", question.InternId);
                    data += "," + question.Difficulty + "," + answer.ResultQuestionA + "," + answer.ResultQuestionB;
                }
                else
                {
                    /// no data is avilable for the question, therefor nothing is written in the fields
                    data += "," + question.Difficulty + ",,";
                }
            }

            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "Answers.csv");

            ///write text in file
            File.WriteAllText(filename, explanation + "\n" + data);
        }

        /// <summary>
        /// Loads a QUestionImageCheckerPage-Object with the set difficulty, or a lower difficulty if no question with a matching difficulty exsist.
        /// If no question can be loaded it will return null
        /// </summary>
        public static IQuestionContent GetQuestion(string surveyId, int difficulty, bool lowerDifficultyOk = true)
        {
            var listQuestions = Questions[surveyId]; /// list containing all questions
            List<IQuestionContent> listAvailableQuestion = new List<IQuestionContent>(); /// list containing all available question
            foreach(IQuestionContent question in listQuestions)
            { 
                if (question.Difficulty == difficulty) /// check if right difficulty
                {
                    if (!DoesAnswersExists(surveyId, question.InternId)) /// check if not already answered
                    {
                        /// question can be added to the list of available Question
                        listAvailableQuestion.Add(question);
                    }
                }
            }
            if (listAvailableQuestion.Count > 0)
            {
                return listAvailableQuestion[RandomNumber.Next(listAvailableQuestion.Count)]; /// return random question
            }
            if (difficulty == 1 || !lowerDifficultyOk)
            {
                return null; /// no more question available
            }
            else
            {
                return GetQuestion(surveyId, difficulty - 1); /// look for question in lower difficulty
            }
        }

        /// <summary>
        /// Returns the specific Question with the matching id from the list
        /// </summary>
        public static IQuestionContent LoadQuestionById(string surveyId, int id) => Questions[surveyId].FirstOrDefault(q => q.InternId == id);

        /// <summary>
        /// Returns the specific Answer with the matching id from the list
        /// </summary>
        public static IUserAnswer LoadAnswerById(string surveyId, int id) => Answers[surveyId].FirstOrDefault(a => a.InternId == id);

        /// <summary>
        /// Returns a list with all answers of given SurveyId
        /// </summary>
        /// <param name="surveyId">SurveyId to search</param>
        /// <returns>List containing all Ansers matching SurveyId. Never null</returns>
        public static List<IUserAnswer> GetAllAnswers(string surveyId) => Answers.TryGetValue(surveyId, out var value) ? value : new List<IUserAnswer>();

        /// <summary>
        /// Returns a list with all questions of given SurveyId
        /// </summary>
        /// <param name="surveyId">SurveyId to search</param>
        /// <returns>List containing all Ansers matching SurveyId. Never null</returns>
        public static List<IQuestionContent> GetAllQuestions(string surveyId) => Questions.TryGetValue(surveyId, out var value) ? value : new List<IQuestionContent>();

        /// <summary>
        /// Searches for a corrosponding answer object and returns true if found, else it will return false
        /// </summary>
        public static bool DoesAnswersExists(string surveyId, int Id)
        {
            if (!Answers.TryGetValue(surveyId, out var answers))
                return false;
            return answers.Any(a => a.InternId == Id);
        }

        /// <summary>
        /// Adds an IUserAnswer object to the right list
        /// </summary>
        public static void AddAnswer(string surveyId, IUserAnswer answer)
        {
            if (!Answers.ContainsKey(surveyId))
                Answers.Add(surveyId, new List<IUserAnswer>());
            Answers[surveyId].Add(answer);
        }

        public static void ExportAnswers() => StorageProvider.ExportAnswers(Answers);
    }
}