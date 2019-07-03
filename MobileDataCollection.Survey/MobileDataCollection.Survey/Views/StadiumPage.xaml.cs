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
        //Binding for Question
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem),typeof(QuestionStadiumPage), typeof(StadiumPage), null, BindingMode.OneWay);

        //Item of Question
        public QuestionStadiumPage QuestionItem
        {
            get { return (QuestionStadiumPage)GetValue(QuestionItemProperty); }
            set { SetValue(QuestionItemProperty, value); }
        }

        //Binding for Answer
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem),typeof(AnswerStadiumPage), typeof(StadiumPage), new AnswerStadiumPage(0, string.Empty, string.Empty), BindingMode.OneWay);

        //Item of Answer
        public AnswerStadiumPage AnswerItem
        {
            get { return (AnswerStadiumPage)GetValue(AnswerItemProperty); }
            set { SetValue(AnswerItemProperty, value); }
        }

        //Binding für Header
        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(string), typeof(StadiumPage), "demo", BindingMode.OneWay);
        //Header
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
            var selectedStadium = (StadiumInlinePicker.SelectedItem as StadiumSubItem)?.StadiumName;
            var selectedPlant = (PlantInlinePicker.SelectedItem as Plant)?.Name;
            if (selectedPlant == null || selectedStadium == null)
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