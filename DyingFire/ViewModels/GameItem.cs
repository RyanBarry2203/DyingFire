using DyingFire.ViewModels; // To get ObservableObject

namespace DyingFire.Models
{
    public enum ItemType { Key, Weapon, Consumable, Clue }

    // Inherits from ObservableObject so the UI updates instantly when selected
    public class GameItem : ObservableObject, IInteractable
    {
        private bool _isSelected;

        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; } // Path to the icon image
        public ItemType Type { get; set; }

        // This supports your "Click Once to Highlight" feature
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(); // Updates the UI border automatically
            }
        }

        public string Interact()
        {
            return $"You used the {Name}.";
        }
    }
}