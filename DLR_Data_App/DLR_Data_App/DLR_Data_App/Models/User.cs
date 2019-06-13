using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Essentials;
using SQLite;

namespace DLR_Data_App.Models
{
  /**
   * Model for users
   */
  public class User
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Name of the user
    public string Username { get; set; }

    // Password stored in SHA512
    public string Password { get; set; }

    // Autologin enabled check
    public bool Autologin { get; set; }

    public User()
    {
      Autologin = false;
    }

    public User(string username, string password)
    {
      Username = username;
      Password = password;
      Autologin = false;
    }
  }
}
