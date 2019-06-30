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
        private List<QuestionDoubleSliderPage> ListQuestionDoubleSliders = new List<QuestionDoubleSliderPage>();
        private List<QuestionStadiumPage> ListQuestionStadiumPage = new List<QuestionStadiumPage>();
        private List<QuestionIntrospectionPage> ListQuestionIntrospectionPage = new List<QuestionIntrospectionPage>();

        private List<AnswerImageCheckerPage> ListAnswerImageCheckerPage = new List<AnswerImageCheckerPage>();
        private List<AnswerDoubleSliderPage> ListAnswerDoubleSliders = new List<AnswerDoubleSliderPage>();
        private List<AnswerStadiumPage> ListAnswerStadiumPage = new List<AnswerStadiumPage>();
        private List<AnswerIntrospectionPage> ListAnswerIntrospectionPage = new List<AnswerIntrospectionPage>();

        public DatabankCommunication()
        {
            CreateQuestions();
        }

        public void CreateQuestions()
        {
            CreateQuestionsForImageChecker();
            CreateQuestionsForDoubleSlider();
            CreateQuestionForIntrospection();
        }
        public void CreateQuestionsForImageChecker()
        {
            QuestionImageCheckerPage question = new QuestionImageCheckerPage(1,"Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 0, 1, 0, "ImageChecker_one_question1_picture1.png", "ImageChecker_one_question1_picture2.png", "ImageChecker_one_question1_picture3.png", "ImageChecker_one_question1_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(2,"Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 1, 0, 1, 1, 0, "ImageChecker_one_question2_picture1.png", "ImageChecker_one_question2_picture2.png", "ImageChecker_one_question2_picture3.png", "ImageChecker_one_question2_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(3,"Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 2, 1, 1, 1, 0, "ImageChecker_two_question3_picture1.png", "ImageChecker_two_question3_picture2.png", "ImageChecker_two_question3_picture3.png", "ImageChecker_two_question3_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(4,"Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 2, 0, 0, 1, 0, "ImageChecker_two_question4_picture1.png", "ImageChecker_two_question4_picture2.png", "ImageChecker_two_question4_picture3.png", "ImageChecker_two_question4_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(5,"Wo sehen sie die Feldfruchtsorte Kartoffel abgebildet?", 3, 1, 0, 1, 0, "ImageChecker_three_question5_picture1.png", "ImageChecker_three_question5_picture2.png", "ImageChecker_three_question5_picture3.png", "ImageChecker_three_question5_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
            question = new QuestionImageCheckerPage(6,"Wo sehen sie die Feldfruchtsorte Gerste abgebildet?", 3, 0, 0, 1, 0, "ImageChecker_three_question6_picture1.png", "ImageChecker_three_question6_picture2.png", "ImageChecker_three_question6_picture3.png", "ImageChecker_three_question6_picture4.png");
            ListQuestionImageCheckerPage.Add(question);
        }

        public void CreateQuestionsForDoubleSlider()
        {
            QuestionDoubleSliderPage question = new QuestionDoubleSliderPage("DoubleSlider_one_question1.png", 7, 4, 1);
            ListQuestionDoubleSliders.Add(question);
            question = new QuestionDoubleSliderPage("DoubleSlider_one_question2.png", 49, 91, 1);
            ListQuestionDoubleSliders.Add(question);
            question = new QuestionDoubleSliderPage("DoubleSlider_one_question3.png", 12, 3, 1);
            ListQuestionDoubleSliders.Add(question);
            question = new QuestionDoubleSliderPage("DoubleSlider_one_question4.png", 64, 94, 1);
            ListQuestionDoubleSliders.Add(question);
            question = new QuestionDoubleSliderPage("DoubleSlider_two_question1.png", 49, 91, 1);
            ListQuestionDoubleSliders.Add(question);
            question = new QuestionDoubleSliderPage("DoubleSlider_two_question2.png", 12, 3, 1);
            ListQuestionDoubleSliders.Add(question);
            question = new QuestionDoubleSliderPage("DoubleSlider_two_question3.png", 64, 94, 1);
            ListQuestionDoubleSliders.Add(question);
            question = new QuestionDoubleSliderPage("DoubleSlider_two_question4.png", 64, 94, 1);
            ListQuestionDoubleSliders.Add(question);

        }

        public void CreateQuestionForIntrospection()
        {
            QuestionIntrospectionPage question = new QuestionIntrospectionPage("Ich kann die Sorte von Feldfrüchten zuverlässig erkennen");
            ListQuestionIntrospectionPage.Add(question);
            question = new QuestionIntrospectionPage("Ich kann phänologische Entwicklungsstadien von Feldfrüchten zuverlässig erkennen");
            ListQuestionIntrospectionPage.Add(question);
            question = new QuestionIntrospectionPage("Ich kann den Bedeckungsgrad des Bodens durch Pflanzen zuverlässig schätzen");
            ListQuestionIntrospectionPage.Add(question);
            question = new QuestionIntrospectionPage("Ich kann den Anteil grüner Pflanzenbestandteile am gesamten Pflanzenmaterial zuverlässig schätzen");
            ListQuestionIntrospectionPage.Add(question);
        }
        
        public QuestionImageCheckerPage LoadQuestionImageChecker(int difficulty)
        {
            for(int i = 0; i < ListQuestionImageCheckerPage.Count; i++)
            {
                QuestionImageCheckerPage question = ListQuestionImageCheckerPage.ElementAt<QuestionImageCheckerPage>(i);
                if(question.Difficulty == difficulty)
                {
                    if(!(SearchListAnswerImageCheckerPage(question.InternId)))
                    {
                        return question;
                    }
                }
            }
            if (difficulty == 1)
            {
                return new QuestionImageCheckerPage(0,"", 0, 0, 0, 0, 0, "", "", "", "");
            }
            else
            {
                return LoadQuestionImageChecker(difficulty - 1);
            }
        }

        public Boolean SearchListAnswerImageCheckerPage(int Id)
        {
            for(int i = 0;i < ListAnswerImageCheckerPage.Count;i++)
            {
                List<AnswerImageCheckerPage> a = ListAnswerImageCheckerPage;
                AnswerImageCheckerPage answer = ListAnswerImageCheckerPage.ElementAt<AnswerImageCheckerPage>(i);
                if (answer.InternId == Id)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddListAnswerImageCheckerPage(AnswerImageCheckerPage answer)
        {
            ListAnswerImageCheckerPage.Add(answer);
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