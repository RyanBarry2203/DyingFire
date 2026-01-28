//using Microsoft.Data.Sqlite; // NEW NAMESPACE
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
            ?Zjkhdfksbsjk
        }
    }
}