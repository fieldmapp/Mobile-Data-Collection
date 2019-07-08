using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DLR_Data_App.Models;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Models.ProjectModel.DatabaseConnectors;
using SQLite;

namespace DLR_Data_App.Services
{
  /**
   * Database class to handle database access
   * 
   * https://docs.microsoft.com/de-de/xamarin/android/data-cloud/data-access/using-sqlite-orm
   */
  class Database
  {
    /**
     * Constructor
     */
    public Database()
    {     
    }

    /**
     * Insert data into the database
     * @param data The contents that will pushed into the database
     * @return Status of inserting data
     */
    public static bool Insert<T>(ref T data)
    {
      int resultInsert = 0;

      // database will be closed after leaving the using statement
      using (SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
      {
        // if table doesn't exist create a new one
        conn.CreateTable<T>();

        // Insert data into table
        resultInsert = conn.Insert(data);
      }

      // check if data was successfully inserted
      if (resultInsert > 0)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /**
     * Update data in database
     * @param data The contents that will updated in the database
     * @return Status of updating data
     */
    public static bool Update<T>(ref T data)
    {
      int resultUpdate = 0;

      // database will be closed after leaving the using statement
      using (SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
      {
        // if table doesn't exist create a new one
        conn.CreateTable<T>();

        // Update data into table
        resultUpdate = conn.Update(data);
      }

      // check if data was successfully updated
      if (resultUpdate > 0)
      {
        return true;
      }
      else
      {
        // if no element was updated the element is inserted into the database
        Insert<T>(ref data);
        return false;
      }
    }

    /**
     * Delete data from database
     * @param data The contents that will deleted from the database
     * @return Status of deleting data
     */
    public static bool Delete<T>(ref T data)
    {
      int resultDelete = 0;

      // database will be closed after leaving the using statement
      using (SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
      {
        // Update data into table
        resultDelete = conn.Delete(data);
      }

      // check if data was successfully deleted
      if (resultDelete > 0)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /**
     * Return all users
     * @return List of all users in database
     */
    public static List<User> ReadUser()
    {
      List<User> result;

      // database will be closed after leaving the using statement
      using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
      {
        // if table doesn't exist create a new one
        conn.CreateTable<User>();

        // get content of table
        result = conn.Table<User>().ToList();
      }

      return result;
    }

    /**
     * Stores project in database
     * Own implementation of foreign keys caused by missing function in Xamarin SQL
     */
    public static bool InsertProject(ref Project project)
    {
      // Load current stored data to project list
      List<Project> projectList = Database.ReadProjects();

      // Check if project already exists and abort insertion if it does
      foreach(Project p in projectList)
      {
        if(p.Title == project.Title)
        {
          return false;
        }
      }

      // Insert project to database
      Database.Insert<Project>(ref project);

      // Add form to project
      foreach (ProjectForm form in project.Formlist)
      {
        ProjectForm formElement = form;
        Database.Insert<ProjectForm>(ref formElement);

        foreach (ProjectFormElements elements in form.ElementList)
        {
          ProjectFormElements controlElement = elements;
          Database.Insert<ProjectFormElements>(ref controlElement);
        }

        ProjectFormMetadata metadataElement = form.Metadata;
        Database.Insert<ProjectFormMetadata>(ref metadataElement);
      }
      return true;
    }

    /**
     * Return all projects
     * @return List of all projects in database
     */
    public static List<Project> ReadProjects()
    {
      List<Project> projectList = new List<Project>();

      // database will be closed after leaving the using statement
      using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
      {
        // if table doesn't exist create a new one
        conn.CreateTable<Project>();

        // get content of table
        projectList = conn.Table<Project>().ToList();
      }

      for(int projectIterator = 0; projectIterator<projectList.Count; projectIterator++)
      {
        Project tempProject = projectList[projectIterator];
        //ReadForms(ref tempProject);
        projectList[projectIterator] = tempProject;
      }
      
      return projectList;
    }

    /**
     * Return all forms for a project
     * @return List of all projects in database
     */
    public static bool ReadForms(ref Project project)
    {
      List<ProjectForm> result;

      // database will be closed after leaving the using statement
      using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
      {
        // if table doesn't exist create a new one
        conn.CreateTable<ProjectFormList>();

        var formList = conn.Query<int>("select formlist.FormId from ProjectFormList where formlist.ProjectId == ?", project.Id);

        // get content of table
        result = conn.Table<ProjectForm>().ToList();
      }

      project.Formlist = result;

      return true;
    }

    /**
     * Create custom table
     */
    public static bool CreateCustomTable(string tableName, List<Field> fieldList)
    {
      bool status = false;

      // Generate query for creating a new table
      string query = "CREATE TABLE " + tableName + "(";
      query += "Id Int PRIMARY KEY AUTOINCREMENT,";
      query += "ProjectId Int,";
      query += "FormId Int,";

      foreach(Field field in fieldList)
      {
        query += field.FieldName + " " + field.FieldType + ",";
      }

      query.Remove(query.Length-1);
      query += ");";

      using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
      {
        try
        {
          conn.Execute(query);
          status = true;
        } catch
        {
          status = false;
        }
        
      }

      return status;
    }

    /**
     * Read custom table
     */
    public static bool ReadCustomTable(string tableName)
    {
      bool status = false;


      return status;
    }

    /**
     * Removes database file
     * @return boolean if removal was successful
     */
    public static bool RemoveDatabase()
    {
      try
      {
        if (File.Exists(App.DatabaseLocation))
        {
          File.Delete(App.DatabaseLocation);
        }
        return true;
      }
      catch (IOException)
      {
        return false;
      }
    }

    /**
     * Select project as current project and deselect other project as current project
     * @param project Select project as current project
     * @return Status of setting new current project
     */
    public static bool SelectCurrentProject(Project project)
    {
      bool result = false;
      Project oldCurrentProject = GetCurrentProject();

      // Get current project and deselect it
      if (oldCurrentProject != null)
      {
        oldCurrentProject.CurrentProject = false;
        Update<Project>(ref oldCurrentProject);
      }
      
      // Set new current project
      project.CurrentProject = true;
      result = Update<Project>(ref project);

      return result;
    }

    /**
     * Returns the current project
     * @return current project
     */
    public static Project GetCurrentProject()
    {
      List<Project> projectList = new List<Project>();

      using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
      {
        // if table doesn't exist create a new one
        conn.CreateTable<Project>();

        // get first element of project list which has current project set as true
        projectList = conn.Query<Project>("SELECT * FROM Project WHERE CurrentProject = 1");
      }

      if(projectList.Count == 1)
      {
        return projectList[0];
      } else
      {
        // if more than one project is selected as current project, deselect all
        for (int i = 0; i < projectList.Count; i++)
        {
          Project oldCurrentProject = projectList[i];
          oldCurrentProject.CurrentProject = false;
          Update<Project>(ref oldCurrentProject);
        }
        return null;
      }
    }
  }
}
