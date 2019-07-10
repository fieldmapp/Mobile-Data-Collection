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
        static Random RandomNumber = new Random();

        /// <summary>
        /// Dictionary containng all lists to save the questions
        /// </summary>
        private static Dictionary<string, List<IQuestionContent>> Questions = new Dictionary<string, List<IQuestionContent>>();

        /// <summary>
        /// Dictionary containing all lists to save the answers
        /// </summary>
        private static Dictionary<string, List<IUserAnswer>> Answers = new Dictionary<string, List<IUserAnswer>>();

        /// <summary>
        /// Used to load questions
        /// </summary>
        private static IQuestionsProvider questionsProvider;

        /// <summary>
        /// Initializes the DatabankCommunication
        /// </summary>
        public static void Initilize(IQuestionsProvider provider)
        {
            questionsProvider = provider;
            CreateQuestionsFromTxt();
            CreateAllNotExistingAnswerTxt();
            CreateNotExistingSurveyMenuItemsTxt();

            ResetAnswersAndSurveyMenuItemsTxt();

            LoadAllAnswersFromTxt();
        }

        /// <summary>
        /// Resets the Answer and SurveyMenuItem txt files
        /// </summary>
        public static void ResetAnswersAndSurveyMenuItemsTxt()
        {
            ResetAllAnswersTxt();
            SaveSurveyMenuItemsTxt(getDefaultSurveyMenuItems());
        }

        /// <summary>
        /// Creates a List with the deafult SurveymenuItems and retuns this list
        /// </summary>
        /// <returns></returns>
        public static List<SurveyMenuItem> getDefaultSurveyMenuItems()
        {
            List<SurveyMenuItem> Items = new List<SurveyMenuItem>()
            {
                new SurveyMenuItem("DoubleSlider", "Bedeckungsgrade", 4, 18, 0, true, Color.White, new List<int>{3,4}),
                new SurveyMenuItem("ImageChecker", "Sortenerkennung", 2, 25, 0, true, Color.White, new List<int>{2}),
                new SurveyMenuItem("Stadium", "Wuchsstadien", 6, 12, 0, true, Color.White, new List<int>{1})
            };
            return Items;
        }

        /// <summary>
        /// Creates all Questions, which are written in this class, could not include all Questions if only synchronized with the txt files
        /// Its probably better to use CreateQuestionsFromTxt
        /// </summary>
        public static void CreateQuestions()
        {
            Questions["ImageChecker"] = CreateQuestionsForImageChecker();
            Questions["DoubleSlider"] = CreateQuestionsForDoubleSlider();
            Questions["Stadium"] = CreateQuestionsForStadium();
            Questions["Introspection"] = CreateQuestionForIntrospection();
        }

        /// <summary>
        /// Creates all Questions, but loads these from the txt files
        /// </summary>
        public static void CreateQuestionsFromTxt()
        {
            Questions["ImageChecker"] = LoadQuestionsForImageCheckerFromTxt();
            Questions["DoubleSlider"] = LoadQuestionsForDoubleSliderFromTxt();
            Questions["Stadium"] = LoadQuestionsForStadiumFromTxt();
            Questions["Introspection"] = LoadQuestionsForIntrospectionFromTxt();
        }
        /// <summary>
        /// Loads all Answers from the txt files
        /// </summary>
        public static void LoadAllAnswersFromTxt()
        {
            Answers["ImageChecker"] = LoadAnswersForImageCheckerPage();
            Answers["DoubleSlider"] = LoadAnswersForDoubleSliderPage();
            Answers["Stadium"] = LoadAnswersForStadiumPage();
            Answers["Introspection"] = LoadAnswersForIntrospectionPage();
        }

        /// <summary>
        /// Saves all Answers in txt files
        /// This will override the old files!
        /// </summary>
        public static void SaveAllAnswersInTxt()
        {
            SaveAnswersForImageCheckerPage();
            SaveAnswersForDoubleSliderPage();
            SaveAnswersForIntrospectionPage();
            SaveAnswersForStadiumPage();
        }

        /// <summary>
        /// Creates all txt files to save the answers
        /// </summary>
        public static void CreateAllNotExistingAnswerTxt()
        {
            CreateNotExistingAnswerTxt("ImageCheckerAnswers.txt");
            CreateNotExistingAnswerTxt("DoubleSliderAnswers.txt");
            CreateNotExistingAnswerTxt("StadiumAnswers.txt");
            CreateNotExistingAnswerTxt("IntrospectionAnswers.txt");
        }

        /// <summary>
        /// Creates a txt with the name of source, if it doesnt already exist
        /// </summary>
        public static void CreateNotExistingAnswerTxt(string source)
        {
            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, source);
            /// if file doenst exist, write it
            if(!File.Exists(filename))
            {
                File.WriteAllText(filename,"END_ANSWERS");
            }
        }

        /// <summary>
        /// Resets all Answer Txt (only writes "END_ANSWERS")
        /// </summary>
        public static void ResetAllAnswersTxt()
        {
            ResetAnswerTxt("ImageCheckerAnswers.txt");
            ResetAnswerTxt("DoubleSliderAnswers.txt");
            ResetAnswerTxt("StadiumAnswers.txt");
            ResetAnswerTxt("IntrospectionAnswers.txt");
        }

        /// <summary>
        /// Writes only "END_ANSWERS" in source
        /// </summary>
        /// <param name="source"></param>
        public static void ResetAnswerTxt(string source)
        {
            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, source);
            /// wirte file
            File.WriteAllText(filename, "END_ANSWERS");
        }

        /// <summary>
        /// Creates Answers.csv with all Answers in one line 
        /// </summary>
        public static void CreateCSV()
        {
            string explination = "UserCode,Date"; /// contains the explination for the data
            string data = "," + DateTime.Now.ToString("yyyy MM dd"); /// contains the data
            /// sort the lists after InternId
            Questions["ImageChecker"] = Questions["ImageChecker"].OrderBy(o => o.InternId).ToList();
            Questions["DoubleSlider"] = Questions["DoubleSlider"].OrderBy(o => o.InternId).ToList();
            Questions["Stadium"] = Questions["Stadium"].OrderBy(o => o.InternId).ToList();
            Questions["Introspection"] = Questions["Introspection"].OrderBy(o => o.InternId).ToList();

            /// write all Introspection Answers in one line
            foreach (QuestionIntrospectionPage question in Questions["Introspection"])
            {
                explination += ",SelectedAnswerQuestion" + question.InternId;
                if (SearchAnswers("Introspection", question.InternId))
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
            foreach (QuestionImageCheckerPage question in Questions["ImageChecker"])
            {
                explination += ",DifficultyQuestion" + question.InternId + ",Img1SelQuestion" + question.InternId + ",Img2SelQuestion" + question.InternId + ",Img3SelQuestion" + question.InternId + ",Img4SelQuestion" + question.InternId;
                if (SearchAnswers("ImageChecker", question.InternId))
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
            foreach (QuestionStadiumPage question in Questions["Stadium"])
            {
                explination += ",DifficultyQuestion" + question.InternId + ",StadiumQuestion" + question.InternId + ",FruitTypeQuestion" + question.InternId;
                if (SearchAnswers("Stadium", question.InternId))
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
            foreach (QuestionDoubleSliderPage question in Questions["DoubleSlider"])
            {
                explination += ",DifficultyQuestion" + question.InternId + ",ResAQuestion" + question.InternId + ",ResBQuestion" + question.InternId;
                if (SearchAnswers("DoubleSlider", question.InternId))
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
            File.WriteAllText(filename, explination + "\n" + data);
        }

        /// <summary>
        /// Creates all questions for the ImageCheckerType, but NOT from the txt file
        /// </summary>
        public static List<IQuestionContent> CreateQuestionsForImageChecker()
        {
            return new List<IQuestionContent>
            {
                new QuestionImageCheckerPage(1, "Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 0, 1, 0, "ImageChecker_one_question1_picture1.png", "ImageChecker_one_question1_picture2.png", "ImageChecker_one_question1_picture3.png", "ImageChecker_one_question1_picture4.png"),
                new QuestionImageCheckerPage(2, "Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 1, 0, 1, 1, 0, "ImageChecker_one_question2_picture1.png", "ImageChecker_one_question2_picture2.png", "ImageChecker_one_question2_picture3.png", "ImageChecker_one_question2_picture4.png"),
                new QuestionImageCheckerPage(3, "Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 2, 1, 1, 1, 0, "ImageChecker_two_question3_picture1.png", "ImageChecker_two_question3_picture2.png", "ImageChecker_two_question3_picture3.png", "ImageChecker_two_question3_picture4.png"),
                new QuestionImageCheckerPage(4, "Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 2, 0, 0, 1, 0, "ImageChecker_two_question4_picture1.png", "ImageChecker_two_question4_picture2.png", "ImageChecker_two_question4_picture3.png", "ImageChecker_two_question4_picture4.png"),
                new QuestionImageCheckerPage(5, "Wo sehen sie die Feldfruchtsorte Kartoffel abgebildet?", 3, 1, 0, 1, 0, "ImageChecker_three_question5_picture1.png", "ImageChecker_three_question5_picture2.png", "ImageChecker_three_question5_picture3.png", "ImageChecker_three_question5_picture4.png"),
                new QuestionImageCheckerPage(6, "Wo sehen sie die Feldfruchtsorte Gerste abgebildet?", 3, 0, 0, 1, 0, "ImageChecker_three_question6_picture1.png", "ImageChecker_three_question6_picture2.png", "ImageChecker_three_question6_picture3.png", "ImageChecker_three_question6_picture4.png")
            };
        }

        /// <summary>
        /// Creates all questions for the DoubleSlider Type, but NOT from the txt file
        /// </summary>
        public static List<IQuestionContent> CreateQuestionsForDoubleSlider()
        {
            return new List<IQuestionContent>
            {
                new QuestionDoubleSliderPage(1, 1, "DoubleSlider_one_question1.png", 7, 4),
                new QuestionDoubleSliderPage(2, 1, "DoubleSlider_one_question2.png", 49, 91),
                new QuestionDoubleSliderPage(3, 1, "DoubleSlider_one_question3.png", 12, 3),
                new QuestionDoubleSliderPage(4, 1, "DoubleSlider_one_question4.png", 64, 94),
                new QuestionDoubleSliderPage(5, 2, "DoubleSlider_two_question1.png", 7, 96),
                new QuestionDoubleSliderPage(6, 2, "DoubleSlider_two_question2.png", 49, 91),
                new QuestionDoubleSliderPage(7, 2, "DoubleSlider_two_question3.png", 12, 3),
                new QuestionDoubleSliderPage(8, 2, "DoubleSlider_two_question4.png", 64, 94),
                new QuestionDoubleSliderPage(9, 3, "DoubleSlider_three_question1.png", 42, 87),
                new QuestionDoubleSliderPage(10, 3, "DoubleSlider_three_question2.png", 6, 0),
                new QuestionDoubleSliderPage(11, 3, "DoubleSlider_three_question3.png", 80, 5),
                new QuestionDoubleSliderPage(12, 3, "DoubleSlider_three_question4.png", 76, 99)
            };
        }

        /// <summary>
        /// Creates all questions for the StadiumType, but NOT from the txt file
        /// </summary>
        public static List<IQuestionContent> CreateQuestionsForStadium()
        {
            /// create needed lists for the questions
            var stadiums1 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Blattentwicklung", "stadium_blattentwicklung.png", 1),
                new StadiumSubItem("Bestockung", "stadium_bestockung.png", 2),
                new StadiumSubItem("Längenwachstum/Schossen", "stadium_laengenwachstumschossen.png", 3)
            };
            var plants1 = new List<Plant>
            {
                new Plant("Kartoffel", "A"),
                new Plant("Mais", "B"),
                new Plant("Weizen", "C"),
                new Plant("Zuckerrübe", "D")
            };
            var stadiums2 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Blattentwicklung", "stadium_blattentwicklung.png",1),
                new StadiumSubItem("Schossen/Haupttrieb", "stadium_laengenwachstumschossen.png",2),
                new StadiumSubItem("Ähren/-Rispenschwellen", "stadium_ahrenrispenschwellen.png",3),
                new StadiumSubItem("Entwicklung der Blütenanlage", "stadium_entwicklungbluetenanlage.png",4),
                new StadiumSubItem("Blüte", "bluete.png",5)
            };
            var plants2 = new List<Plant>
            {
                new Plant("Gerste", "A"),
                new Plant("Kartoffel", "B"),
                new Plant("Raps", "C"),
                new Plant("Weizen", "D")
            };
            var stadiums3 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Längenwachstum/Schossen", "stadium_laengenwachstumschossen.png", 1),
                new StadiumSubItem("Entwicklung vegetativer Pflanzenteile/Rübenkörper", "stadium_entwicklungvegpflanzruebenk.png", 2),
                new StadiumSubItem("Entwicklung der Blütenanlage", "stadium_entwicklungbluetenanlage.png", 3),
                new StadiumSubItem("Fruchtentwicklung", "bluete.png", 4)
            };
            var plants3 = new List<Plant>
            {
                new Plant("Kartoffel", "A"),
                new Plant("Mais", "B"),
                new Plant("Raps", "C"),
                new Plant("Weizen", "D"),
                new Plant("Zuckerrübe", "E")
            };
            return new List<IQuestionContent>
            {
                //TODO: Add Support for multiple correkt answers (as seen in original survey 1 stadium answer 3)
                new QuestionStadiumPage(1, 1, "Stadium_one_question1.png", stadiums1.ToList(), plants1.ToList(), 1, "C"),
                new QuestionStadiumPage(2, 1, "Stadium_one_question2.png", stadiums1.ToList(), plants1.ToList(), 1, "D"),
                new QuestionStadiumPage(3, 1, "Stadium_one_question3.png", stadiums1.ToList(), plants1.ToList(), 1, "B"), // wrong picture needs replacement, ask Frau Truckenbrodt
                new QuestionStadiumPage(4, 1, "Stadium_one_question4.png", stadiums1.ToList(), plants1.ToList(), 1, "A"),
                new QuestionStadiumPage(5, 1, "Stadium_one_question5.png", stadiums1.ToList(), plants1.ToList(), 3, "C"),
                new QuestionStadiumPage(6, 1, "Stadium_one_question6.png", stadiums1.ToList(), plants1.ToList(), 2, "C"),

                new QuestionStadiumPage(7, 2, "Stadium_two_question1.png", stadiums2.ToList(), plants2.ToList(), 5, "B"),
                new QuestionStadiumPage(8, 2, "Stadium_two_question2.png", stadiums2.ToList(), plants2.ToList(), 4, "C"),
                new QuestionStadiumPage(9, 2, "Stadium_two_question3.png", stadiums2.ToList(), plants2.ToList(), 5, "C"),
                new QuestionStadiumPage(10, 2, "Stadium_two_question4.png", stadiums2.ToList(), plants2.ToList(), 1, "B"),
                new QuestionStadiumPage(11, 2, "Stadium_two_question5.png", stadiums2.ToList(), plants2.ToList(), 2, "C"),
                new QuestionStadiumPage(12, 2, "Stadium_two_question6.png", stadiums2.ToList(), plants2.ToList(), 3, "A"),

                new QuestionStadiumPage(13, 3, "Stadium_three_question1.png", stadiums3.ToList(), plants3.ToList(), 2, "E"), // wrong picture needs replacement, ask Frau Truckenbrodt
                new QuestionStadiumPage(14, 3, "Stadium_three_question2.png", stadiums3.ToList(), plants3.ToList(), 4, "D"),
                new QuestionStadiumPage(15, 3, "Stadium_three_question3.png", stadiums3.ToList(), plants3.ToList(), 3, "B"),
                new QuestionStadiumPage(16, 3, "Stadium_three_question4.png", stadiums3.ToList(), plants3.ToList(), 3, "A"),
                new QuestionStadiumPage(17, 3, "Stadium_three_question5.png", stadiums3.ToList(), plants3.ToList(), 4, "C"),
                new QuestionStadiumPage(18, 3, "Stadium_three_question6.png", stadiums3.ToList(), plants3.ToList(), 1, "D")
            };
        }

        /// <summary>
        /// Creates all questions for the IntrospectionType, but NOT from the txt file
        /// </summary>
        public static List<IQuestionContent> CreateQuestionForIntrospection()
        {
            return new List<IQuestionContent>
            {
                new QuestionIntrospectionPage(1,"Ich kann die Sorte von Feldfrüchten zuverlässig erkennen"),
                new QuestionIntrospectionPage(2, "Ich kann phänologische Entwicklungsstadien von Feldfrüchten zuverlässig erkennen"),
                new QuestionIntrospectionPage(3, "Ich kann den Bedeckungsgrad des Bodens durch Pflanzen zuverlässig schätzen"),
                new QuestionIntrospectionPage(4, "Ich kann den Anteil grüner Pflanzenbestandteile am gesamten Pflanzenmaterial zuverlässig schätzen")
            };
        }

        /// <summary>
        /// Loads a QUestionImageCheckerPage-Object with the set difficulty, or a lower difficulty if no question with a matching difficulty exsist.
        /// If no question can be loaded it will return null
        /// </summary>
        public static IQuestionContent LoadQuestion(string surveyId, int difficulty, bool lowerDifficultyOk = true)
        {
            var listQuestions = Questions[surveyId]; /// list containing all questions
            List<IQuestionContent> listAvailableQuestion = new List<IQuestionContent>(); /// list containing all available question
            foreach(IQuestionContent question in listQuestions)
            { 
                if (question.Difficulty == difficulty) /// check if right difficulty
                {
                    if (!SearchAnswers(surveyId, question.InternId)) /// check if not already answered
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
                return LoadQuestion(surveyId, difficulty - 1); /// look for question in lower difficulty
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
        /// Returns a list with all answers
        /// </summary>
        public static List<IUserAnswer> GetAllAnswers(string surveyId) => Answers[surveyId];

        /// <summary>
        /// Returns a list with all questions
        /// </summary>
        public static List<IQuestionContent> GetAllQuestions(string surveyId) => Questions[surveyId];

        /// <summary>
        /// Searches for a corrosponding answer object and returns true if found, else it will return false
        /// </summary>
        public static bool SearchAnswers(string surveyId, int Id)
        {
            if (!Answers.TryGetValue(surveyId, out var answers))
                return false;
            return answers.Any(a => a.InternId == Id);
        }

        /// <summary>
        /// Adds an IUserAnswer Object to the right list
        /// </summary>
        public static void AddAnswer(string surveyId, IUserAnswer answer)
        {
            if (!Answers.ContainsKey(surveyId))
                Answers.Add(surveyId, new List<IUserAnswer>());
            Answers[surveyId].Add(answer);
        }

        /// <summary>
        /// Creates a set of example answers for testing purposes
        /// Overrides old answer list
        /// </summary>
        public static void ExampleAnswers()
        {
            Answers["ImageChecker"] = new List<IUserAnswer>
            {
                new AnswerImageCheckerPage(1,1,1,0,0),
                new AnswerImageCheckerPage(2,0,1,0,1),
                new AnswerImageCheckerPage(3,0,1,0,1)
            };
            Answers["DoubleSlider"] = new List<IUserAnswer>
            {
                new AnswerDoubleSliderPage(1,10,20),
                new AnswerDoubleSliderPage(2,40,30),
                new AnswerDoubleSliderPage(3,60,10),
                new AnswerDoubleSliderPage(4,34,34)
            };
            Answers["Introspection"] = new List<IUserAnswer>
            {
                new AnswerIntrospectionPage(1,3),
                new AnswerIntrospectionPage(1,4)
            };
            Answers["Stadium"] = new List<IUserAnswer>
            {
                new AnswerStadiumPage(1,"A",1),
                new AnswerStadiumPage(2,"A",2)
            };
        }
        /// <summary>
        /// Creates a set of example SurveyMenuItems for testing purposes
        /// Overrides existing SurveyMenuItems 
        /// </summary>
        public static void ExampleSurveyMenuItems()
        {
            List<SurveyMenuItem> list =  new List<SurveyMenuItem>
            {
                new SurveyMenuItem("DoubleSlider", "Bedeckungsgrade", 4, 18, 0, true, Color.White, new List<int>{3,4}),
                new SurveyMenuItem("ImageChecker", "Sortenerkennung", 2, 25, 0, true, Color.White, new List<int>{2}),
                new SurveyMenuItem("Stadium", "Wuchsstadien", 6, 12, 0, true, Color.White, new List<int>{1})
            };

            SaveSurveyMenuItemsTxt(list);
        }

        /// <summary>
        /// Creates SurveyMenuItems.txt and writes "END_SURVEYMENUITEMS", if it doesnt already exist
        /// </summary>
        public static void CreateNotExistingSurveyMenuItemsTxt()
        {
            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "SurveyMenuItems.txt");

            if (!File.Exists(filename))
            {
                /// If file doesnt exist, then create one
                SaveSurveyMenuItemsTxt(getDefaultSurveyMenuItems());
            }
        }

        /// <summary>
        /// Resets the SurveyMenuItems.txt file
        /// </summary>
        public static void ResetSurveyMenuItemsTxt()
        {
            SaveSurveyMenuItemsTxt(getDefaultSurveyMenuItems());
        }
        /// <summary>
        /// Saves each SurveyMenuItem as text in SurveyMenuItems.txt
        /// </summary>
        public static void SaveSurveyMenuItemsTxt(List<SurveyMenuItem> ListSurveyMenuItems)
        {
            string text = "";
            foreach(SurveyMenuItem item in ListSurveyMenuItems)
            {
                /// write each object in own line
                text += item.Id + ";";
                text += item.ChapterName + ";";
                text += item.AnswersNeeded + ";";
                text += item.MaximumQuestionNumber + ";";
                text += item.AnswersGiven + ";";
                text += item.Unlocked + ";";
                text += item.BackgroundColor.R + "," + item.BackgroundColor.G + "," + item.BackgroundColor.B + ";";
                foreach(int id in item.IntrospectionQuestion)
                {
                    /// list is one Attribute, the int in the list are seperated with ","
                    text += id + ",";
                }
                text = text.Remove(text.Length-1);
                text += "\n";
            }
            text += "END_SURVEYMENUITEMS";

            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "SurveyMenuItems.txt");

            /// write text in file
            File.WriteAllText(filename, text);
        }

        /// <summary>
        /// Loads SurveyMenuItems from SurveyMenuItems.txt
        /// Returns a List containing all loaded elements
        /// </summary>
        /// <returns></returns>
        public static List<SurveyMenuItem> LoadSurveyMenuItemsFromTxt()
        {
            List<SurveyMenuItem> tempList = new List<SurveyMenuItem>();

            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "SurveyMenuItems.txt");

            /// read the  file
            string content = File.ReadAllText(filename);
            StringReader stringReader = new StringReader(content);
            string line = stringReader.ReadLine();
            while (!line.Equals("END_SURVEYMENUITEMS"))
            {
                /// decode each line
                String[] attributes = line.Split(';');
                string id = attributes[0];
                string chapterName = attributes[1];
                int answersNeeded = Convert.ToInt32(attributes[2]);
                int maximumQuestionNumber = Convert.ToInt32(attributes[3]);
                int answerGiven = Convert.ToInt32(attributes[4]);
                bool unlocked = Convert.ToBoolean(attributes[5]);
                //List<Double> colors = attributes[6].Split(',').Select(Double.Parse).ToList();
                //Color background = Color.FromArgb(Convert.ToInt32(colors.ElementAt(0)),Convert.ToInt32(colors.ElementAt(1)),Convert.ToInt32(colors.ElementAt(2)));
                Color background = Color.White;
                List<int> introspectionQuestion = attributes[7].Split(',').Select(Int32.Parse).ToList();

                /// create new object and add it to the list
                SurveyMenuItem item = new SurveyMenuItem(id, chapterName, answersNeeded, maximumQuestionNumber, answerGiven, unlocked, background, introspectionQuestion);
                tempList.Add(item);

                /// read new line
                line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Loads all questions for the ImageCheckerType from ImageCheckerQuestions.txt
        /// Returns a List containing these Questions
        /// </summary>
        public static List<IQuestionContent> LoadQuestionsForImageCheckerFromTxt()
        {
            List<IQuestionContent> tempList = new List<IQuestionContent>();

            /// Read text from the txt file
            String Text = questionsProvider.LoadTextFromTxt("ImageCheckerQuestions.txt");
            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();

            
            while (!(Line.Equals("END_QUESTIONS"))) /// for each line
            {
                /// split line by ';'
                String[] attributes = Line.Split(';');

                /// attributes should now contain all the needed information, but only in Strings
                /// convert information in right data type
                int id = Convert.ToInt32(attributes[0]);
                string questionText = attributes[1];
                int difficulty = Convert.ToInt32(attributes[2]);
                int img1Corr = Convert.ToInt32(attributes[3]);
                int img2Corr = Convert.ToInt32(attributes[4]);
                int img3Corr = Convert.ToInt32(attributes[5]);
                int img4Corr = Convert.ToInt32(attributes[6]);
                string img1Sour = attributes[7];
                string img2Sour = attributes[8];
                string img3Sour = attributes[9];
                string img4Sour = attributes[10];

                /// create new Qobject and add to the list
                QuestionImageCheckerPage question = new QuestionImageCheckerPage(id, questionText, difficulty, img1Corr, img2Corr, img3Corr, img4Corr, img1Sour, img2Sour, img3Sour, img4Sour);
                tempList.Add(question);

                /// read new line
                Line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Saves the answers from ListAnswerImageCheckerPage in ImageCheckerAnswers.txt
        /// every answer is saved as an own line in the txt: int id;int img1Selected;int img2Selected;int img3Selected;int img4Selected
        /// The answers end with "END_ANSWERS"
        /// This will override the old file!
        /// </summary>
        public static void SaveAnswersForImageCheckerPage()
        {
            /// save the answers as seperate lines
            string text = "";
            foreach(AnswerImageCheckerPage answer in Answers["ImageChecker"])
            {
                text += answer.InternId.ToString() + ";";
                text += answer.Image1Selected.ToString() + ";";
                text += answer.Image2Selected.ToString() + ";";
                text += answer.Image4Selected.ToString() + ";";
                text += answer.Image4Selected.ToString() + "\n";
            }
            text += "END_ANSWERS";

            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "ImageCheckerAnswers.txt");

            /// write text in file
            File.WriteAllText(filename, text);
        }

        /// <summary>
        /// Loads all Answers for the AnswerImageCheckerPage from ImageCheckerAnswer.txt
        /// </summary>
        /// <returns>List with all Answers</returns>
        public static List<IUserAnswer> LoadAnswersForImageCheckerPage()
        {
            List<IUserAnswer> tempList = new List<IUserAnswer>();

            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "ImageCheckerAnswers.txt");

            /// read the  file
            string content = File.ReadAllText(filename);
            StringReader stringReader = new StringReader(content);
            string line = stringReader.ReadLine();
            while (!line.Equals("END_ANSWERS"))
            {
                /// decode each line
                String[] attributes = line.Split(';');
                int id = Convert.ToInt32(attributes[0]);
                int img1Sel = Convert.ToInt32(attributes[1]);
                int img2Sel = Convert.ToInt32(attributes[2]);
                int img3Sel = Convert.ToInt32(attributes[3]);
                int img4Sel = Convert.ToInt32(attributes[4]);
                /// create new object and add it to the list
                AnswerImageCheckerPage answer = new AnswerImageCheckerPage(id, img1Sel, img2Sel, img3Sel, img4Sel);
                tempList.Add(answer);

                /// read new line
                line = stringReader.ReadLine();
            }
            return tempList;

        }

        /// <summary>
        /// Loads all questions for the DoubleSliderType from DoubleSliderQuestions.txt
        /// </summary>
        public static List<IQuestionContent> LoadQuestionsForDoubleSliderFromTxt()
        {
            List<IQuestionContent> tempList = new List<IQuestionContent>();

            /// get text from txt file
            String Text = questionsProvider.LoadTextFromTxt("DoubleSliderQuestions.txt");
            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();
            while (!(Line.Equals("END_QUESTIONS")))
            {
                /// decode each line
                String[] attributes = Line.Split(';');

                int id = Convert.ToInt32(attributes[0]);
                int difficulty = Convert.ToInt32(attributes[1]);
                string imgSour = attributes[2];
                int ans1 = Convert.ToInt32(attributes[3]);
                int ans2 = Convert.ToInt32(attributes[4]);

                /// create new object and add it to the list
                QuestionDoubleSliderPage question = new QuestionDoubleSliderPage(id, difficulty, imgSour, ans1, ans2);
                tempList.Add(question);

                /// read new line
                Line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Saves the answers from ListAnswerDoubleSliderPage in DoubleSliderAnswers.txt
        /// every answer is saved as an own line in the txt: int id;int ResultQuestionA;int ResultQuestionB
        /// The answers end with "END_ANSWERS"
        /// This will override the old file!
        /// </summary>
        public static void SaveAnswersForDoubleSliderPage()
        {
            /// save each answer as a line
            string text = "";
            foreach (AnswerDoubleSliderPage answer in Answers["DoubleSlider"])
            {
                text += answer.InternId.ToString() + ";";
                text += answer.ResultQuestionA.ToString() + ";";
                text += answer.ResultQuestionB.ToString() + "\n";
            }
            text += "END_ANSWERS";

            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "DoubleSliderAnswers.txt");

            /// write text in file
            File.WriteAllText(filename, text);
        }

        /// <summary>
        /// Loads all Answers for the AnswerDoubleSliderPage from DoubleSliderAnswer.txt
        /// </summary>
        /// <returns>List with all Answers</returns>
        public static List<IUserAnswer> LoadAnswersForDoubleSliderPage()
        {
            List<IUserAnswer> tempList = new List<IUserAnswer>();

            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "DoubleSliderAnswers.txt");

            /// get text
            string content = File.ReadAllText(filename);
            StringReader stringReader = new StringReader(content);
            string line = stringReader.ReadLine();
            while (!line.Equals("END_ANSWERS"))
            {
                /// decode each line
                String[] attributes = line.Split(';');
                int id = Convert.ToInt32(attributes[0]);
                int resA = Convert.ToInt32(attributes[1]);
                int resB = Convert.ToInt32(attributes[2]);

                /// craete object and add it to the list
                AnswerDoubleSliderPage answer = new AnswerDoubleSliderPage(id, resA, resB);
                tempList.Add(answer);

                /// read new line
                line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Loads all questions for the StadiumType from StadiumQuestions.txt
        /// </summary>
        public static List<IQuestionContent> LoadQuestionsForStadiumFromTxt()
        {
            /// read text from txt file
            String Text = questionsProvider.LoadTextFromTxt("StadiumQuestions.txt");
            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();

            /// create needed lists for the questions
            // TODO: Create a system to also store and load those 
            var stadiums1 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Blattentwicklung", "stadium_blattentwicklung.png", 1),
                new StadiumSubItem("Bestockung", "stadium_bestockung.png", 2),
                new StadiumSubItem("Längenwachstum/Schossen", "stadium_laengenwachstumschossen.png", 3)
            };
            var plants1 = new List<Plant>
            {
                new Plant("Kartoffel", "A"),
                new Plant("Mais", "B"),
                new Plant("Weizen", "C"),
                new Plant("Zuckerrübe", "D")
            };
            var stadiums2 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Blattentwicklung", "stadium_blattentwicklung.png",1),
                new StadiumSubItem("Schossen/Haupttrieb", "stadium_laengenwachstumschossen.png",2),
                new StadiumSubItem("Ähren/-Rispenschwellen", "stadium_ahrenrispenschwellen.png",3),
                new StadiumSubItem("Entwicklung der Blütenanlage", "stadium_entwicklungbluetenanlage.png",4),
                new StadiumSubItem("Blüte", "bluete.png",5)
            };
            var plants2 = new List<Plant>
            {
                new Plant("Gerste", "A"),
                new Plant("Kartoffel", "B"),
                new Plant("Raps", "C"),
                new Plant("Weizen", "D")
            };
            var stadiums3 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Längenwachstum/Schossen", "stadium_laengenwachstumschossen.png", 1),
                new StadiumSubItem("Entwicklung vegetativer Pflanzenteile/Rübenkörper", "stadium_entwicklungvegpflanzruebenk.png", 2),
                new StadiumSubItem("Entwicklung der Blütenanlage", "stadium_entwicklungbluetenanlage.png", 3),
                new StadiumSubItem("Fruchtentwicklung", "bluete.png", 4)
            };
            var plants3 = new List<Plant>
            {
                new Plant("Kartoffel", "A"),
                new Plant("Mais", "B"),
                new Plant("Raps", "C"),
                new Plant("Weizen", "D"),
                new Plant("Zuckerrübe", "E")
            };
            var tempList = new List<IQuestionContent>();
            while (!(Line.Equals("END_QUESTIONS")))
            {
                /// decode each line
                String[] attributes = Line.Split(';');

                int id = Convert.ToInt32(attributes[0]);
                int difficulty = Convert.ToInt32(attributes[1]);
                string imgSour = attributes[2];
                string stadiumList = attributes[3];
                string plantList = attributes[4];
                int stadiumCorr = Convert.ToInt32(attributes[5]);
                string plantCorr = attributes[6];

                /// give the question the right set of satdium and plant lists
                var stadium = stadiums1;
                var plant = plants1;

                if (stadiumList.Equals("stadiums2"))
                {
                    stadium = stadiums2;
                }
                if (stadiumList.Equals("stadiums3"))
                {
                    stadium = stadiums3;
                }
                if (plantList.Equals("plants2"))
                {
                    plant = plants2;
                }
                if (plantList.Equals("plants3"))
                {
                    plant = plants3;
                }

                /// create new object and add it to the list
                QuestionStadiumPage question = new QuestionStadiumPage(id, difficulty, imgSour, stadium, plant, stadiumCorr, plantCorr);
                tempList.Add(question);

                /// read new line
                Line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Saves the answers from ListAnswerStadiumPage in StadiumAnswers.txt
        /// every answer is saved as an own line in the txt: int id;String AnswerFruitType;String AnswerStadium
        /// The answers end with "END_ANSWERS"
        /// This will override the old file!
        /// </summary>
        public static void SaveAnswersForStadiumPage()
        {
            /// save each answer an an own line
            string text = "";
            foreach (AnswerStadiumPage answer in Answers["Stadium"])
            {
                text += answer.InternId.ToString() + ";";
                text += answer.AnswerFruitType + ";";
                text += answer.AnswerStadium + "\n";
            }
            text += "END_ANSWERS";

            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "StadiumAnswers.txt");

            /// write text in file
            File.WriteAllText(filename, text);
        }

        /// <summary>
        /// Loads all Answers for the AnswerStadiumPage from StadiumAnswer.txt
        /// </summary>
        /// <returns>List with all Answers</returns>
        public static List<IUserAnswer> LoadAnswersForStadiumPage()
        {
            List<IUserAnswer> tempList = new List<IUserAnswer>();

            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "StadiumAnswers.txt");

            /// get text from txt file
            string content = File.ReadAllText(filename);
            StringReader stringReader = new StringReader(content);
            string line = stringReader.ReadLine();
            while (!line.Equals("END_ANSWERS"))
            {
                /// decode each line
                String[] attributes = line.Split(';');
                int id = Convert.ToInt32(attributes[0]);
                String answerFruitType = attributes[1];
                int anserStadium = Convert.ToInt32(attributes[2]);

                /// create new Object and add it to the list
                AnswerStadiumPage answer = new AnswerStadiumPage(id, answerFruitType, anserStadium);
                tempList.Add(answer);

                /// read new line
                line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Loads all questions for the IntrospectionType from IntrospectionQuestions.txt
        /// </summary>
        public static List<IQuestionContent> LoadQuestionsForIntrospectionFromTxt()
        {
            /// get text
            String Text = questionsProvider.LoadTextFromTxt("IntrospectionQuestions.txt");
            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();
            var tempList = new List<IQuestionContent>();
            while (!(Line.Equals("END_QUESTIONS")))
            {
                /// decode each line
                String[] attributes = Line.Split(';');

                int id = Convert.ToInt32(attributes[0]);
                string questionText = attributes[1];

                /// create new object and add it to the list
                QuestionIntrospectionPage question = new QuestionIntrospectionPage(id, questionText);
                tempList.Add(question);

                /// read new line
                Line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Saves the answers from ListAnswerIntrospectionPage in IntrospectionAnswers.txt
        /// every answer is saved as an own line in the txt: int id;int SelectedAnswer
        /// The answers end with "END_ANSWERS"
        /// This will override the old file!
        /// </summary>
        public static void SaveAnswersForIntrospectionPage()
        {
            /// save each answer as own line
            string text = "";
            foreach (AnswerIntrospectionPage answer in Answers["Introspection"])
            {
                text += answer.InternId.ToString() + ";";
                text += answer.SelectedAnswer.ToString() + "\n";
            }
            text += "END_ANSWERS";

            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "IntrospectionAnswers.txt");

            /// write text in txt file
            File.WriteAllText(filename, text);
        }

        /// <summary>
        /// Loads all Answers for the AnswerIntrospectionPage from IntrospectionAnswer.txt
        /// </summary>
        /// <returns>List with all Answers</returns>
        public static List<IUserAnswer> LoadAnswersForIntrospectionPage()
        {
            List<IUserAnswer> tempList = new List<IUserAnswer>();

            /// get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "IntrospectionAnswers.txt");

            /// get text from txt
            string content = File.ReadAllText(filename);
            StringReader stringReader = new StringReader(content);
            string line = stringReader.ReadLine();
            while (!line.Equals("END_ANSWERS"))
            {
                /// decode each line
                String[] attributes = line.Split(';');
                int id = Convert.ToInt32(attributes[0]);
                int selectedAnswer = Convert.ToInt32(attributes[1]);

                /// create new object and add it to the List
                AnswerIntrospectionPage answer = new AnswerIntrospectionPage(id, selectedAnswer);
                tempList.Add(answer);

                line = stringReader.ReadLine();
            }
            return tempList;
        }
    }
}