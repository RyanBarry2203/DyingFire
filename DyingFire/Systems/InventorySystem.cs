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
        private MainViewModel _vm;

        // Dictionary to map Item Types/Names to their specific behaviors
        private readonly Dictionary<ItemType, IItemUsageStrategy> _typeStrategies;
        private readonly Dictionary<string, IItemUsageStrategy> _specialItemStrategies;

        public ObservableCollection<GameItem> QuickBar { get; set; }
        public ObservableCollection<GameItem> FullInventory { get; set; }

        public ICommand UseItemCommand { get; }
        public ICommand EquipItemCommand { get; }
        public ICommand SelectQuickSlotCommand { get; }

        public InventorySystem(MainViewModel vm)
        {
            _vm = vm;
            QuickBar = new ObservableCollection<GameItem>(new GameItem[5]);
            FullInventory = new ObservableCollection<GameItem>();

            _typeStrategies = new Dictionary<ItemType, IItemUsageStrategy>
            {
                { ItemType.Consumable, new ConsumableStrategy() },
                { ItemType.Clue, new ClueStrategy() },
                { ItemType.Key, new DefaultEquipStrategy() },
                { ItemType.Weapon, new DefaultEquipStrategy() }
            };

            _specialItemStrategies = new Dictionary<string, IItemUsageStrategy>
            {
                { "Spirit Box", new SpiritBoxStrategy() }
            };

            UseItemCommand = new RelayCommand<GameItem>(UseItem);
            EquipItemCommand = new RelayCommand<GameItem>(EquipItem);
            SelectQuickSlotCommand = new RelayCommand<GameItem>(SelectQuickSlot);
        }

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

        public void RemoveItem(GameItem item)
        {
            FullInventory.Remove(item);
            int index = QuickBar.IndexOf(item);
            if (index >= 0) QuickBar[index] = null;
        }

        private void EquipItem(GameItem item)
        {
            if (item == null) return;
            if (QuickBar.Contains(item))
            {
                int index = QuickBar.IndexOf(item);
                QuickBar[index] = null;
                item.IsSelected = false;
                return;
            }
            for (int i = 0; i < QuickBar.Count; i++)
            {
                if (QuickBar[i] == null)
                {
                    QuickBar[i] = item;
                    return;
                }
            }
            _vm.ShowMessage("QUICKBAR FULL", "Your quickbar is full.");
        }

        private void SelectQuickSlot(GameItem item)
        {
            if (item == null) return;
            foreach (var slotItem in QuickBar.Where(i => i != null)) slotItem.IsSelected = false;
            item.IsSelected = true;
        }
    }
}