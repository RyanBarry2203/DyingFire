namespace DyingFire.Models
{
    public class Location
    {
        public int ID { get; set; }
        public string ImagePath { get; set; } // The main background image
        public string Description { get; set; }

        // Navigation IDs. 
        // If ID is -1, it means you can't go that way (the arrow will be hidden).
        public int LocationToNorth { get; set; } = -1;
        public int LocationToEast { get; set; } = -1; // Right
        public int LocationToSouth { get; set; } = -1; // Back/Down
        public int LocationToWest { get; set; } = -1; // Left

        // Constructor
        public Location(int id, string imagePath, string desc)
        {
            ID = id;
            ImagePath = imagePath;
            Description = desc;
        }
    }
}