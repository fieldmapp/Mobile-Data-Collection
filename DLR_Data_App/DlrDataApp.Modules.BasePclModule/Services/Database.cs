using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensions.Extensions.TextBlob;

namespace DlrDataApp.Modules.Base.Shared.Services
{
    /// <summary>
    /// Provides Database access. Uses <see cref="SQLiteNetExtensions"/> to allow usage of foreign keys.
    /// Each Method is provided both with and without the option to use a <see cref="SQLiteConnection"/> 
    /// which enables use of Transactions.
    /// </summary>
    /// <see cref="https://docs.microsoft.com/de-de/xamarin/android/data-cloud/data-access/using-sqlite-orm"/>
    public class Database
    {
        /// <summary>
        /// Implementation of <see cref="ITextBlobSerializer"/> which uses JSON.
        /// </summary>
        private class JsonTranslatorHelper : ITextBlobSerializer
        {
            public object Deserialize(string text, Type type)
            {
                // use JsonTranslator.GetFromJson
                MethodInfo method = typeof(JsonTranslator).GetMethod(nameof(JsonTranslator.GetFromJson));
                MethodInfo generic = method.MakeGenericMethod(type);
                var a = generic.Invoke(null, new[] { text });
                return a;
            }

            public string Serialize(object element)
            {
                return JsonTranslator.GetJson(element);
            }
        }
        private string DatabasePath { get; }

        /// <summary>
        /// Creates a new wrapper around the database located at a specified path.
        /// </summary>
        /// <param name="databasePath"></param>
        public Database(string databasePath)
        {
            DatabasePath = databasePath;
            TextBlobOperations.SetTextSerializer(new JsonTranslatorHelper());
        }

        /// <summary>
        /// Creates a new <see cref="SQLiteConnection"/>.
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
        /// <returns><see cref="string"/> which representates the transaction point.</returns>
        public static string SaveTransactionPoint(SQLiteConnection conn)
        {
            return conn.SaveTransactionPoint();
        }

        /// <summary>
        /// Undoes everything done from transaction represented by given <see cref="string"/> transactionPointName on given <see cref="SQLiteConnection"/> conn.
        /// </summary>
        /// <param name="transactionPointName"><see cref="string"/> which representates the transaction point</param>
        /// <param name="conn">Active <see cref="SQLiteConnection"/></param>
        public static void RollbackChanges(string transactionPointName, SQLiteConnection conn)
        {
            conn.RollbackTo(transactionPointName);
        }

        /// <summary>
        /// Saves everything done from transaction represented by given <see cref="string"/> transactionPointName on given <see cref="SQLiteConnection"/> conn.
        /// </summary>
        /// <param name="transactionPointName"><see cref="string"/> which representates the transaction point</param>
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
        /// <typeparam name="T">Type of object to be inserted</typeparam>
        /// <param name="data">The object that will be inserted</param>
        /// <param name="conn">Connection which will be used</param>
        /// <returns>Success of the operation</returns>
        public static bool Insert<T>(T data, SQLiteConnection conn = null)
        {
            // if table doesn't exist create a new one
            conn.CreateTable<T>();

            // Insert data into table
            return conn.Insert(data) > 0;
        }

        /// <summary>
        /// Inserts or updates data (based on the primary key) in the database.
        /// </summary>
        /// <typeparam name="T">Type of object to be updated or inserted</typeparam>
        /// <param name="data">Object which will be updated or inserted</param>
        /// <returns>Success of the operation</returns>
        public bool InsertOrUpdate<T>(T data) => RunWithConnection(c => InsertOrUpdate(data, c));

        /// <summary>
        /// Inserts or updates data (based on the primary key) in the database.
        /// </summary>
        /// <typeparam name="T">Type of object to be updated or inserted</typeparam>
        /// <param name="data">Object which will be updated or inserted</param>
        /// <param name="conn">Connection which will be used</param>
        /// <returns>Success of the operation</returns>
        public static bool InsertOrUpdate<T>(T data, SQLiteConnection conn)
        {
            // if table doesn't exist create a new one
            conn.CreateTable<T>();

            return conn.InsertOrReplace(data) > 0;
        }

        /// <summary>
        /// Inserts or updates data (based on the primary key) including its children in the database.
        /// </summary>
        /// <typeparam name="T">Type of object to be updated or inserted</typeparam>
        /// <param name="data">Object which will be updated or inserted</param>
        /// <param name="recursive">Indicates if recursive children will be included in this process</param>
        /// <returns>Success of the operation</returns>
        public bool InsertOrUpdateWithChildren<T>(T data, bool recursive = true) => RunWithConnection(c => InsertOrUpdateWithChildren(data, c, recursive));

