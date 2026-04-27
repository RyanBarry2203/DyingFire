using DyingFire.Models;
using DyingFire.ViewModels;
using System;

namespace DyingFire.Strategies
{
    public interface IItemUsageStrategy
    {
        void Use(GameItem item, MainViewModel vm);
    }
    public class ConsumableStrategy : IItemUsageStrategy
    {
        public void Use(GameItem item, MainViewModel vm)
        {
            vm.Sanity = 100;
            vm.ShowMessage("RELIEF", item.UseMessage ?? "You consumed the item.");
            vm.Inventory.RemoveItem(item);
        }
    }
    public class ClueStrategy : IItemUsageStrategy
    {
        public void Use(GameItem item, MainViewModel vm)
        {
            vm.ShowMessage("READING", item.Lore);
        }
    }
    public class SpiritBoxStrategy : IItemUsageStrategy
    {
        public void Use(GameItem item, MainViewModel vm)
        {
            if (vm.IsParanormalActivityPresent)
            {
                string[] clues = item.UseMessage.Split('|');
                vm.ShowMessage("SPIRIT BOX", clues[new Random().Next(clues.Length)]);
                vm.IsParanormalActivityPresent = false;
            }
            else
            {
                vm.ShowMessage("SPIRIT BOX", "*Just empty static... nothing is here right now.*");
            }
        }
    }
    public class DefaultEquipStrategy : IItemUsageStrategy
    {
        public void Use(GameItem item, MainViewModel vm)
        {
            vm.ShowMessage("ITEM", $"The {item.Name} cannot be used like this. Try equipping it to interact with objects.");
        }
    }
}