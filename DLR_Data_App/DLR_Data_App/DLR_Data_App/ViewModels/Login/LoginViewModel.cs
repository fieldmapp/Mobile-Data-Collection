using System.Collections.Generic;
using System.Linq;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using DlrDataApp.Modules.Base.Shared;

namespace DLR_Data_App.ViewModels.Login
{
    public class LoginViewModel
    {
        /// <summary>
        /// Checks login with stored data
        /// </summary>
        public bool Check_Information(string checkUsername)
        {
            // Check if input empty
            if (string.IsNullOrWhiteSpace(checkUsername))
            {
                return false;
            }
            
            var user = (App.Current as App).Database.Read<User>().FirstOrDefault(u => u.Username == checkUsername);
            (App.Current as App).CurrentUser = user;
            return user != null;
        }
    }
}
