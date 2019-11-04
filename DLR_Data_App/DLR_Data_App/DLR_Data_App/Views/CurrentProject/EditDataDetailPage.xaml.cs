using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.CurrentProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditDataDetailPage
    {
        private readonly Project _workingProject = Database.GetCurrentProject();
        public List<View> ElementList = new List<View>();

        public EditDataDetailPage(Dictionary<string,string> projectData)
        {
            void WriteInfoToView(View element)
            {
                if (element is Entry entry)
                {
                    entry.Text = projectData[entry.StyleId];
                }
                else if (element is Picker picker)
                {
                    picker.SelectedIndex = picker.Items.IndexOf(projectData[picker.StyleId]);
                }
                else if (element is Label label && label.StyleId != null)
                {
                    var containedInfo = string.Empty;
                    if (label.StyleId.Contains("Lat"))
                        containedInfo = "Lat";
                    else if (label.StyleId.Contains("Long"))
                        containedInfo = "Long";
                    if (containedInfo != string.Empty)
                    {
                        var styleId = label.StyleId.Replace(containedInfo, string.Empty);
                        var locationInfo = projectData[styleId];
                        var locationStartIndex = locationInfo.IndexOf(containedInfo) + $"{containedInfo}:".Length;
                        string locationString = string.Empty;
                        for (int i = locationStartIndex; i < locationInfo.Length; i++)
                        {
                            if (locationInfo[i] == ' ')
                                break;
                            locationString += locationInfo[i];
                        }
                        label.Text = locationString;
                    }
                }
            }

            InitializeComponent();

            if (_workingProject == null)
            {
                Title = AppResources.currentproject;
            }
            else
            {
                var translatedProject = Helpers.TranslateProjectDetails(_workingProject);
                Title = translatedProject.Title;
                return;
            }

            var pages = UpdateView();

            foreach (var contentPage in pages)
            {
                Children.Add(contentPage);
            }

            Helpers.WalkElements(pages, WriteInfoToView);
        }

        


        protected override bool OnBackButtonPressed()
        {
            Navigation.PopAsync();
            return true;
        }

        public List<ContentPage> UpdateView()
        {
            var _pages = new List<ContentPage>();

            if (_workingProject == null)
                return _pages;

            foreach (var projectForm in _workingProject.FormList)
            {
                var content = FormFactory.GenerateForm(projectForm, _workingProject, DisplayAlert);
                ElementList.AddRange(content.ElementList);
                _pages.Add(content.Form);
            }

            return _pages;
        }
    }
}