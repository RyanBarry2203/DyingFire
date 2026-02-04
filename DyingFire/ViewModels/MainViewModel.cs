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
            var library = new Location(1, "/Assets/Images/Library.png", "Ritual Room");
            var hallway = new Location(2, "/Assets/Images/hallway.png", "Spooky Hallway");
            var room2 = new Location(3, "/Assets/Images/room2.png", "Starting Bedroom");

            var normalStairs = new Location(4, "/Assets/Images/Stairs.png", "Normal Stairs");
            var spookyStairs = new Location(5, "/Assets/Images/fellaOnTheStairs.png", "Fella On The Stairs!");
            var spookyHallway = new Location(6, "/Assets/Images/darkSpookyHallway.png", "Dark Hallway");
            var bedRoom = new Location(7, "/Assets/Images/AnotherBedroom.png", "Bedroom");
            var jumpScareF1 = new Location(8, "/Assets/Images/JumpScareFrame1.png", "JumpScareF1");
            var jumpScareF2 = new Location(9, "/Assets/Images/jumpScareFrame2.png", "JumpScareF2");
            var yellowHallway = new Location(10, "/Assets/Images/yellowHallwayShadow.png", "Yellow Hallway");
            var scaryMirror = new Location(11, "/Assets/Images/scaryMirror.png", "Scary Mirror");

            // 2. Connect Rooms
            library.LocationToNorth = 2;      // Room 1 -> Hallway
            hallway.LocationToSouth = 1;    // Hallway -> Room 1
            hallway.LocationToNorth = 3;    // Hallway -> Room 2
            hallway.LocationToEast = -1;     // Hallway -> Room 1
            room2.LocationToSouth = 2;      // Room 2 -> Hallway

            normalStairs.LocationToSouth = 2;    // Normal Stairs -> Hallway
            normalStairs.LocationToNorth = 5;    // Normal Stairs -> Spooky Stairs
            spookyStairs.LocationToSouth = 4;    // Spooky Stairs -> Normal Stairs
            spookyStairs.LocationToEast = 6;    // Spooky Stairs -> Spooky Hallway
            spookyHallway.LocationToSouth = 5;    // Spooky Hallway -> Spooky Stairs
            spookyHallway.LocationToNorth = 7;    // Spooky Hallway -> Bedroom
            bedRoom.LocationToSouth = 6;    // Bedroom -> Spooky Hallway
            bedRoom.LocationToNorth = 8;    // Bedroom -> JumpScareF1
            jumpScareF1.LocationToSouth = 9;    // JumpScareF1 -> Bedroom
            jumpScareF1.LocationToNorth = 9;    // JumpScareF1 -> JumpScareF2
            jumpScareF1.LocationToEast = 9;
            jumpScareF1.LocationToWest = 9;    // JumpScareF2 -> JumpScareF1
            jumpScareF2.LocationToSouth = 7;    // JumpScareF2 -> JumpScareF1
            jumpScareF2.LocationToNorth = 10;    // JumpScareF2 -> Yellow Hallway
            yellowHallway.LocationToSouth = 8;    // Yellow Hallway -> JumpScareF2
            yellowHallway.LocationToEast = 11;    // Yellow Hallway -> Scary Mirror
            scaryMirror.LocationToSouth = 10;    // Scary Mirror -> Yellow Hallway




            // 3. Add to Map
            _allLocations.Add(library);
            _allLocations.Add(hallway);
            _allLocations.Add(room2);


            _allLocations.Add(normalStairs);
            _allLocations.Add(spookyStairs);
            _allLocations.Add(spookyHallway);
            _allLocations.Add(bedRoom);
            _allLocations.Add(jumpScareF1);
            _allLocations.Add(jumpScareF2);
            _allLocations.Add(yellowHallway);
            _allLocations.Add(scaryMirror);

            // 4. Start Game
            CurrentLocation = room2;

            // Add dummy key
            //QuickBar.Add(new GameItem { Name = "Rusty Key", Type = ItemType.Key, Description = "An old key covered in rust. I wonder if it still works" });

            var door = new InteractableObject { Name = "Wooden Door", X = 595, Y = 175, Width = 90, Height = 250, IsLocked = true, RequiredItem = "Rusty Key", LockedMessage = "The door is locked, i wonder if that rusty key hole works?", TargetLocationID = 4};

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

            if (CurrentLocation != null && CurrentLocation.Interactables != null)
            {
                foreach (var interactable in CurrentLocation.Interactables)
                {
                    if (interactable.IsLocked == false)
                    {
                        // Logic to allow movement through this interactable if needed

                    }
                }
            }
        }
        public void EnterLocation(int locationID)
        {
            var location = _allLocations.FirstOrDefault(x => x.ID == locationID);
            if (location != null) CurrentLocation = location;
        }
    }
}