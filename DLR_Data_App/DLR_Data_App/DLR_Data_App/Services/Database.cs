using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DLR_Data_App.Models;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Models.ProjectModel.DatabaseConnectors;
using SQLite;

namespace DLR_Data_App.Services
{
    /**
     * Database class to handle database access
     * 
     * https://docs.microsoft.com/de-de/xamarin/android/data-cloud/data-access/using-sqlite-orm
     */
    public class Database
    {
        private static void CheckValidSqlName(string name)
        {
            bool isValidChar(char c) => char.IsLetterOrDigit(c) || c == '_';
            if (!name.All(isValidChar))
                throw new ArgumentException($"{nameof(name)} is not a valid SQL name");
        }

        /**
         * Insert data into the database
         * @param data The contents that will pushed into the database
         * @return Status of inserting data
         */
        public static bool Insert<T>(ref T data)
        {
            int resultInsert;

            // database will be closed after leaving the using statement
            using (var conn = new SQLiteConnection(App.DatabaseLocation))
            {
                // if table doesn't exist create a new one
                conn.CreateTable<T>();

                // Insert data into table
                resultInsert = conn.Insert(data);
            }

            // check if data was successfully inserted
            return resultInsert > 0;
        }

        /**
         * Update data in database
         * @param data The contents that will updated in the database
         * @return Status of updating data
         */
        public static bool Update<T>(ref T data)
        {
            int resultUpdate;

            // database will be closed after leaving the using statement
            using (var conn = new SQLiteConnection(App.DatabaseLocation))
            {
                // if table doesn't exist create a new one
                conn.CreateTable<T>();

                // Update data into table
                resultUpdate = conn.Update(data);
            }

            // check if data was successfully updated
            if (resultUpdate > 0)
            {
                return true;
            }
            else
            {
                // if no element was updated the element is inserted into the database
                Insert(ref data);
                return false;
            }
        }

        /**
         * Delete data from database
         * @param data The contents that will deleted from the database
         * @return Status of deleting data
         */
        public static bool Delete<T>(ref T data)
        {
            int resultDelete;

            // database will be closed after leaving the using statement
            using (var conn = new SQLiteConnection(App.DatabaseLocation))
            {
                // Update data into table
                resultDelete = conn.Delete(data);
            }

            // check if data was successfully deleted
            return resultDelete > 0;
        }

        /**
         * Return all users
         * @return List of all users in database
         */
        public static List<User> ReadUser()
        {
            List<User> result;

            // database will be closed after leaving the using statement
            using (var conn = new SQLiteConnection(App.DatabaseLocation))
            {
                // if table doesn't exist create a new one
                conn.CreateTable<User>();

                // get content of table
                result = conn.Table<User>().ToList();
            }

            return result;
        }

        /**
         * Delete project with its forms
         * @param project Project that should be deleted
         */
        public static void DeleteProject(Project project)
        {
            var queryForms = "DELETE FROM ProjectForm WHERE Id=?";
            var queryFormElementList = "DELETE FROM ProjectFormElementList WHERE ElementId=?";
            var queryFormElements = "DELETE FROM ProjectFormElements WHERE Id=?";
            //var queryFormMetadata = "DELETE FROM ProjectFormMetadata WHERE Id=?";
            var queryUserConnection = "DELETE FROM ProjectUserConnection WHERE ProjectId=?";

            using (var conn = new SQLiteConnection(App.DatabaseLocation))
            {
                // remove all elements of a form
                foreach (var form in project.FormList)
                {
                    // remove elements
                    foreach (var element in form.ElementList)
                    {
                        conn.Execute(queryFormElements, element.Id);
                        conn.Execute(queryFormElementList, element.Id);
                    }

                    // remove metadata
                    // conn.Execute(queryFormMetadata, form.Metadata.Id);

                    // remove form
                    conn.Execute(queryForms, form.Id);
                }

                // remove connection between user and project
                conn.Execute(queryUserConnection, project.Id);

                // remove project
                conn.Delete(project);
            }
        }

