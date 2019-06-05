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
        ObservableCollection<StadiumSubItem> TestCollection = new ObservableCollection<StadiumSubItem>()
        {
            new StadiumSubItem(){ImageSource = ImageSource.FromResource("schossen.png"), StadiumName="Schossen"},
            new StadiumSubItem(){ImageSource = ImageSource.FromResource("bestockung.png"), StadiumName="Bestockung"},
            new StadiumSubItem(){ImageSource = ImageSource.FromResource("blattentwicklung.png"), StadiumName="Blattentwicklung"}
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
			InitializeComponent();
            DemoInlinePicker.ItemSource = TestCollection;
            DemoInlinePicker2.ItemSource = TestCollection2;
        }
    }
}