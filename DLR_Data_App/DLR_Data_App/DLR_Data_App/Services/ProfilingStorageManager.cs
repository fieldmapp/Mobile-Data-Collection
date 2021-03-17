//Main contributors: Henning Woydt
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using DLR_Data_App.Models.Profiling;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DLR_Data_App.Services
{
    public static class ProfilingStorageManager
    {
        private const double MaxMinutesToContinueProfiling = 5;

        /// <summary>
        /// Dictionary containng all lists to save the questions
        /// </summary>
        private static Dictionary<string, List<IQuestionContent>> TranslatedQuestions = new Dictionary<string, List<IQuestionContent>>();

        /// <summary>
        /// Dictionary containing all lists to save the answers. The order of each List is crucial to determine streak and should not be changed ever
        /// </summary>
        private static Dictionary<string, List<IUserAnswer>> Answers => Result.UserAnswers;

        private static Dictionary<string, string> Translations => CurrentProfiling.Translations;

        /// <summary>
        /// List containing all ProfilingMenuItems, which contain all relevant information over the different profiling types
        /// </summary>
        public static ObservableCollection<ProfilingMenuItem> ProfilingMenuItems { get; private set; } = new ObservableCollection<ProfilingMenuItem>();

        /// <summary>
        /// Used to load questions
        /// </summary>
        private static IStorageProvider StorageProvider => (Application.Current as App).StorageProvider;

        private static string CurrentProfilingId => CurrentProfiling.ProfilingId;

        private static ProfilingData CurrentProfiling;

        private static ProfilingResult Result;

        /// <summary>
        /// The name of the user currently logged into the app
        /// </summary>
        private static int UserId;

        public static int ProjectsFilledSinceLastProfilingCompletion = 0;

        /// <summary>
        /// Initializes the <see cref="ProfilingStorageManager"/>
        /// </summary>
        public static void Initilize(int userId)
        {
            UserId = userId;
        }

        public static void SetProfiling(ProfilingData profiling)
        {
            CurrentProfiling = profiling;
            if (CurrentProfiling == null)
                return;

            LoadTranslatedProfilingMenuItems();
            
            Result = LoadAnswerToContinue();

            foreach (var subProfilingAnswers in Result.UserAnswers)
            {
                var subProfilingId = subProfilingAnswers.Key;
                var subProfiling = profiling.ProfilingMenuItems.FirstOrDefault(p => p.Id == subProfilingId);
                if (subProfiling == null)
                    continue;
                foreach (var answer in subProfilingAnswers.Value)
                {
                    subProfiling.ApplyAnswer(answer);
                }
            }

            foreach (var subProfilingQuestions in TranslatedQuestions)
            {
                var subProfilingId = subProfilingQuestions.Key;
                var subProfiling = profiling.ProfilingMenuItems.FirstOrDefault(p => p.Id == subProfilingId);
                if (subProfiling != null)
                    subProfiling.SetQuestions(subProfilingQuestions.Value);
            }

            ProfilingMenuItems.SetTo(profiling.ProfilingMenuItems);
            foreach (var profilingMenuItem in ProfilingMenuItems)
            {
                profilingMenuItem.ChapterName = Helpers.GetCurrentLanguageTranslation(Translations, profilingMenuItem.ChapterName);
            }

        }

        private static void LoadTranslatedProfilingMenuItems()
        {
            TranslatedQuestions = CurrentProfiling.Questions;
            foreach (var question in TranslatedQuestions.SelectMany(q => q.Value))
            {
                question.Translate(Translations);
            }
        }

        /// <summary>
        /// Looks for the last completed profiling and returns its date
        /// </summary>
        /// <returns><see cref="DateTime"/> of newest answered profiling</returns>
        public static bool IsProfilingModuleLoaded(string profilingId)
        {
            return Database.ReadProfilings().Any(r => r.ProfilingId == profilingId);
        }

        /// <summary>
        /// Looks for the last completed profiling and returns its date
        /// </summary>
        /// <returns><see cref="DateTime"/> of newest answered profiling</returns>
        public static DateTime GetLastCompletedProfilingDate(string profilingId)
        {
            var results = Database.ReadProfilingResults().Where(r => r.UserId == UserId && r.ProfilingId == profilingId);
            return results.Any() ? results.Max(e => e.TimeStamp) : DateTime.MinValue;
        }

        private static ProfilingResult LoadAnswerToContinue()
        {
            var savedProfilings = Database.ReadProfilingResults();
            var currentProfiling = savedProfilings.FirstOrDefault(r => r.UserId == UserId && Math.Abs((r.TimeStamp - DateTime.UtcNow).TotalMinutes) < MaxMinutesToContinueProfiling);
            if (currentProfiling == null)
            {
                currentProfiling = new ProfilingResult
                {
                    ProfilingId = CurrentProfilingId,
                    UserAnswers = new Dictionary<string, List<IUserAnswer>>(),
                    UserId = UserId
                };
            }
            return currentProfiling;
        }

        /// <summary>
        /// Saves all answers.
        /// This will override old saved data!
        /// </summary>
        public static void SaveCurrentAnswer()
        {
            Result.TimeStamp = DateTime.UtcNow;
            Database.SetOrUpdateProfilingAnswer(Result);
        }

        /// <summary>
        /// Creates Answers.csv with all Answers in one line 
        /// </summary>
        public static void CreateCSV()
        {
            string explanation = "UserCode,Date"; /// contains the explanation for the data
            string data = "," + DateTime.Now.ToString("yyyy MM dd"); /// contains the data

            /// write all Introspection Answers in one line
            foreach (QuestionIntrospectionPage question in TranslatedQuestions["Introspection"].OrderBy(o => o.InternId))
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
            foreach (QuestionImageCheckerPage question in TranslatedQuestions["ImageChecker"].OrderBy(o => o.InternId))
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
            foreach (QuestionStadiumPage question in TranslatedQuestions["Stadium"].OrderBy(o => o.InternId))
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
            foreach (QuestionDoubleSliderPage question in TranslatedQuestions["DoubleSlider"].OrderBy(o => o.InternId))
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
        public static IQuestionContent GetQuestion(string profilingId, int difficulty, bool lowerDifficultyOk = true)
        {
            var listQuestions = TranslatedQuestions[profilingId]; /// list containing all questions
            List<IQuestionContent> listAvailableQuestion = new List<IQuestionContent>(); /// list containing all available question
            foreach(IQuestionContent question in listQuestions)
            {
                const bool ignoreDifficulty = true;

                if (ignoreDifficulty || question.Difficulty == difficulty) /// check if right difficulty
                {
                    if (!DoesAnswersExists(profilingId, question.InternId)) /// check if not already answered
                    {
                        /// question can be added to the list of available Question
                        listAvailableQuestion.Add(question);
                    }
                }
            }
            if (listAvailableQuestion.Count > 0)
            {
                return listAvailableQuestion[App.RandomProvider.Next(listAvailableQuestion.Count)]; /// return random question
            }
            if (difficulty == 1 || !lowerDifficultyOk)
            {
                return null; /// no more question available
            }
            else
            {
                return GetQuestion(profilingId, difficulty - 1); /// look for question in lower difficulty
            }
        }

        /// <summary>
        /// Returns the specific Question with the matching id from the list
        /// </summary>
        public static IQuestionContent LoadQuestionById(string profilingId, int id) => TranslatedQuestions[profilingId].FirstOrDefault(q => q.InternId == id);

        /// <summary>
        /// Returns the specific Answer with the matching id from the list
        /// </summary>
        public static IUserAnswer LoadAnswerById(string profilingId, int id) => Answers[profilingId].FirstOrDefault(a => a.InternId == id);

        /// <summary>
        /// Returns a list with all answers of given ProfilingId
        /// </summary>
        /// <param name="profilingId">ProfilingId to search</param>
        /// <returns>List containing all Ansers matching ProfilingId. Never null</returns>
        public static List<IUserAnswer> GetAllAnswers(string profilingId) => Answers.TryGetValue(profilingId, out var value) ? value : new List<IUserAnswer>();

        /// <summary>
        /// Returns a list with all questions of given ProfilingId
        /// </summary>
        /// <param name="profilingId">ProfilingId to search</param>
        /// <returns>List containing all Ansers matching ProfilingId. Never null</returns>
        public static List<IQuestionContent> GetAllQuestions(string profilingId) => TranslatedQuestions.TryGetValue(profilingId, out var value) ? value : new List<IQuestionContent>();

        /// <summary>
        /// Searches for a corrosponding answer object and returns true if found, else it will return false
        /// </summary>
        public static bool DoesAnswersExists(string profilingId, int Id)
        {
            if (!Answers.TryGetValue(profilingId, out var answers))
                return false;
            return answers.Any(a => a.InternId == Id);
        }

        /// <summary>
        /// Adds an IUserAnswer object to the corresponding list
        /// </summary>
        public static void AddAnswer(string profilingId, IUserAnswer answer)
        {
            if (!Answers.ContainsKey(profilingId))
                Answers.Add(profilingId, new List<IUserAnswer>());
            Answers[profilingId].Add(answer);
        }

        /// <summary>
        /// Exports the answers using the provided <see cref="StorageProvider"/>
        /// </summary>

        public static void ExportAnswers()
        {
            StorageProvider.ExportAnswers(new ProfilingResults { Results = Database.ReadProfilingResults().Where(p => p.UserId == UserId).ToList() });
        }
    }
}