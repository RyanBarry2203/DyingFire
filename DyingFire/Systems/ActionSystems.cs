using DyingFire.Models;
using DyingFire.States;
using DyingFire.ViewModels;
using System.Linq;
using System.Windows.Input;

namespace DyingFire.Systems
{
    public class ActionSystem
    {
        private MainViewModel _vm;

        public ICommand MoveCommand { get; }
        public ICommand InteractCommand { get; }
        public ICommand StopHidingCommand { get; }

        public ActionSystem(MainViewModel vm)
        {
            _vm = vm;
            MoveCommand = new RelayCommand<string>(Move);
            InteractCommand = new RelayCommand<InteractableObject>(Interact);
            StopHidingCommand = new RelayCommand<object>(_ => StopHiding());
        }

        private void Move(string direction)
        {
            if (_vm.StateManager.CurrentState is HidingState) return;

            int newLocID = -1;
            if (direction == "North") newLocID = _vm.CurrentLocation.LocationToNorth;
            else if (direction == "South") newLocID = _vm.CurrentLocation.LocationToSouth;
            else if (direction == "East") newLocID = _vm.CurrentLocation.LocationToEast;
            else if (direction == "West") newLocID = _vm.CurrentLocation.LocationToWest;

            if (newLocID != -1)
            {
                _vm.CurrentLocation = _vm.AllLocations.FirstOrDefault(x => x.ID == newLocID) ?? _vm.CurrentLocation;
                _vm.BackgroundImage = _vm.CurrentLocation.ImagePath;
                _vm.IsPopupVisible = false;
            }
        }

        private void Interact(InteractableObject obj)
        {
            if (obj == null || _vm.StateManager.CurrentState is HidingState) return;

            if (obj.CanHideInside)
            {
                _vm.StateManager.PushState(new HidingState(_vm, obj));
                return;
            }

            if (obj.IsLocked)
            {
                var activeItem = _vm.Inventory.QuickBar.FirstOrDefault(x => x != null && x.IsSelected);
                if (activeItem != null && activeItem.Name == obj.RequiredItem)
                {
                    obj.IsLocked = false;
                    if (obj.TargetLocationID > 0)
                    {
                        _vm.CurrentLocation = _vm.AllLocations.FirstOrDefault(x => x.ID == obj.TargetLocationID) ?? _vm.CurrentLocation;
                        _vm.BackgroundImage = _vm.CurrentLocation.ImagePath; 
                        _vm.ShowMessage("UNLOCKED", $"You used the {activeItem.Name} to unlock the door and walked through.");
                        return;
                    }
                    _vm.ShowMessage("UNLOCKED", $"You used the {activeItem.Name} to unlock the {obj.Name}.");
                }
                else
                {
                    _vm.ShowMessage("LOCKED", obj.LockedMessage);
                }
                return;
            }

            if (obj.TargetLocationID > 0)
            {
                _vm.CurrentLocation = _vm.AllLocations.FirstOrDefault(x => x.ID == obj.TargetLocationID) ?? _vm.CurrentLocation;
                _vm.BackgroundImage = _vm.CurrentLocation.ImagePath; 
                return;
            }

            if (obj.ItemsInside.Count > 0)
            {
                string foundItems = string.Join("\n", obj.ItemsInside.Select(i => i.Name));
                foreach (var item in obj.ItemsInside) _vm.Inventory.FullInventory.Add(item);
                obj.ItemsInside.Clear();
                _vm.ShowMessage("ITEM FOUND", "Added to Inventory:\n\n" + foundItems);
            }
            if (obj.ItemsInside.Count > 0)
            {
                string foundItems = string.Join("\n", obj.ItemsInside.Select(i => i.Name));
                foreach (var item in obj.ItemsInside) _vm.Inventory.FullInventory.Add(item);
                obj.ItemsInside.Clear();
                _vm.ShowMessage("ITEM FOUND", "Added to Inventory:\n\n" + foundItems);
            }
            else if (obj.ActionText == "Search")
            {
                _vm.ShowMessage("EMPTY", $"You searched the {obj.Name} but found nothing useful.");
            }
        }

        private void StopHiding()
        {
            if (_vm.StateManager.CurrentState is HidingState)
            {
                _vm.StateManager.PopState();
            }
        }
    }
}