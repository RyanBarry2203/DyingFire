using DyingFire.Models;
using DyingFire.States;
using DyingFire.ViewModels;
using System.Linq;

namespace DyingFire.Strategies.Interaction
{
    // Interface for handlers that decide if they should run and then perform an action.
    public interface IInteractionHandler
    {
        // Return true if this handler is responsible for the clicked object.
        bool CanHandle(InteractableObject obj);

        // Execute the interaction using the main view model.
        void Handle(InteractableObject obj, MainViewModel vm);
    }

    // Handles hiding behavior.
    // Runs when the object supports hiding.
    public class HideInteraction : IInteractionHandler
    {
        // True when this interactable can hide the player.
        public bool CanHandle(InteractableObject obj) => obj.CanHideInside;

        // Pushes a HidingState onto the state manager so the player hides.
        public void Handle(InteractableObject obj, MainViewModel vm) => vm.StateManager.PushState(new HidingState(vm, obj));
    }

    // Handles locked objects that need an item to open.
    public class LockedDoorInteraction : IInteractionHandler
    {
        // True when the object is locked.
        public bool CanHandle(InteractableObject obj) => obj.IsLocked;

        // If a selected quickbar item matches the required item, unlock.
        // Otherwise show the locked message.
        public void Handle(InteractableObject obj, MainViewModel vm)
        {
            // Find the currently selected quickbar item.
            var activeItem = vm.Inventory.QuickBar.FirstOrDefault(x => x != null && x.IsSelected);

            // If we have the correct item, unlock and show an unlocked message.
            if (activeItem != null && activeItem.Name == obj.RequiredItem)
            {
                obj.IsLocked = false;
                vm.ShowMessage("UNLOCKED", $"You used the {activeItem.Name} to unlock the {obj.Name}.");
            }
            else
            {
                // Otherwise notify the player the object is locked.
                vm.ShowMessage("LOCKED", obj.LockedMessage);
            }
        }
    }

    // Handles moving the player to another location.
    public class TransitionInteraction : IInteractionHandler
    {
        // True for interactables that point to another room and are not locked.
        public bool CanHandle(InteractableObject obj) => obj.TargetLocationID > 0 && !obj.IsLocked;

        // Set the current location to the target and update the background image.
        public void Handle(InteractableObject obj, MainViewModel vm)
        {
            vm.CurrentLocation = vm.AllLocations.FirstOrDefault(x => x.ID == obj.TargetLocationID) ?? vm.CurrentLocation;
            vm.BackgroundImage = vm.CurrentLocation.ImagePath;
        }
    }

    // Handles containers that contain items.
    public class LootInteraction : IInteractionHandler
    {
        // True when there are items inside the interactable.
        public bool CanHandle(InteractableObject obj) => obj.ItemsInside.Count > 0;

        // Move each found item into the player's full inventory and clear the container.
        public void Handle(InteractableObject obj, MainViewModel vm)
        {
            // Build a simple list of item names to show in the popup.
            string foundItems = string.Join("\n", obj.ItemsInside.Select(i => i.Name));

            // Transfer items to the player's inventory.
            foreach (var item in obj.ItemsInside) vm.Inventory.FullInventory.Add(item);

            // Remove items from the container so they cannot be collected again.
            obj.ItemsInside.Clear();

            // Inform the player which items were added.
            vm.ShowMessage("ITEM FOUND", "Added to Inventory:\n\n" + foundItems);
        }
    }

    // Fallback handler when nothing else applies.
    public class EmptySearchInteraction : IInteractionHandler
    {
        // True when the object has no items, no transition, and is not a hiding spot.
        public bool CanHandle(InteractableObject obj) => obj.ItemsInside.Count == 0 && obj.TargetLocationID == 0 && !obj.CanHideInside;

        // Show a simple message indicating nothing useful was found.
        public void Handle(InteractableObject obj, MainViewModel vm)
        {
            vm.ShowMessage("EMPTY", $"You searched the {obj.Name} but found nothing useful.");
        }
    }
}   