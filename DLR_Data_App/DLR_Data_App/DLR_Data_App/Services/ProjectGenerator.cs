using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Models.ProjectModel.DatabaseConnectors;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DLR_Data_App.Services
{
    /**
     * Class for creating and managing a project
     */
    public class ProjectGenerator
    {
        Project _workingProject;
        private readonly string _zipFile;

        /**
         * Constructor
         */
        public ProjectGenerator(string file)
        {
            _zipFile = file;
        }

        /**
         * Start the generating project process and runs all necessary functions
         * - creating new project
         * - extract zip
         * - generate database tables
         * - generate forms
         * @returns bool Status
         */
        public async Task<bool> GenerateProject()
        {
            _workingProject = new Project();
            bool status;

            // parsing form files
            Parser parser = new Parser(ref _workingProject);
            status = await parser.ParseZip(_zipFile, Path.Combine(App.FolderLocation, "unzip"));

            // check if parsing failed
            if (!status)
            {
                return false;
            }

            // create tables for project
            status = GenerateDatabaseTable();

            return status;
        }

        /**
         * Create a database table which represents each form and 
         * @returns bool Status
         */
        private bool GenerateDatabaseTable()
        {
            // Insert current working project with forms to database 
            var status = Database.InsertProject(ref _workingProject);

            // check status
            if (!status)
            {
                return false;
            }

            // get created id from database and set it in workingProject
            var projectList = Database.ReadProjects();
            foreach (var project in projectList)
            {
                if (project.Title == _workingProject.Title
                  && project.Authors == _workingProject.Authors
                  && project.Description == _workingProject.Description)
                {
                    _workingProject.Id = project.Id;
                }
            }

            // combine project and user
            var projectUserConnection = new ProjectUserConnection
            {
                ProjectId = _workingProject.Id,
                UserId = App.CurrentUser.Id,
                Weight = 0
            };

            status = Database.Insert(ref projectUserConnection);

            return status;
        }

    }
}
