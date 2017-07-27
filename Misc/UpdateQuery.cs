using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GTA_RP.Misc
{
    class UpdateQuery : Query
    {
        public UpdateQuery(string query, Action<MySqlDataReader> code = null) : base(query, code)
        {

        }

        public override void Execute()
        {
            mysqlCommand.ExecuteNonQuery();
        }
    }
}
