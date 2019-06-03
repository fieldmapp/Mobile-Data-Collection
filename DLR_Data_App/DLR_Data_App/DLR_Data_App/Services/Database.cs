using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SQLite;

namespace DLR_Data_App.Services
{
  /*
   * Database class to handle database access
   * 
   * https://docs.microsoft.com/de-de/xamarin/android/data-cloud/data-access/using-sqlite-orm
   */
  class Database
  {
    string dbName;
    string dbPath;

    /*
     * Constructor
     */
    public Database()
    {
      this.dbName = "dlrdata.sqlite";
      this.dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), this.dbName);

      // If database doesn't exist create a new database
      CreateDatabase()
    }

    /*
     * Create a new database file
     */
    public bool CreateDatabase()
    {
      var db = new SQLiteConnection(this.dbPath);
      return false;
    }

    /*
     * Create a new table schema
     * @param tablename Name of the table
     * @param header 2D-Array to define the fields inside of the new table
     */
    public bool CreateTable(string tablename, string[][] header)
    {
      return false;
    }

    /*
     * Edit an existing table schema
     */
    public bool EditTable()
    {
      return false;
    }

    /*
     * Delete an existing table
     */
    public bool DeleteTable()
    {
      return false;
    }
  }
}
