using DyingFire.Models;
using DyingFire.ViewModels;
using System;

namespace DyingFire.Strategies
{
    // Strategy contract for using an item.
    // The view model passes the item and itself so the strategy can update game state and UI.
    public interface IItemUsageStrategy
    {
        // Execute the use action for the provided item and view model.
        void Use(GameItem item, MainViewModel vm);
    }

    // ConsumableStrategy: items that restore sanity or are consumed on use.
    public class ConsumableStrategy : IItemUsageStrategy
    {
        // Use restores sanity, shows a message, and removes the item from inventory.
        public void Use(GameItem item, MainViewModel vm)
        {
            // Fully restore the player's sanity.
            vm.Sanity = 100;

            // Show a short popup to the player. Use UseMessage if present, otherwise a default message.
            vm.ShowMessage("RELIEF", item.UseMessage ?? "You consumed the item.");

            // Remove the consumed item from the player's inventory.
            vm.Inventory.RemoveItem(item);
        }
    }

    // ClueStrategy: reading or inspecting an item that reveals lore.
    public class ClueStrategy : IItemUsageStrategy
    {
        // Use shows the lore text to the player.
        public void Use(GameItem item, MainViewModel vm)
        {
            // Display the item's lore in a message popup.
            vm.ShowMessage("READING", item.Lore);
        }
    }

    // SpiritBoxStrategy: special item that gives a random clue when paranormal activity is present.
    public class SpiritBoxStrategy : IItemUsageStrategy
    {
        // Use checks for paranormal activity and returns a random clue from the UseMessage.
        public void Use(GameItem item, MainViewModel vm)
        {
            // If paranormal activity is flagged, pick one clue and display it.
            if (vm.IsParanormalActivityPresent)
            {
                // Split the UseMessage on '|' to get multiple clue options.
                string[] clues = item.UseMessage.Split('|');

                // Pick a random clue and show it.
                vm.ShowMessage("SPIRIT BOX", clues[new Random().Next(clues.Length)]);

                // Clear the paranormal flag after giving a response.
                vm.IsParanormalActivityPresent = false;
            }
            else
            {
                // If nothing is happening, show a static/empty response.
                vm.ShowMessage("SPIRIT BOX", "*Just empty static... nothing is here right now.*");
            }
        }
    }

    // DefaultEquipStrategy: fallback for equippable items that are not usable directly.
    public class DefaultEquipStrategy : IItemUsageStrategy
    {
        // Use informs the player that this item cannot be used this way.
        public void Use(GameItem item, MainViewModel vm)
        {
            // Show a message guiding the player to equip the item instead of using it directly.
            vm.ShowMessage("ITEM", $"The {item.Name} cannot be used like this. Try equipping it to interact with objects.");
        }
    }
}