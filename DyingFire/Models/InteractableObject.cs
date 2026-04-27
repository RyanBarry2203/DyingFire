using System.Collections.ObjectModel;
using DyingFire.ViewModels;

namespace DyingFire.Models
{
    public class InteractableObject : ObservableObject
    {
        // Unique id from the database. Used to look up this object and to reference it elsewhere.
        public int ID { get; set; }

        // Display name shown in the UI and used in messages.
        public string Name { get; set; }

        // Position and size are used by the view to place the clickable area on the background image.
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        // Backing field for the lock state so we can notify the UI when it changes.
        private bool _isLocked;
        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; OnPropertyChanged(); } // When lock state changes the UI updates automatically.
        }

        // The name of an item required to unlock this object (for example a key).
        // Other systems check this value before allowing an interaction that consumes or requires an item.
        public string RequiredItem { get; set; }

        // Message shown when the player tries to interact but the object is locked.
        public string LockedMessage { get; set; } = "It's locked.";

        // If this object causes a room transition, this holds the target location id.
        // ActionSystem and interaction handlers use this to change the player's location.
        public int TargetLocationID { get; set; } = 0;

        // If true, player can hide inside this object (for example a wardrobe or closet).
        // Hiding logic and the hiding state use this flag to allow or prevent hiding.
        public string HidingImagePath { get; set; }
        public bool CanHideInside { get; set; } = false;

        // Items that are stored inside this interactable (for example loot in a drawer).
        // The inventory system or interaction handlers will move items from here into the player's inventory.
        public ObservableCollection<GameItem> ItemsInside { get; set; }

        // Ensure the collection is initialized so callers can add or enumerate safely.
        public InteractableObject()
        {
            ItemsInside = new ObservableCollection<GameItem>();
        }

        // ActionText is a simple helper used by the UI to show what action will occur if clicked.
        // If the object can hide inside, show "Hide".
        // If it leads to another room or is locked, show "Interact".
        // Otherwise show "Search" to indicate the player can look for items.
        public string ActionText
        {
            get
            {
                if (CanHideInside) return "Hide";
                if (TargetLocationID > 0 || IsLocked) return "Interact";
                return "Search";
            }
        }
    }
}