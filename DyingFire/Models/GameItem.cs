using DyingFire.ViewModels;

namespace DyingFire.Models
{
    // Simple enum for the type of the item.
    // Other systems read this to decide behavior for example InventorySystem or ActionSystem.
    public enum ItemType { Key, Weapon, Consumable, Clue }

    // GameItem is a model that the UI binds to.
    // It implements IInteractable so systems can call Interact() in a uniform way.
    public class GameItem : ObservableObject, IInteractable
    {
        // Backing field for selection state.
        // The view model and UI set IsSelected when the player selects an item in the inventory.
        private bool _isSelected;

        // The display name of the item.
        // UI labels and inventory lists use this.
        public string Name { get; set; }

        // A short description shown in the UI.
        public string Description { get; set; }

        // Extra flavor text about the item.
        // Not required, but shown in item detail views.
        public string Lore { get; set; }

        // Path to the image file used by the UI to render the item.
        public string ImagePath { get; set; }

        // The logical type of the item. Other code checks this to take type-specific actions.
        public ItemType Type { get; set; }

        // Optional message shown when the item is used.
        // Systems that execute item usage can show this text to the player.
        public string UseMessage { get; set; }

        // Tracks whether the item is selected in the UI.
        // The UI binds to IsSelected and to BorderColor to show selection visually.
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                // Set the backing field so the value is stored.
                _isSelected = value;

                // Notify the UI that this property changed.
                // ObservableObject's OnPropertyChanged raises PropertyChanged so WPF updates bindings.
                OnPropertyChanged();

                // Also notify that BorderColor changed because BorderColor depends on IsSelected.
                // This keeps the visual selection indicator in sync with the boolean flag.
                OnPropertyChanged(nameof(BorderColor));
            }
        }

        // Computed property used by the view to pick a border color.
        // The view reads this and does not directly depend on IsSelected.
        public string BorderColor => IsSelected ? "#B38B54" : "Transparent";

        // IInteractable implementation.
        // Calling this returns a simple usage string.
        // This method does not mutate global state. Systems call it to get a message to show.
        public string Interact()
        {
            return $"You used the {Name}.";
        }
    }
}