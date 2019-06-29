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
	public partial class StadiumPage : ContentPage
	{
        public int AnswersGiven { get; set; }
        //Answers needed to complete Question Category (Muss definiert werden: entweder alle verfügbaren Fragen oder feste Zahl)
        public int AnswersNeeded { get; set; }

        //Binding for Question
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem),typeof(QuestionStadiumPage), typeof(StadiumPage), new QuestionStadiumPage(0, 0, null, null, string.Empty, string.Empty), BindingMode.OneWay);

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
        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header),
            typeof(String), typeof(DoubleSliderPage), "demo", BindingMode.OneWay);
        //Header
        public String Header
        {
            get { return (String)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        ObservableCollection<StadiumSubItem> TestCollection = new ObservableCollection<StadiumSubItem>()
        {
            new StadiumSubItem(){ImageSource = "schossen.png", StadiumName="Schossen"},
            new StadiumSubItem(){ImageSource = "bestockung.png", StadiumName="Bestockung"},
            new StadiumSubItem(){ImageSource = "blattentwicklung.png", StadiumName="Blattentwicklung"}
        };
        ObservableCollection<Plant> TestCollection2 = new ObservableCollection<Plant>()
        {
            new Plant(){Name="Kartoffel"},
            new Plant(){Name="Mais" },
            new Plant(){Name="Weizen" },
            new Plant(){Name="Zuckerrübe" }
        };
        public StadiumPage()
		{
            this.AnswersGiven = 1;
            InitializeComponent();
            DemoInlinePicker.ItemSource = TestCollection;
            DemoInlinePicker2.ItemSource = TestCollection2;
            QuestionNumber.BindingContext = this;
            this.Header = $"Frage {this.AnswersGiven}/{this.AnswersNeeded + 1}";
        }
        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            //TBD:
            //Warning (?)
            //Return to Homescreen
            if (this.AnswersGiven < this.AnswersNeeded)
            {
                this.AnswersGiven++;
                // Set new Question
                this.Header = $"Frage {this.AnswersGiven}/{this.AnswersNeeded + 1}";
            }
        }
        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {
            //TBD:
            //Warning (?)
            //Return to Homescreen
        }
    }
}