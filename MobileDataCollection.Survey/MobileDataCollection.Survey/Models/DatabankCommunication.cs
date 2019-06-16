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
        private SQLiteConnection conn;
        Random RandomNumber = new Random();

        public DatabankCommunication()
        {
            CreateDatabank();
        }

        public void CreateConnectionToDb()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
        }

        public void CreateDatabank()
        {
            CreateConnectionToDb();
            CreateTable();
            //CreateQuestions();
        }

        private void CreateTable()
        { 
            conn.DropTable<QuestionImageCheckerPage>();
            conn.DropTable<AnswerImageCheckerPage>();

            conn.CreateTable<Test>();
            //conn.CreateTable<QuestionImageCheckerPage>();
        }
        public void CreateQuestionsForImageChecker()
        {
            QuestionImageCheckerPage question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 1, 0, 0, 1, 0, "Q1_G1_F1_B1_klein.png", "Q1_G1_F1_B2_klein.png", "Q1_G1_F1_B3_klein.png", "Q1_G1_F1_B4_klein.png");
            conn.Insert(question);
            question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 1, 0, 1, 1, 0, "Q1_G1_F2_B1_klein.png", "Q1_G1_F2_B2_klein.png", "Q1_G1_F2_B3_klein.png", "Q1_G1_F2_B4_klein.png");
            conn.Insert(question);
            question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Raps abgebildet?", 2, 1, 1, 1, 0, "Q1_G2_F1_B1_klein.png", "Q1_G2_F1_B2_klein.png", "Q1_G2_F1_B3_klein.png", "Q1_G2_F1_B4_klein.png");
            conn.Insert(question);
            question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Weizen abgebildet?", 2, 0, 0, 1, 0, "Q1_G2_F2_B1_klein.png", "Q1_G2_F2_B2_klein.png", "Q1_G2_F2_B3_klein.png", "Q1_G2_F2_B4_klein.png");
            conn.Insert(question);
            question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Kartoffel abgebildet?", 3, 1, 0, 1, 0, "Q1_G3_F1_B1_klein.png", "Q1_G3_F1_B2_klein.png", "Q1_G3_F1_B3_klein.png", "Q1_G3_F1_B4_klein.png");
            conn.Insert(question);
            question = new QuestionImageCheckerPage("Wo sehen sie die Feldfruchtsorte Gerste abgebildet?", 3, 0, 0, 1, 0, "Q1_G3_F2_B1_klein.png", "Q1_G3_F2_B2_klein.png", "Q1_G3_F2_B3_klein.png", "Q1_G3_F2_B4_klein.png");
            conn.Insert(question);
        }

        public void CreateQuestionsForDoubleSlider()
        {
            QuestionDoubleSliderPage question = new QuestionDoubleSliderPage("Q3G1B1_klein.png", 7, 4, 1);
            conn.Insert(question);
            question = new QuestionDoubleSliderPage("Q3G1B2_klein.png", 49, 91, 1);
            conn.Insert(question);
            question = new QuestionDoubleSliderPage("Q3G1B3_klein.png", 12, 3, 1);
            conn.Insert(question);
            question = new QuestionDoubleSliderPage("Q3G1B4_klein.png", 64, 94, 1);
        }

        public void CreateQuestionForIntrospection()
        {
            QuestionIntrospectionPage question = new QuestionIntrospectionPage("Ich kann die Sorte von Feldfrüchten zuverlässig erkennen");
            conn.Insert(question);
            question = new QuestionIntrospectionPage("Ich kann phänologische Entwicklungsstadien von Feldfrüchten zuverlässig erkennen");
            conn.Insert(question);
            question = new QuestionIntrospectionPage("Ich kann den Bedeckungsgrad des Bodens durch Pflanzen zuverlässig schätzen");
            conn.Insert(question);
            question = new QuestionIntrospectionPage("Ich kann den Anteil grüner Pflanzenbestandteile am gesamten Pflanzenmaterial zuverlässig schätzen");
            conn.Insert(question);
        }
        
        public QuestionImageCheckerPage LoadQuestionImageCkecker(int difficulty)
        {
            string query = String.Format("SELECT * FROM QuestionImageCheckerPage LEFT OUTER JOIN AnswerImageCheckerPage ON QuestionImageCheckerPage.InternId = AnswerImageCheckerPage.InternId WHERE Difficulty = {0}", difficulty);
            IEnumerable<QuestionImageCheckerPage> foo = conn.Query<QuestionImageCheckerPage>(query);
            List<QuestionImageCheckerPage> listOfResults = foo.ToList<QuestionImageCheckerPage>();
            RandomNumber = new Random();
            QuestionImageCheckerPage temp = listOfResults.ElementAt(RandomNumber.Next(0,listOfResults.Count));
            return temp;
        }

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
    }
}