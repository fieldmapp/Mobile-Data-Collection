using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class AnswerImageCheckerPage : BindableObject
    {
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(InternId), typeof(int), typeof(AnswerImageCheckerPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty Image1SelectedProperty = BindableProperty.Create(nameof(Image1Selected), typeof(int), typeof(AnswerImageCheckerPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty Image2SelectedProperty = BindableProperty.Create(nameof(Image2Selected), typeof(int), typeof(AnswerImageCheckerPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty Image3SelectedProperty = BindableProperty.Create(nameof(Image3Selected), typeof(int), typeof(AnswerImageCheckerPage), 0, BindingMode.OneWay);
        public static readonly BindableProperty Image4SelectedProperty = BindableProperty.Create(nameof(Image4Selected), typeof(int), typeof(AnswerImageCheckerPage), 0, BindingMode.OneWay);


        /// <summary>
        /// Intern Id for Answers of this Type, corrosponds to same number as in QuestionImageCheckerPage
        /// </summary>
        [PrimaryKey]
        public int InternId
        {
            get => (int)GetValue(InternIdProperty);
            set => SetValue(InternIdProperty, value);
        }

        /// <summary>
        /// Reflects whether Image 1 is selected
        /// </summary>
        public int Image1Selected
        {
            get => (int)GetValue(Image1SelectedProperty);
            set => SetValue(Image1SelectedProperty, value);
        }

        /// <summary>
        /// Reflects whether Image 2 is selected
        /// </summary>
        public int Image2Selected
        {
            get => (int)GetValue(Image2SelectedProperty);
            set => SetValue(Image2SelectedProperty, value);
        }

        /// <summary>
        /// Reflects whether Image 3 is selected
        /// </summary>
        public int Image3Selected
        {
            get => (int)GetValue(Image3SelectedProperty);
            set => SetValue(Image3SelectedProperty, value);
        }

        /// <summary>
        /// Reflects whether Image 4 is selected
        /// </summary>
        public int Image4Selected
        {
            get => (int)GetValue(Image4SelectedProperty);
            set => SetValue(Image4SelectedProperty, value);
        }

        /// <summary>
        /// Is a reference to the belonging Question
        /// </summary>

        public AnswerImageCheckerPage(int Id,int selected1, int selected2, int selected3, int selected4)
        {
            InternId = Id;
            Image1Selected = selected1;
            Image2Selected = selected2;
            Image3Selected = selected3;
            Image4Selected = selected4;
        }
    }
}
