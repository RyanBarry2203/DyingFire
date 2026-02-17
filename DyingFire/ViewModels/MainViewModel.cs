using DyingFire.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DyingFire.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private List<Location> _allLocations;

        private Location _currentLocation;
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set { _currentLocation = value; OnPropertyChanged(); }
        }

        public ObservableCollection<GameItem> QuickBar { get; set; }

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

            MoveCommand = new RelayCommand<string>(Move);
            ToggleInventoryCommand = new RelayCommand<object>(_ => IsInventoryVisible = !IsInventoryVisible);
            ClosePopupCommand = new RelayCommand<object>(_ => IsPopupVisible = false);
            InteractCommand = new RelayCommand<InteractableObject>(Interact);
            LootItemCommand = new RelayCommand<GameItem>(LootItem);
            SelectQuickSlotCommand = new RelayCommand<GameItem>(SelectQuickSlot);

            LoadGameWorld();
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
                }
                else
                {
                    ShowMessage("LOCKED", obj.LockedMessage);
                }
                return;
            }

            if (obj.TargetLocationID > 0)
            {
                var teleportRoom = _allLocations.FirstOrDefault(x => x.ID == obj.TargetLocationID);
                if (teleportRoom != null)
                {
                    CurrentLocation = teleportRoom;
                }
                IsPopupVisible = false;
                return;
            }

            if (obj.ItemsInside.Count > 0)
            {
                ShowMessage("SEARCHED", "You searched the " + obj.Name + " and found items!");
                foreach (var item in obj.ItemsInside)
                {
                    CurrentLocation.RoomItems.Add(item);
                }
                obj.ItemsInside.Clear();
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