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
    }
}
