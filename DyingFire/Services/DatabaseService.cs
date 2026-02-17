using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using DyingFire.Models;

namespace DyingFire.Services
{
    public class DatabaseService
    {
        private string _connectionString;

        public DatabaseService()
        {
            string dbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData.mdf");
            _connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbFile};Integrated Security=True";
        }

        public List<Location> LoadFullWorld()
        {
            var locations = new List<Location>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // --- PASS 1: LOAD LOCATIONS ---
                string sqlLoc = "SELECT * FROM Locations";
                using (var cmd = new SqlCommand(sqlLoc, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var loc = new Location(
                            Convert.ToInt32(reader["ID"]),
                            reader["ImagePath"].ToString(),
                            reader["Name"].ToString()
                        );

                        // Safely handle DBNulls for navigation
                        loc.LocationToNorth = reader["NorthID"] != DBNull.Value ? Convert.ToInt32(reader["NorthID"]) : -1;
                        loc.LocationToEast = reader["EastID"] != DBNull.Value ? Convert.ToInt32(reader["EastID"]) : -1;
                        loc.LocationToSouth = reader["SouthID"] != DBNull.Value ? Convert.ToInt32(reader["SouthID"]) : -1;
                        loc.LocationToWest = reader["WestID"] != DBNull.Value ? Convert.ToInt32(reader["WestID"]) : -1;

                        locations.Add(loc);
                    }
                }

                // --- PASS 2: LOAD INTERACTABLES ---
                string sqlInt = "SELECT * FROM Interactables";
                using (var cmd = new SqlCommand(sqlInt, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var interactable = new InteractableObject
                        {
                            Name = reader["Name"].ToString(),
                            X = Convert.ToDouble(reader["X"]),
                            Y = Convert.ToDouble(reader["Y"]),
                            Width = Convert.ToDouble(reader["Width"]),
                            Height = Convert.ToDouble(reader["Height"]),
                            IsLocked = reader["IsLocked"] != DBNull.Value && Convert.ToBoolean(reader["IsLocked"]),
                            RequiredItem = reader["RequiredItem"] != DBNull.Value ? reader["RequiredItem"].ToString() : null,
                            LockedMessage = reader["LockedMessage"] != DBNull.Value ? reader["LockedMessage"].ToString() : "It's locked.",
                            TargetLocationID = reader["TargetLocationID"] != DBNull.Value ? Convert.ToInt32(reader["TargetLocationID"]) : 0
                        };

                        // Find the room this object belongs to
                        if (reader["ParentLocationID"] != DBNull.Value)
                        {
                            int parentID = Convert.ToInt32(reader["ParentLocationID"]);
                            var parentRoom = locations.FirstOrDefault(x => x.ID == parentID);

                            if (parentRoom != null)
                            {
                                // FIXED: Changed 'obj' to 'interactable'
                                parentRoom.Interactables.Add(interactable);
                            }
                        }
                    }
                }

                // --- PASS 3: LOAD ITEMS ---
                string sqlItem = "SELECT * FROM GameItems";
                using (var cmd = new SqlCommand(sqlItem, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new GameItem
                        {
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            ImagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : null,
                            Type = (ItemType)Convert.ToInt32(reader["Type"]) // Cast INT from DB to ENUM safely
                        };

                        // Is it on the floor?
                        if (reader["ParentLocationID"] != DBNull.Value)
                        {
                            int roomID = Convert.ToInt32(reader["ParentLocationID"]);
                            var room = locations.FirstOrDefault(r => r.ID == roomID);
                            if (room != null) room.RoomItems.Add(item);
                        }
                        // Is it in a chest?
                        else if (reader["ParentInteractableID"] != DBNull.Value)
                        {
                            // HARD CODED FIX FOR LEARNING: 
                            int chestID = Convert.ToInt32(reader["ParentInteractableID"]);

                            // Logic: Find the chest in the Ritual Room (Room 3)
                            var ritualRoom = locations.FirstOrDefault(x => x.ID == 3);
                            var chest = ritualRoom?.Interactables.FirstOrDefault(x => x.Name == "Old Chest");

                            if (chest != null) chest.ItemsInside.Add(item);
                        }
                    }
                }
            }

            return locations;
        }
    }
}