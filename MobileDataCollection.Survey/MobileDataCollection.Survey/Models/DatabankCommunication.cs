using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using MobileDataCollection.Survey.Models;

namespace MobileDataCollection.Survey.Models
{
    public class DatabankCommunication : ContentView
    {
        Random RandomNumber = new Random();

        private List<QuestionImageCheckerPage> ListQuestionImageCheckerPage = new List<QuestionImageCheckerPage>();
        private List<QuestionDoubleSliderPage> ListQuestionDoubleSliderPage = new List<QuestionDoubleSliderPage>();
        private List<QuestionStadiumPage> ListQuestionStadiumPage = new List<QuestionStadiumPage>();
        private List<QuestionIntrospectionPage> ListQuestionIntrospectionPage = new List<QuestionIntrospectionPage>();

        private List<AnswerImageCheckerPage> ListAnswerImageCheckerPage = new List<AnswerImageCheckerPage>();
        private List<AnswerDoubleSliderPage> ListAnswerDoubleSliderPage = new List<AnswerDoubleSliderPage>();
        private List<AnswerStadiumPage> ListAnswerStadiumPage = new List<AnswerStadiumPage>();
        private List<AnswerIntrospectionPage> ListAnswerIntrospectionPage = new List<AnswerIntrospectionPage>();

