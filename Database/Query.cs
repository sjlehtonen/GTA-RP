using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GTA_RP.Database
{
    abstract class Query
    {
        private String queryString;
        protected Action<MySqlDataReader> code;
        protected MySqlCommand mysqlCommand;

        public Query(String query, Action<MySqlDataReader> code = null)
        {
            this.queryString = query;
            this.code = code;
            mysqlCommand = new MySqlCommand(query, DBManager.Instance().Connection);
        }

        public Query AddValue(string key, object value)
        {
            mysqlCommand.Parameters.AddWithValue(key, value);
            return this;
        }

        abstract public void Execute();
    }
}
