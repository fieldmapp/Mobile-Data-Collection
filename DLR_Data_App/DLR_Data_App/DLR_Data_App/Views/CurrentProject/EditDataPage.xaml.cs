﻿using System;
using System.ComponentModel;
using DLR_Data_App.Localizations;
using DLR_Data_App.ViewModels.CurrentProject;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.CurrentProject
{
    /**
     * Class for editing data in database
     */
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditDataPage
    {
        private readonly EditDataViewModel _viewModel;

        /**
         * Constructor
         */
        public EditDataPage()
        {
            _viewModel = new EditDataViewModel();
            BindingContext = _viewModel;

            InitializeComponent();
        }

        /**
         * Display help message
         */
        public async void HelpClicked(object sender, EventArgs eventArgs)
        {
            await DisplayAlert(AppResources.help, AppResources.edithelp, AppResources.okay);
        }

        /**
         * Date changed
         */
        private void DateSelection_OnDateSelected(object sender, DateChangedEventArgs e)
        {
            var dateTime = DateSelection.Date + TimeSelection.Time;
            DataList.SelectedItem = _viewModel.UpdateSelection(dateTime);
        }

        /**
         * Time changed
         */
        private void TimeSelection_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var dateTime = DateSelection.Date + TimeSelection.Time;
            DataList.SelectedItem = _viewModel.UpdateSelection(dateTime);
        }

        /**
         * Data set selected
         */
        private void DataList_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            Navigation.PushAsync(new EditDataDetailPage());
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopAsync();
            return true;
        }
    }
}