﻿using MobileDataCollection.Survey.Controls;
using System;
using System.Collections.Generic;
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
        /*public enum Stadium
        {
            None,
            Blattentwicklung,
            Bestockung,
            Schossen
        }
        public enum PlantType
        {
            None,
            A,
            B,
            C,
            D
        }
        public Stadium SelectedStadium { private set; get; }
        public PlantType SelectedPlantType { private set; get; }
        Dictionary<Stadium, ContentButton> StadiumButtonDictionary;
        Dictionary<PlantType, ContentButton> SelectedPlantType;*/
		public StadiumPage()
		{
			InitializeComponent();
            /*StadiumButtonDictionary = new Dictionary<Stadium, ContentButton>()
            {
                { Stadium.Blattentwicklung, BlattentwicklungButton},
                { Stadium.Bestockung, BestockungButton },
                { Stadium.Schossen, SchossenButton }
            };
            BlattentwicklungButton.Command = new Command(() => StadiumButtonClicked(Stadium.Blattentwicklung));
            BestockungButton.Command = new Command(() => StadiumButtonClicked(Stadium.Bestockung));
            SchossenButton.Command = new Command(() => StadiumButtonClicked(Stadium.Schossen));*/
        }
        /*
        private void StadiumButtonClicked(Stadium blattentwicklung)
        {
            foreach (var button in StadiumButtonDictionary.Values)
            {
                if (!(button.Content is Frame frame))
                    throw new NotImplementedException($"The Content of every Value in {nameof(StadiumButtonDictionary)} must be a frame.");
                frame.BorderColor = Color.LightGray;
            }
            (StadiumButtonDictionary[blattentwicklung].Content as Frame).BorderColor = Color.Black;
        }*/
    }
}