using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagementSystem
{
    public class DataAccess : IDisposable
    {
        private readonly SqlConnection sqlcon;

        public DataAccess()
        {
          

            sqlcon = new SqlConnection(
@"Server=DESKTOP-S3IFP7G;
  Database=Library Management System;
  Integrated Security=True;
  Encrypt=False;
  TrustServerCertificate=True;
  MultipleActiveResultSets=True;");



   

            sqlcon.Open();
          

        }

        public SqlConnection Connection => sqlcon;

        public void Dispose()
        {
            if (sqlcon.State == ConnectionState.Open)
                sqlcon.Close();
        }
    }
}
