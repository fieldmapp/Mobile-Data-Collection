using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.OdkProjects.Shared.ViewModels.CurrentProject;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DlrDataApp.Modules.OdkProjects.Shared.Views.CurrentProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditDataPage
    {
        private readonly EditDataViewModel _viewModel;
        public EditDataPage()
        {
            _viewModel = new EditDataViewModel();
            BindingContext = _viewModel;

            InitializeComponent();

            Appearing += EditDataPage_Appearing;
        }

        private void EditDataPage_Appearing(object sender, EventArgs e)
        {
            _viewModel.GetDataFromDatabase();
        }


        private void DataList_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            _ = Shell.Current.Navigation.PushPage(new EditDataDetailPage(_viewModel.ProjectsData[e.ItemIndex]));
        }
    }
}