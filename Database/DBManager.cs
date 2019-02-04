using System;
using MySql.Data.MySqlClient;
using GTA_RP.Misc;
using GTA_RP.Database;

namespace GTA_RP
{
    /// <summary>
    /// Class for handling all database interactions such as connecting and queries.
    /// </summary>
    class DBManager : Singleton<DBManager>
    {
        public DBManager() { }

        public string Password { get; set; }
        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        /// <summary>
        /// Executes a raw query
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>MysqlCommand</returns>
        public static MySqlCommand SimpleQuery(String query)
        {
            DBManager.Instance().IsConnect();
            return new MySqlCommand(query, DBManager.Instance().connection);
        }


        /// <summary>
        /// Checks if table is empty
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns>True if empty, false if not empty</returns>
        public static Boolean IsTableEmpty(String tableName)
        {
            bool isEmpty = true;
            string queryString = "SELECT * FROM " + tableName;
            DBManager.SelectQuery(queryString, (MySqlDataReader reader) =>
            {
                isEmpty = !reader.HasRows;
            }).Execute();
            return isEmpty;
        }

        
        /// <summary>
        /// Database query wrapper for select statement
        /// </summary>
        /// <param name="code">Code block to run</param>
        /// <param name="query">Query string</param>
        /// <param name="values">Key value pairs</param>
        public static SelectQuery SelectQuery(String query, Action<MySqlDataReader> code)
        {
            DBManager.Instance().IsConnect();
            return new SelectQuery(query, code);
        }

        /// <summary>
        /// Database query wrapper for update statement
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>Query object</returns>
        public static Query UpdateQuery(String query)
        {
            DBManager.Instance().IsConnect();
            return new NormalQuery(query);
        }

        /// <summary>
        /// Database query wrapper for insert statement
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>Query object</returns>
        public static Query InsertQuery(String query)
        {
            DBManager.Instance().IsConnect();
            return new NormalQuery(query);
        }

        /// <summary>
        /// Database query wrapper for delete statement
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>Query object</returns>
        public static Query DeleteQuery(String query)
        {
            return InsertQuery(query);

        }
        /// <summary>
        /// Loads connection info from configuration file
        /// </summary>
        /// <returns>Connection string</returns>
        private String LoadConnectionInfo()
        {
            string dbName = ConfigManager.ReadStringValue("database", "dbname");
            string username = ConfigManager.ReadStringValue("database", "username");
            string password = ConfigManager.ReadStringValue("database", "password");
            string server = ConfigManager.ReadStringValue("database", "server");
            return String.Format("Server={0}; database={1}; UID={2}; password={3}", server, dbName, username, password);
            //return "Server=" + server + "; database=" + dbName + "; UID=" + username + "; password=" + password;
        }
        
        /// <summary>
        /// Checks if database connection is open
        /// </summary>
        /// <returns>Returns true if connection is open or if it has been opened</returns>
        public bool IsConnect()
        {
            bool result = true;
            if (Connection == null)
            {
                string connstring = LoadConnectionInfo();
                connection = new MySqlConnection(connstring);
                connection.Open();
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Closes the database connection
        /// </summary>
        public void Close()
        {
            connection.Close();
        }
    }
}
