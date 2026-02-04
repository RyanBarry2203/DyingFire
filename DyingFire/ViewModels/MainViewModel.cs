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
            // 1. Initialize Service
            var db = new DyingFire.Services.DatabaseService();

            // 2. Fetch Data (This one line replaces 50 lines of code!)
            _allLocations = db.LoadFullWorld();

            // 3. Start Game
            // We set the start room to ID 1 (The Cell)
            CurrentLocation = _allLocations.FirstOrDefault(x => x.ID == 1);
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