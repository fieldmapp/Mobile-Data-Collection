//Main contributors: Maximilian Enderling, Maya Koehnen
using DLR_Data_App.Models.Survey;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Survey
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StadiumPage : ContentPage, ISurveyPage
	{
        /// <summary>
        /// Bindings of QuestionItem, AnswerItem and Header
        /// </summary>
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem),typeof(QuestionStadiumPage), typeof(StadiumPage), null, BindingMode.OneWay);
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem), typeof(AnswerStadiumPage), typeof(StadiumPage), null, BindingMode.OneWay);
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

        public static readonly BindableProperty EvaluationTextColorProperty = BindableProperty.Create(nameof(EvaluationTextColor), typeof(Color), typeof(StadiumPage), Color.Transparent, BindingMode.OneWay);

        /// <summary>
        /// Color for the Evaluation Button
        /// </summary>
        public Color EvaluationTextColor
        {
            get { return (Color)GetValue(EvaluationTextColorProperty); }
            set { SetValue(EvaluationTextColorProperty, value); }
        }

        IQuestionContent ISurveyPage.QuestionItem => QuestionItem;

        IUserAnswer ISurveyPage.AnswerItem => AnswerItem;

        ObservableCollection<StadiumSubItem> StadiumCollection;

        ObservableCollection<Plant> PlantCollection;

        public event EventHandler<PageResult> PageFinished;

        public StadiumPage(QuestionStadiumPage question, int answersGiven, int answersNeeded)
		{
            InitializeComponent();
            QuestionItem = question;
            StadiumCollection = new ObservableCollection<StadiumSubItem>(question.Stadiums);
            PlantCollection = new ObservableCollection<Plant>(question.Plants);
            Picture.BindingContext = this;
            QuestionText.BindingContext = this;
            StadiumInlinePicker.ItemSource = StadiumCollection;
            PlantInlinePicker.ItemSource = PlantCollection;
            Header = $"Frage {answersGiven + 1}/{answersNeeded} Id {question.InternId}";
            HeaderText.BindingContext = this;
            PageFinished += StadiumPage_PageFinished;
            EvalButton.BindingContext = this;
            EvaluationTextColor = answersGiven >= answersNeeded ? Color.Green : Color.LightGray;
        }

        private void StadiumPage_PageFinished(object sender, PageResult e)
        {
            if (e != PageResult.Evaluation)
            {
                PlantInlinePicker.Reset();
                StadiumInlinePicker.Reset();
            }
            PageFinished -= StadiumPage_PageFinished;
        }

        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            int selectedStadium = 0;
            if (StadiumInlinePicker.SelectedItem != null)
            {
                 selectedStadium = (StadiumInlinePicker.SelectedItem as StadiumSubItem).InternNumber;
            }
            var selectedPlant = (PlantInlinePicker.SelectedItem as Plant)?.InternLetter;
            if (selectedPlant == null || selectedStadium == 0)
            {
                DisplayAlert("Hinweis", "Bitte vervollständigen Sie Ihre Auswahl um fortzufahren.", "OK");
                return;
            }

            AnswerItem = new AnswerStadiumPage(QuestionItem.InternId, selectedPlant, selectedStadium);
            PageFinished?.Invoke(this, PageResult.Continue);
        }

        void OnAuswertungButtonClicked(object sender, EventArgs e)
        {
            PageFinished?.Invoke(this, PageResult.Evaluation);
        }
        
        protected override bool OnBackButtonPressed()
        {
            PageFinished?.Invoke(this, PageResult.Abort);
            return true;
        }
    }
}