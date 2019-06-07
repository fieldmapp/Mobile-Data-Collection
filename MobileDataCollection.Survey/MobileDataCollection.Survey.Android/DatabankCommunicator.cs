using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using SQLite;
using System.IO;
using MobileDataCollection.Survey.Models;

[assembly:Dependency(typeof(DatabankCommunicator))]
namespace MobileDataCollection.Survey.Models
{
    public class DatabankCommunicator //: ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var dbName = "QuestionDatabank.sqlite";
            var dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(dbPath, dbName);
            var conn = new SQLiteConnection(path);
            return conn;
        }



    }
}