        /**
         * Stores project in database
         * Own implementation of foreign keys caused by missing function in Xamarin SQL
         */
        public static bool InsertProject(ref Project project)
        {
            bool status;

            // Load current stored data to project list
            var projectList = ReadProjects();

            // Check if project already exists and abort insertion if it does
            foreach (var p in projectList)
            {
                if (p.Title == project.Title)
                {
                    return false;
                }
            }

            // Insert project to database
            status = Insert(ref project);
            if (!status)
            {
                return false;
            }

            // Add form to project
            foreach (var form in project.FormList)
            {
                var formElement = form;
                formElement.ProjectId = project.Id;
                status = Insert(ref formElement);
                if (!status)
                {
                    return false;
                }

                var metadata = form.Metadata;
                status = Insert(ref metadata);
                if (!status)
                {
                    return false;
                }

                foreach (var elements in form.ElementList)
                {
                    // store each form with its controls
                    var controlElement = elements;
                    status = Insert(ref controlElement);
                    if (!status)
                    {
                        return false;
                    }

                    // combine project with control elements
                    var combineElementList = new ProjectFormElementList
                    {
                        ElementId = controlElement.Id,
                        FormId = formElement.Id,
                        MetadataId = form.Metadata.Id
                    };
                    status = Insert(ref combineElementList);
                    if (!status)
                    {
                        return false;
                    }
                }
            }

            // Create custom table
            status = CreateCustomTable(ref project);

            return status;
        }

        /**
         * Return all projects
         * @return List of all projects in database
         */
        public static List<Project> ReadProjects()
        {
            List<Project> projectList;

            // database will be closed after leaving the using statement
            using (var conn = new SQLiteConnection(App.DatabaseLocation))
            {
                // if table doesn't exist create a new one
                conn.CreateTable<Project>();

                // get content of table
                projectList = conn.Table<Project>().ToList();
            }

            for (var projectIterator = 0; projectIterator < projectList.Count; projectIterator++)
            {
                var tempProject = projectList[projectIterator];
                ReadForms(ref tempProject);
                projectList[projectIterator] = tempProject;
            }

            return projectList;
        }

        /**
         * Stores all forms in project
         * @param project referencing to project and inserting forms
         */
        public static void ReadForms(ref Project project)
        {
            // database will be closed after leaving the using statement
            using (var conn = new SQLiteConnection(App.DatabaseLocation))
            {
                // get all forms, which belong to the selected project
                project.FormList = conn.Query<ProjectForm>("select * from ProjectForm where ProjectId == ?", project.Id);

                // get the list of the connections between all elements and forms
                var elementConnection = conn.Query<ProjectFormElementList>("select * from ProjectFormElementList");

                // walk through the list
                foreach (var connection in elementConnection)
                {
                    // get single element of connection list
                    var element = conn.Query<ProjectFormElements>("select * from ProjectFormElements where Id == ?", connection.ElementId);
                    if (element.Count <= 0) continue;

                    // check if element list in project is initialized, if not initialize
                    if (project.FormList.Find(form => form.Id == connection.FormId).ElementList == null)
                    {
                        project.FormList.Find(form => form.Id == connection.FormId).ElementList = new List<ProjectFormElements>();
                    }

                    // add element to form
                    project.FormList.Find(form => form.Id == connection.FormId).ElementList.Add(element.First());
                }
            }
        }

