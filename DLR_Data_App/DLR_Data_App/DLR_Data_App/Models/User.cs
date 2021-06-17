using DlrDataApp.Modules.Base.Shared;
using SQLite;

namespace DLR_Data_App.Models
{
    /// <summary>
    /// Model for users
    /// </summary>
    public class User : IUser
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        
        public string Username { get; set; }
    }
}
