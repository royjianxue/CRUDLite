using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace CRUDLiteLibrary
{
    internal class DataBaseLogic
    {
        const string connectionString = "Data Source=Coding_Time_Tracker.db";
        internal static SQLiteConnection CreateConnection()
        {
           
            var conn = new SQLiteConnection(connectionString);
            conn.Open();
            return conn;
        }
        internal static void CreateTable()
        {
            using (var conn = CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                            @"
                            CREATE TABLE IF NOT EXISTS CodingTracker 
                            (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Date TEXT,
                                Hour DOUBLE
                            )
                            ";
                    cmd.ExecuteNonQuery();
                    Console.ReadKey();

                }
            }
        }


    }
}

