using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    public class DatabankCommunication
    {
        static Random RandomNumber = new Random();

        private static Dictionary<string, List<IQuestionContent>> Questions = new Dictionary<string, List<IQuestionContent>>();

        private static Dictionary<string, List<IUserAnswer>> Answers = new Dictionary<string, List<IUserAnswer>>();

        private IQuestionsProvider questionsProvider;

        /// <summary>
        /// Creates an DatabankCommunication with a QuestionProvider
        /// </summary>
        public DatabankCommunication(IQuestionsProvider provider)
        {
            questionsProvider = provider;
            CreateQuestionsFromTxt();
            LoadAllAnswersFromTxt();
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
        public void CreateQuestionsFromTxt()
        {
            Questions["ImageChecker"] = LoadQuestionsForImageCheckerFromTXT();
            Questions["DoubleSlider"] = LoadQuestionsForDoubleSliderFromTXT();
            Questions["Stadium"] = LoadQuestionsForStadiumFromTXT();
            Questions["Introspection"] = LoadQuestionsForIntrospectionFromTXT();
        }
        /// <summary>
        /// Loads all Answers from the txt files
        /// </summary>
        public void LoadAllAnswersFromTxt()
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
        public void SaveAllAnswersInTxt()
        {
            SaveAnswersForImageCheckerPage();
            SaveAnswersForDoubleSliderPage();
            SaveAnswersForIntrospectionPage();
            SaveAnswersForStadiumPage();
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
            // create needed lists for the questions
            var stadiums1 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Blattentwicklung", "stadium_blattentwicklung.png"),
                new StadiumSubItem("Bestockung", "stadium_bestockung.png"),
                new StadiumSubItem("Längenwachstum/Schossen", "stadium_laengenwachstumschossen.png")
            };
            var plants1 = new List<Plant>
            {
                new Plant("Kartoffel"),
                new Plant("Mais"),
                new Plant("Weizen"),
                new Plant("Zuckerrübe")
            };
            var stadiums2 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Blattentwicklung", "stadium_blattentwicklung.png"),
                new StadiumSubItem("Schossen/Haupttrieb", "stadium_laengenwachstumschossen.png"),
                new StadiumSubItem("Ähren/-Rispenschwellen", "stadium_ahrenrispenschwellen.png"),
                new StadiumSubItem("Entwicklung der Blütenanlage", "stadium_entwicklungbluetenanlage.png"),
                new StadiumSubItem("Blüte", "bluete.png")
            };
            var plants2 = new List<Plant>
            {
                new Plant("Gerste"),
                new Plant("Kartoffel"),
                new Plant("Raps"),
                new Plant("Weizen")
            };
            var stadiums3 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Längenwachstum/Schossen", "stadium_laengenwachstumschossen.png"),
                new StadiumSubItem("Entwicklung vegetativer Pflanzenteile/Rübenkörper", "stadium_entwicklungvegpflanzruebenk.png"),
                new StadiumSubItem("Entwicklung der Blütenanlage", "stadium_entwicklungbluetenanlage.png"),
                new StadiumSubItem("Fruchtentwicklung", "bluete.png")
            };
            var plants3 = new List<Plant>
            {
                new Plant("Kartoffel"),
                new Plant("Mais"),
                new Plant("Raps"),
                new Plant("Weizen"),
                new Plant("Zuckerrübe")
            };
            return new List<IQuestionContent>
            {
                //TODO: Add Support for multiple correkt answers (as seen in original survey 1 stadium answer 3)
                new QuestionStadiumPage(1, 1, "Stadium_one_question1.png", stadiums1.ToList(), plants1.ToList(), "Blattentwicklung", "Kartoffel"),
                new QuestionStadiumPage(2, 1, "Stadium_one_question2.png", stadiums1.ToList(), plants1.ToList(), "Blattentwicklung", "Zuckerrübe"),
                new QuestionStadiumPage(3, 1, "Stadium_one_question3.png", stadiums1.ToList(), plants1.ToList(), "Blattentwicklung", "Mais"),
                new QuestionStadiumPage(4, 1, "Stadium_one_question4.png", stadiums1.ToList(), plants1.ToList(), "Blattentwicklung", "Kartoffel"),
                new QuestionStadiumPage(5, 1, "Stadium_one_question5.png", stadiums1.ToList(), plants1.ToList(), "Längenwachstum/Schossen", "Weizen"),
                new QuestionStadiumPage(6, 1, "Stadium_one_question6.png", stadiums1.ToList(), plants1.ToList(), "Bestockung", "Weizen"),
                new QuestionStadiumPage(7, 2, "Stadium_two_question1.png", stadiums2.ToList(), plants2.ToList(), "Blüte", "Kartoffel"),
                new QuestionStadiumPage(8, 2, "Stadium_two_question2.png", stadiums2.ToList(), plants2.ToList(), "Entwicklung der Blütenanlage", "Raps"),
                new QuestionStadiumPage(9, 2, "Stadium_two_question3.png", stadiums2.ToList(), plants2.ToList(), "Blüte", "Raps"),
                new QuestionStadiumPage(10, 2, "Stadium_two_question4.png", stadiums2.ToList(), plants2.ToList(), "Blattentwicklung", "Kartoffel"),
                new QuestionStadiumPage(11, 2, "Stadium_two_question5.png", stadiums2.ToList(), plants2.ToList(), "Schossen/Haupttrieb", "Raps"),
                new QuestionStadiumPage(12, 2, "Stadium_two_question6.png", stadiums2.ToList(), plants2.ToList(), "Ähren/-Rispenschwellen", "Gerste"),
                new QuestionStadiumPage(13, 3, "Stadium_three_question1.png", stadiums3.ToList(), plants3.ToList(), "Entwicklung vegetativer Pflanzenteile/Rübenkörper", "Zuckerrübe"),
                new QuestionStadiumPage(14, 3, "Stadium_three_question2.png", stadiums3.ToList(), plants3.ToList(), "Fruchtentwicklung", "Weizen"),
                new QuestionStadiumPage(15, 3, "Stadium_three_question3.png", stadiums3.ToList(), plants3.ToList(), "Entwicklung der Blütenanlage", "Mais"),
                new QuestionStadiumPage(16, 3, "Stadium_three_question4.png", stadiums3.ToList(), plants3.ToList(), "Entwicklung der Blütenanlage", "Kartoffel"),
                new QuestionStadiumPage(17, 3, "Stadium_three_question5.png", stadiums3.ToList(), plants3.ToList(), "Fruchtentwicklung", "Raps"),
                new QuestionStadiumPage(18, 3, "Stadium_three_question6.png", stadiums3.ToList(), plants3.ToList(), "Längenwachstum/Schossen", "Weizen")
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
        public static IQuestionContent LoadQuestion(string surveyId, int difficulty)
        {
            var listQuestions = Questions[surveyId];
            List<IQuestionContent> listQuestion = new List<IQuestionContent>();
            for (int i = 0; i < listQuestions.Count; i++)
            {
                IQuestionContent question = listQuestions[i];
                if (question.Difficulty == difficulty) // check if right difficulty
                {
                    if (!SearchAnswers(surveyId, question.InternId)) // check if not already answered
                    {
                        listQuestion.Add(question);
                    }
                }
            }
            if (listQuestion.Count > 0)
            {
                return listQuestion[RandomNumber.Next(listQuestion.Count)]; // return random question
            }
            if (difficulty == 1)
            {
                return null; // no more question available
            }
            else
            {
                return LoadQuestion(surveyId, difficulty - 1); // look for question in lower difficulty
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
        /// Creates a set of example Answers for testing purposes
        /// Overrides old answer list
        /// </summary>
        public void ExampleAnswers()
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
                new AnswerStadiumPage(1,"Blattentwicklung","Kartoffel"),
                new AnswerStadiumPage(2,"Blattentwicklung","Zuckerrübe")
            };
        }

        /// <summary>
        /// Loads all questions for the ImageCheckerType from ImageCheckerQuestions.txt
        /// Returns a List containing these Questions
        /// </summary>
        public List<IQuestionContent> LoadQuestionsForImageCheckerFromTXT()
        {
            List<IQuestionContent> tempList = new List<IQuestionContent>();

            // Read text from the txt file
            String Text = questionsProvider.LoadTextFromTXT("ImageCheckerQuestions.txt");
            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();

            
            while (!(Line.Equals("END_QUESTIONS"))) // for each line
            {
                // split line by ';'
                String[] attributes = Line.Split(';');

                // attributes should now contain all the needed information, but only in Strings
                // convert information in right data type
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

                // create new Qobject and add to the list
                QuestionImageCheckerPage question = new QuestionImageCheckerPage(id, questionText, difficulty, img1Corr, img2Corr, img3Corr, img4Corr, img1Sour, img2Sour, img3Sour, img4Sour);
                tempList.Add(question);

                Line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Saves the answers from ListAnswerImageCheckerPage in ImageCheckerAnswers.txt
        /// every answer is saved as an own line in the txt: int id;int img1Selected;int img2Selected;int img3Selected;int img4Selected
        /// The answers end with "END_ANSWERS"
        /// This will override the old file
        /// </summary>
        public void SaveAnswersForImageCheckerPage()
        {
            // save the answers as seperate lines
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

            // get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "ImageCheckerAnswers.txt");

            // write text in file
            File.WriteAllText(filename, text);
        }

        /// <summary>
        /// Loads all Answers for the AnswerImageCheckerPage from ImageCheckerAnswer.txt
        /// </summary>
        /// <returns>List with all Answers</returns>
        public List<IUserAnswer> LoadAnswersForImageCheckerPage()
        {
            List<IUserAnswer> tempList = new List<IUserAnswer>();

            // get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "ImageCheckerAnswers.txt");

            // read the  file
            string content = File.ReadAllText(filename);
            StringReader stringReader = new StringReader(content);
            string line = stringReader.ReadLine();
            while (!line.Equals("END_ANSWERS"))
            {
                // decode each line
                String[] attributes = line.Split(';');
                int id = Convert.ToInt32(attributes[0]);
                int img1Sel = Convert.ToInt32(attributes[1]);
                int img2Sel = Convert.ToInt32(attributes[2]);
                int img3Sel = Convert.ToInt32(attributes[3]);
                int img4Sel = Convert.ToInt32(attributes[4]);
                // create new object and add it to the list
                AnswerImageCheckerPage answer = new AnswerImageCheckerPage(id, img1Sel, img2Sel, img3Sel, img4Sel);
                tempList.Add(answer);

                line = stringReader.ReadLine();
            }
            return tempList;

        }

        /// <summary>
        /// Loads all questions for the DoubleSliderType from DoubleSliderQuestions.txt
        /// </summary>
        public List<IQuestionContent> LoadQuestionsForDoubleSliderFromTXT()
        {
            List<IQuestionContent> tempList = new List<IQuestionContent>();

            // get text from txt file
            String Text = questionsProvider.LoadTextFromTXT("DoubleSliderQuestions.txt");
            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();
            while (!(Line.Equals("END_QUESTIONS")))
            {
                // decode each line
                String[] attributes = Line.Split(';');

                int id = Convert.ToInt32(attributes[0]);
                int difficulty = Convert.ToInt32(attributes[1]);
                string imgSour = attributes[2];
                int ans1 = Convert.ToInt32(attributes[3]);
                int ans2 = Convert.ToInt32(attributes[4]);

                // create new object and add it to the list
                QuestionDoubleSliderPage question = new QuestionDoubleSliderPage(id, difficulty, imgSour, ans1, ans2);
                tempList.Add(question);

                // read new line
                Line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Saves the answers from ListAnswerDoubleSliderPage in DoubleSliderAnswers.txt
        /// every answer is saved as an own line in the txt: int id;int ResultQuestionA;int ResultQuestionB
        /// The answers end with "END_ANSWERS"
        /// This will override the old file
        /// </summary>
        public void SaveAnswersForDoubleSliderPage()
        {
            // save each answer as a line
            string text = "";
            foreach (AnswerDoubleSliderPage answer in Answers["DoubleSlider"])
            {
                text += answer.InternId.ToString() + ";";
                text += answer.ResultQuestionA.ToString() + ";";
                text += answer.ResultQuestionB.ToString() + "\n";
            }
            text += "END_ANSWERS";

            // get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "DoubleSliderAnswers.txt");

            // write text in file
            File.WriteAllText(filename, text);
        }

        /// <summary>
        /// Loads all Answers for the AnswerDoubleSliderPage from DoubleSliderAnswer.txt
        /// </summary>
        /// <returns>List with all Answers</returns>
        public List<IUserAnswer> LoadAnswersForDoubleSliderPage()
        {
            List<IUserAnswer> tempList = new List<IUserAnswer>();

            // get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "DoubleSliderAnswers.txt");

            // get text
            string content = File.ReadAllText(filename);
            StringReader stringReader = new StringReader(content);
            string line = stringReader.ReadLine();
            while (!line.Equals("END_ANSWERS"))
            {
                // decode each line
                String[] attributes = line.Split(';');
                int id = Convert.ToInt32(attributes[0]);
                int resA = Convert.ToInt32(attributes[1]);
                int resB = Convert.ToInt32(attributes[2]);

                // craete object and add it to the list
                AnswerDoubleSliderPage answer = new AnswerDoubleSliderPage(id, resA, resB);
                tempList.Add(answer);

                line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Loads all questions for the StadiumType from StadiumQuestions.txt
        /// </summary>
        public List<IQuestionContent> LoadQuestionsForStadiumFromTXT()
        {
            // read text from txt file
            String Text = questionsProvider.LoadTextFromTXT("StadiumQuestions.txt");
            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();

            // create needed lists for the questions
            var stadiums1 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Blattentwicklung", "stadium_blattentwicklung.png"),
                new StadiumSubItem("Bestockung", "stadium_bestockung.png"),
                new StadiumSubItem("Längenwachstum/Schossen", "stadium_laengenwachstumschossen.png")
            };
            var plants1 = new List<Plant>
            {
                new Plant("Kartoffel"),
                new Plant("Mais"),
                new Plant("Weizen"),
                new Plant("Zuckerrübe")
            };
            var stadiums2 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Blattentwicklung", "stadium_blattentwicklung.png"),
                new StadiumSubItem("Schossen/Haupttrieb", "stadium_laengenwachstumschossen.png"),
                new StadiumSubItem("Ähren/-Rispenschwellen", "stadium_ahrenrispenschwellen.png"),
                new StadiumSubItem("Entwicklung der Blütenanlage", "stadium_entwicklungbluetenanlage.png"),
                new StadiumSubItem("Blüte", "bluete.png")
            };
            var plants2 = new List<Plant>
            {
                new Plant("Gerste"),
                new Plant("Kartoffel"),
                new Plant("Raps"),
                new Plant("Weizen")
            };
            var stadiums3 = new List<StadiumSubItem>
            {
                new StadiumSubItem("Längenwachstum/Schossen", "stadium_laengenwachstumschossen.png"),
                new StadiumSubItem("Entwicklung vegetativer Pflanzenteile/Rübenkörper", "stadium_entwicklungvegpflanzruebenk.png"),
                new StadiumSubItem("Entwicklung der Blütenanlage", "stadium_entwicklungbluetenanlage.png"),
                new StadiumSubItem("Fruchtentwicklung", "bluete.png")
            };
            var plants3 = new List<Plant>
            {
                new Plant("Kartoffel"),
                new Plant("Mais"),
                new Plant("Raps"),
                new Plant("Weizen"),
                new Plant("Zuckerrübe")
            };
            var tempList = new List<IQuestionContent>();
            while (!(Line.Equals("END_QUESTIONS")))
            {
                // decode each line
                String[] attributes = Line.Split(';');

                int id = Convert.ToInt32(attributes[0]);
                int difficulty = Convert.ToInt32(attributes[1]);
                string imgSour = attributes[2];
                string stadiumList = attributes[3];
                string plantList = attributes[4];
                string stadiumCorr = attributes[5];
                string plantCorr = attributes[6];

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

                // create new object and add it to the list
                QuestionStadiumPage question = new QuestionStadiumPage(id, difficulty, imgSour, stadium, plant, stadiumCorr, plantCorr);
                tempList.Add(question);

                Line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Saves the answers from ListAnswerStadiumPage in StadiumAnswers.txt
        /// every answer is saved as an own line in the txt: int id;String AnswerFruitType;String AnswerStadium
        /// The answers end with "END_ANSWERS"
        /// This will override the old file
        /// </summary>
        public void SaveAnswersForStadiumPage()
        {
            // save each answer an an own line
            string text = "";
            foreach (AnswerStadiumPage answer in Answers["Stadium"])
            {
                text += answer.InternId.ToString() + ";";
                text += answer.AnswerFruitType + ";";
                text += answer.AnswerStadium + "\n";
            }
            text += "END_ANSWERS";

            // get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "StadiumAnswers.txt");

            // write text in file
            File.WriteAllText(filename, text);
        }

        /// <summary>
        /// Loads all Answers for the AnswerStadiumPage from StadiumAnswer.txt
        /// </summary>
        /// <returns>List with all Answers</returns>
        public List<IUserAnswer> LoadAnswersForStadiumPage()
        {
            List<IUserAnswer> tempList = new List<IUserAnswer>();

            // get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "StadiumAnswers.txt");

            // get text from txt file
            string content = File.ReadAllText(filename);
            StringReader stringReader = new StringReader(content);
            string line = stringReader.ReadLine();
            while (!line.Equals("END_ANSWERS"))
            {
                // decode each line
                String[] attributes = line.Split(';');
                int id = Convert.ToInt32(attributes[0]);
                String answerFruitType = attributes[1];
                String anserStadium = attributes[2];

                // create new Object and add it to the list
                AnswerStadiumPage answer = new AnswerStadiumPage(id, answerFruitType, anserStadium);
                tempList.Add(answer);

                line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Loads all questions for the IntrospectionType from IntrospectionQuestions.txt
        /// </summary>
        public List<IQuestionContent> LoadQuestionsForIntrospectionFromTXT()
        {
            // get text
            String Text = questionsProvider.LoadTextFromTXT("IntrospectionQuestions.txt");
            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();
            var tempList = new List<IQuestionContent>();
            while (!(Line.Equals("END_QUESTIONS")))
            {
                // decode each line
                String[] attributes = Line.Split(';');

                int id = Convert.ToInt32(attributes[0]);
                string questionText = attributes[1];

                // create new object and add it to the list
                QuestionIntrospectionPage question = new QuestionIntrospectionPage(id, questionText);
                tempList.Add(question);

                Line = stringReader.ReadLine();
            }
            return tempList;
        }

        /// <summary>
        /// Saves the answers from ListAnswerIntrospectionPage in IntrospectionAnswers.txt
        /// every answer is saved as an own line in the txt: int id;int SelectedAnswer
        /// The answers end with "END_ANSWERS"
        /// This will override the old file
        /// </summary>
        public void SaveAnswersForIntrospectionPage()
        {
            // save each answer as own line
            string text = "";
            foreach (AnswerIntrospectionPage answer in Answers["Introspection"])
            {
                text += answer.InternId.ToString() + ";";
                text += answer.SelectedAnswer.ToString() + "\n";
            }
            text += "END_ANSWERS";

            // get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "IntrospectionAnswers.txt");

            // write text in txt file
            File.WriteAllText(filename, text);
        }

        /// <summary>
        /// Loads all Answers for the AnswerIntrospectionPage from IntrospectionAnswer.txt
        /// </summary>
        /// <returns>List with all Answers</returns>
        public List<IUserAnswer> LoadAnswersForIntrospectionPage()
        {
            List<IUserAnswer> tempList = new List<IUserAnswer>();

            // get filepath
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(path, "IntrospectionAnswers.txt");

            // get text from txt
            string content = File.ReadAllText(filename);
            StringReader stringReader = new StringReader(content);
            string line = stringReader.ReadLine();
            while (!line.Equals("END_ANSWERS"))
            {
                // decode each line
                String[] attributes = line.Split(';');
                int id = Convert.ToInt32(attributes[0]);
                int selectedAnswer = Convert.ToInt32(attributes[1]);

                // create new object and add it to the List
                AnswerIntrospectionPage answer = new AnswerIntrospectionPage(id, selectedAnswer);
                tempList.Add(answer);

                line = stringReader.ReadLine();
            }
            return tempList;
        }
    }
}