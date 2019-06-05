using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.ModelForDatabank
{
    class Student
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string name { get; set; }
        public string address { get; set; }
    }
}
