using DyingFire.Models;
using DyingFire.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DyingFire.Systems
{
    public class InventorySystem
    {
        private MainViewModel _vm;

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

            UseItemCommand = new RelayCommand<GameItem>(UseItem);
            EquipItemCommand = new RelayCommand<GameItem>(EquipItem);
            SelectQuickSlotCommand = new RelayCommand<GameItem>(SelectQuickSlot);
        }

        private void UseItem(GameItem item)
        {
            if (item == null) return;

            if (item.Name == "Sanity Pills")
            {
                _vm.Sanity = 100;
                _vm.ShowMessage("RELIEF", "You swallowed the pills. Your heart rate slows down.");
                FullInventory.Remove(item);
                QuickBar[QuickBar.IndexOf(item)] = null;
            }
            else if (item.Name == "Spirit Box")
            {
                if (_vm.IsParanormalActivityPresent)
                {
                    string[] clues = { "STATIC... 'stay in the light'... STATIC", "STATIC... 'it hunts by noise'... STATIC" };
                    _vm.ShowMessage("SPIRIT BOX", clues[new Random().Next(clues.Length)]);
                    _vm.IsParanormalActivityPresent = false;
                }
                else
                {
                    _vm.ShowMessage("SPIRIT BOX", "*Just empty static... nothing is here right now.*");
                }
            }
            else if (item.Type == ItemType.Clue)
            {
                _vm.ShowMessage("READING", item.Lore);
            }
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