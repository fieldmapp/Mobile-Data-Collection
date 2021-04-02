﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using FormatWith;

namespace DlrDataApp.Modules.OdkProjectsSharedModule.ViewModels.CurrentProject
{
    public class EditDataViewModel : BaseViewModel
    {
        public List<ProjectForm> FormList { get; set; }
        public ObservableCollection<PreviewElement> ElementList { get; set; }
        public TableData ProjectTableData { get; set; }

        private Project _workingProject;

        public List<Dictionary<string, string>> ProjectsData { private set; get; }

        public EditDataViewModel()
        {
            Title = AppResources.editdata;

            _workingProject =  Database.GetCurrentProject();
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
            ProjectTableData = Database.ReadCustomTable(ref _workingProject);
            ElementList.Clear();

            if (ProjectTableData == null)
                return;

            var savesCount = ProjectTableData.ValueList[0].Count;
            ProjectsData = new List<Dictionary<string, string>>();

            // add the amount of elements that are available in db
            for (var i = 0; i < savesCount; i++)
            {
                Dictionary<string, string> saveData = new Dictionary<string, string>();
                for (int j = 0; j < ProjectTableData.RowNameList.Count; j++)
                {
                    //if this throws an exception, its probably due to the bad way ProjectTableData is structured (prone to corruption)
                    if (ProjectTableData.RowNameList[j] != "ProjectId")
                        saveData.Add(ProjectTableData.RowNameList[j], ProjectTableData.ValueList[j][i]);
                }

                var previewElement = new PreviewElement();
                previewElement.Timestamp = saveData["Timestamp"];
                previewElement.Data = _workingProject.PreviewPattern.FormatWith(saveData);

                ProjectsData.Add(saveData);
                ElementList.Add(previewElement);
            }
        }
    }
}
