using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GTA_RP.Database
{
    /// <summary>
    /// SQL Insert, UPDATE, DELETE query wrapper
    /// </summary>
    class NormalQuery : Query
    {
        public NormalQuery(string query, Action<MySqlDataReader> code = null) : base(query, code){ }

        public override void Execute()
        {
            mysqlCommand.ExecuteNonQuery();
        }
    }
}
