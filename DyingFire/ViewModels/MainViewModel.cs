using DyingFire.Models;
using DyingFire.Services;
using DyingFire.States;
using DyingFire.Systems;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;

namespace DyingFire.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        // Toggle to enable monster behavior for testing or gameplay.
        public bool IsMonsterEnabled { get; set; } = false;

        // Service that loads game data from the SQLite file.
        public DatabaseService DBService { get; }

        // Manages the stack of game states like HidingState.
        public GameStateManager StateManager { get; }

        // Handles the player's inventory and item usage.
        public InventorySystem Inventory { get; }

        // Encapsulates move / interact commands and maps them to UI commands.
        public ActionSystem Actions { get; }

        // Controls monster movement and distance logic.
        public MonsterAIService MonsterAI { get; }

        // Runs the game timer, ticks, and high level game loop logic.
        public GameLoopSystem GameLoop { get; }

        // Plays background music, tension and SFX.
        public AudioService Audio { get; } = new AudioService();

        // All loaded locations in the world.
        public List<Location> AllLocations { get; set; }

        // Shortcut to configuration values loaded by DBService.
        public Dictionary<string, string> Config => DBService.Config;

        // Flag set by game systems to indicate paranormal events.
        public bool IsParanormalActivityPresent { get; set; } = false;

        // Backing field for Sanity shown in the UI.
        private int _sanity = 100;
        // Player sanity value that the UI binds to.
        public int Sanity { get { return _sanity; } set { _sanity = value; OnPropertyChanged(); } }

        // Backing field for Vitality shown in the UI.
        private int _vitality = 100;
        // Player vitality value that the UI binds to.
        public int Vitality { get { return _vitality; } set { _vitality = value; OnPropertyChanged(); } }

        // Backing field for the currently active location.
        private Location _currentLocation;
        // CurrentLocation is the room the player is in; changing it notifies bindings and movement availability.
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set { _currentLocation = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanMoveNorth)); OnPropertyChanged(nameof(CanMoveEast)); OnPropertyChanged(nameof(CanMoveSouth)); OnPropertyChanged(nameof(CanMoveWest)); }
        }

        // Backing field for the background image path the UI shows.
        private string _backgroundImage;
        // BackgroundImage is read by the view to show the room image.
        public string BackgroundImage { get { return _backgroundImage; } set { _backgroundImage = value; OnPropertyChanged(); } }

        // Backing field for whether the hiding screen is active.
        private bool _isHidingUI;
        // IsHidingUI tells the view to show hiding UI and also disables movement.
        public bool IsHidingUI
        {
            get { return _isHidingUI; }
            set { _isHidingUI = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanMoveNorth)); OnPropertyChanged(nameof(CanMoveEast)); OnPropertyChanged(nameof(CanMoveSouth)); OnPropertyChanged(nameof(CanMoveWest)); }
        }

        // Convenience properties used by the UI to enable/disable move buttons.
        public bool CanMoveNorth => CurrentLocation != null && CurrentLocation.LocationToNorth != -1 && !IsHidingUI;
        public bool CanMoveEast => CurrentLocation != null && CurrentLocation.LocationToEast != -1 && !IsHidingUI;
        public bool CanMoveSouth => CurrentLocation != null && CurrentLocation.LocationToSouth != -1 && !IsHidingUI;
        public bool CanMoveWest => CurrentLocation != null && CurrentLocation.LocationToWest != -1 && !IsHidingUI;


        // Backing for the item currently selected in the inventory details view.
        private GameItem _selectedInventoryItem;
        // SelectedInventoryItem is used by the UI when showing item details.
        public GameItem SelectedInventoryItem { get { return _selectedInventoryItem; } set { _selectedInventoryItem = value; OnPropertyChanged(); } }

        // Backing for the inventory panel visibility.
        private bool _isInventoryVisible;
        // Controls whether the inventory panel is visible in the UI.
        public bool IsInventoryVisible { get { return _isInventoryVisible; } set { _isInventoryVisible = value; OnPropertyChanged(); } }

        // Backing for whether a popup message is shown.
        private bool _isPopupVisible;
        // Controls whether the message popup is visible.
        public bool IsPopupVisible { get { return _isPopupVisible; } set { _isPopupVisible = value; OnPropertyChanged(); } }

        // Backing for the popup title text.
        private string _popupTitle;
        // Title shown in the popup.
        public string PopupTitle { get { return _popupTitle; } set { _popupTitle = value; OnPropertyChanged(); } }

        // Backing for the popup message body.
        private string _popupMessage;
        // Message shown in the popup body.
        public string PopupMessage { get { return _popupMessage; } set { _popupMessage = value; OnPropertyChanged(); } }

        // Expose quickbar collection directly for binding.
        public ObservableCollection<GameItem> QuickBar => Inventory.QuickBar;
        // Expose full inventory collection for binding.
        public ObservableCollection<GameItem> FullInventory => Inventory.FullInventory;


        // Commands exposed to the UI are provided by the systems to keep logic out of the view model.
        public ICommand MoveCommand => Actions.MoveCommand;
        public ICommand InteractCommand => Actions.InteractCommand;
        public ICommand StopHidingCommand => Actions.StopHidingCommand;
        public ICommand UseItemCommand => Inventory.UseItemCommand;
        public ICommand EquipItemCommand => Inventory.EquipItemCommand;
        public ICommand SelectQuickSlotCommand => Inventory.SelectQuickSlotCommand;
        // ToggleInventoryCommand and ClosePopupCommand are constructed locally.
        public ICommand ToggleInventoryCommand { get; }
        public ICommand ClosePopupCommand { get; }

        // Constructor wires services, systems and commands.
        public MainViewModel()
        {
            // Create the database service used to load data.
            DBService = new DatabaseService();

            // Create the state manager that holds game states.
            StateManager = new GameStateManager();

            // Create the monster AI controller.
            MonsterAI = new MonsterAIService();

            // Create systems that depend on this view model.
            Inventory = new InventorySystem(this);
            Actions = new ActionSystem(this);
            GameLoop = new GameLoopSystem(this);

            // Simple command to toggle the inventory UI.
            ToggleInventoryCommand = new RelayCommand<object>(_ => IsInventoryVisible = !IsInventoryVisible);

            // Close popup command; quits the app on GAME OVER, otherwise hides the popup.
            ClosePopupCommand = new RelayCommand<object>(_ =>
            {
                if (PopupTitle == "GAME OVER") Application.Current.Shutdown();
                else IsPopupVisible = false;
            });

            // If we are in the XAML designer do not try to load data.
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;

            // Load locations, interactables and items from the database asynchronously.
            LoadGameWorldAsync();
        }

        // Loads the full game world on a background thread and initializes starting state.
        private async void LoadGameWorldAsync()
        {
            // Ask the DBService to read everything.
            AllLocations = await DBService.LoadFullWorldAsync();

            // If data loaded, pick the starting location and start audio and the game loop.
            if (AllLocations != null && AllLocations.Count > 0)
            {
                // Start the player in location ID 1 if it exists.
                CurrentLocation = AllLocations.FirstOrDefault(x => x.ID == 1);
                // Set the UI background image to match the current location.
                BackgroundImage = CurrentLocation.ImagePath;

                // Start the main background music using the config value.
                Audio.PlayBGM(Config["BGM_Main"]);

                // Start the repeating game loop timer.
                GameLoop.Start();
            }
        }

        // Shows a popup with a title and message.
        // Other systems call this to display messages to the player.
        public void ShowMessage(string title, string message)
        {
            PopupTitle = title;
            PopupMessage = message;
            IsPopupVisible = true;
        }
    }
}