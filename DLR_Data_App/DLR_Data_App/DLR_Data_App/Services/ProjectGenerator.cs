using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Models.ProjectModel.DatabaseConnectors;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DLR_Data_App.Services
{
    public class ProjectGenerator
    {
        Project _workingProject;
        private readonly string _zipFile;

        public ProjectGenerator(string file)
        {
            _zipFile = file;
        }

        /// <summary>
        /// Starts the project generation process and runs all necessary functions: creating new project , extract zip, generate database tables, generate forms.
        /// </summary>
        public async Task<bool> GenerateProject()
        {
            _workingProject = new Project();

            // parsing form files
            Parser parser = new Parser(ref _workingProject);

            // check if parsing failed
            if (!await parser.ParseZip(_zipFile, Path.Combine(App.FolderLocation, "unzip")))
                return false;

            // create tables for project
            return GenerateDatabaseTable();
        }
        
        /// <summary>
        /// Creates a database table which represents each form.
        /// </summary>
        private bool GenerateDatabaseTable()
        {
            if (!Database.InsertProject(ref _workingProject))
                return false;

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
                UserId = App.CurrentUser.Id
            };

            return Database.Insert(ref projectUserConnection);
        }

    }
}
