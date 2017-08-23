using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GTA_RP.Database
{
    /// <summary>
    /// Query base class
    /// </summary>
    abstract class Query
    {
        private String queryString;
        protected Action<MySqlDataReader> code;
        protected MySqlCommand mysqlCommand;

        /// <summary>
        /// Constructor for query
        /// </summary>
        /// <param name="query">Sql query</param>
        /// <param name="code">Code to run</param>
        public Query(String query, Action<MySqlDataReader> code = null)
        {
            this.queryString = query;
            this.code = code;
            mysqlCommand = new MySqlCommand(query, DBManager.Instance().Connection);
        }

        /// <summary>
        /// Adds key and value to the query
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>New query with key and value added</returns>
        public Query AddValue(string key, object value)
        {
            mysqlCommand.Parameters.AddWithValue(key, value);
            return this;
        }

        abstract public void Execute();
    }
}
