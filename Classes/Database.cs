using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace CategoryProducts
{
    public class Database
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=(Your Key);Initial Catalog=(Your DB);Integrated Security=True");
        public virtual void OpenConnection() {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }
        public virtual void CloseConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }
        public SqlConnection GetConnection() {
            return sqlConnection;
        }
       

    }
}