        public DatabankCommunication()
        {
            CreateQuestions();
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
        /// Creates all questions for the ImageCheckerType
        /// </summary>
        public void CreateQuestionsForImageChecker()
        {
            QuestionImageCheckerPage question = new QuestionImageCheckerPage(1,"Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 0, 1, 0, "Q1_G1_F1_B1_klein.png", "Q1_G1_F1_B2_klein.png", "Q1_G1_F1_B3_klein.png", "Q1_G1_F1_B4_klein.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(2,"Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 1, 0, 1, 1, 0, "Q1_G1_F2_B1_klein.png", "Q1_G1_F2_B2_klein.png", "Q1_G1_F2_B3_klein.png", "Q1_G1_F2_B4_klein.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(3,"Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 2, 1, 1, 1, 0, "Q1_G2_F1_B1_klein.png", "Q1_G2_F1_B2_klein.png", "Q1_G2_F1_B3_klein.png", "Q1_G2_F1_B4_klein.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(4,"Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 2, 0, 0, 1, 0, "Q1_G2_F2_B1_klein.png", "Q1_G2_F2_B2_klein.png", "Q1_G2_F2_B3_klein.png", "Q1_G2_F2_B4_klein.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(5,"Wo sehen sie die Feldfruchtsorte Kartoffel abgebildet?", 3, 1, 0, 1, 0, "Q1_G3_F1_B1_klein.png", "Q1_G3_F1_B2_klein.png", "Q1_G3_F1_B3_klein.png", "Q1_G3_F1_B4_klein.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(6,"Wo sehen sie die Feldfruchtsorte Gerste abgebildet?", 3, 0, 0, 1, 0, "Q1_G3_F2_B1_klein.png", "Q1_G3_F2_B2_klein.png", "Q1_G3_F2_B3_klein.png", "Q1_G3_F2_B4_klein.png");
            ListQuestionImageCheckerPage.Add(question);
        }

        /// <summary>
        /// Creates all questions for the DoubleSlider Type
        /// </summary>
        public void CreateQuestionsForDoubleSlider()
        {
            QuestionDoubleSliderPage question = new QuestionDoubleSliderPage(1,1,"Q3G1B1_klein.png", 7, 4);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(2,1,"Q3G1B2_klein.png", 49, 91);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(3,1,"Q3G1B3_klein.png", 12, 3);
            ListQuestionDoubleSliderPage.Add(question);
            question = new QuestionDoubleSliderPage(4,1,"Q3G1B4_klein.png", 64, 94);
            ListQuestionDoubleSliderPage.Add(question);
        }

        /// <summary>
        /// Creates all questions for the StadiumType
        /// </summary>
        public void CreateQuestionsForStadium()
        {

        }

        /// <summary>
        /// Creates all questions for the IntrospectionType
        /// </summary>
        public void CreateQuestionForIntrospection()
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
        public QuestionImageCheckerPage LoadQuestionImageCheckerPage(int difficulty)
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
        public Boolean SearchListAnswerImageCheckerPage(int Id)
        {
            for(int i = 0;i < ListAnswerImageCheckerPage.Count;i++)
            {
                AnswerImageCheckerPage answer = ListAnswerImageCheckerPage.ElementAt<AnswerImageCheckerPage>(i);
                if (answer.InternId == Id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds an AnswerImageCheckerPage Object to the List ListAnswerImageCheckerPage
        /// </summary>
        public void AddListAnswerImageCheckerPage(AnswerImageCheckerPage answer)
        {
            ListAnswerImageCheckerPage.Add(answer);
        }

        /// <summary>
        /// Loads a QUestionDoubleSliderPage-Object with the set difficulty, or a lower difficulty if no question with a matching difficulty exsist.
        /// If no question can be loaded it will return an Object, with the ID 0
        /// </summary>
        public QuestionDoubleSliderPage LoadQuestionDoubleSliderPage(int difficulty)
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
        public Boolean SearchListAnswerDoubleSliderPage(int Id)
        {
            for (int i = 0; i < ListAnswerDoubleSliderPage.Count; i++)
            {
                AnswerDoubleSliderPage answer = ListAnswerDoubleSliderPage.ElementAt<AnswerDoubleSliderPage>(i);
                if (answer.InternId == Id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds an AnswerDoubleSliderPage-Object to the List ListAnswerDoubleSliderPage
        /// </summary>
        public void AddListAnswerDoubleSliderPage(AnswerDoubleSliderPage answer)
        {
            ListAnswerDoubleSliderPage.Add(answer);
        }

        /// <summary>
        /// Loads a QUestionStadiumPage-Object with the set difficulty, or a lower difficulty if no question with a matching difficulty exsist.
        /// If no question can be loaded it will return an Object, with the ID 0
        /// </summary>
        public QuestionStadiumPage LoadQuestionStadiumPage(int difficulty)
        {
            List<QuestionStadiumPage> ListQuestion = new List<QuestionStadiumPage>();
            for (int i = 0; i < ListQuestionStadiumPage.Count; i++)
            {
                QuestionStadiumPage question = ListQuestionStadiumPage.ElementAt<QuestionStadiumPage>(i);
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
                return ListQuestion.ElementAt<QuestionStadiumPage>(RandomNumber.Next(ListQuestion.Count));
            }
            if (difficulty == 1)
            {
                return new QuestionStadiumPage(0, 0, null,null,"","");
            }
            else
            {
                return LoadQuestionStadiumPage(difficulty - 1);
            }
        }

        /// <summary>
        /// Searches an AnswerStadiumPage-Object with the corrosponding Id
        /// </summary>
        public Boolean SearchListAnswerStadiumPage(int Id)
        {
            for (int i = 0; i < ListAnswerStadiumPage.Count; i++)
            {
                AnswerStadiumPage answer = ListAnswerStadiumPage.ElementAt<AnswerStadiumPage>(i);
                if (answer.InternId == Id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds an AnswerStadiumPage-Object to the List ListAnswerDoubleSliderPage
        /// </summary>
        public void AddListAnswerStadiumPage(AnswerStadiumPage answer)
        {
            ListAnswerStadiumPage.Add(answer);
        }

        /// <summary>
        /// Loads a QUestionIntrospectionPage-Object with the set Id
        /// </summary>
        public QuestionIntrospectionPage LoadQuestionIntrospectionPage(int id)
        {
            for (int i = 0; i < ListQuestionIntrospectionPage.Count; i++)
            {
                QuestionIntrospectionPage question = ListQuestionIntrospectionPage.ElementAt<QuestionIntrospectionPage>(i);
                if (question.InternId == id)
                {
                    return question;
                }
            }
            return new QuestionIntrospectionPage(0, "");

        }

        /// <summary>
        /// Searches an AnswerStadiumPage-Object with the corrosponding Id
        /// </summary>
        public Boolean SearchListAnswerIntrospectionPage(int Id)
        {
            for (int i = 0; i < ListAnswerIntrospectionPage.Count; i++)
            {
                AnswerIntrospectionPage answer = ListAnswerIntrospectionPage.ElementAt<AnswerIntrospectionPage>(i);
                if (answer.InternId == Id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds an AnswerStadiumPage-Object to the List ListAnswerDoubleSliderPage
        /// </summary>
        public void AddListAnswerIntrospectionPage(AnswerIntrospectionPage answer)
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