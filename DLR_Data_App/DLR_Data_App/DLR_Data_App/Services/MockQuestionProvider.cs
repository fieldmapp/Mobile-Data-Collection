using DLR_Data_App.Models.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLR_Data_App.Services
{
    public class MockQuestionProvider : IStorageProvider
    {
        /// <summary>
        /// Creates a List with the deafult ProfilingmenuItems and retuns this list
        /// </summary>
        public static List<ProfilingMenuItem> GenerateProfilingMenuItems()
        {
            return new List<ProfilingMenuItem>()
            {
                new ProfilingMenuItem("DoubleSlider", "Bedeckungsgrade", 4, new List<int>{3,4}),
                new ProfilingMenuItem("ImageChecker", "Sortenerkennung", 2, new List<int>{2}),
                new ProfilingMenuItem("Stadium", "Wuchsstadien", 6,  new List<int>{1})
            };
        }

        /// <summary>
        /// Creates all mock questions for the ImageCheckerType, but NOT from the txt file
        /// </summary>
        public List<IQuestionContent> CreateQuestionsForImageChecker()
        {
            return new List<IQuestionContent>
            {
                new QuestionImageCheckerPage(1, "Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 1, 0, 0, "ImageChecker_one_question1_picture1.png", "ImageChecker_one_question1_picture2.png", "ImageChecker_one_question1_picture3.png", "ImageChecker_one_question1_picture4.png"),
                new QuestionImageCheckerPage(2, "Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 1, 0, 1, 1, 0, "ImageChecker_one_question2_picture1.png", "ImageChecker_one_question2_picture2.png", "ImageChecker_one_question2_picture3.png", "ImageChecker_one_question2_picture4.png"),
                new QuestionImageCheckerPage(3, "Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 2, 1, 1, 1, 0, "ImageChecker_two_question3_picture1.png", "ImageChecker_two_question3_picture2.png", "ImageChecker_two_question3_picture3.png", "ImageChecker_two_question3_picture4.png"),
                new QuestionImageCheckerPage(4, "Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 2, 0, 0, 1, 0, "ImageChecker_two_question4_picture1.png", "ImageChecker_two_question4_picture2.png", "ImageChecker_two_question4_picture3.png", "ImageChecker_two_question4_picture4.png"),
                new QuestionImageCheckerPage(5, "Wo sehen sie die Feldfruchtsorte Kartoffel abgebildet?", 3, 1, 0, 1, 0, "ImageChecker_three_question5_picture1.png", "ImageChecker_three_question5_picture2.png", "ImageChecker_three_question5_picture3.png", "ImageChecker_three_question5_picture4.png"),
                new QuestionImageCheckerPage(6, "Wo sehen sie die Feldfruchtsorte Gerste abgebildet?", 3, 0, 0, 1, 0, "ImageChecker_three_question6_picture1.png", "ImageChecker_three_question6_picture2.png", "ImageChecker_three_question6_picture3.png", "ImageChecker_three_question6_picture4.png")
            };
        }

        /// <summary>
        /// Creates all mock questions for the DoubleSlider Type, but NOT from the txt file
        /// </summary>
        public List<IQuestionContent> CreateQuestionsForDoubleSlider()
        {
            return new List<IQuestionContent>
            {
                new QuestionDoubleSliderPage(1, 1, "DoubleSlider_one_question1.png", 60, 23),
                new QuestionDoubleSliderPage(2, 1, "DoubleSlider_one_question2.png", 43, 99),
                new QuestionDoubleSliderPage(3, 1, "DoubleSlider_one_question3.png", 12, 3),
                new QuestionDoubleSliderPage(4, 1, "DoubleSlider_one_question4.png", 90, 1),
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
        /// Creates all mock questions for the StadiumType
        /// </summary>
        public List<IQuestionContent> CreateQuestionsForStadium()
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
            var questionText = "Ordnen Sie dem Bild eine Feldfruchtsorte und das/die Entwicklungstadium/-stadien zu.";
            return new List<IQuestionContent>
            {
                new QuestionStadiumPage(1, 1, "Stadium_one_question1.png", stadiums1.ToList(), plants1.ToList(), 1, "C", questionText),
                new QuestionStadiumPage(2, 1, "Stadium_one_question2.png", stadiums1.ToList(), plants1.ToList(), 1, "D", questionText),
                new QuestionStadiumPage(3, 1, "Stadium_one_question3.png", stadiums1.ToList(), plants1.ToList(), 1, "B", questionText),
                new QuestionStadiumPage(4, 1, "Stadium_one_question4.png", stadiums1.ToList(), plants1.ToList(), 1, "A", questionText),
                new QuestionStadiumPage(5, 1, "Stadium_one_question5.png", stadiums1.ToList(), plants1.ToList(), 3, "C", questionText),
                new QuestionStadiumPage(6, 1, "Stadium_one_question6.png", stadiums1.ToList(), plants1.ToList(), 2, "C", questionText),

                new QuestionStadiumPage(7, 2, "Stadium_two_question1.png", stadiums2.ToList(), plants2.ToList(), 5, "B", questionText),
                new QuestionStadiumPage(8, 2, "Stadium_two_question2.png", stadiums2.ToList(), plants2.ToList(), 4, "C", questionText),
                new QuestionStadiumPage(9, 2, "Stadium_two_question3.png", stadiums2.ToList(), plants2.ToList(), 5, "D", questionText),
                new QuestionStadiumPage(10, 2, "Stadium_two_question4.png", stadiums2.ToList(), plants2.ToList(), 1, "B", questionText),
                new QuestionStadiumPage(11, 2, "Stadium_two_question5.png", stadiums2.ToList(), plants2.ToList(), 2, "D", questionText),
                new QuestionStadiumPage(12, 2, "Stadium_two_question6.png", stadiums2.ToList(), plants2.ToList(), 3, "A", questionText),

                new QuestionStadiumPage(13, 3, "Stadium_three_question1.png", stadiums3.ToList(), plants3.ToList(), 2, "E", questionText),
                new QuestionStadiumPage(14, 3, "Stadium_three_question2.png", stadiums3.ToList(), plants3.ToList(), 4, "D", questionText),
                new QuestionStadiumPage(15, 3, "Stadium_three_question3.png", stadiums3.ToList(), plants3.ToList(), 3, "B", questionText),
                new QuestionStadiumPage(16, 3, "Stadium_three_question4.png", stadiums3.ToList(), plants3.ToList(), 3, "A", questionText),
                new QuestionStadiumPage(17, 3, "Stadium_three_question5.png", stadiums3.ToList(), plants3.ToList(), 4, "C", questionText),
                new QuestionStadiumPage(18, 3, "Stadium_three_question6.png", stadiums3.ToList(), plants3.ToList(), 1, "D", questionText)
            };
        }

        /// <summary>
        /// Creates all questions for the IntrospectionType, but NOT from the txt file
        /// </summary>
        public List<IQuestionContent> CreateQuestionForIntrospection()
        {
            return new List<IQuestionContent>
            {
                new QuestionIntrospectionPage(1,"Ich kann die Sorte von Feldfrüchten zuverlässig erkennen"),
                new QuestionIntrospectionPage(2, "Ich kann phänologische Entwicklungsstadien von Feldfrüchten zuverlässig erkennen"),
                new QuestionIntrospectionPage(3, "Ich kann den Bedeckungsgrad des Bodens durch Pflanzen zuverlässig schätzen"),
                new QuestionIntrospectionPage(4, "Ich kann den Anteil grüner Pflanzenbestandteile am gesamten Pflanzenmaterial zuverlässig schätzen")
            };
        }

        public Dictionary<string, List<IQuestionContent>> LoadQuestions()
        {
            return new Dictionary<string, List<IQuestionContent>>
            {
                ["ImageChecker"] = CreateQuestionsForImageChecker(),
                ["DoubleSlider"] = CreateQuestionsForDoubleSlider(),
                ["Stadium"] = CreateQuestionsForStadium(),
                ["Introspection"] = CreateQuestionForIntrospection()
            };
        }

        public ProfilingData LoadProfilingData()
        {
            return new ProfilingData
            {
                ProfilingId = "fieldcampagneprofiling", ProfilingMenuItems = new List<ProfilingMenuItem>()
                {
                    new ProfilingMenuItem("DoubleSlider", "Bedeckungsgrade", 4, new List<int> { 3, 4 }),
                    new ProfilingMenuItem("ImageChecker", "Sortenerkennung", 2, new List<int> { 2 }),
                    new ProfilingMenuItem("Stadium", "Wuchsstadien", 6, new List<int> { 1 })
                }
            };
        }

        public void SaveAnswers(ProfilingResults results) { }

        public ProfilingResults LoadAnswers(string username)
        {
            var answers = new Dictionary<string, List<IUserAnswer>>
            {
                ["ImageChecker"] = new List<IUserAnswer>
                {
                    new AnswerImageCheckerPage(1,1,1,0,0),
                    new AnswerImageCheckerPage(2,0,1,0,1),
                    new AnswerImageCheckerPage(3,0,1,0,1)
                },
                ["DoubleSlider"] = new List<IUserAnswer>
                {
                    new AnswerDoubleSliderPage(1,10,20),
                    new AnswerDoubleSliderPage(2,40,30),
                    new AnswerDoubleSliderPage(3,60,10),
                    new AnswerDoubleSliderPage(4,34,34)
                },
                ["Introspection"] = new List<IUserAnswer>
                {
                    new AnswerIntrospectionPage(1,3),
                    new AnswerIntrospectionPage(1,4)
                },
                ["Stadium"] = new List<IUserAnswer>
                {
                    new AnswerStadiumPage(1,"A",1),
                    new AnswerStadiumPage(2,"A",2)
                }
            };
            var profilingResult = new ProfilingResult() { TimeStamp = DateTime.UtcNow, UserAnswers = answers };
            return new ProfilingResults() { UserId = username, Results = new List<ProfilingResult>() { profilingResult } };
        }

        public void ExportAnswers(ProfilingResults results) { }

        public void ExportDatabase(string content) { }

        public Dictionary<string, string> LoadTranslations()
        {
            throw new NotImplementedException();
        }
    }
}
