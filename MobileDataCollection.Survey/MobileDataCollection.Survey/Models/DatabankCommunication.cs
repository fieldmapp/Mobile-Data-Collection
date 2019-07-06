using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MobileDataCollection.Survey.Models
{
    public class DatabankCommunication
    {
        static Random RandomNumber = new Random();

        private static List<QuestionImageCheckerPage> ListQuestionImageCheckerPage = new List<QuestionImageCheckerPage>();
        private static List<QuestionDoubleSliderPage> ListQuestionDoubleSliderPage = new List<QuestionDoubleSliderPage>();
        private static List<QuestionStadiumPage> ListQuestionStadiumPage = new List<QuestionStadiumPage>();
        private static List<QuestionIntrospectionPage> ListQuestionIntrospectionPage = new List<QuestionIntrospectionPage>();

        private static List<AnswerImageCheckerPage> ListAnswerImageCheckerPage = new List<AnswerImageCheckerPage>();
        private static List<AnswerDoubleSliderPage> ListAnswerDoubleSliderPage = new List<AnswerDoubleSliderPage>();
        private static List<AnswerStadiumPage> ListAnswerStadiumPage = new List<AnswerStadiumPage>();
        private static List<AnswerIntrospectionPage> ListAnswerIntrospectionPage = new List<AnswerIntrospectionPage>();

        private IQuestionsProvider questionsProvider;

        /// <summary>
        /// Creates an DatabankCommunication with a QuestionProvider
        /// </summary>
        public DatabankCommunication(IQuestionsProvider provider)
        {
            questionsProvider = provider;

            CreateQuestions();
            //CreateQuestionsFromTXT();
        }



        /// <summary>
        /// Creates all Questions
        /// </summary>
        public void CreateQuestions()
        {
            CreateQuestionsForImageChecker();
            CreateQuestionsForDoubleSlider();
            CreateQuestionsForStadium();
            CreateQuestionForIntrospection();
        }

        /// <summary>
        /// Creates all Questions, buts loads these from the txt files
        /// </summary>
        public void CreateQuestionsFromTXT()
        {
            LoadQuestionsForImageCheckerFromTXT();
            LoadQuestionsForDoubleSliderFromTXT();
            LoadQuestionsForStadiumFromTXT();
            LoadQuestionsForIntrospectionFromTXT();
        }

        /// <summary>
        /// Return a List containing all available Questions
        /// </summary>
        public static List<IQuestionContent> GetAllQuestions()
        {
            return ListQuestionDoubleSliderPage.Cast<IQuestionContent>()
                .Concat(ListQuestionImageCheckerPage.Cast<IQuestionContent>())
                .Concat(ListQuestionStadiumPage.Cast<IQuestionContent>()).ToList();
        }

        /// <summary>
        /// Loads all questions for the ImageCheckerType from ImageCheckerQuestions.txt
        /// </summary>
        public void LoadQuestionsForImageCheckerFromTXT()
        {
            String Text = questionsProvider.LoadQuestionsFromTXT("ImageCheckerQuestions.txt");

            /*
            String Filename = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "test.txt");
            File.WriteAllText(Filename, "abc");
            var assembly = typeof(Object).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("AboutResources.txt");
            Stream stream = typeof(DatabankCommunication).Assembly.GetManifestResourceStream("Assets.ImageCheckerQuestions.txt");
            Text = stream.ToString();
            */


            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();
            char[] splitCharacters = new char[';'];

            while(!(Line.Equals("END_Questions")))
            {
                String[] attributes = Line.Split(splitCharacters);

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

                QuestionImageCheckerPage question = new QuestionImageCheckerPage(id, questionText, difficulty, img1Corr, img2Corr, img3Corr, img4Corr, img1Sour, img2Sour, img3Sour, img4Sour);
                ListQuestionImageCheckerPage.Add(question);

                Line = stringReader.ReadLine();
            }
        }

        /// <summary>
        /// Loads all questions for the DoubleSliderType from DoubleSliderQuestions.txt
        /// </summary>
        public static void LoadQuestionsForDoubleSliderFromTXT()
        {
            String Text = "";
            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();
            char[] splitCharacters = new char[';'];

            while (!(Line.Equals("END_QUESTIONS")))
            {
                String[] attributes = Line.Split(splitCharacters);

                int id = Convert.ToInt32(attributes[0]);
                int difficulty = Convert.ToInt32(attributes[1]);
                string imgSour = attributes[2];
                int ans1 = Convert.ToInt32(attributes[3]);
                int ans2 = Convert.ToInt32(attributes[4]);


                QuestionDoubleSliderPage question = new QuestionDoubleSliderPage(id, difficulty, imgSour, ans1, ans2);
                ListQuestionDoubleSliderPage.Add(question);

                Line = stringReader.ReadLine();
            }
        }

        /// <summary>
        /// Loads all questions for the StadiumType from StadiumQuestions.txt
        /// </summary>
        public static void LoadQuestionsForStadiumFromTXT()
        {
            String Text = "";
            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();
            char[] splitCharacters = new char[';'];

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

            while (!(Line.Equals("END_Questions")))
            {
                String[] attributes = Line.Split(splitCharacters);

                int id = Convert.ToInt32(attributes[0]);
                int difficulty = Convert.ToInt32(attributes[1]);
                string imgSour = attributes[2];
                string stadiumList = attributes[3];
                string plantList = attributes[4];
                string stadiumCorr = attributes[5];
                string plantCorr = attributes[6];

                var stadium = stadiums1;
                var plant = plants1;

                if(stadiumList.Equals("stadiums2"))
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

                QuestionStadiumPage question = new QuestionStadiumPage(id, difficulty, imgSour, stadium, plant, stadiumCorr, plantCorr);
                ListQuestionStadiumPage.Add(question);

                Line = stringReader.ReadLine();
            }
        }

        /// <summary>
        /// Loads all questions for the IntrospectionType from IntrospectionQuestions.txt
        /// </summary>
        public static void LoadQuestionsForIntrospectionFromTXT()
        {
            //TODO: change this method to work with right Questions, work on txt
            String Text = "";
            StringReader stringReader = new StringReader(Text);
            String Line = stringReader.ReadLine();
            char[] splitCharacters = new char[';'];

            while (!(Line.Equals("END_Questions")))
            {
                String[] attributes = Line.Split(splitCharacters);

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

                QuestionImageCheckerPage question = new QuestionImageCheckerPage(id, questionText, difficulty, img1Corr, img2Corr, img3Corr, img4Corr, img1Sour, img2Sour, img3Sour, img4Sour);
                ListQuestionImageCheckerPage.Add(question);

                Line = stringReader.ReadLine();
            }
        }

        /// <summary>
        /// Creates all questions for the ImageCheckerType
        /// </summary>
        public static void CreateQuestionsForImageChecker()
        {
            QuestionImageCheckerPage question = new QuestionImageCheckerPage(1, "Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 0, 1, 0, "ImageChecker_one_question1_picture1.png", "ImageChecker_one_question1_picture2.png", "ImageChecker_one_question1_picture3.png", "ImageChecker_one_question1_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(2, "Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 1, 0, 1, 1, 0, "ImageChecker_one_question2_picture1.png", "ImageChecker_one_question2_picture2.png", "ImageChecker_one_question2_picture3.png", "ImageChecker_one_question2_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(3, "Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 2, 1, 1, 1, 0, "ImageChecker_two_question3_picture1.png", "ImageChecker_two_question3_picture2.png", "ImageChecker_two_question3_picture3.png", "ImageChecker_two_question3_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(4, "Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 2, 0, 0, 1, 0, "ImageChecker_two_question4_picture1.png", "ImageChecker_two_question4_picture2.png", "ImageChecker_two_question4_picture3.png", "ImageChecker_two_question4_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(5, "Wo sehen sie die Feldfruchtsorte Kartoffel abgebildet?", 3, 1, 0, 1, 0, "ImageChecker_three_question5_picture1.png", "ImageChecker_three_question5_picture2.png", "ImageChecker_three_question5_picture3.png", "ImageChecker_three_question5_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(6, "Wo sehen sie die Feldfruchtsorte Gerste abgebildet?", 3, 0, 0, 1, 0, "ImageChecker_three_question6_picture1.png", "ImageChecker_three_question6_picture2.png", "ImageChecker_three_question6_picture3.png", "ImageChecker_three_question6_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
        }

        /// <summary>
        /// Creates all questions for the DoubleSlider Type
        /// </summary>
        public static void CreateQuestionsForDoubleSlider()
        {
            QuestionDoubleSliderPage question = new QuestionDoubleSliderPage(1, 1, "DoubleSlider_one_question1.png", 7, 4);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(2, 1, "DoubleSlider_one_question2.png", 49, 91);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(3, 1, "DoubleSlider_one_question3.png", 12, 3);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(4, 1, "DoubleSlider_one_question4.png", 64, 94);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(5, 2, "DoubleSlider_two_question1.png", 7, 96);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(6, 2, "DoubleSlider_two_question2.png", 49, 91);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(7, 2, "DoubleSlider_two_question3.png", 12, 3);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(8, 2, "DoubleSlider_two_question4.png", 64, 94);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(9, 3, "DoubleSlider_three_question1.png", 42, 87);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(10, 3, "DoubleSlider_three_question2.png", 6, 0);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(11, 3, "DoubleSlider_three_question3.png", 80, 5);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(12, 3, "DoubleSlider_three_question4.png", 76, 99);
            ListQuestionDoubleSliderPage.Add(question);
        }

        /// <summary>
        /// Creates all questions for the StadiumType
        /// </summary>
        public static void CreateQuestionsForStadium()
        {
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
            ListQuestionStadiumPage = new List<QuestionStadiumPage>
            {
                new QuestionStadiumPage(1, 1, "Stadium_one_question1.png", stadiums1.ToList(), plants1.ToList(), "Blattentwicklung", "Kartoffel"),
                new QuestionStadiumPage(2, 1, "Stadium_one_question2.png", stadiums1.ToList(), plants1.ToList(), "Blattentwicklung", "Zuckerrübe"),
                //TODO: Add Support for multiple correkt answers (as seen in original survey 1 stadium answer 3)
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
        /// Creates all questions for the IntrospectionType
        /// </summary>
        public static void CreateQuestionForIntrospection()
        {
            QuestionIntrospectionPage question = new QuestionIntrospectionPage(1,"Ich kann die Sorte von Feldfrüchten zuverlässig erkennen");
            ListQuestionIntrospectionPage.Add(question);
            question = new QuestionIntrospectionPage(2,"Ich kann phänologische Entwicklungsstadien von Feldfrüchten zuverlässig erkennen");
            ListQuestionIntrospectionPage.Add(question);
            question = new QuestionIntrospectionPage(3,"Ich kann den Bedeckungsgrad des Bodens durch Pflanzen zuverlässig schätzen");
            ListQuestionIntrospectionPage.Add(question);
            question = new QuestionIntrospectionPage(4,"Ich kann den Anteil grüner Pflanzenbestandteile am gesamten Pflanzenmaterial zuverlässig schätzen");
            ListQuestionIntrospectionPage.Add(question);
        }

        /// <summary>
        /// Loads a QUestionImageCheckerPage-Object with the set difficulty, or a lower difficulty if no question with a matching difficulty exsist.
        /// If no question can be loaded it will return an Object, with the ID 0
        /// </summary>
        public static QuestionImageCheckerPage LoadQuestionImageCheckerPage(int difficulty)
        {
            List<QuestionImageCheckerPage> ListQuestion = new List<QuestionImageCheckerPage>();
            for(int i = 0; i < ListQuestionImageCheckerPage.Count; i++)
            {
                QuestionImageCheckerPage question = ListQuestionImageCheckerPage.ElementAt<QuestionImageCheckerPage>(i);
                if(question.Difficulty == difficulty)
                {
                    if(!(SearchListAnswerImageCheckerPage(question.InternId)))
                    {
                        ListQuestion.Add(question);
                    }
                }
            }
            if(ListQuestion.Count > 0)
            {
                return ListQuestion.ElementAt<QuestionImageCheckerPage>(RandomNumber.Next(ListQuestion.Count));
            }
            if (difficulty == 1)
            {
                return new QuestionImageCheckerPage(0,"", 0, 0, 0, 0, 0, "", "", "", "");
            }
            else
            {
                return LoadQuestionImageCheckerPage(difficulty - 1);
            }
        }

        /// <summary>
        /// Searches an AnswerImageCheckerPage-Object with the corrosponding Id
        /// </summary>
        public static bool SearchListAnswerImageCheckerPage(int Id) => ListAnswerImageCheckerPage.Any(a => a.InternId == Id);

        /// <summary>
        /// Adds an AnswerImageCheckerPage Object to the List ListAnswerImageCheckerPage
        /// </summary>
        public static void AddListAnswerImageCheckerPage(AnswerImageCheckerPage answer)
        {
            ListAnswerImageCheckerPage.Add(answer);
        }

        /// <summary>
        /// Loads a QUestionDoubleSliderPage-Object with the set difficulty, or a lower difficulty if no question with a matching difficulty exsist.
        /// If no question can be loaded it will return an Object, with the ID 0
        /// </summary>
        public static QuestionDoubleSliderPage LoadQuestionDoubleSliderPage(int difficulty)
        {
            List<QuestionDoubleSliderPage> ListQuestion = new List<QuestionDoubleSliderPage>();
            for (int i = 0; i < ListQuestionDoubleSliderPage.Count; i++)
            {
                QuestionDoubleSliderPage question = ListQuestionDoubleSliderPage.ElementAt<QuestionDoubleSliderPage>(i);
                if (question.Difficulty == difficulty)
                {
                    if (!(SearchListAnswerDoubleSliderPage(question.InternId)))
                    {
                        ListQuestion.Add(question);
                    }
                }
            }
            if (ListQuestion.Count > 0)
            {
                return ListQuestion.ElementAt<QuestionDoubleSliderPage>(RandomNumber.Next(ListQuestion.Count));
            }
            if (difficulty == 1)
            {
                return new QuestionDoubleSliderPage(0, 0,"", 0, 0);
            }
            else
            {
                return LoadQuestionDoubleSliderPage(difficulty - 1);
            }
        }

        /// <summary>
        /// Searches an AnswerDoubleSliderPage-Object with the corrosponding Id
        /// </summary>
        public static bool SearchListAnswerDoubleSliderPage(int Id) => ListAnswerDoubleSliderPage.Any(a => a.InternId == Id);

        /// <summary>
        /// Adds an AnswerDoubleSliderPage-Object to the List ListAnswerDoubleSliderPage
        /// </summary>
        public static void AddListAnswerDoubleSliderPage(AnswerDoubleSliderPage answer)
        {
            ListAnswerDoubleSliderPage.Add(answer);
        }

        /// <summary>
        /// Loads a QUestionStadiumPage-Object with the set difficulty, or a lower difficulty if no question with a matching difficulty exsist.
        /// If no question can be loaded it will return object with id 0
        /// </summary>
        public static QuestionStadiumPage LoadQuestionStadiumPage(int difficulty)
        {
            List<QuestionStadiumPage> ListQuestion = new List<QuestionStadiumPage>();
            for (int i = 0; i < ListQuestionStadiumPage.Count; i++)
            {
                QuestionStadiumPage question = ListQuestionStadiumPage.ElementAt<QuestionStadiumPage>(i);
                if (question.Difficulty == difficulty)
                {
                    if (!SearchListAnswerDoubleSliderPage(question.InternId))
                    {
                        ListQuestion.Add(question);
                    }
                }
            }
            if (ListQuestion.Count > 0)
            {
                return ListQuestion.ElementAt<QuestionStadiumPage>(RandomNumber.Next(ListQuestion.Count));
            }
            if (difficulty == 1)
            {
                return new QuestionStadiumPage(0, 0, null, null, null, "", correctAnswerFruitType: "");
            }
            else
            {
                return LoadQuestionStadiumPage(difficulty - 1);
            }
        }

        /// <summary>
        /// Searches an AnswerStadiumPage-Object with the corrosponding Id
        /// </summary>
        public static bool SearchListAnswerStadiumPage(int Id) => ListAnswerStadiumPage.Any(a => a.InternId == Id);

        /// <summary>
        /// Adds an AnswerStadiumPage-Object to the List ListAnswerDoubleSliderPage
        /// </summary>
        public static void AddListAnswerStadiumPage(AnswerStadiumPage answer)
        {
            ListAnswerStadiumPage.Add(answer);
        }

        /// <summary>
        /// Loads a QuestionIntrospectionPage-Object with the set Id
        /// </summary>
        public static QuestionIntrospectionPage LoadQuestionIntrospectionPage(int id)
        {
            return ListQuestionIntrospectionPage.FirstOrDefault(q => q.InternId == id) ?? new QuestionIntrospectionPage(0, "");
        }

        /// <summary>
        /// Loads a QuestionDoubleSliderPage with the set id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>QustionDoubleSliderPage with wanted id if existent. Otherwise null</returns>
        public static QuestionDoubleSliderPage LoadQuestionDoubleSliderPageById(int id)
        {
            return ListQuestionDoubleSliderPage.FirstOrDefault(q => q.InternId == id);
        }

        /// <summary>
        /// Loads a QuestionImageCheckerPage with the set id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>QuestionImageCheckerPage with wanted id if existent. Otherwise null</returns>
        public static QuestionImageCheckerPage LoadQuestionImageCheckerPageById(int id)
        {
            return ListQuestionImageCheckerPage.FirstOrDefault(q => q.InternId == id);
        }

        /// <summary>
        /// Loads a QuestionStadiumPage with the set id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>QuestionStadiumPage with wanted id if existent. Otherwise null</returns>
        public static QuestionStadiumPage LoadQuestionStadiumPageById(int id)
        {
            return ListQuestionStadiumPage.FirstOrDefault(q => q.InternId == id);
        }

        /// <summary>
        /// Searches an AnswerStadiumPage-Object with the corrosponding Id
        /// </summary>
        public static bool SearchListAnswerIntrospectionPage(int Id) => ListAnswerIntrospectionPage.Any(q => q.InternId == Id);

        /// <summary>
        /// Adds an AnswerStadiumPage-Object to the List ListAnswerDoubleSliderPage
        /// </summary>
        public static void AddListAnswerIntrospectionPage(AnswerIntrospectionPage answer)
        {
            ListAnswerIntrospectionPage.Add(answer);
        }

        /*
        public QuestionDoubleSliderPage LoadQuestionDoubleSlider(int difficulty)
        {
            string query = String.Format("SELECT * FROM QuestionDoubleSliderPage LEFT OUTER JOIN AnswerDoubleSliderPage ON QuestionDoubleSliderPage.InternId = AnswerDoubleSliderPage.InternId WHERE Difficulty = {0}", difficulty);
            IEnumerable<QuestionDoubleSliderPage> foo = conn.Query<QuestionDoubleSliderPage>(query);
            List<QuestionDoubleSliderPage> listOfResults = foo.ToList<QuestionDoubleSliderPage>();
            RandomNumber = new Random();
            QuestionDoubleSliderPage temp = listOfResults.ElementAt(RandomNumber.Next(0, listOfResults.Count));
            return temp;
        }

        public QuestionIntrospectionPage LoadQuestionIntrospection(int difficulty)
        {
            string query = String.Format("SELECT * FROM QuestionIntrospectionPage LEFT OUTER JOIN AnswerIntrospectionPage ON QuestionDoubleSliderPage.InternId = AnswerIntrospectionPage.InternId WHERE Difficulty = {0}", difficulty);
            IEnumerable<QuestionIntrospectionPage> foo = conn.Query<QuestionIntrospectionPage>(query);
            List<QuestionIntrospectionPage> listOfResults = foo.ToList<QuestionIntrospectionPage>();
            RandomNumber = new Random();
            QuestionIntrospectionPage temp = listOfResults.ElementAt(RandomNumber.Next(0, listOfResults.Count));
            return temp;
        }

        public QuestionStadiumPage LoadQuestionStadium(int difficulty)
        {
            string query = String.Format("SELECT * FROM QuestionStadiumPage LEFT OUTER JOIN AnswerStadiumPage ON QuestionStadiumPage.InternId = AnswerStadiumPage.InternId WHERE Difficulty = {0}", difficulty);
            IEnumerable<QuestionStadiumPage> foo = conn.Query<QuestionStadiumPage>(query);
            List<QuestionStadiumPage> listOfResults = foo.ToList<QuestionStadiumPage>();
            RandomNumber = new Random();
            QuestionStadiumPage temp = listOfResults.ElementAt(RandomNumber.Next(0, listOfResults.Count));
            return temp;
        }

        public void CreateAnswerImageChecker(int id, int answer1, int answer2, int answer3, int answer4)
        {
            AnswerImageCheckerPage AICP = new AnswerImageCheckerPage(id, answer1, answer2, answer3, answer4);
            conn.Insert(AICP);
            string query = String.Format("SELECT * FROM AnswerImageCheckerPage WHERE InternID = {0}", id);
            IEnumerable<AnswerImageCheckerPage> foo = conn.Query<AnswerImageCheckerPage>(query);
            List<AnswerImageCheckerPage> liste = foo.ToList<AnswerImageCheckerPage>();
        }
        */
    }
}