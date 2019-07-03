using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;

namespace MobileDataCollection.Survey.Models
{
    public class QuestionStadiumPage : BindableObject, IQuestionContent
    {
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(QuestionStadiumPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty DifficultyProperty = BindableProperty.Create(nameof(Difficulty), typeof(int), typeof(QuestionStadiumPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty StadiumSubItemsProperty = BindableProperty.Create(nameof(Stadiums), typeof(List<StadiumSubItem>), typeof(QuestionStadiumPage), null, BindingMode.OneWay);
        public static readonly BindableProperty TestCollection2Property = BindableProperty.Create(nameof(Plants), typeof(List<Plant>), typeof(QuestionStadiumPage), null, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerFruitTypeProperty = BindableProperty.Create(nameof(CorrectAnswerFruitType), typeof(string), typeof(QuestionStadiumPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerStadiumProperty = BindableProperty.Create(nameof(CorrectAnswerStadium), typeof(string), typeof(QuestionStadiumPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(string), typeof(QuestionStadiumPage), string.Empty, BindingMode.OneWay);
            
        /// <summary>
        /// Intern Id only for this Type Of Question
        /// </summary>
        public int InternId
        {

            get => (int)GetValue(InternIdProperty);
            set => SetValue(InternIdProperty, value);
        }

        public string Image
        {
            get => (string)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
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
        public List<Plant> Plants
        {
            get => (List<Plant>)GetValue(TestCollection2Property);
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



        public QuestionStadiumPage(int internId, int difficulty, string image, List<StadiumSubItem> stadiums, List<Plant> plants, string correctAnswerStadium, string correctAnswerFruitType)
        {
            if (!stadiums.Any(s => s.StadiumName == correctAnswerStadium))
                throw new ArgumentException("Argument needs to be contained in "+nameof(stadiums), nameof(correctAnswerStadium));
            if (!plants.Any(p => p.Name == correctAnswerFruitType))
                throw new ArgumentException("Argument needs to be contained in " + nameof(plants), nameof(correctAnswerStadium));

            InternId = internId;
            Image = image;
            Difficulty = difficulty;
            Stadiums = stadiums;
            Plants = plants;
            CorrectAnswerFruitType = correctAnswerFruitType;
            CorrectAnswerStadium = correctAnswerStadium;
        }
    }
}