        /// <summary>
        /// Inserts or updates data (based on the primary key) including its children in the database.
        /// </summary>
        /// <typeparam name="T">Type of object to be updated or inserted</typeparam>
        /// <param name="data">Object which will be updated or inserted</param>
        /// <param name="conn">Connection which will be used</param>
        /// <param name="recursive">Indicates if recursive children will be included in this process</param>
        /// <returns>Success of the operation</returns>
        public static bool InsertOrUpdateWithChildren<T>(T data, SQLiteConnection conn, bool recursive = true)
        {
            // if table doesn't exist create a new one
            conn.CreateTable<T>();

            conn.InsertOrReplaceWithChildren(data, recursive);
            return true;
        }



        /// <summary>
        /// Updates data in database
        /// </summary>
        /// <typeparam name="T">Type of object to be updated</typeparam>
        /// <param name="data">Object which will be updated</param>
        /// <returns>Success of the operation</returns>
        public bool Update<T>(T data) => RunWithConnection(c => Update(data, c));

        /// <summary>
        /// Updates data in database
        /// </summary>
        /// <typeparam name="T">Type of object to be updated</typeparam>
        /// <param name="data">Object which will be updated</param>
        /// <param name="conn">Connection which will be used</param>
        /// <returns>Success of the operation</returns>
        public static bool Update<T>(T data, SQLiteConnection conn)
        {
            // TODO: Check if this function is actually needed
            int resultUpdate;
            // if table doesn't exist create a new one
            conn.CreateTable<T>();

            // Update data into table
            resultUpdate = conn.Update(data);

            // check if data was successfully updated
            return resultUpdate > 0;
        }

        /// <summary>
        /// Deletes data from database
        /// </summary>
        /// <typeparam name="T">Type of object to be deleted</typeparam>
        /// <param name="data">Object to be deleted</param>
        /// <returns>Success of the operation</returns>
        public bool Delete<T>(T data) => RunWithConnection(c => Delete(data, c));

        /// <summary>
        /// Deletes data from database
        /// </summary>
        /// <typeparam name="T">Type of object to be deleted</typeparam>
        /// <param name="data">Object to be deleted</param>
        /// <param name="conn">Connection which will be used</param>
        /// <returns>Success of the operation</returns>
        public bool Delete<T>(T data, SQLiteConnection conn)
        {
            conn.CreateTable<T>();
            int resultDelete;

            resultDelete = conn.Delete(data);

            return resultDelete > 0;
        }

        /// <summary>
        /// Gets the active instance of a class. Only one Element may be active at a time. 
        /// Can be set with <see cref="SetActiveElement{T, U}(bool)"/>
        /// This is persistent, even when the app is closed.
        /// </summary>
        /// <typeparam name="T">Type of active element</typeparam>
        /// <typeparam name="U">Type of helper class which must implement <see cref="IActiveElementInfo{T}"/></typeparam>
        /// <param name="recursive">Indicates if children of the active element should be fetched</param>
        /// <returns>Active Instance of the class</returns>
        public T GetActiveElement<T, U>(bool recursive = true)
            where T : class
            where U : class, IActiveElementInfo<T>, new()
            => RunWithConnection(c => GetActiveElement<T, U>(c, recursive));

        /// <summary>
        /// Gets the active instance of a class. Only one Element may be active at a time.
        /// Can be set with <see cref="SetActiveElement{T, U}(bool)"/>
        /// This is persistent, even when the app is closed.
        /// </summary>
        /// <typeparam name="T">Type of active element</typeparam>
        /// <typeparam name="U">Implementation of <see cref="IActiveElementInfo{T}"/></typeparam>
        /// <param name="recursive">Indicates if children of the active element should be fetched</param>
        /// <param name="conn">Connection which will be used</param>
        /// <returns>Active Instance of the class</returns>
        public T GetActiveElement<T, U>(SQLiteConnection conn, bool recursive = true)
            where T : class
            where U : class, IActiveElementInfo<T>, new()
        {
            var activeElementInfo = ReadWithChildren<U>(conn, recursive).FirstOrDefault() ?? new U();
            return activeElementInfo.ActiveElement;
        }


        /// <summary>
        /// Sets the active instance of a class. Only one Element may be active at a time.
        /// Can be fetched with <see cref="GetActiveElement{T, U}(bool)"/>
        /// This is persistent, even when the app is closed.
        /// </summary>
        /// <typeparam name="T">Type of active element</typeparam>
        /// <typeparam name="U">Implementation of <see cref="IActiveElementInfo{T}"/></typeparam>
        /// <param name="element">Element which should be the active one</param>
        /// <returns>Active Instance of the class</returns>
        public bool SetActiveElement<T, U>(T element)
            where T : class
            where U : class, IActiveElementInfo<T>, new()
            => RunWithConnection(c => SetActiveElement<T, U>(c, element));

