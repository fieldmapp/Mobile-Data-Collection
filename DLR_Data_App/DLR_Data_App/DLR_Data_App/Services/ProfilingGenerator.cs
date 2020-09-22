using DLR_Data_App.Models.Profiling;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLR_Data_App.Services
{
    public static class ProfilingGenerator
    {
        /// <summary>
        /// Starts the project generation process and runs all necessary functions: creating new project , extract zip, generate database tables, generate forms.
        /// </summary>
        public static async Task<bool> GenerateProfiling(string _zipFile)
        {
            ProfilingData parsedProfiling = await ProfilingParser.ParseZip(_zipFile, Path.Combine(App.FolderLocation, "unzip"));
            if (parsedProfiling == null)
                return false;

            // create tables for project

            using (var dbConn = Database.CreateConnection())
            {
                var startPoint = Database.SaveTransactionPoint(dbConn);
                if (Database.InsertProfiling(ref parsedProfiling, dbConn))
                {
                    Database.CommitChanges(startPoint, dbConn);
                    return true;
                }
                else
                {
                    Database.RollbackChanges(startPoint, dbConn);
                    return false;
                }
            }
        }
    }
}
