using DyingFire.Models;
using System;
using System.Data.SQLite;

namespace DyingFire.Factories
{
    public static class GameFactory
    {
        // Create a Location from the current reader row.
        // The caller (DatabaseService) passes a reader positioned on a Locations row.
        public static Location CreateLocation(SQLiteDataReader reader)
        {
            return new Location(
                // Map the database ID column to the Location ID.
                Convert.ToInt32(reader["ID"]),
                // Map the image path used by the UI to show the room background.
                reader["ImagePath"].ToString(),
                // Map the human-readable name/description for the room.
                reader["Name"].ToString()
            )
            {
                // Neighboring room IDs; -1 means no neighbor in that direction.
                LocationToNorth = reader["NorthID"] != DBNull.Value ? Convert.ToInt32(reader["NorthID"]) : -1,
                LocationToEast  = reader["EastID"]  != DBNull.Value ? Convert.ToInt32(reader["EastID"])  : -1,
                LocationToSouth = reader["SouthID"] != DBNull.Value ? Convert.ToInt32(reader["SouthID"]) : -1,
                LocationToWest  = reader["WestID"]  != DBNull.Value ? Convert.ToInt32(reader["WestID"])  : -1,
                // IsDark flag affects sanity drain and visibility logic elsewhere.
                IsDark = reader["IsDark"] != DBNull.Value && Convert.ToBoolean(reader["IsDark"]),
                // NoiseLevel is used by MonsterAI to choose where to move.
                NoiseLevel = reader["NoiseLevel"] != DBNull.Value ? Convert.ToInt32(reader["NoiseLevel"]) : 0
            };
        }

        // Create an InteractableObject from the current reader row.
        // The DatabaseService attaches the created object to a Location after creation.
        public static InteractableObject CreateInteractable(SQLiteDataReader reader)
        {
            return new InteractableObject
            {
                // Unique id used to reference this interactable.
                ID = Convert.ToInt32(reader["ID"]),
                // Display name shown in popups and messages.
                Name = reader["Name"].ToString(),
                // Position on the background image where the clickable area is placed.
                X = Convert.ToDouble(reader["X"]),
                Y = Convert.ToDouble(reader["Y"]),
                // Size of the clickable area in the view.
                Width = Convert.ToDouble(reader["Width"]),
                Height = Convert.ToDouble(reader["Height"]),
                // Whether the object is locked; used by LockedDoorInteraction.
                IsLocked = reader["IsLocked"] != DBNull.Value && Convert.ToBoolean(reader["IsLocked"]),
                // The item name required to unlock this object (e.g. a key).
                RequiredItem = reader["RequiredItem"] != DBNull.Value ? reader["RequiredItem"].ToString() : null,
                // Message shown when the player interacts but the object is locked.
                LockedMessage = reader["LockedMessage"] != DBNull.Value ? reader["LockedMessage"].ToString() : "It's locked.",
                // If > 0 this interactable transitions the player to another Location by ID.
                TargetLocationID = reader["TargetLocationID"] != DBNull.Value ? Convert.ToInt32(reader["TargetLocationID"]) : 0,
                // Whether this object can be used as a hiding spot (used by HideInteraction/HidingState).
                CanHideInside = reader["CanHideInside"] != DBNull.Value && Convert.ToBoolean(reader["CanHideInside"]),
                // Optional image to show when the player is hiding inside this object.
                HidingImagePath = reader["HidingImagePath"] != DBNull.Value ? reader["HidingImagePath"].ToString() : null
            };
        }

        // Create a GameItem from the current reader row.
        // Items are attached either to an Interactable or to the player's inventory later.
        public static GameItem CreateItem(SQLiteDataReader reader)
        {
            return new GameItem
            {
                // Item display name.
                Name = reader["Name"].ToString(),
                // Short description shown in the inventory UI.
                Description = reader["Description"].ToString(),
                // Optional image path used by the UI.
                ImagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : null,
                // Item type stored as int in the DB and cast to the enum here.
                Type = (ItemType)Convert.ToInt32(reader["Type"]),
                // Optional lore text shown when reading the item.
                Lore = reader["Lore"] != DBNull.Value ? reader["Lore"].ToString() : null,
                // Optional use message or metadata used by item strategies.
                UseMessage = reader["UseMessage"] != DBNull.Value ? reader["UseMessage"].ToString() : null
            };
        }
    }
}