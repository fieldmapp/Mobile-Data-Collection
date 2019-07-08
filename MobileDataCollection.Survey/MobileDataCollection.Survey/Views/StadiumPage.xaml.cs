using MobileDataCollection.Survey.Controls;
using MobileDataCollection.Survey.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDataCollection.Survey.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StadiumPage : ContentPage, ISurveyPage
	{
        /// <summary>
        /// Bindings of QuestionItem, AnswerItem and Header
        /// </summary>
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem),typeof(QuestionStadiumPage), typeof(StadiumPage), null, BindingMode.OneWay);
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem), typeof(AnswerStadiumPage), typeof(StadiumPage), new AnswerStadiumPage(0, string.Empty, 0), BindingMode.OneWay);
        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(string), typeof(StadiumPage), "demo", BindingMode.OneWay);

        /// <summary>
        /// Item of the given Question
        /// </summary>
        public QuestionStadiumPage QuestionItem
        {
            get { return (QuestionStadiumPage)GetValue(QuestionItemProperty); }
            set { SetValue(QuestionItemProperty, value); }
        }

        /// <summary>
        /// Item of the corresponding answer of the question
        /// </summary>
        public AnswerStadiumPage AnswerItem
        {
            get { return (AnswerStadiumPage)GetValue(AnswerItemProperty); }
            set { SetValue(AnswerItemProperty, value); }
        }

        /// <summary>
        /// Item of the Header (given answers and number of answers that are missing)
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        IQuestionContent ISurveyPage.QuestionItem => QuestionItem;

        IUserAnswer ISurveyPage.AnswerItem => AnswerItem;

        ObservableCollection<StadiumSubItem> StadiumCollection;

        ObservableCollection<Plant> PlantCollection;

        public event EventHandler PageFinished;

        public StadiumPage(QuestionStadiumPage question, int answersGiven, int answersNeeded)
		{
            InitializeComponent();
            QuestionItem = question;
            StadiumCollection = new ObservableCollection<StadiumSubItem>(question.Stadiums);
            PlantCollection = new ObservableCollection<Plant>(question.Plants);
            Picture.BindingContext = this;
            StadiumInlinePicker.ItemSource = StadiumCollection;
            PlantInlinePicker.ItemSource = PlantCollection;
            Header = $"Frage {answersGiven}/{answersNeeded + 1}";
            HeaderText.BindingContext = this;
        }
        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            int selectedStadium = (StadiumInlinePicker.SelectedItem as StadiumSubItem).InternNumber;
            var selectedPlant = (PlantInlinePicker.SelectedItem as Plant)?.InternLetter;
            if (selectedPlant == null || selectedStadium == 0)
                return;

            AnswerItem = new AnswerStadiumPage(QuestionItem.InternId, selectedPlant, selectedStadium);
            PageFinished?.Invoke(this, null);
        }
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
            PageFinished?.Invoke(this, null);
        }
        
        protected override bool OnBackButtonPressed()
        {
            PageFinished?.Invoke(this, null);
            return true;
        }
    }
}