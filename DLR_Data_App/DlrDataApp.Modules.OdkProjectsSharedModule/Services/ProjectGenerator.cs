using DlrDataApp.Modules.OdkProjects.Shared;
using DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjects.Shared.Services;
using SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using DlrDataApp.Modules.Base.Shared.Services;

namespace DlrDataApp.Modules.OdkProjects.Shared.Services
{
    public class ProjectGenerator
    {
        Project _workingProject;
        private readonly string _zipFile;
        Database Database => OdkProjectsModule.Instance.Database;

        public ProjectGenerator(string file)
        {
            _zipFile = file;
        }

        /// <summary>
        /// Starts the project generation process and runs all necessary functions: creating new project, extract zip, generate database tables, generate forms.
        /// </summary>
        public async Task<bool> GenerateProject()
        {
            _workingProject = new Project();

            // parsing form files
            ProjectParser parser = new ProjectParser(_workingProject);
            var moduleHost = OdkProjectsModule.Instance.ModuleHost;

            // check if parsing failed
            if (!await parser.ParseZip(_zipFile, Path.Combine(moduleHost.App.FolderLocation, "unzip")))
                return false;

            // create tables for project

            using (var dbConn = moduleHost.App.Database.CreateConnection())
            {
                var startPoint = Database.SaveTransactionPoint(dbConn);
                if (GenerateDatabaseTable(dbConn))
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
        
        /// <summary>
        /// Creates a database table which represents each form.
        /// </summary>
        private bool GenerateDatabaseTable(SQLiteConnection dbConn)
        {
            if (!Database.InsertOrUpdateWithChildren(_workingProject, dbConn))
                return false;

            // TODO: somewhere ProjectUserConnection went missing. Reintroduce it!
            // combine project and user
            //var projectUserConnection = new ProjectUserConnection
            //{
            //    ProjectId = _workingProject.Id,
            //    UserId = App.CurrentUser.Id
            //};
            // 
            // return Database.Insert(projectUserConnection, dbConn);

            return true;
        }

    }
}
