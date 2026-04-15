using System.Collections.ObjectModel;
namespace DyingFire.Models
{
    public class Location
    {
        public int ID { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public int NoiseLevel { get; set; } = 0;
        public bool IsDark { get; set; } = false;

        public ObservableCollection<GameItem> RoomItems { get; set; }
        public ObservableCollection<InteractableObject> Interactables { get; set; }

        public int LocationToNorth { get; set; } = -1;
        public int LocationToEast { get; set; } = -1;
        public int LocationToSouth { get; set; } = -1;
        public int LocationToWest { get; set; } = -1;

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