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
        public bool Check_Information(string checkUsername)
        {
            // Check if input empty
            if (string.IsNullOrWhiteSpace(checkUsername))
            {
                return false;
            }
            
            var user = Database.ReadUsers().FirstOrDefault(u => u.Username == checkUsername);
            App.CurrentUser = user;
            return user != null;
        }
    }
}
