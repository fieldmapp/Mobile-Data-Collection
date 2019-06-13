using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DLR_Data_App.Models;
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
    /*
     * Constructor
     */
    public Database()
    {     
    }

    /*
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

    /*
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

      // check if data was successfully inserted
      if (resultUpdate > 0)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /*
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

    /*
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

    /*
     * Return all projects
     * @return List of all projects in database
     */
    public static List<Project> ReadProjects()
    {
      List<Project> result;

      // database will be closed after leaving the using statement
      using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
      {
        // if table doesn't exist create a new one
        conn.CreateTable<Project>();

        // get content of table
        result = conn.Table<Project>().ToList();
      }

      return result;
    }

    

  }
}
