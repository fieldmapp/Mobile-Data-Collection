//Main contributors: Maya Koehnen
using DlrDataApp.Modules.ProfilingSharedModule.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DlrDataApp.Modules.ProfilingSharedModule.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EvaluationMainPage : ContentPage
    {
        /// <summary>
        /// This class is the csharp definition of the Design of the main evaluation page.
        /// It has as property an ObservableCollection of EvaluationItems. An EvaluationItem has a name, an overall 
        /// result (percentage) and three results for each difficulty level (percentages)
        /// </summary>
        /// Definition of the  ObservableCollection of EvaluationItems (Here with dummy data)
        public ObservableCollection<EvaluationItem> EvaluationItems;
        /// Constructor of the MainPage
        public EvaluationMainPage(List<EvaluationItem> evalItems)
        {
            ///Initialize the Components
            InitializeComponent();
            EvaluationItems = new ObservableCollection<EvaluationItem>(evalItems);
            ///Set the ItemSource for the ListView which displays the list of results for the different question categories
            CatList.ItemsSource = EvaluationItems;
        }
    }
}