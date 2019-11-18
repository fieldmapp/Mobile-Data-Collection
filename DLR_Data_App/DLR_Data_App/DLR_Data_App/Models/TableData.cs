using System.Collections.Generic;

namespace DLR_Data_App.Models
{
    /// <summary>
    /// Model used for handling content of database
    /// </summary>
    public class TableData
    {
        public List<string> RowNameList { get; set; }
        public List<List<string>> ValueList { get; set; }

        public TableData()
        {
            RowNameList = new List<string>();
            ValueList = new List<List<string>>();
        }
    }
}
