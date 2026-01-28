using DyingFire.Models;
//using DyingFire.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace DyingFire.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        //private DatabaseService _dbService;
        private List<Location> _allLocations; // The Map

        // --- PROPERTIES ---
        public ObservableCollection<GameItem> QuickBar { get; set; }

        private Location _currentLocation;
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set { _currentLocation = value; OnPropertyChanged(); }
        }

        // --- CONSTRUCTOR ---
        public MainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;

            // _dbService = new DatabaseService(); // Database disabled for now
            QuickBar = new ObservableCollection<GameItem>();

            for (int i = 0; i < 5; i++)
            {
                QuickBar.Add(null); // Initialize with empty slots
            }

            LoadGameWorld();
        }

        private void LoadGameWorld()
        {
            _allLocations = new List<Location>();

            // 1. Create Rooms
            var room1 = new Location(1, "/Assets/Images/room1.png", "The Starting Cell");
            var hallway = new Location(2, "/Assets/Images/hallway.png", "The Dark Hallway");
            var room2 = new Location(3, "/Assets/Images/room2.png", "The Ritual Room");

            // 2. Connect Rooms
            room1.LocationToNorth = 2;      // Room 1 -> Hallway
            hallway.LocationToSouth = 1;    // Hallway -> Room 1
            hallway.LocationToNorth = 3;    // Hallway -> Room 2
            room2.LocationToSouth = 2;      // Room 2 -> Hallway

            var room4 = new Location(4, "/Assets/Images/room4.png", "Abandoned Library");
            room4.LocationToWest = 2;      // Room 4 -> Hallway

            hallway.LocationToEast = 4;    // Hallway -> Room 4
            room4.LocationToWest = 3;


            // 3. Add to Map
            _allLocations.Add(room1);
            _allLocations.Add(hallway);
            _allLocations.Add(room2);
            _allLocations.Add(room4);

            // 4. Start Game
            CurrentLocation = room1;

            // Add dummy key
            //QuickBar.Add(new GameItem { Name = "Rusty Key", Type = ItemType.Key, Description = "An old key covered in rust. I wonder if it still works" });

            var door = new InteractableObject { Name = "Wooden Door", X = 400, Y = 150, Width = 180, Height = 250, IsLocked = true, RequiredItem = "Rusty Key", LockedMessage = "The door is locked, i wonder if that rusty key hole works?", TargetLocationID = 4};

            hallway.Interactables.Add(door);

            var chest = new InteractableObject { Name = "Old Chest", X = 425, Y = 400, Width = 200, Height = 100 };

            chest.ItemsInside.Add(new GameItem
            {
                Name = "Rusty Key",
                Type = ItemType.Key,
                Description = "An old key covered in rust. I wonder if it still works",
                //ImagePath = "/Assets/Images/rusty_key.png"
            });

            room2.Interactables.Add(chest);
        }

        // --- MOVEMENT LOGIC ---
        public void Move(string direction)
        {
            if (CurrentLocation == null) return;

            int newLocationID = -1;

            switch (direction)
            {
                case "North": newLocationID = CurrentLocation.LocationToNorth; break;
                case "South": newLocationID = CurrentLocation.LocationToSouth; break;
                case "East": newLocationID = CurrentLocation.LocationToEast; break;
                case "West": newLocationID = CurrentLocation.LocationToWest; break;
            }

            if (newLocationID != -1)
            {
                var newRoom = _allLocations.FirstOrDefault(x => x.ID == newLocationID);
                if (newRoom != null) CurrentLocation = newRoom;
            }
        }
        public void EnterLocation(int locationID)
        {
            var location = _allLocations.FirstOrDefault(x => x.ID == locationID);
            if (location != null) CurrentLocation = location;
        }
    }
}