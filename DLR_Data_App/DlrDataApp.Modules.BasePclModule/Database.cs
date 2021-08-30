using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace DlrDataApp.Modules.Base.Shared
{
    /// <summary>
    /// Class which handles database access
    /// </summary>
    /// <see cref="https://docs.microsoft.com/de-de/xamarin/android/data-cloud/data-access/using-sqlite-orm"/>
    public class Database
    {
        string DatabasePath { get; }
        public Database(string databasePath)
        {
            DatabasePath = databasePath;
        }

        /// <summary>
        /// Checks if a given string is a name which is safe to use in SQL. Throws an <see cref="System.ArgumentException"/> if its not.
        /// </summary>
        /// <param name="name">String to check</param>
        public static string MakeValidSqlName(string name)
        {
            bool isValidChar(char c) => char.IsLetterOrDigit(c) || c == '_';

            string safeName = new string(name.Where(isValidChar).ToArray());

            if (safeName != name)
                Console.WriteLine($"{nameof(name)} is not a valid SQL name");

            return safeName;
        }

        /// <summary>
        /// Creates a new <see cref="SQLiteConnection"/>, based on the file with path <see cref="App.DatabaseLocation"/>
        /// </summary>
        /// <returns><see cref="SQLiteConnection"/> which should be closed and disposed after use</returns>
        public SQLiteConnection CreateConnection() => new SQLiteConnection(DatabasePath);

        public T RunWithConnection<T>(Func<SQLiteConnection, T> func)
        {
            using (var conn = CreateConnection())
            {
                return func(conn);
            }
        }

        /// <summary>
        /// Opens a SQL transaction. 
        /// Use <see cref="RollbackChanges"/> to rollback everything done from this point. 
        /// Use <see cref="CommitChanges(string, SQLiteConnection)"/> to save everything done from this point.
        /// </summary>
        /// <param name="conn">An active <see cref="SQLiteConnection"/></param>
        /// <returns><see cref="String"/> which representates the transaction point.</returns>
        public static string SaveTransactionPoint(SQLiteConnection conn)
        {
            return conn.SaveTransactionPoint();
        }

        /// <summary>
        /// Undoes everything done from transaction represented by given <see cref="String"/> transactionPointName on given <see cref="SQLiteConnection"/> conn.
        /// </summary>
        /// <param name="transactionPointName"><see cref="String"/> which representates the transaction point</param>
        /// <param name="conn">Active <see cref="SQLiteConnection"/></param>
        public static void RollbackChanges(string transactionPointName, SQLiteConnection conn)
        {
            conn.RollbackTo(transactionPointName);
        }

        /// <summary>
        /// Saves everything done from transaction represented by given <see cref="String"/> transactionPointName on given <see cref="SQLiteConnection"/> conn.
        /// </summary>
        /// <param name="transactionPointName"><see cref="String"/> which representates the transaction point</param>
        /// <param name="conn">Active <see cref="SQLiteConnection"/></param>
        public static void CommitChanges(string transactionPointName, SQLiteConnection conn)
        {
            conn.Release(transactionPointName);
        }

        /// <summary>
        /// Inserts data into the database
        /// </summary>
        /// <typeparam name="T">The type of both the database content and the content to add to the database</typeparam>
        /// <param name="data">The contents that will pushed into the database</param>
        /// <returns>Status of inserting data</returns>
        public bool Insert<T>(T data) => RunWithConnection(c => Insert(data, c));

        /// <summary>
        /// Inserts data into the database
        /// </summary>
        /// <typeparam name="T">The type of both the database content and the content to add to the database</typeparam>
        /// <param name="data">The contents that will pushed into the database</param>
        /// <returns>Status of inserting data</returns>
        public static bool Insert<T>(T data, SQLiteConnection conn)
        {
            // if table doesn't exist create a new one
            conn.CreateTable<T>();

            // Insert data into table
            return conn.Insert(data) > 0;
        }

        public bool InsertOrUpdate<T>(T data) => RunWithConnection(c => InsertOrUpdate(data, c));

        public static bool InsertOrUpdate<T>(T data, SQLiteConnection conn)
        {
            // if table doesn't exist create a new one
            conn.CreateTable<T>();

            return conn.InsertOrReplace(data) > 0;
        }

        public bool InsertOrUpdateWithChildren<T>(T data, bool recursive = false) => RunWithConnection(c => InsertOrUpdateWithChildren(data, c, recursive));

        public static bool InsertOrUpdateWithChildren<T>(T data, SQLiteConnection conn, bool recursive = false)
        {
            // if table doesn't exist create a new one
            conn.CreateTable<T>();

            conn.InsertOrReplaceWithChildren(data, recursive);
            return true;
        }



        /// <summary>
        /// Updates data in database
        /// </summary>
        /// <typeparam name="T">The type of both the database content and the content to be updated in the database</typeparam>
        /// <param name="data">The contents that will updated in the database</param>
        /// <returns>Status of updating data</returns>
        public bool Update<T>(T data) => RunWithConnection(c => Update(data, c));

        /// <summary>
        /// Updates data in database
        /// </summary>
        /// <typeparam name="T">The type of both the database content and the content to be updated in the database</typeparam>
        /// <param name="data">The contents that will updated in the database</param>
        /// <returns>Status of updating data</returns>
        public static bool Update<T>(T data, SQLiteConnection conn)
        {
            int resultUpdate;
            // if table doesn't exist create a new one
            conn.CreateTable<T>();

            // Update data into table
            resultUpdate = conn.Update(data);

            // check if data was successfully updated
            if (resultUpdate > 0)
            {
                return true;
            }
            else
            {
                // if no element was updated the element is inserted into the database
                Insert(data, conn);
                return false;
            }
        }

        /// <summary>
        /// Deletes data from database
        /// </summary>
        /// <typeparam name="T">The type of both the database content and the content to be deleted from the database</typeparam>
        /// <param name="data">The contents that will deleted from the database</param>
        /// <returns>Status of deleting data</returns>
        public bool Delete<T>(T data) => RunWithConnection(c => Delete(data, c));

        /// <summary>
        /// Deletes data from database
        /// </summary>
        /// <typeparam name="T">The type of both the database content and the content to be deleted from the database</typeparam>
        /// <param name="data">The contents that will deleted from the database</param>
        /// <returns>Status of deleting data</returns>
        public bool Delete<T>(T data, SQLiteConnection conn)
        {
            conn.CreateTable<T>();
            int resultDelete;
            // Update data into table
            resultDelete = conn.Delete(data);
            // check if data was successfully deleted
            return resultDelete > 0;
        }

        // <summary>
        // Deletes all data from the database which are belonging to a specific project.
        // </summary>
        // <param name="project">Project of which all data should be deleted</param>
        //public static void DeleteProject(Project project, SQLiteConnection conn)
        //{
        //    var queryForms = "DELETE FROM ProjectForm WHERE Id=?";
        //    var queryFormElementList = "DELETE FROM ProjectFormElementList WHERE ElementId=?";
        //    var queryFormElements = "DELETE FROM ProjectFormElements WHERE Id=?";
        //    //var queryFormMetadata = "DELETE FROM ProjectFormMetadata WHERE Id=?";
        //    //var queryUserConnection = "DELETE FROM ProjectUserConnection WHERE ProjectId=?";
        //
        //    // remove all elements of a form
        //    foreach (var form in project.FormList)
        //    {
        //        // remove elements
        //        foreach (var element in form.ElementList)
        //        {
        //            conn.Execute(queryFormElements, element.Id);
        //            conn.Execute(queryFormElementList, element.Id);
        //        }
        //
        //        // remove metadata
        //        // conn.Execute(queryFormMetadata, form.Metadata.Id);
        //
        //        // remove form
        //        conn.Execute(queryForms, form.Id);
        //    }
        //
        //    // remove connection between user and project
        //    //conn.Execute(queryUserConnection, project.Id);
        //
        //    // remove project
        //    conn.Delete(project);
        //}

        //public static void DeleteProfiling(ProfilingData profiling, SQLiteConnection conn)
        //{
        //    conn.Delete(profiling);
        //    conn.Execute($"DELETE FROM {nameof(ProfilingResult)} WHERE {nameof(ProfilingResult.ProfilingId)}={profiling.ProfilingId}");
        //}

        //public static bool InsertProfiling(ref ProfilingData newProfiling, SQLiteConnection conn)
        //{
        //    var profilingList = ReadProfilings(conn);
        //    var newProfilingId = newProfiling.ProfilingId;
        //
        //    if (profilingList.Any(p => p.ProfilingId == newProfilingId))
        //        return false;
        //
        //    return Insert(ref newProfiling, conn);
        //}
        public List<T> Read<T>() where T : new() => RunWithConnection(Read<T>);

        public static List<T> Read<T>(SQLiteConnection conn) where T : new()
        {
            conn.CreateTable<T>();

            // get content of table
            var list = conn.Table<T>().ToList();

            return list;
        }

        static Dictionary<Type, PropertyInfo> TypeToPrimaryKeyAttribute = new Dictionary<Type, PropertyInfo>();
        public static object GetPrimaryKeyValue<T>(T element)
        {
            PropertyInfo propInfo;
            if (!TypeToPrimaryKeyAttribute.TryGetValue(typeof(T), out propInfo))
            {
                var primaryKeyProperties = typeof(T).GetProperties()
                    .Where(prop => prop.IsDefined(typeof(PrimaryKeyAttribute), false)).ToList();
                if (primaryKeyProperties.Count != 1)
                    throw new ArgumentException($"Type {typeof(T)} needs exactly one Property marked with {typeof(PrimaryKeyAttribute)}.");
                propInfo = primaryKeyProperties.First();
            }
            return propInfo.GetValue(element);
        }
        public List<T> ReadWithChildren<T>(bool recursive = false) where T : class, new() => RunWithConnection(conn => ReadWithChildren<T>(conn, recursive));
        public static List<T> ReadWithChildren<T>(SQLiteConnection conn, bool recursive = false) where T : class, new()
        {
            var content = Read<T>(conn);
            return content.Select(element => GetWithChildren<T>(conn, recursive, element)).ToList();
        }

        private static T GetWithChildren<T>(SQLiteConnection conn, bool recursive, T element) where T : class, new()
        {
            if (element == null)
                return null;
            return conn.GetWithChildren<T>(GetPrimaryKeyValue(element), recursive);
        }
        public T Find<T>(Func<T, bool> predicate) where T : new()
        {
            using (var conn = CreateConnection())
            {
                return Find(conn, predicate);
            }
        }
        public static T Find<T>(SQLiteConnection conn, Func<T, bool> predicate) where T : new()
        {
            conn.CreateTable<T>();
            var value = conn.Table<T>().FirstOrDefault(predicate);
            return value;
        }
        public T FindWithChildren<T>(Func<T, bool> predicate, bool recursive = false) where T : class, new() => RunWithConnection(conn => FindWithChildren<T>(conn, predicate, recursive));
        public static T FindWithChildren<T>(SQLiteConnection conn, Func<T, bool> predicate, bool recursive = false) where T : class, new()
        {
            var element = Find(conn, predicate);
            return GetWithChildren<T>(conn, recursive, element);
        }

        /// <summary>
        /// Stores project in database.
        /// This uses an own implementation of foreign keys, because this function is missing in the SQLite implementation which is supported for Xamarin.
        /// </summary>
        /// <param name="project">Project to be inserted</param>
        /// <returns>True if project insertion was successful</returns>
        //public static bool InsertProject(ref Project project, SQLiteConnection conn)
        //{
        //    // Load current stored data to project list
        //    var projectList = ReadProjects(conn);
        //
        //    // Check if project already exists and abort insertion if it does
        //    foreach (var p in projectList)
        //    {
        //        if (p.Title == project.Title)
        //        {
        //            return false;
        //        }
        //    }
        //
        //    // Insert project to database
        //    if (!Insert(ref project, conn))
        //        return false;
        //
        //    // Add form to project
        //    foreach (var form in project.FormList)
        //    {
        //        var formElement = form;
        //        formElement.ProjectId = project.Id;
        //        if (!Insert(ref formElement, conn))
        //            return false;
        //
        //        var metadata = form.Metadata;
        //        if (!Insert(ref metadata, conn))
        //            return false;
        //
        //        foreach (var elements in form.ElementList)
        //        {
        //            // store each form with its controls
        //            var controlElement = elements;
        //            if (!Insert(ref controlElement, conn))
        //                return false;
        //
        //            // combine project with control elements
        //            var combineElementList = new ProjectFormElementList
        //            {
        //                ElementId = controlElement.Id,
        //                FormId = formElement.Id,
        //                MetadataId = form.Metadata.Id
        //            };
        //            if (!Insert(ref combineElementList, conn))
        //                return false;
        //        }
        //    }
        //
        //    // Create custom table
        //    return CreateCustomTable(ref project, conn);
        //}

        /// <summary>
        /// Reads all projects from the database.
        /// </summary>
        /// <returns>List of all projects in database</returns>
        //public static List<Project> ReadProjects(SQLiteConnection conn)
        //{
        //    List<Project> projectList;
        //    // if table doesn't exist create a new one
        //    conn.CreateTable<Project>();
        //
        //    // get content of table
        //    projectList = conn.Table<Project>().ToList();
        //
        //    for (var projectIterator = 0; projectIterator < projectList.Count; projectIterator++)
        //    {
        //        var tempProject = projectList[projectIterator];
        //        ReadForms(ref tempProject, conn);
        //        projectList[projectIterator] = tempProject;
        //    }
        //
        //    return projectList;
        //}

        /// <summary>
        /// Stores all forms in project
        /// </summary>
        /// <param name="project">Project to which its forms will be added from the database</param>
        //public static void ReadForms(ref Project project, SQLiteConnection conn)
        //{
        //    // get all forms, which belong to the selected project
        //    project.FormList = conn.Query<ProjectForm>("select * from ProjectForm where ProjectId == ?", project.Id);
        //
        //    // get the list of the connections between all elements and forms
        //    var elementConnection = conn.Query<ProjectFormElementList>("select * from ProjectFormElementList");
        //
        //    // walk through the list
        //    foreach (var connection in elementConnection)
        //    {
        //        // get single element of connection list
        //        var element = conn.Query<ProjectFormElements>("select * from ProjectFormElements where Id == ?", connection.ElementId);
        //        if (element.Count <= 0) continue;
        //
        //        // check if element list in project is initialized, if not initialize
        //        var matchingProjectForm = project.FormList.Find(form => form.Id == connection.FormId);
        //        if (matchingProjectForm != null)
        //        {
        //            if (matchingProjectForm.ElementList == null)
        //            {
        //                matchingProjectForm.ElementList = new List<ProjectFormElements>();
        //            }
        //
        //            // add element to form
        //            matchingProjectForm.ElementList.Add(element.First());
        //        }
        //    }
        //}

        /// <summary>
        /// Creates a custom table in the database, based on a supplied project.
        /// </summary>
        /// <param name="project">Project containing information of forms</param>
        /// <returns>True if creation was successful</returns>
        //public static bool CreateCustomTable(ref Project project, SQLiteConnection conn)
        //{
        //    var tableName = project.GetTableName();
        //    tableName = MakeValidSqlName(tableName);
        //    // Generate query for creating a new table
        //    var query = "CREATE TABLE IF NOT EXISTS " + tableName + "(";
        //    query += "Id INTEGER PRIMARY KEY AUTOINCREMENT, ";
        //    query += "ProjectId INTEGER, ";
        //    query += "Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP, ";
        //
        //    foreach (var form in project.FormList)
        //    {
        //        foreach (var element in form.ElementList)
        //        {
        //            var elementName = MakeValidSqlName(element.Name);
        //            query += elementName + " VARCHAR, ";
        //        }
        //    }
        //
        //    query = query.Remove(query.Length - 2);
        //    query += ");";
        //
        //    try
        //    {
        //        conn.Execute(query);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Reads the custom table of a supplied project.
        /// </summary>
        /// <param name="project">Project of which the table data should be read from the database</param>
        /// <returns>Data contained in the table belonging to the project</returns>
        //public static TableData ReadCustomTable(ref Project project, SQLiteConnection conn)
        //{
        //    var tableName = project.GetTableName();
        //
        //    try
        //    {
        //        var datalist = new TableData();
        //
        //        var tableInfo = conn.GetTableInfo(tableName);
        //        tableName = MakeValidSqlName(tableName);
        //        // getting highest id in table
        //        var queryLastId = $"SELECT MAX(ID) FROM {tableName} AS int";
        //        var lastElementId = conn.ExecuteScalar<int>(queryLastId);
        //
        //        foreach (var tableColumn in tableInfo)
        //        {
        //            var elementList = new List<string>();
        //
        //            for (var i = 0; i <= lastElementId; i++)
        //            {
        //                var columnName = MakeValidSqlName(tableColumn.Name);
        //                var query = $"SELECT {columnName} FROM {tableName} WHERE ID={i}";
        //                var element = conn.ExecuteScalar<string>(query);
        //                if (element != null)
        //                    elementList.Add(element);
        //            }
        //
        //            datalist.RowNameList.Add(tableColumn.Name);
        //            datalist.ValueList.Add(elementList);
        //        }
        //        return datalist;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// Inserts a row of data to a projects table.
        /// </summary>
        /// <param name="tableName">Name of the projects table</param>
        /// <param name="fieldNames">List containing the names of all fields</param>
        /// <param name="fieldValues">List containing the values of all fields</param>
        /// <param name="id">Id of the row. If not supplied (=null) SQLite will find a suitable id</param>
        /// <returns>True if insertion was successful</returns>
        //public static bool InsertCustomValues(string tableName, List<string> fieldNames, List<string> fieldValues, SQLiteConnection conn, int? id = null)
        //{
        //    tableName = MakeValidSqlName(tableName);
        //    var query = $"INSERT INTO {tableName} (";
        //
        //    if (id != null)
        //        query += $"Id, ";
        //
        //    foreach (var name in fieldNames)
        //    {
        //        var elementName = MakeValidSqlName(name);
        //        query += $"{elementName}, ";
        //    }
        //
        //    query = query.Remove(query.Length - 2);
        //    query += ") VALUES (";
        //
        //    if (id != null)
        //        query += $"{id}, ";
        //
        //    List<object> queryParams = new List<object>();
        //
        //    foreach (var values in fieldValues)
        //    {
        //        queryParams.Add(values);
        //        query += "?, ";
        //    }
        //
        //    query = query.Remove(query.Length - 2);
        //    query += ");";
        //
        //    try
        //    {
        //        conn.Execute(query, queryParams.ToArray());
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Updates a row of data in a projects table.
        /// </summary>
        /// <param name="tableName">Name of the projects table</param>
        /// <param name="id">Id of the row</param>
        /// <param name="fieldNames">List containing the names of all fields</param>
        /// <param name="fieldValues">List containing the values of all fields</param>
        /// <returns>True if update was successful</returns>
        //public static bool UpdateCustomValuesById(string tableName, int id, List<string> fieldNames, List<string> fieldValues, SQLiteConnection conn)
        //{
        //    tableName = MakeValidSqlName(tableName);
        //    string query = $"DELETE FROM {tableName} WHERE Id={id}";
        //    try
        //    {
        //        conn.Execute(query);
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return InsertCustomValues(tableName, fieldNames, fieldValues, id);
        //}

        /// <summary>
        /// Removes the database file.
        /// </summary>
        /// <returns>True if removal was successful</returns>
        public bool DeleteDatabase()
        {
            try
            {
                if (File.Exists(DatabasePath))
                {
                    File.Delete(DatabasePath);
                }
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        //public static bool SetCurrentProfiling(ProfilingData profiling, SQLiteConnection conn)
        //{
        //    bool result;
        //    var previousProfiling = GetCurrentProfiling(conn);
        //
        //    // Get current project and deselect it
        //    if (previousProfiling != null)
        //    {
        //        previousProfiling.IsCurrentProfiling = false;
        //        Update(ref previousProfiling, conn);
        //    }
        //
        //    // Set new current project
        //    profiling.IsCurrentProfiling = true;
        //    result = Update(ref profiling, conn);
        //
        //    return result;
        //}

        //public static ProfilingData GetCurrentProfiling(SQLiteConnection conn)
        //{
        //    return ReadProfilings(conn).Find(p => p.IsCurrentProfiling);
        //}

        //public static bool SetOrUpdateProfilingAnswer(ProfilingResult result, SQLiteConnection conn)
        //{
        //    if (result.ProfilingResultId < 0)
        //    {
        //        result.ProfilingResultId = 0;
        //        return Insert(ref result, conn);
        //    }
        //    else
        //    {
        //        return Update(ref result, conn);
        //    }
        //}

        //public bool SetCurrentProject(Project project, SQLiteConnection conn)
        //{
        //    bool result;
        //    var oldCurrentProject = GetCurrentProject(conn);
        //
        //    // Get current project and deselect it
        //    if (oldCurrentProject != null)
        //    {
        //        oldCurrentProject.CurrentProject = false;
        //        Update(ref oldCurrentProject, conn);
        //    }
        //
        //    // Set new current project
        //    project.CurrentProject = true;
        //    result = Update(ref project, conn);
        //
        //    return result;
        //}

        /// <summary>
        /// Set a project as current project. Will unset previous selected project.
        /// </summary>
        /// <param name="project">Project which should be set as active project</param>
        /// <returns>True if switching of current project was successful</returns>
        //public static bool SetCurrentProject(Project project)
        //{
        //    using (var conn = CreateConnection())
        //    {
        //        return SetCurrentProject(project, conn);
        //    }
        //}

        //public static Project GetCurrentProject(SQLiteConnection conn)
        //{
        //    return ReadProjects(conn).Find(project => project.CurrentProject);
        //}

        /// <summary>
        /// Searches the database for the currently active project
        /// </summary>
        /// <returns>Project which has CurrentProject set to true</returns>
        //public static Project GetCurrentProject()
        //{
        //    using (var conn = CreateConnection())
        //    {
        //        return GetCurrentProject(conn);
        //    }
        //}
    }
}
