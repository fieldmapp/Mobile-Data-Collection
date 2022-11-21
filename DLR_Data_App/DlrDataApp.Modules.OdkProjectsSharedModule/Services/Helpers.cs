using DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel;
using DlrDataApp.Modules.Base.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlrDataApp.Modules.Base.Shared.Services;

namespace DlrDataApp.Modules.OdkProjects.Shared.Services
{
    static class Helpers
    {
        /// <summary>
        /// Translates the details of a project to the runtime system language
        /// </summary>
        /// <param name="project">Project to be translated</param>
        /// <returns>New project with details in correct language (or with hints that the detail is missing)</returns>
        public static Project TranslateProjectDetails(Project project)
        {
            var authors = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(project.Authors, project.Languages);
            if (string.IsNullOrWhiteSpace(authors))
            {
                authors = SharedResources.noauthor;
            }

            var title = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(project.Title, project.Languages);
            if (string.IsNullOrWhiteSpace(title))
            {
                title = SharedResources.notitle;
            }

            var description = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(project.Description, project.Languages);
            if (string.IsNullOrWhiteSpace(description))
            {
                description = SharedResources.nodescription;
            }

            var tempProject = new Project
            {
                Authors = authors,
                Title = title,
                Description = description
            };


            return tempProject;
        }

        /// <summary>
        /// Translates the details of multiple projects to the runtime system language
        /// </summary>
        /// <param name="project">Projects to be translated</param>
        /// <returns>New projects with details in correct language (or with hints that the detail is missing)</returns>
        public static List<Project> TranslateProjectDetails(List<Project> projectList) => projectList.Select(TranslateProjectDetails).ToList();

        /// <summary>
        /// Creates the unique table name of a project. This is based on the title and the id.
        /// </summary>
        /// <param name="project">Project of which the table name should be retured.</param>
        /// <returns>Unique table name for the given project.</returns>
        public static string GetTableName(this Project project) => OdkDataExtractor.GetEnglishStringFromJsonList(project.Title, project.Languages) + "_" + project.Id;

        public static Project GetCurrentProject(this Database db)
        {
            return db.GetActiveElement<Project, ActiveProjectInfo>();
        }

        public static bool SetCurrentProject(this Database db, Project project)
        {
            return db.SetActiveElement<Project, ActiveProjectInfo>(project);
        }
    }
}
