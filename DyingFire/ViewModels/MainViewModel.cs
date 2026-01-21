using DyingFire.Models;
using DyingFire.Services;
using System.Collections.ObjectModel; // Required for Lists that update UI
using System.Linq;

using System.ComponentModel; // Make sure this is at the top
using System.Windows;

namespace DyingFire.ViewModels
{
    // Inherit from ObservableObject now!
    public class MainViewModel : ObservableObject
    {
        private DatabaseService _dbService;

        // --- PROPERTIES FOR UI BINDING ---

        // The Quick Bar (Requirement: ObservableCollections)
        public ObservableCollection<GameItem> QuickBar { get; set; }

        // The Current View (The big image)
        private Location _currentLocation;
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged(); // Updates the big image when this changes
            }
        }

        // --- CONSTRUCTOR ---
        public MainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            // ----------------------

            _dbService = new DatabaseService();
            QuickBar = new ObservableCollection<GameItem>();

            LoadDummyData();
        }

        private void LoadDummyData()
        {
            // Create a test item
            QuickBar.Add(new GameItem
            {
                Name = "Rusty Key",
                ImagePath = "/Assets/Images/key_icon.png", // We will add this later
                Type = ItemType.Key
            });

            // Create a test room
            CurrentLocation = new Location(1, "/Assets/Images/room_start.jpg", "A dark room.");
        }
    }
}