using DyingFire.Models;
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
        private List<Location> _allLocations;
        private DispatcherTimer _gameTimer;

        private int _sanity = 100;
        public int Sanity
        {
            get { return _sanity; }
            set { _sanity = value; OnPropertyChanged(); }
        }

        private int _vitality = 100;
        public int Vitality
        {
            get { return _vitality; }
            set { _vitality = value; OnPropertyChanged(); }
        }

        public int MonsterLocationID { get; set; } = 8;

        private Location _currentLocation;
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set { _currentLocation = value; OnPropertyChanged(); }
        }

        public ObservableCollection<GameItem> QuickBar { get; set; }

        public ObservableCollection<GameItem> FullInventory { get; set; }

        private GameItem _selectedInventoryItem;
        public GameItem SelectedInventoryItem
        {
            get { return _selectedInventoryItem; }
            set { _selectedInventoryItem = value; OnPropertyChanged(); }
        }

        public ICommand EquipItemCommand { get; }

        private bool _isInventoryVisible;
        public bool IsInventoryVisible
        {
            get { return _isInventoryVisible; }
            set { _isInventoryVisible = value; OnPropertyChanged(); }
        }

        private bool _isPopupVisible;
        public bool IsPopupVisible
        {
            get { return _isPopupVisible; }
            set { _isPopupVisible = value; OnPropertyChanged(); }
        }

        private string _popupTitle;
        public string PopupTitle
        {
            get { return _popupTitle; }
            set { _popupTitle = value; OnPropertyChanged(); }
        }

        private string _popupMessage;
        public string PopupMessage
        {
            get { return _popupMessage; }
            set { _popupMessage = value; OnPropertyChanged(); }
        }

        public ICommand MoveCommand { get; }
        public ICommand ToggleInventoryCommand { get; }
        public ICommand ClosePopupCommand { get; }
        public ICommand InteractCommand { get; }
        public ICommand LootItemCommand { get; }
        public ICommand SelectQuickSlotCommand { get; }

        public MainViewModel()
        {
            QuickBar = new ObservableCollection<GameItem>(new GameItem[5]);

            FullInventory = new ObservableCollection<GameItem>();
            EquipItemCommand = new RelayCommand<GameItem>(EquipItem);

            MoveCommand = new RelayCommand<string>(Move);
            ToggleInventoryCommand = new RelayCommand<object>(_ => IsInventoryVisible = !IsInventoryVisible);
            ClosePopupCommand = new RelayCommand<object>(_ => IsPopupVisible = false);
            InteractCommand = new RelayCommand<InteractableObject>(Interact);
            LootItemCommand = new RelayCommand<GameItem>(LootItem);
            SelectQuickSlotCommand = new RelayCommand<GameItem>(SelectQuickSlot);

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            LoadGameWorld();
            StartGameLoop();
        }

        private void LoadGameWorld()
        {
            var db = new DyingFire.Services.DatabaseService();
            _allLocations = db.LoadFullWorld();

            if (_allLocations != null && _allLocations.Count > 0)
            {
                CurrentLocation = _allLocations.FirstOrDefault(x => x.ID == 1);
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
            if (CurrentLocation == null) return;

            if (CurrentLocation.IsDark)
            {
                Sanity = Math.Max(0, Sanity - 5);
            }
            else
            {
                Sanity = Math.Min(100, Sanity + 2);
            }

            foreach (var loc in _allLocations)
            {
                if (loc.NoiseLevel > 0) loc.NoiseLevel--;
            }

            MoveMonsterAI();
        }

        private void MoveMonsterAI()
        {
            var monsterRoom = _allLocations.FirstOrDefault(x => x.ID == MonsterLocationID);
            if (monsterRoom == null) return;

            var adjacentIds = new List<int> { monsterRoom.LocationToNorth, monsterRoom.LocationToSouth, monsterRoom.LocationToEast, monsterRoom.LocationToWest };
            adjacentIds.RemoveAll(id => id == -1);

            var adjacentRooms = _allLocations.Where(x => adjacentIds.Contains(x.ID)).ToList();

            var targetRoom = adjacentRooms.OrderByDescending(x => x.NoiseLevel).FirstOrDefault();

            if (targetRoom != null && targetRoom.NoiseLevel > 0)
            {
                MonsterLocationID = targetRoom.ID;
            }
            else if (adjacentRooms.Count > 0)
            {
                Random rnd = new Random();
                MonsterLocationID = adjacentRooms[rnd.Next(adjacentRooms.Count)].ID;
            }

            if (MonsterLocationID == CurrentLocation.ID)
            {
                TriggerJumpscare();
            }
        }

        private void GenerateNoise(int sourceLocationId, int noiseLevel)
        {
            var sourceRoom = _allLocations.FirstOrDefault(x => x.ID == sourceLocationId);
            if (sourceRoom == null) return;

            sourceRoom.NoiseLevel = noiseLevel;

            var adjacentIds = new List<int> { sourceRoom.LocationToNorth, sourceRoom.LocationToSouth, sourceRoom.LocationToEast, sourceRoom.LocationToWest };
            foreach (var id in adjacentIds)
            {
                var adjRoom = _allLocations.FirstOrDefault(x => x.ID == id);
                if (adjRoom != null && adjRoom.NoiseLevel < noiseLevel - 1)
                {
                    adjRoom.NoiseLevel = noiseLevel - 1;
                }
            }
        }

        private void TriggerJumpscare()
        {
            Vitality -= 50;
            ShowMessage("YOU WERE CAUGHT", "The creature found you in the dark...");

            MonsterLocationID = 1;
        }
        private void ShowMessage(string title, string message)
        {
            PopupTitle = title;
            PopupMessage = message;
            IsPopupVisible = true;
        }

        private void Move(string direction)
        {
            if (CurrentLocation == null) return;

            int newLocID = -1;

            if (direction == "North") newLocID = CurrentLocation.LocationToNorth;
            else if (direction == "South") newLocID = CurrentLocation.LocationToSouth;
            else if (direction == "East") newLocID = CurrentLocation.LocationToEast;
            else if (direction == "West") newLocID = CurrentLocation.LocationToWest;

            if (newLocID != -1)
            {
                var newRoom = _allLocations.FirstOrDefault(x => x.ID == newLocID);
                if (newRoom != null)
                {
                    CurrentLocation = newRoom;
                    GenerateNoise(CurrentLocation.ID, 2);
                }
                IsPopupVisible = false;
            }
        }

        private void Interact(InteractableObject obj)
        {
            if (obj == null) return;

            if (obj.IsLocked)
            {
                var activeItem = QuickBar.FirstOrDefault(x => x != null && x.IsSelected);
                if (activeItem != null && activeItem.Name == obj.RequiredItem)
                {
                    obj.IsLocked = false;
                    CurrentLocation = CurrentLocation;
                    ShowMessage("UNLOCKED", "You used the " + activeItem.Name + " to unlock the " + obj.Name + ".");
                    GenerateNoise(CurrentLocation.ID, 3);
                }
                else
                {
                    ShowMessage("LOCKED", obj.LockedMessage);
                    GenerateNoise(CurrentLocation.ID, 1);
                }
                return;
            }

            if (obj.TargetLocationID > 0)
            {
                var teleportRoom = _allLocations.FirstOrDefault(x => x.ID == obj.TargetLocationID);
                if (teleportRoom != null) CurrentLocation = teleportRoom;
                IsPopupVisible = false;
                return;
            }

            if (obj.ItemsInside.Count > 0)
            {
                string foundItems = "";
                foreach (var item in obj.ItemsInside)
                {
                    if (string.IsNullOrEmpty(item.Lore))
                        item.Lore = "An object found abandoned in the dark. Its origins are unknown, but it feels cold to the touch.";

                    FullInventory.Add(item);
                    foundItems += item.Name + "\n";
                }
                obj.ItemsInside.Clear();
                GenerateNoise(CurrentLocation.ID, 3);
                ShowMessage("ITEM FOUND", "Added to Inventory:\n\n" + foundItems);
            }
            else
            {
                ShowMessage("EMPTY", "The " + obj.Name + " is empty.");
            }
        }


        private void LootItem(GameItem item)
        {
            if (item == null) return;
            CurrentLocation.RoomItems.Remove(item);

            for (int i = 0; i < QuickBar.Count; i++)
            {
                if (QuickBar[i] == null)
                {
                    QuickBar[i] = item;
                    break;
                }
            }
            ShowMessage("ITEM FOUND", "You picked up: " + item.Name);
        }

        private void EquipItem(GameItem item)
        {
            if (item == null) return;

            for (int i = 0; i < QuickBar.Count; i++)
            {
                if (QuickBar[i] == null)
                {
                    QuickBar[i] = item;
                    return;
                }
            }
            ShowMessage("QUICKBAR FULL", "Your quickbar is full. You must remove an item first.");
        }

        private void SelectQuickSlot(GameItem item)
        {
            if (item == null) return;

            foreach (var slotItem in QuickBar)
            {
                if (slotItem != null)
                {
                    slotItem.IsSelected = false;
                }
            }
            item.IsSelected = true;
        }
    }
}