        /**
         * Create custom table
         * @param project Project containing information of forms
         */
        public static bool CreateCustomTable(ref Project project)
        {
            bool status;
            var tableName = Parser.LanguageJsonStandard(project.Title, project.Languages) + "_" + project.Id;
            CheckValidSqlName(tableName);
            // Generate query for creating a new table
            var query = "CREATE TABLE IF NOT EXISTS " + tableName + "(";
            query += "Id INTEGER PRIMARY KEY AUTOINCREMENT, ";
            query += "ProjectId INTEGER, ";
            query += "Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP, ";

            foreach (var form in project.FormList)
            {
                foreach (var element in form.ElementList)
                {
                    CheckValidSqlName(element.Name);
                    query += element.Name + " VARCHAR, ";
                }
            }

            query = query.Remove(query.Length - 2);
            query += ");";

            using (var conn = new SQLiteConnection(App.DatabaseLocation))
            {
                try
                {
                    conn.Execute(query);
                    status = true;
                }
                catch
                {
                    status = false;
                }
            }

            return status;
        }

        /**
         * Read custom table
         * @param project Current working project
         * @return TableData with field names and elements
         */
        public static TableData ReadCustomTable(ref Project project)
        {
            var tableName = Parser.LanguageJsonStandard(project.Title, project.Languages) + "_" + project.Id;

            using (var conn = new SQLiteConnection(App.DatabaseLocation))
            {
                try
                {
                    var datalist = new TableData();

                    var tableInfo = conn.GetTableInfo(tableName);
                    CheckValidSqlName(tableName);
                    // getting highest id in table
                    var queryLastId = $"SELECT MAX(ID) FROM {tableName} AS int";
                    var lastElementId = conn.ExecuteScalar<int>(queryLastId);

                    foreach (var tableColumn in tableInfo)
                    {
                        var elementList = new List<string>();

                        for (var i = 0; i <= lastElementId; i++)
                        {
                            CheckValidSqlName(tableColumn.Name);
                            var query = $"SELECT {tableColumn.Name} FROM {tableName} WHERE ID={i}";
                            var element = conn.ExecuteScalar<string>(query);
                            if (element != null)
                                elementList.Add(element);
                        }

                        datalist.RowNameList.Add(tableColumn.Name);
                        datalist.ValueList.Add(elementList);
                    }
                    return datalist;
                }
                catch
                {
                    return null;
                }
            }
        }

        /**
         * Inserting measurement data
         * @param tableName Name of project table
         * @param fieldNames field names
         * @param fieldValues values corresponding to field names
         * @return Success
         */
        public static bool InsertCustomValues(string tableName, List<string> fieldNames, List<string> fieldValues)
        {
            bool status;
            CheckValidSqlName(tableName);
            var query = $"INSERT INTO {tableName} (";

            foreach (var name in fieldNames)
            {
                CheckValidSqlName(name);
                query += $"{name}, ";
            }

            query = query.Remove(query.Length - 2);
            query += ") VALUES (";

            List<object> queryParams = new List<object>();

            foreach (var values in fieldValues)
            {
                queryParams.Add(values);
                query += "?, ";
            }

            query = query.Remove(query.Length - 2);
            query += ");";
            
            using (var conn = new SQLiteConnection(App.DatabaseLocation))
            {
                try
                {
                    conn.Execute(query, queryParams.ToArray());
                    status = true;
                }
                catch
                {
                    status = false;
                }
            }

            return status;
        }

        /**
         * Removes database file
         * @return boolean if removal was successful
         */
        public static bool RemoveDatabase()
        {
            try
            {
                if (File.Exists(App.DatabaseLocation))
                {
                    File.Delete(App.DatabaseLocation);
                }
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        /**
         * Select project as current project and deselect other project as current project
         * @param project Select project as current project
         * @return Status of setting new current project
         */
        public static bool SelectCurrentProject(Project project)
        {
            bool result;
            var oldCurrentProject = GetCurrentProject();

            // Get current project and deselect it
            if (oldCurrentProject != null)
            {
                oldCurrentProject.CurrentProject = false;
                Update(ref oldCurrentProject);
            }

            // Set new current project
            project.CurrentProject = true;
            result = Update(ref project);

            return result;
        }

        /**
         * Returns the current project
         * @return current project
         */
        public static Project GetCurrentProject()
        {
            var projectList = ReadProjects();
            return projectList.Find(project => project.CurrentProject);
        }
    }
}
