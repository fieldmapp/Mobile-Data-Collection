using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Services
{
  /*
   * Class for creating and managing a project
   */
  class ProjectGenerator
  {
    Database database;
    string projectName;
    string projectDescription;

    /*
     * Constructor
     */
    public ProjectGenerator()
    {
      this.database = new Database();

      // parsing form files
      ParseFiles();

      // create tables for project
      GenerateDatabaseTable();
    }

    /*
     * Collect files and parse informations
     */
    private void ParseFiles()
    {

    }
    
    /*
     * Create a database table which represents each form
     */
    private void GenerateDatabaseTable()
    {
      
    }
  }
}
