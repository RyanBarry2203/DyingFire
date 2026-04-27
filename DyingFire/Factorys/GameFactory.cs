using DyingFire.Models;
using System;
using System.Data.SQLite;

namespace DyingFire.Factories
{
    public static class GameFactory
    {
        public static Location CreateLocation(SQLiteDataReader reader)
        {
            return new Location(
                Convert.ToInt32(reader["ID"]),
                reader["ImagePath"].ToString(),
                reader["Name"].ToString()
            )
            {
                LocationToNorth = reader["NorthID"] != DBNull.Value ? Convert.ToInt32(reader["NorthID"]) : -1,
                LocationToEast = reader["EastID"] != DBNull.Value ? Convert.ToInt32(reader["EastID"]) : -1,
                LocationToSouth = reader["SouthID"] != DBNull.Value ? Convert.ToInt32(reader["SouthID"]) : -1,
                LocationToWest = reader["WestID"] != DBNull.Value ? Convert.ToInt32(reader["WestID"]) : -1,
                IsDark = reader["IsDark"] != DBNull.Value && Convert.ToBoolean(reader["IsDark"]),
                NoiseLevel = reader["NoiseLevel"] != DBNull.Value ? Convert.ToInt32(reader["NoiseLevel"]) : 0
            };
        }

        public static InteractableObject CreateInteractable(SQLiteDataReader reader)
        {
            return new InteractableObject
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
                CanHideInside = reader["CanHideInside"] != DBNull.Value && Convert.ToBoolean(reader["CanHideInside"]),
                HidingImagePath = reader["HidingImagePath"] != DBNull.Value ? reader["HidingImagePath"].ToString() : null
            };
        }

        public static GameItem CreateItem(SQLiteDataReader reader)
        {
            return new GameItem
            {
                Name = reader["Name"].ToString(),
                Description = reader["Description"].ToString(),
                ImagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : null,
                Type = (ItemType)Convert.ToInt32(reader["Type"]),
                Lore = reader["Lore"] != DBNull.Value ? reader["Lore"].ToString() : null,
                UseMessage = reader["UseMessage"] != DBNull.Value ? reader["UseMessage"].ToString() : null
            };
        }
    }
}