using DyingFire.Models;
using DyingFire.States;
using DyingFire.ViewModels;
using System.Linq;

namespace DyingFire.Strategies.Interaction
{
    public interface IInteractionHandler
    {
        bool CanHandle(InteractableObject obj);
        void Handle(InteractableObject obj, MainViewModel vm);
    }

    public class HideInteraction : IInteractionHandler
    {
        public bool CanHandle(InteractableObject obj) => obj.CanHideInside;
        public void Handle(InteractableObject obj, MainViewModel vm) => vm.StateManager.PushState(new HidingState(vm, obj));
    }

    public class LockedDoorInteraction : IInteractionHandler
    {
        public bool CanHandle(InteractableObject obj) => obj.IsLocked;
        public void Handle(InteractableObject obj, MainViewModel vm)
        {
            var activeItem = vm.Inventory.QuickBar.FirstOrDefault(x => x != null && x.IsSelected);
            if (activeItem != null && activeItem.Name == obj.RequiredItem)
            {
                obj.IsLocked = false;
                vm.ShowMessage("UNLOCKED", $"You used the {activeItem.Name} to unlock the {obj.Name}.");
            }
            else
            {
                vm.ShowMessage("LOCKED", obj.LockedMessage);
            }
        }
    }

    public class TransitionInteraction : IInteractionHandler
    {
        public bool CanHandle(InteractableObject obj) => obj.TargetLocationID > 0 && !obj.IsLocked;
        public void Handle(InteractableObject obj, MainViewModel vm)
        {
            vm.CurrentLocation = vm.AllLocations.FirstOrDefault(x => x.ID == obj.TargetLocationID) ?? vm.CurrentLocation;
            vm.BackgroundImage = vm.CurrentLocation.ImagePath;
        }
    }

    public class LootInteraction : IInteractionHandler
    {
        public bool CanHandle(InteractableObject obj) => obj.ItemsInside.Count > 0;
        public void Handle(InteractableObject obj, MainViewModel vm)
        {
            string foundItems = string.Join("\n", obj.ItemsInside.Select(i => i.Name));
            foreach (var item in obj.ItemsInside) vm.Inventory.FullInventory.Add(item);
            obj.ItemsInside.Clear();
            vm.ShowMessage("ITEM FOUND", "Added to Inventory:\n\n" + foundItems);
        }
    }

    public class EmptySearchInteraction : IInteractionHandler
    {
        public bool CanHandle(InteractableObject obj) => obj.ItemsInside.Count == 0 && obj.TargetLocationID == 0 && !obj.CanHideInside;
        public void Handle(InteractableObject obj, MainViewModel vm)
        {
            vm.ShowMessage("EMPTY", $"You searched the {obj.Name} but found nothing useful.");
        }
    }
}