using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DyingFire.Models;
using DyingFire.Factories;

namespace DyingFire.Services
{
    public class DatabaseService
    {
        private string _connectionString;
        public Dictionary<string, string> Config { get; private set; } = new Dictionary<string, string>();

        public DatabaseService()
        {
            string dbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData", "GameData.db");
            _connectionString = $"Data Source={dbFile};Version=3;";
        }

        public async Task<List<Location>> LoadFullWorldAsync()
        {
            return await Task.Run(() =>
            {
                var locations = new List<Location>();

                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = new SQLiteCommand("SELECT * FROM GameConfig", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) Config[reader["ConfigKey"].ToString()] = reader["ConfigValue"].ToString();
                    }

                    using (var cmd = new SQLiteCommand("SELECT * FROM Locations", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) locations.Add(GameFactory.CreateLocation(reader));
                    }

                    using (var cmd = new SQLiteCommand("SELECT * FROM Interactables", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var interactable = GameFactory.CreateInteractable(reader);
                            if (reader["ParentLocationID"] != DBNull.Value)
                            {
                                int parentID = Convert.ToInt32(reader["ParentLocationID"]);
                                locations.FirstOrDefault(x => x.ID == parentID)?.Interactables.Add(interactable);
                            }
                        }
                    }

                    using (var cmd = new SQLiteCommand("SELECT * FROM GameItems", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = GameFactory.CreateItem(reader);
                            if (reader["ParentInteractableID"] != DBNull.Value)
                            {
                                int containerID = Convert.ToInt32(reader["ParentInteractableID"]);
                                locations.SelectMany(loc => loc.Interactables).FirstOrDefault(i => i.ID == containerID)?.ItemsInside.Add(item);
                            }
                        }
                    }
                }
                return locations;
            });
        }
    }
}