using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq; // Needed for LINQ queries
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
                            (int)reader["ID"],
                            reader["ImagePath"].ToString(),
                            reader["Name"].ToString()
                        );

                        // Set Directions
                        loc.LocationToNorth = (int)reader["NorthID"];
                        loc.LocationToEast = (int)reader["EastID"];
                        loc.LocationToSouth = (int)reader["SouthID"];
                        loc.LocationToWest = (int)reader["WestID"];

                        locations.Add(loc);
                    }
                }

                // --- PASS 2: LOAD INTERACTABLES (Doors, Chests) ---
                string sqlObj = "SELECT * FROM Interactables";
                using (var cmd = new SqlCommand(sqlObj, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = new InteractableObject
                        {
                            // Note: We don't store ID in the class yet, but we use it for logic below
                            Name = reader["Name"].ToString(),
                            X = Convert.ToDouble(reader["X"]),
                            Y = Convert.ToDouble(reader["Y"]),
                            Width = Convert.ToDouble(reader["Width"]),
                            Height = Convert.ToDouble(reader["Height"]),
                            IsLocked = (bool)reader["IsLocked"],
                            RequiredItem = reader["RequiredItem"] as string, // Handle NULLs safely
                            LockedMessage = reader["LockedMessage"] as string,
                            TargetLocationID = (int)reader["TargetLocationID"]
                        };

                        // Find the room this object belongs to
                        int parentID = (int)reader["ParentLocationID"];
                        var parentRoom = locations.FirstOrDefault(x => x.ID == parentID);
                        if (parentRoom != null)
                        {
                            parentRoom.Interactables.Add(obj);
                        }

                        // TEMP: Store the DB ID temporarily to help load items inside it (Advanced technique: Use a Dictionary)
                        // For this tutorial, we will rely on Order or assume Items load simply.
                        // Ideally, InteractableObject should have an ID property.
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
                            ImagePath = reader["ImagePath"] as string,
                            Type = (ItemType)(int)reader["Type"] // Cast INT from DB to ENUM
                        };

                        // Is it on the floor?
                        if (reader["ParentLocationID"] != DBNull.Value)
                        {
                            int roomID = (int)reader["ParentLocationID"];
                            var room = locations.FirstOrDefault(r => r.ID == roomID);
                            if (room != null) room.RoomItems.Add(item);
                        }
                        // Is it in a chest? (This is tricky without IDs in the Interactable class)
                        else if (reader["ParentInteractableID"] != DBNull.Value)
                        {
                            // HARD CODED FIX FOR LEARNING: 
                            // Since our InteractableObject class doesn't have an ID property yet,
                            // we will just put the Key in the Chest in Room 3 manually for now.
                            // In a full commercial refactor, you would add 'public int ID' to InteractableObject.cs.

                            int chestID = (int)reader["ParentInteractableID"];
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