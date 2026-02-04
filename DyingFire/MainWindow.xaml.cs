using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Needed for Mouse Move
using DyingFire.ViewModels;
using DyingFire.Models;
using System.Linq;

namespace DyingFire
{
    public partial class MainWindow : Window
    {
        private MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            this.DataContext = _vm;
        }

        // --- 1. MOVEMENT ---
        private void MoveNorth_Click(object sender, RoutedEventArgs e) { _vm.Move("North"); }
        private void MoveSouth_Click(object sender, RoutedEventArgs e) { _vm.Move("South"); }
        private void MoveWest_Click(object sender, RoutedEventArgs e) { _vm.Move("West"); }
        private void MoveEast_Click(object sender, RoutedEventArgs e) { _vm.Move("East"); }

        // --- 2. INVENTORY TOGGLE ---
        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryScreen.Visibility == Visibility.Visible)
            {
                InventoryScreen.Visibility = Visibility.Collapsed;
            }
            else
            {
                InventoryScreen.Visibility = Visibility.Visible;
            }

            var button = sender as FrameworkElement;
            var obj = button.DataContext as InteractableObject;
            if (obj == null) return;

            if (obj.IsLocked)
            {
                var activeItem = _vm.QuickBar.FirstOrDefault(x => x != null && x.IsSelected);

                if (activeItem != null && activeItem.Name == obj.RequiredItem)
                {
                    obj.IsLocked = false;
                    ShowGameMessage("UNLOCKED", $"You used the {activeItem.Name} to unlock the {obj.Name}.");
                }
                else
                {
                    ShowGameMessage("LOCKED", obj.LockedMessage);
                }
                return;
            }
            if (obj.TargetLocationID > 0)
            {
                _vm.EnterLocation(obj.TargetLocationID);
                return;
            }
            if (obj.ItemsInside.Count > 0)
            {
                ShowGameMessage("Searched", $"You found items in the {obj.Name}.");
                foreach (var item in obj.ItemsInside)
                {
                    _vm.CurrentLocation.RoomItems.Add(item);
                }
                obj.ItemsInside.Clear();
            }
            else
            {
                ShowGameMessage("EMPTY", $"The {obj.Name} is empty.");
            }
        }

        // --- 3. CUSTOM CURSOR LOGIC ---
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(this);
            CustomCursor.Margin = new Thickness(p.X, p.Y, 0, 0);
        }

        // --- 4. POPUP SYSTEM ---
        private void ShowGameMessage(string title, string message)
        {
            PopupTitle.Text = title;
            PopupMessage.Text = message;
            GamePopup.Visibility = Visibility.Visible;
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            GamePopup.Visibility = Visibility.Collapsed;
        }

        // --- 5. INTERACTION (SEARCHING) ---
        private void Interactable_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            var obj = button.DataContext as InteractableObject;

            if (obj == null) return;

            // 1. CHECK IF LOCKED
            if (obj.IsLocked)
            {
                // Find the selected key in the QuickBar
                var activeItem = _vm.QuickBar.FirstOrDefault(x => x != null && x.IsSelected);

                if (activeItem != null && activeItem.Name == obj.RequiredItem)
                {
                    obj.IsLocked = false;
                    _vm.CurrentLocation.LocationToEast = obj.TargetLocationID;
                    _vm.CurrentLocation = _vm.CurrentLocation; // Refresh location to update UI
                    ShowGameMessage("UNLOCKED", $"You used the {activeItem.Name} to unlock the {obj.Name}.");
                    // Optional: _vm.QuickBar[indexOfItem] = null; // Destroy key?

                }
                else
                {
                    ShowGameMessage("LOCKED", obj.LockedMessage);
                }
                return; // Stop here! Don't try to enter or search yet.
            }

            // 2. CHECK IF IT IS A DOOR (Unlocked)
            if (obj.TargetLocationID > 0)
            {
                _vm.EnterLocation(obj.TargetLocationID);
                return; // Stop here! You entered the room.
            }

            // 3. IF IT IS A CONTAINER (Chest/Pile)
            if (obj.ItemsInside.Count > 0)
            {
                ShowGameMessage("SEARCHED", $"You searched the {obj.Name} and found items!");
                foreach (var item in obj.ItemsInside)
                {
                    _vm.CurrentLocation.RoomItems.Add(item);
                }
                obj.ItemsInside.Clear();
            }
            else
            {
                // Only show "Empty" if it's not a door and has no items
                ShowGameMessage("EMPTY", $"The {obj.Name} is empty.");
            }
        }

        // --- 6. LOOTING ITEMS ---
        private void RoomItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            var item = button.DataContext as GameItem;

            if (item != null)
            {
                // Remove from floor
                _vm.CurrentLocation.RoomItems.Remove(item);

                // Add to first empty slot in QuickBar
                for (int i = 0; i < _vm.QuickBar.Count; i++)
                {
                    if (_vm.QuickBar[i] == null)
                    {
                        _vm.QuickBar[i] = item;
                        break;
                    }
                }

                ShowGameMessage("ITEM FOUND", $"You picked up: {item.Name}");
            }
        }

        // --- 7. QUICK SLOT (Highlight Item) ---
        private void QuickSlot_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            var item = button.DataContext as GameItem;

            if (item != null)
            {
                foreach (var slotItem in _vm.QuickBar)
                {
                    if (slotItem != null) slotItem.IsSelected = false;
                }
                item.IsSelected = true;
            }
        }
    }
}