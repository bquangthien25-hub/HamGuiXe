using System.Data.SqlClient;

namespace ParkingApp.Utils
{
    public class Database
    {
        private static string connectionString =
      @"Server=.\SQLEXPRESS;
     Database=ParkingDB;
        Trusted_Connection=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
