using DyingFire.Models;
using DyingFire.Strategies;
using DyingFire.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DyingFire.Systems
{
    public class InventorySystem
    {
        // Reference to the main view model so we can update UI and call ShowMessage, etc.
        private MainViewModel _vm;

        // Map item types to the strategy that runs when those items are used.
        private readonly Dictionary<ItemType, IItemUsageStrategy> _typeStrategies;
        // Map specific item names to special strategies (overrides type strategies).
        private readonly Dictionary<string, IItemUsageStrategy> _specialItemStrategies;

        // The quickbar is a small fixed-size collection the player can equip items into.
        public ObservableCollection<GameItem> QuickBar { get; set; }
        // The player's full inventory list.
        public ObservableCollection<GameItem> FullInventory { get; set; }

        // Commands exposed to the UI for using, equipping, and selecting quick slots.
        public ICommand UseItemCommand { get; }
        public ICommand EquipItemCommand { get; }
        public ICommand SelectQuickSlotCommand { get; }

        public InventorySystem(MainViewModel vm)
        {
            // Store view model reference for later use.
            _vm = vm;

            // Initialize quickbar with five empty slots.
            QuickBar = new ObservableCollection<GameItem>(new GameItem[5]);
            // Start with an empty full inventory.
            FullInventory = new ObservableCollection<GameItem>();

            // Register default behaviors by item type.
            _typeStrategies = new Dictionary<ItemType, IItemUsageStrategy>
            {
                { ItemType.Consumable, new ConsumableStrategy() },
                { ItemType.Clue, new ClueStrategy() },
                { ItemType.Key, new DefaultEquipStrategy() },
                { ItemType.Weapon, new DefaultEquipStrategy() }
            };

            // Register any special-case items by name.
            _specialItemStrategies = new Dictionary<string, IItemUsageStrategy>
            {
                { "Spirit Box", new SpiritBoxStrategy() }
            };

            // Wire UI commands to local methods.
            UseItemCommand = new RelayCommand<GameItem>(UseItem);
            EquipItemCommand = new RelayCommand<GameItem>(EquipItem);
            SelectQuickSlotCommand = new RelayCommand<GameItem>(SelectQuickSlot);
        }

        // Called when the player clicks "Use" on an inventory item.
        // If a special named strategy exists use it, otherwise use the strategy for the item's type.
        private void UseItem(GameItem item)
        {
            if (item == null) return;

            if (_specialItemStrategies.TryGetValue(item.Name, out var specialStrategy))
            {
                specialStrategy.Use(item, _vm);
            }
            else if (_typeStrategies.TryGetValue(item.Type, out var typeStrategy))
            {
                typeStrategy.Use(item, _vm);
            }
        }

        // Remove an item from the full inventory and also clear it from the quickbar if equipped.
        public void RemoveItem(GameItem item)
        {
            FullInventory.Remove(item);
            int index = QuickBar.IndexOf(item);
            if (index >= 0) QuickBar[index] = null;
        }

        // Equip or unequip an item into the quickbar.
        // If the item is already in a quickbar slot it is removed (unequipped).
        // Otherwise the first empty quickbar slot is filled.
        private void EquipItem(GameItem item)
        {
            if (item == null) return;

            // If item is already in the quickbar, remove it and clear selection.
            if (QuickBar.Contains(item))
            {
                int index = QuickBar.IndexOf(item);
                QuickBar[index] = null;
                item.IsSelected = false;
                return;
            }

            // Otherwise find the first empty slot and put the item there.
            for (int i = 0; i < QuickBar.Count; i++)
            {
                if (QuickBar[i] == null)
                {
                    QuickBar[i] = item;
                    return;
                }
            }

            // If no empty slot was found show a message via the view model.
            _vm.ShowMessage("QUICKBAR FULL", "Your quickbar is full.");
        }

        // Select a quickbar slot item so it becomes the active item for interactions.
        private void SelectQuickSlot(GameItem item)
        {
            if (item == null) return;

            // Clear selection on all equipped quickbar items.
            foreach (var slotItem in QuickBar.Where(i => i != null)) slotItem.IsSelected = false;
            // Mark the chosen item as selected.
            item.IsSelected = true;
        }
    }
}