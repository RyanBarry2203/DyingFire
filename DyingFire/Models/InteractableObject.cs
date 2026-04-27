using System.Collections.ObjectModel;
using DyingFire.ViewModels;

namespace DyingFire.Models
{
    public class InteractableObject : ObservableObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        private bool _isLocked;
        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; OnPropertyChanged(); }
        }

        public string RequiredItem { get; set; }
        public string LockedMessage { get; set; } = "It's locked.";
        public int TargetLocationID { get; set; } = 0;

        // NEW: Hiding Mechanic
        public bool CanHideInside { get; set; } = false;

        public ObservableCollection<GameItem> ItemsInside { get; set; }

        public InteractableObject()
        {
            ItemsInside = new ObservableCollection<GameItem>();
        }

        public string ActionText
        {
            get
            {
                if (CanHideInside) return "Hide";
                if (TargetLocationID > 0 || IsLocked) return "Interact";
                return "Search";
            }
        }
    }
}