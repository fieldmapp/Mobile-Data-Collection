using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;

namespace DLR_Data_App.ViewModels.CurrentProject
{
    /**
     * View model for edit page
     */
    public class EditDataViewModel : BaseViewModel
    {
        public List<ProjectForm> FormList { get; set; }
        public ObservableCollection<PreviewElement> ElementList { get; set; }
        public TableData ProjectData { get; set; }

        private Project _workingProject;

        /**
         * Constructor
         * @param elementNameList lists column names of datasets for project
         */
        public EditDataViewModel()
        {
            Title = AppResources.editdata;

            _workingProject = Database.GetCurrentProject();
            FormList = _workingProject.FormList;

            ElementList = new ObservableCollection<PreviewElement>();

            GetDataFromDb();
        }

        /**
         * Load data stored in db
         */
        public void GetDataFromDb()
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
