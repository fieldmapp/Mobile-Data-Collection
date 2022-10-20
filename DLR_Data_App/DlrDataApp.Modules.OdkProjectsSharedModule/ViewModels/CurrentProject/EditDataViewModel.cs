using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DlrDataApp.Modules.OdkProjects.Shared.Models;
using DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Localization;
using FormatWith;
using System.Linq;

namespace DlrDataApp.Modules.OdkProjects.Shared.ViewModels.CurrentProject
{
    public class EditDataViewModel : BaseViewModel
    {
        public List<ProjectForm> FormList { get; set; }
        public ObservableCollection<PreviewElement> ElementList { get; set; }

        private Project _workingProject;

        public List<ProjectResult> ProjectsData { private set; get; }

        public EditDataViewModel()
        {
            Title = SharedResources.editdata;

            _workingProject = OdkProjectsModule.Instance.Database.GetActiveElement<Project, ActiveProjectInfo>();
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
            ProjectsData = _workingProject.Results;
            ElementList.Clear();

            foreach (var result in _workingProject.Results)
            {
                var previewElement = new PreviewElement
                {
                    Timestamp = result.Data["Timestamp"],
                    Data = _workingProject.PreviewPattern.FormatWith(result.Data)
                };

                ProjectsData.Add(result);
                ElementList.Add(previewElement);
            }
        }
    }
}
