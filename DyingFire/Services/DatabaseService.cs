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
        // The connection string points to the local GameData.db file.
        // The constructor builds this from the application's base directory.
        private string _connectionString;

        // Simple key/value configuration loaded from the GameConfig table.
        // Other systems read values from this Config dictionary after loading the world.
        public Dictionary<string, string> Config { get; private set; } = new Dictionary<string, string>();

        public DatabaseService()
        {
            // Build the path to the SQLite file shipped with the game.
            string dbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData", "GameData.db");
            _connectionString = $"Data Source={dbFile};Version=3;"; // used by SQLiteConnection
        }

        // LoadFullWorldAsync reads all locations, interactables and items and returns them as model objects.
        // This method runs the blocking database work on a background thread so the UI thread is not blocked.
        public async Task<List<Location>> LoadFullWorldAsync()
        {
            return await Task.Run(() =>
            {
                var locations = new List<Location>();

                // Open a connection to the SQLite database file.
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();

                    // Read simple game configuration into the Config dictionary.
                    // Other systems use Config keys (for example audio paths or difficulty settings).
                    using (var cmd = new SQLiteCommand("SELECT * FROM GameConfig", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) Config[reader["ConfigKey"].ToString()] = reader["ConfigValue"].ToString();
                    }

                    // Load all location rows and convert each row into a Location model using GameFactory.
                    // GameFactory maps the reader columns to the Location properties.
                    using (var cmd = new SQLiteCommand("SELECT * FROM Locations", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) locations.Add(GameFactory.CreateLocation(reader));
                    }

                    // Load interactable objects and attach each to its parent location by ParentLocationID.
                    // The reader must include a ParentLocationID column for this to work.
                    // Interactable objects are created via GameFactory.CreateInteractable.
                    using (var cmd = new SQLiteCommand("SELECT * FROM Interactables", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var interactable = GameFactory.CreateInteractable(reader);
                            if (reader["ParentLocationID"] != DBNull.Value)
                            {
                                int parentID = Convert.ToInt32(reader["ParentLocationID"]);
                                // Find the location with matching ID and add the interactable to its collection.
                                locations.FirstOrDefault(x => x.ID == parentID)?.Interactables.Add(interactable);
                            }
                        }
                    }

                    // Load items and attach each to its parent interactable by ParentInteractableID.
                    // Items are created with GameFactory.CreateItem and then added to the ItemsInside collection.
                    using (var cmd = new SQLiteCommand("SELECT * FROM GameItems", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = GameFactory.CreateItem(reader);
                            if (reader["ParentInteractableID"] != DBNull.Value)
                            {
                                int containerID = Convert.ToInt32(reader["ParentInteractableID"]);
                                // Search all locations' interactables for the matching container and add the item.
                                locations.SelectMany(loc => loc.Interactables).FirstOrDefault(i => i.ID == containerID)?.ItemsInside.Add(item);
                            }
                        }
                    }
                }
                // Return the fully populated list of locations. The caller (usually MainViewModel) will store these.
                return locations;
            });
        }
    }
}