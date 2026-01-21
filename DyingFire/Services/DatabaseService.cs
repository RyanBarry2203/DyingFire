using Microsoft.Data.Sqlite; // NEW NAMESPACE
using System.IO;

namespace DyingFire.Services
{
    public class DatabaseService
    {
        private string DbName;
        private string ConnectionString;

        public DatabaseService()
        {
            string folder = System.AppDomain.CurrentDomain.BaseDirectory;
            DbName = System.IO.Path.Combine(folder, "DyingFire.db");
            ConnectionString = "Data Source=" + DbName; // Version=3 is not needed anymore

            //InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            // Microsoft.Data.Sqlite creates the file automatically when you Open(),
            // so we don't need the "CreateFile" command anymore.

            // Note the spelling: SqliteConnection (Only 'S' is capital)
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS PlayerStats (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Level INTEGER,
                        HellFlameAmount INTEGER,
                        DateSaved TEXT
                    )";

                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}