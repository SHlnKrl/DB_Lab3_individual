using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DB_Lab3_individual
{
    class DBUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "localhost";
            int port = 3306;
            string database = "lab1_individual";
            string username = "Horse";
            string password = "hard_pass";

            return DBMySQL.GetDBConnection(host, port, database, username, password);
        }
    }
}
