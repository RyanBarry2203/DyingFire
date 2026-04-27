using System.Collections.ObjectModel;
namespace DyingFire.Models
{
    // A Location is a room the player can be in.
    // The MainViewModel holds the current Location and the UI binds to its ImagePath and Description.
    public class Location
    {
        // Unique id that comes from the database.
        // GameFactory sets this when loading locations.
        // Other systems use the ID to find or reference this location.
        public int ID { get; set; }

        // Path to the background image shown by the UI when the player is in this room.
        // The MainViewModel.BackgroundImage is set from this value on movement.
        public string ImagePath { get; set; }

        // Short text describing the room that the UI can display.
        public string Description { get; set; }

        // NoiseLevel can be used by AI or game logic to decide how noisy this room is.
        // GameLoop or MonsterAIService may read this to react to the player.
        public int NoiseLevel { get; set; } = 0;

        // If true, the room is dark. The UI or game logic can check this to modify visibility.
        public bool IsDark { get; set; } = false;

        // Items lying in the room. This collection is bound to the UI inventory/search view.
        // Interaction handlers and the InventorySystem move items between this collection and the player's inventory.
        public ObservableCollection<GameItem> RoomItems { get; set; }

        // Interactable objects (doors, drawers, hiding spots) that appear in the room.
        // The UI uses the positions and sizes to render clickable areas.
        // ActionSystem and interaction handlers use this list to handle clicks.
        public ObservableCollection<InteractableObject> Interactables { get; set; }

        // Neighboring location IDs used for movement.
        // ActionSystem.Move reads these properties to find the id of the room in a given direction.
        // A value of -1 means there is no location in that direction.
        public int LocationToNorth { get; set; } = -1;
        public int LocationToEast { get; set; } = -1;
        public int LocationToSouth { get; set; } = -1;
        public int LocationToWest { get; set; } = -1;

        // Construct a Location with required basic data.
        // The GameFactory calls this when creating Location instances from the database.
        // The constructor also initializes the collections so callers can add items and interactables safely.
        public Location(int id, string imagePath, string desc)
        {
            ID = id;
            ImagePath = imagePath;
            Description = desc;

            RoomItems = new ObservableCollection<GameItem>();
            Interactables = new ObservableCollection<InteractableObject>();
        }
    }
}