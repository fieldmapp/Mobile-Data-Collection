using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class QuestionStadiumPage : BindableObject, IQuestionContent
    {
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(QuestionStadiumPage), 0, BindingMode.OneWay);
        //public static readonly BindableProperty NumberOfPossibleAnswersProperty = BindableProperty.Create(nameof(NumberOfPossibleAnswers), typeof(int), typeof(QuestionStadiumPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty DifficultyProperty = BindableProperty.Create(nameof(Difficulty), typeof(int), typeof(QuestionStadiumPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty StadiumSubItemsProperty = BindableProperty.Create(nameof(Stadiums), typeof(List<StadiumSubItem>), typeof(QuestionStadiumPage), null, BindingMode.OneWay);
        public static readonly BindableProperty TestCollection2Property = BindableProperty.Create(nameof(Plants), typeof(ObservableCollection<Plant>), typeof(QuestionStadiumPage), null, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerFruitTypeProperty = BindableProperty.Create(nameof(CorrectAnswerFruitType), typeof(string), typeof(QuestionStadiumPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerStadiumProperty = BindableProperty.Create(nameof(CorrectAnswerStadium), typeof(string), typeof(QuestionStadiumPage), string.Empty, BindingMode.OneWay);

        //TODO: Add List with new class (eg FruitTypeSubItem) - noch aktuell?
        //TODO: Think of a way to store correct answer - noch aktuell?

        /// <summary>
        /// Intern Id only for this Type Of Question
        /// </summary>
        public int InternId
        {

            get => (int)GetValue(InternIdProperty);
            set => SetValue(InternIdProperty, value);

        }

        /// <summary>
        /// Contains all possible stadiums
        /// </summary>
        public List<StadiumSubItem> Stadiums
        {
            get => (List<StadiumSubItem>)GetValue(StadiumSubItemsProperty);
            set => SetValue(StadiumSubItemsProperty, value);
        }
         
        /// <summary>
        /// Contains all possible fruit types?
        /// </summary>
        public ObservableCollection<Plant> Plants
        {
            get => (ObservableCollection<Plant>)GetValue(TestCollection2Property);
            set => SetValue(TestCollection2Property, value);
        }

        public string CorrectAnswerFruitType
        {
            get => (string)GetValue(CorrectAnswerFruitTypeProperty);
            set => SetValue(CorrectAnswerFruitTypeProperty, value);
        }

        public string CorrectAnswerStadium
        {
            get => (string)GetValue(CorrectAnswerStadiumProperty);
            set => SetValue(CorrectAnswerStadiumProperty, value);
        }

        public int Difficulty
        {
            get => (int)GetValue(DifficultyProperty);
            set => SetValue(DifficultyProperty, value);
        }



        public QuestionStadiumPage(int internId, int difficulty, List<StadiumSubItem> stadiumSubItems, ObservableCollection<Plant> testCollection2, string correctAnswerFruitType, string correctAnswerStadium)
        {
            InternId = internId;
            Difficulty = difficulty;
            Stadiums = stadiumSubItems;
            Plants = testCollection2;
            CorrectAnswerFruitType = correctAnswerFruitType;
            CorrectAnswerStadium = correctAnswerStadium;
        }
    }
}
