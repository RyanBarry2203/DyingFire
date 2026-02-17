using DyingFire.ViewModels;

namespace DyingFire.Models
{
    public enum ItemType { Key, Weapon, Consumable, Clue }

    public class GameItem : ObservableObject, IInteractable
    {
        private bool _isSelected;

        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public ItemType Type { get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BorderColor)); 
            }
        }

        public string BorderColor => IsSelected ? "#E6C898" : "Transparent";

        public string Interact()
        {
            return $"You used the {Name}.";
        }
    }
}