using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;

namespace DLR_Data_App.ViewModels.CurrentProject
{
    public class EditDataViewModel : BaseViewModel
    {
        public List<ProjectForm> FormList { get; set; }
        public ObservableCollection<PreviewElement> ElementList { get; set; }
        public TableData ProjectData { get; set; }

        private Project _workingProject;
        
        public EditDataViewModel()
        {
            Title = AppResources.editdata;

            _workingProject = Database.GetCurrentProject();
            FormList = _workingProject.FormList;

            ElementList = new ObservableCollection<PreviewElement>();

            GetDataFromDatabase();
        }

        /// <summary>
        /// Loads data stored in database.
        /// </summary>
        public void GetDataFromDatabase()
        {
            // Get data from DB
            ProjectData = Database.ReadCustomTable(ref _workingProject);
            ElementList.Clear();

            if (ProjectData == null)
                return;

            // add the amount of elements that are available in db
            for (var i = 0; i < ProjectData.ValueList[0].Count; i++)
            {
                ElementList.Add(new PreviewElement());
            }

            for (int rowNameCounter = 0; rowNameCounter < ProjectData.ValueList.Count; rowNameCounter++)
            {
                var dataRow = ProjectData.ValueList[rowNameCounter];
                for (int elementCounter = 0; elementCounter < dataRow.Count; elementCounter++)
                {
                    var dataElement = dataRow[elementCounter];
                    if (ProjectData.RowNameList[rowNameCounter] == "Timestamp")
                        ElementList[elementCounter].Timestamp = dataElement ?? AppResources.corruptentry;
                    else
                        ElementList[elementCounter].Data += dataElement + " ; ";
                }
            }
        }
    }
}
