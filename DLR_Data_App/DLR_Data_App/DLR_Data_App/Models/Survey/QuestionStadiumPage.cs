//Main contributors: Maximilian Enderling, Max Moebius
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;

namespace DLR_Data_App.Models.Survey
{
    public class QuestionStadiumPage : BindableObject, IQuestionContent
    {
        /// <summary>
        ///Binding of parameters in QuestionItem of QuestionStadiumPage
        /// </summary>
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(QuestionStadiumPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty DifficultyProperty = BindableProperty.Create(nameof(Difficulty), typeof(int), typeof(QuestionStadiumPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty StadiumsProperty = BindableProperty.Create(nameof(Stadiums), typeof(List<StadiumSubItem>), typeof(QuestionStadiumPage), null, BindingMode.OneWay);
        public static readonly BindableProperty PlantsProperty = BindableProperty.Create(nameof(Plants), typeof(List<Plant>), typeof(QuestionStadiumPage), null, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerFruitTypeProperty = BindableProperty.Create(nameof(CorrectAnswerFruitType), typeof(string), typeof(QuestionStadiumPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty CorrectAnswerStadiumProperty = BindableProperty.Create(nameof(CorrectAnswerStadium), typeof(int), typeof(QuestionStadiumPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(string), typeof(QuestionStadiumPage), string.Empty, BindingMode.OneWay);
        public static readonly BindableProperty QuestionTextProperty = BindableProperty.Create(nameof(QuestionText), typeof(string), typeof(QuestionIntrospectionPage), string.Empty, BindingMode.OneWay);

        /// <summary>
        /// Intern Id only for this type of question(StadiumPage)
        /// </summary>
        public int InternId
        {

            get => (int)GetValue(InternIdProperty);
            set => SetValue(InternIdProperty, value);
        }

        /// <summary>
        /// Contains the image
        /// </summary>
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
            get => (List<StadiumSubItem>)GetValue(StadiumsProperty);
            set => SetValue(StadiumsProperty, value);
        }

        /// <summary>
        /// Question which will be shown to the user
        /// </summary>
        public string QuestionText
        {
            get => (string)GetValue(QuestionTextProperty);
            set => SetValue(QuestionTextProperty, value);
        }

        /// <summary>
        /// Contains all possible fruit types
        /// </summary>
        public List<Plant> Plants
        {
            get => (List<Plant>)GetValue(PlantsProperty);
            set => SetValue(PlantsProperty, value);
        }

        /// <summary>
        /// Contains the correct fruittype
        /// </summary>
        public string CorrectAnswerFruitType
        {
            get => (string)GetValue(CorrectAnswerFruitTypeProperty);
            set => SetValue(CorrectAnswerFruitTypeProperty, value);
        }

        /// <summary>
        /// Contains the correct stadium
        /// </summary>
        public int CorrectAnswerStadium
        {
            get => (int)GetValue(CorrectAnswerStadiumProperty);
            set => SetValue(CorrectAnswerStadiumProperty, value);
        }

        /// <summary>
        /// Difficulty of the question. Must be in range 1 to <see /cref="HighestQuestionDifficulty"/> (inclusive)
        /// </summary>
        public int Difficulty
        {
            get => (int)GetValue(DifficultyProperty);
            set => SetValue(DifficultyProperty, value);
        }

        /// <summary>
        /// The constructor of QuestionItem in StadiumPage
        /// </summary>
        /// <param name="internId"></param>
        /// <param name="difficulty"></param>
        /// <param name="image"></param>
        /// <param name="stadiums"></param>
        /// <param name="plants"></param>
        /// <param name="correctAnswerStadium"></param>
        /// <param name="correctAnswerFruitType"></param>
        public QuestionStadiumPage(int internId, int difficulty, string image, List<StadiumSubItem> stadiums, List<Plant> plants, int correctAnswerStadium, string correctAnswerFruitType, string questionText)
        {
            if (!stadiums.Any(s => s.InternNumber == correctAnswerStadium))
                throw new ArgumentException("Argument needs to be contained in "+nameof(stadiums), nameof(correctAnswerStadium));
            if (!plants.Any(p => p.InternLetter == correctAnswerFruitType))
                throw new ArgumentException("Argument needs to be contained in " + nameof(plants), nameof(correctAnswerFruitType));

            InternId = internId;
            Image = image;
            Difficulty = difficulty;
            Stadiums = stadiums;
            Plants = plants;
            CorrectAnswerFruitType = correctAnswerFruitType;
            CorrectAnswerStadium = correctAnswerStadium;
            QuestionText = questionText;
        }
    }
}
