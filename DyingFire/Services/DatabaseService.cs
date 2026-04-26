using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DyingFire.Models;

namespace DyingFire.Services
{
    public class DatabaseService
    {
        private string _connectionString;

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

                    string sqlLoc = "SELECT * FROM Locations";
                    using (var cmd = new SQLiteCommand(sqlLoc, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var loc = new Location(
                                Convert.ToInt32(reader["ID"]),
                                reader["ImagePath"].ToString(),
                                reader["Name"].ToString()
                            );

                            loc.LocationToNorth = reader["NorthID"] != DBNull.Value ? Convert.ToInt32(reader["NorthID"]) : -1;
                            loc.LocationToEast = reader["EastID"] != DBNull.Value ? Convert.ToInt32(reader["EastID"]) : -1;
                            loc.LocationToSouth = reader["SouthID"] != DBNull.Value ? Convert.ToInt32(reader["SouthID"]) : -1;
                            loc.LocationToWest = reader["WestID"] != DBNull.Value ? Convert.ToInt32(reader["WestID"]) : -1;

                            locations.Add(loc);
                        }
                    }

                    string sqlInt = "SELECT * FROM Interactables";
                    using (var cmd = new SQLiteCommand(sqlInt, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var interactable = new InteractableObject
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString(),
                                X = Convert.ToDouble(reader["X"]),
                                Y = Convert.ToDouble(reader["Y"]),
                                Width = Convert.ToDouble(reader["Width"]),
                                Height = Convert.ToDouble(reader["Height"]),
                                IsLocked = reader["IsLocked"] != DBNull.Value && Convert.ToBoolean(reader["IsLocked"]),
                                RequiredItem = reader["RequiredItem"] != DBNull.Value ? reader["RequiredItem"].ToString() : null,
                                LockedMessage = reader["LockedMessage"] != DBNull.Value ? reader["LockedMessage"].ToString() : "It's locked.",
                                TargetLocationID = reader["TargetLocationID"] != DBNull.Value ? Convert.ToInt32(reader["TargetLocationID"]) : 0,
                                CanHideInside = reader["CanHideInside"] != DBNull.Value && Convert.ToBoolean(reader["CanHideInside"])
                            };

                            if (reader["ParentLocationID"] != DBNull.Value)
                            {
                                int parentID = Convert.ToInt32(reader["ParentLocationID"]);
                                var parentRoom = locations.FirstOrDefault(x => x.ID == parentID);
                                if (parentRoom != null) parentRoom.Interactables.Add(interactable);
                            }
                        }
                    }

                    string sqlItem = "SELECT * FROM GameItems";
                    using (var cmd = new SQLiteCommand(sqlItem, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new GameItem
                            {
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                ImagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : null,
                                Type = (ItemType)Convert.ToInt32(reader["Type"]),
                                Lore = reader["Lore"] != DBNull.Value ? reader["Lore"].ToString() : null
                            };

                            if (reader["ParentLocationID"] != DBNull.Value)
                            {
                                int roomID = Convert.ToInt32(reader["ParentLocationID"]);
                                var room = locations.FirstOrDefault(r => r.ID == roomID);
                                if (room != null) room.RoomItems.Add(item);
                            }
                            else if (reader["ParentInteractableID"] != DBNull.Value)
                            {
                                int chestID = Convert.ToInt32(reader["ParentInteractableID"]);
                                var container = locations.SelectMany(loc => loc.Interactables).FirstOrDefault(inter => inter.ID == chestID);
                                if (container != null) container.ItemsInside.Add(item);
                            }
                        }
                    }
                }
                return locations;
            });
        }
    }
}