using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DyingFire.ViewModels;

namespace DyingFire.Models
{
    public class InteractableObject : ObservableObject
    {

        public string Name { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public ObservableCollection<GameItem> ItemsInside { get; set; }

        public InteractableObject()
        {
            ItemsInside = new ObservableCollection<GameItem>();
        }
    }
}
