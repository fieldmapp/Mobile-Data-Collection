using DlrDataApp.Modules.ProfilingSharedModule;
using DlrDataApp.Modules.ProfilingSharedModule.Models;
using DlrDataApp.Modules.SharedModule;
using System.IO;
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
            var app = ProfilingSharedModule.Instance.ModuleHost.App;
            ProfilingData parsedProfiling = await ProfilingParser.ParseZip(_zipFile, Path.Combine(app.FolderLocation, "unzip"));
            if (parsedProfiling == null)
                return false;

            // create tables for project

            using (var dbConn = app.Database.CreateConnection())
            {
                var startPoint = Database.SaveTransactionPoint(dbConn);
                if (Database.Insert(parsedProfiling, dbConn))
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
