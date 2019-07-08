using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Models.ProjectModel.DatabaseConnectors;
using DLR_Data_App.Views;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Services
{
  /**
   * Class for creating and managing a project
   */
  class ProjectGenerator
  {
    Project workingProject;
    string zipfile;

    /**
     * Constructor
     */
    public ProjectGenerator(string file)
    {
      zipfile = file;
    }

    /**
     * Start the generating project process and runs all neccesary functions
     * - creating new project
     * - extract zip
     * - generate database tables
     * - generate forms
     * @returns bool Status
     */
    public async Task<bool> GenerateProject()
    {
      this.workingProject = new Project();
      bool status = false;
      
      // parsing form files
      Parser parser = new Parser(ref workingProject);
      status = await parser.ParseZip(zipfile, Path.Combine(App.FolderLocation, "unzip"));

      // check if parsing failed
      if (!status)
      {
        return false;
      }

      // create forms for project
      foreach (ProjectForm form in workingProject.Formlist)
      {
        GenerateForm(form);
      }
      
      // create tables for project
      status = GenerateDatabaseTable();
            
      return status;
    }
    
    /**
     * Create a database table which represents each form and 
     * @returns bool Status
     */
    private bool GenerateDatabaseTable()
    {
      // Insert current working project to database
      bool status = Database.InsertProject(ref workingProject);

      // check status
      if(!status)
      {
        return status;
      }

      // get created id from database and set it in workingProject
      var projectList = Database.ReadProjects();
      foreach(var project in projectList)
      {
        if(project.Title == workingProject.Title
          && project.Secret == workingProject.Secret
          && project.Authors == workingProject.Authors
          && project.Description == workingProject.Description)
        {
          workingProject.Id = project.Id;
        }
      }

      // get user id
      if(App.CurrentUser.Id == 0)
      {
        var userList = Database.ReadUser();
        foreach (var user in userList)
        {
          if (user.Username == App.CurrentUser.Username
            && user.Password == App.CurrentUser.Password)
          {
            App.CurrentUser.Id = user.Id;
          }
        }
      }
      

      // combine project and user
      ProjectUserConnection projectUserConnection = new ProjectUserConnection();
      projectUserConnection.ProjectId = workingProject.Id;
      projectUserConnection.UserId = App.CurrentUser.Id;
      projectUserConnection.Weight = 0;

      status = Database.Insert<ProjectUserConnection>(ref projectUserConnection);

      if (!status)
      {
        return status;
      }

      return status;
    }

    /**
     * Generating forms from parsed informations
     * @param form Elements that should be generated
     */
    private void GenerateForm(ProjectForm form)
    {
      ContentPage contentPage = new ContentPage();
      
      var scrollview = new ScrollView();
      contentPage.Content = scrollview;

      var stack = new StackLayout();

      // walk through list of elements and generate form containing elements
      foreach (ProjectFormElements element in form.ElementList)
      {
        // show name of element
        Label label = new Label
        {
          Text = element.Label
        };
        stack.Children.Add(label);

        // input text
        if (element.Type == "inputText")
        {
          Entry entry = new Entry
          {
            Placeholder = element.Hint,
            Keyboard = Keyboard.Default
          };

          stack.Children.Add(entry);
        }

        // Selecting one item of list
        if (element.Type == "inputSelectOne")
        {
          Picker picker = new Picker
          {
            Title = element.Label,
            VerticalOptions = LayoutOptions.CenterAndExpand
          };

          stack.Children.Add(picker);
        }

        // Show an entry with only numeric input
        // As a walkaround for an existing Samsung keyboard bug a normal keyboard layout is used
        if (element.Type == "inputNumeric")
        {
          Entry entry = new Entry
          {
            Placeholder = element.Hint,
            Keyboard = Keyboard.Default
          };

          stack.Children.Add(entry);
        }

        //------------------------
        // Special commands

        // Display a ruler on the side of the screen
        if (element.Type == "inputText" && element.Name.Contains("propRuler"))
        {

        }

        // Display a checkbox with name "unknown"
        if (element.Type == "inputText" && element.Name.Contains("propRuler"))
        {

        }

        // Add save button
        Button saveButton = new Button
        {
          Text = AppResources.save
        };

        // Add reset button
        Button resetButton = new Button
        {
          Text = AppResources.reset
        };
      }
    }
  }
}
