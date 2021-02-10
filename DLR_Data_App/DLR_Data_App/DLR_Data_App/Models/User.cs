using SQLite;

namespace DLR_Data_App.Models
{
    /// <summary>
    /// Model for users
    /// </summary>
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        
        public string Username { get; set; }
    }
}