        /// <summary>
        /// Sets the active instance of a class. Only one Element may be active at a time.
        /// Can be fetched with <see cref="GetActiveElement{T, U}(bool)"/>
        /// This is persistent, even when the app is closed.
        /// </summary>
        /// <typeparam name="T">Type of active element</typeparam>
        /// <typeparam name="U">Implementation of <see cref="IActiveElementInfo{T}"/></typeparam>
        /// <param name="element">Element which should be the active one</param>
        /// <param name="conn">Connection which will be used</param>
        /// <returns>Active Instance of the class</returns>
        public bool SetActiveElement<T, U>(SQLiteConnection conn, T element)
            where T : class
            where U : class, IActiveElementInfo<T>, new()
        {
            var previousActiveElementInfo = ReadWithChildren<U>(conn, true/*maybe false could be used here?*/).FirstOrDefault() ?? new U();
            previousActiveElementInfo.ActiveElement = element;
            return InsertOrUpdateWithChildren(previousActiveElementInfo, true);
        }

        /// <summary>
        /// Reads all instances of a class from the database
        /// </summary>
        /// <typeparam name="T">Type of class which should be read</typeparam>
        /// <returns>List of all instances saved in the database</returns>
        public List<T> Read<T>() where T : new() => RunWithConnection(Read<T>);

        /// <summary>
        /// Reads all instances of a class from the database
        /// </summary>
        /// <typeparam name="T">Type of class which should be read</typeparam>
        /// <param name="conn">Connection which will be used</param>
        /// <returns>List of all instances saved in the database</returns>
        public static List<T> Read<T>(SQLiteConnection conn) where T : new()
        {
            conn.CreateTable<T>();

            // get content of table
            var list = conn.Table<T>().ToList();

            return list;
        }

        private static Dictionary<Type, PropertyInfo> TypeToPrimaryKeyAttribute = new Dictionary<Type, PropertyInfo>();
        /// <summary>
        /// Returns the primary key of an object.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="element">Object of which primary key should be obtained</param>
        /// <returns>Primary Key value</returns>
        /// <exception cref="ArgumentException"/>
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

        /// <summary>
        /// Reads all element of a class, including its children.
        /// </summary>
        /// <typeparam name="T">Type of class of which all elements should be read</typeparam>
        /// <param name="recursive">Indicates if recursive children will be included in this process</param>
        /// <returns>List of all elements</returns>
        public List<T> ReadWithChildren<T>(bool recursive = true) where T : class, new() => RunWithConnection(conn => ReadWithChildren<T>(conn, recursive));

        /// <summary>
        /// Reads all element of a class, including its children.
        /// </summary>
        /// <typeparam name="T">Type of class of which all elements should be read</typeparam>
        /// <param name="conn">Connection which will be used</param>
        /// <param name="recursive">Indicates if recursive children will be included in this process</param>
        /// <returns>List of all elements</returns>
        public static List<T> ReadWithChildren<T>(SQLiteConnection conn, bool recursive = true) where T : class, new()
        {
            var content = Read<T>(conn);
            return content.Select(element => GetWithChildren(conn, recursive, element)).ToList();
        }

        private static T GetWithChildren<T>(SQLiteConnection conn, bool recursive, T element) where T : class, new()
        {
            if (element == null)
                return null;
            return conn.GetWithChildren<T>(GetPrimaryKeyValue(element), recursive);
        }

        /// <summary>
        /// Returns an object from the database, based on a condition
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="predicate">Predicate which selects the correct object</param>
        /// <returns>First object for which the predicate returned true</returns>
        public T Find<T>(Func<T, bool> predicate) where T : new()
        {
            using (var conn = CreateConnection())
            {
                return Find(conn, predicate);
            }
        }

        /// <summary>
        /// Returns an object from the database, based on a condition
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="conn">Connection which will be used</param>
        /// <param name="predicate">Predicate which selects the correct object</param>
        /// <returns>First object for which the predicate returned true</returns>
        public static T Find<T>(SQLiteConnection conn, Func<T, bool> predicate) where T : new()
        {
            conn.CreateTable<T>();
            var value = conn.Table<T>().FirstOrDefault(predicate);
            return value;
        }

        /// <summary>
        /// Returns an object from the database, based on a condition. 
        /// Also populates the objects with their respective children
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="conn">Connection which will be used</param>
        /// <param name="predicate">Predicate which selects the correct object</param>
        /// <param name="recursive">Indicates if recursive children will be included in this process</param>
        /// <returns>List of all objects</returns>
        public T FindWithChildren<T>(Func<T, bool> predicate, bool recursive = true) where T : class, new() => RunWithConnection(conn => FindWithChildren(conn, predicate, recursive));

        /// <summary>
        /// Returns an object from the database, based on a condition. 
        /// Also populates the objects with their respective children
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="conn">Connection which will be used</param>
        /// <param name="predicate">Predicate which selects the correct object</param>
        /// <param name="recursive">Indicates if recursive children will be included in this process</param>
        /// <param name="conn">Connection which will be used</param>
        /// <returns>List of all objects</returns>
        public static T FindWithChildren<T>(SQLiteConnection conn, Func<T, bool> predicate, bool recursive = true) where T : class, new()
        {
            var element = Find(conn, predicate);
            return GetWithChildren(conn, recursive, element);
        }

        /// <summary>
        /// Removes the database file.
        /// </summary>
        /// <returns>True if removal was successful</returns
        /// <exception cref="IOException"/>
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
    }
}
