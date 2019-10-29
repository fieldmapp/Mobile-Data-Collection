using System.Collections.Generic;
using System.Linq;
using DLR_Data_App.Models;
using DLR_Data_App.Services;

namespace DLR_Data_App.ViewModels.Login
{
    /**
     * View model for login
     */
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
            if (string.IsNullOrWhiteSpace(checkUsername) || string.IsNullOrWhiteSpace(checkPassword))
            {
                return false;
            }

            var matchedUser = userList.FirstOrDefault(u => u.Username == checkUsername && u.Password == checkPassword);
            App.CurrentUser = matchedUser;
            return matchedUser != null;
        }
    }
}
