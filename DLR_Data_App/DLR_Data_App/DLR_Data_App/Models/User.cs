using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Essentials;
using SQLite;

/**
 * User Class
 */
namespace Login.Models
{
  public class User
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool Autologin { get; set; }

    public User() { }
    public User(string Username, string Password)
    {
      this.Username = Username;
      this.Password = Password;
      this.Autologin = false;
    }
  }
}
