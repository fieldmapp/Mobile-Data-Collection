using System.Collections.Generic;
using DLR_Data_App.Models;
using DLR_Data_App.Services;

namespace DLR_Data_App.ViewModels.Login
{
  public class LoginViewModel
  {
    /**
     * Checks login with stored data
     */
    public bool Check_Information(string checkUsername, string checkPassword)
    {
      // Get all users from database
      List<User> userList = Database.ReadUser();

      // Check if input empty
      if (checkUsername == ""
          || checkPassword == "")
      {
        return false;
      }

      // Check if entry match with user in database
      foreach (User user in userList)
      {
        // If user is found set as current user
        if (user.Username == checkUsername
            && user.Password == checkPassword)
        {
          App.CurrentUser = user;
          return true;
        }
      }

      return false;
    }
  }
}
