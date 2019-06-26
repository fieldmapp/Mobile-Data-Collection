using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class QuestionStadiumPage : BindableObject, IQuestionContent
    {
        //AnswersNeeded auch hier rein?
        //AnswersNeeded auch hier rein?
        public static readonly BindableProperty StadiumSubItemsProperty = BindableProperty.Create(nameof(StadiumSubItems), typeof(List<StadiumSubItem>), typeof(QuestionStadiumPage), null, BindingMode.OneWay);
        public static readonly BindableProperty TestCollection2Property = BindableProperty.Create(nameof(TestCollection2), typeof(ObservableCollection<Plant>), typeof(QuestionStadiumPage), null, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerFruitTypeProperty = BindableProperty.Create(nameof(CorrectAnswerFruitType), typeof(string), typeof(QuestionStadiumPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerStadiumProperty = BindableProperty.Create(nameof(CorrectAnswerStadium), typeof(string), typeof(QuestionStadiumPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty LevelProperty = BindableProperty.Create(nameof(Level), typeof(int), typeof(QuestionStadiumPage), 0, BindingMode.OneWay);

        //TODO: Add List with new class (eg FruitTypeSubItem)
        //TODO: Think of a way to store correct answer

        /// <summary>
        /// Contains all possible stadiums
        /// </summary>
        List<StadiumSubItem> StadiumSubItems
        {
            get => (List<StadiumSubItem>)GetValue(StadiumSubItemsProperty);
            set => SetValue(StadiumSubItemsProperty, value);
        }
         
        /// <summary>
        /// Contains all possible fruit types?
        /// </summary>
        ObservableCollection<Plant> TestCollection2
        {
            get => (ObservableCollection<Plant>)GetValue(TestCollection2Property);
            set => SetValue(TestCollection2Property, value);
        }

        string CorrectAnswerFruitType
        {
            get => (string)GetValue(CorrectAnswerFruitTypeProperty);
            set => SetValue(CorrectAnswerFruitTypeProperty, value);
        }

        string CorrectAnswerStadium
        {
            get => (string)GetValue(CorrectAnswerStadiumProperty);
            set => SetValue(CorrectAnswerStadiumProperty, value);
        }

        int Level
        {
            get => (int)GetValue(LevelProperty);
            set => SetValue(LevelProperty, value);
        }



        public QuestionStadiumPage(List<StadiumSubItem> stadiumSubItems, ObservableCollection<Plant> testCollection2, string correctAnswerFruitType, string correctAnswerStadium, int level)
        {
            StadiumSubItems = stadiumSubItems;
            TestCollection2 = testCollection2;
            CorrectAnswerFruitType = correctAnswerFruitType;
            CorrectAnswerStadium = correctAnswerStadium;
            Level = level;
        }
    }
}
