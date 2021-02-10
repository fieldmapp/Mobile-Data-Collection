using System;
using System.Collections.Generic;
using System.ComponentModel;
using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using DLR_Data_App.ViewModels.CurrentProject;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.CurrentProject
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
            var projectData = new Dictionary<string, string>();
            for (int i = 0; i < _viewModel.ProjectData.RowNameList.Count; i++)
            {
                if (_viewModel.ProjectData.RowNameList[i] != "ProjectId")
                    projectData.Add(_viewModel.ProjectData.RowNameList[i], _viewModel.ProjectData.ValueList[i][e.ItemIndex]);
            }
            _ = this.PushPage(new EditDataDetailPage(projectData));
        }
    }
}