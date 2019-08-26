using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DLR_Data_App.Localizations;
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
      public ObservableCollection<ProjectFormElements> ElementList { get; set; }
      public ProjectForm SelectedProjectForm { get; set; }

      private Project _workingProject;
      private List<string> _elementNameList;
      
      /**
       * Constructor
       * @param elementNameList lists column names of datasets for project
       */
      public EditDataViewModel(List<string> elementNameList)
      {
        Title = AppResources.editdata;
        _elementNameList = elementNameList;

        _workingProject = Database.GetCurrentProject();
        FormList = _workingProject.FormList;

        ElementList = new ObservableCollection<ProjectFormElements>();
      }

      /**
       * Update List of selected data sets
       */
      public void UpdateList()
      {
        Database.ReadCustomTable(ref _workingProject, ref _elementNameList);

        ElementList.Clear();

      }

      public void UpdateSelection(DateTime dateTime)
      {
        
      }
    }
}
