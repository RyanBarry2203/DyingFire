using DyingFire.Models;
using DyingFire.Services;
using DyingFire.States;
using DyingFire.Systems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows;

namespace DyingFire.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public bool CanMoveNorth => CurrentLocation != null && CurrentLocation.LocationToNorth != -1 && !IsHidingUI;
        public bool CanMoveEast => CurrentLocation != null && CurrentLocation.LocationToEast != -1 && !IsHidingUI;
        public bool CanMoveSouth => CurrentLocation != null && CurrentLocation.LocationToSouth != -1 && !IsHidingUI;
        public bool CanMoveWest => CurrentLocation != null && CurrentLocation.LocationToWest != -1 && !IsHidingUI;

        // Core Systems
        private DatabaseService _dbService;
        private DispatcherTimer _gameTimer;
        public GameStateManager StateManager { get; }
        public InventorySystem Inventory { get; }
        public ActionSystem Actions { get; }

        //  Game Data
        public List<Location> AllLocations { get; set; }
        public bool IsParanormalActivityPresent { get; set; } = false;

        private int _sanity = 100;
        public int Sanity { get { return _sanity; } set { _sanity = value; OnPropertyChanged(); } }

        private int _vitality = 100;
        public int Vitality { get { return _vitality; } set { _vitality = value; OnPropertyChanged(); } }

        private Location _currentLocation;
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanMoveNorth));
                OnPropertyChanged(nameof(CanMoveEast));
                OnPropertyChanged(nameof(CanMoveSouth));
                OnPropertyChanged(nameof(CanMoveWest));
            }
        }

        private string _backgroundImage;
        public string BackgroundImage { get { return _backgroundImage; } set { _backgroundImage = value; OnPropertyChanged(); } }

        private bool _isHidingUI;
        public bool IsHidingUI
        {
            get { return _isHidingUI; }
            set
            {
                _isHidingUI = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanMoveNorth));
                OnPropertyChanged(nameof(CanMoveEast));
                OnPropertyChanged(nameof(CanMoveSouth));
                OnPropertyChanged(nameof(CanMoveWest));
            }
        }

        public AudioService Audio { get; } = new AudioService();

        // UI State
        private GameItem _selectedInventoryItem;
        public GameItem SelectedInventoryItem { get { return _selectedInventoryItem; } set { _selectedInventoryItem = value; OnPropertyChanged(); } }

        private bool _isInventoryVisible;
        public bool IsInventoryVisible { get { return _isInventoryVisible; } set { _isInventoryVisible = value; OnPropertyChanged(); } }

        private bool _isPopupVisible;
        public bool IsPopupVisible { get { return _isPopupVisible; } set { _isPopupVisible = value; OnPropertyChanged(); } }

        private string _popupTitle;
        public string PopupTitle { get { return _popupTitle; } set { _popupTitle = value; OnPropertyChanged(); } }

        private string _popupMessage;
        public string PopupMessage { get { return _popupMessage; } set { _popupMessage = value; OnPropertyChanged(); } }

        // Exposing Data & Commands for XAML Bindings
        public ObservableCollection<GameItem> QuickBar => Inventory.QuickBar;
        public ObservableCollection<GameItem> FullInventory => Inventory.FullInventory;

        public ICommand MoveCommand => Actions.MoveCommand;
        public ICommand InteractCommand => Actions.InteractCommand;
        public ICommand StopHidingCommand => Actions.StopHidingCommand;
        public ICommand UseItemCommand => Inventory.UseItemCommand;
        public ICommand EquipItemCommand => Inventory.EquipItemCommand;
        public ICommand SelectQuickSlotCommand => Inventory.SelectQuickSlotCommand;

        public ICommand ToggleInventoryCommand { get; }
        public ICommand ClosePopupCommand { get; }

        public MainViewModel()
        {
            _dbService = new DatabaseService();
            StateManager = new GameStateManager();

            // Initialize Systems
            Inventory = new InventorySystem(this);
            Actions = new ActionSystem(this);

            ToggleInventoryCommand = new RelayCommand<object>(_ => IsInventoryVisible = !IsInventoryVisible);
            ClosePopupCommand = new RelayCommand<object>(_ => IsPopupVisible = false);

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;

            LoadGameWorldAsync();
        }

        private async void LoadGameWorldAsync()
        {
            AllLocations = await _dbService.LoadFullWorldAsync();
            if (AllLocations != null && AllLocations.Count > 0)
            {
                CurrentLocation = AllLocations.FirstOrDefault(x => x.ID == 1);
                BackgroundImage = CurrentLocation.ImagePath;
                Audio.PlayBGM("/Assets/Audio/horrorAtmosphere.mp3");
                StartGameLoop();
            }
        }

        private void StartGameLoop()
        {
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(3);
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            StateManager.Update();

            if (StateManager.CurrentState is HidingState) return;

            if (CurrentLocation != null && CurrentLocation.IsDark) Sanity = Math.Max(0, Sanity - 5);
            else Sanity = Math.Min(100, Sanity + 2);

            IsParanormalActivityPresent = new Random().Next(100) < 20;
        }

        public void ShowMessage(string title, string message)
        {
            PopupTitle = title;
            PopupMessage = message;
            IsPopupVisible = true;
        }
    }
}