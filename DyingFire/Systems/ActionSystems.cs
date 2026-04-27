using DyingFire.Models;
using DyingFire.States;
using DyingFire.Strategies.Interaction;
using DyingFire.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DyingFire.Systems
{
    public class ActionSystem
    {
        private MainViewModel _vm;
        private List<IInteractionHandler> _interactionChain;

        public ICommand MoveCommand { get; }
        public ICommand InteractCommand { get; }
        public ICommand StopHidingCommand { get; }

        public ActionSystem(MainViewModel vm)
        {
            _vm = vm;

            _interactionChain = new List<IInteractionHandler>
            {
                new HideInteraction(),
                new LockedDoorInteraction(),
                new TransitionInteraction(),
                new LootInteraction(),
                new EmptySearchInteraction()
            };

            MoveCommand = new RelayCommand<string>(Move);
            InteractCommand = new RelayCommand<InteractableObject>(Interact);
            StopHidingCommand = new RelayCommand<object>(_ => StopHiding());
        }

        private void Move(string direction)
        {
            if (_vm.StateManager.CurrentState is HidingState) return;

            int newLocID = -1;

            switch (direction)
            {
                case "North":
                    newLocID = _vm.CurrentLocation.LocationToNorth;
                    break;
                case "South":
                    newLocID = _vm.CurrentLocation.LocationToSouth;
                    break;
                case "East":
                    newLocID = _vm.CurrentLocation.LocationToEast;
                    break;
                case "West":
                    newLocID = _vm.CurrentLocation.LocationToWest;
                    break;
            }

            if (newLocID != -1)
            {
                _vm.CurrentLocation = _vm.AllLocations.FirstOrDefault(x => x.ID == newLocID) ?? _vm.CurrentLocation;
                _vm.BackgroundImage = _vm.CurrentLocation.ImagePath;
                _vm.IsPopupVisible = false;
                _vm.GameLoop.CheckPlayerMoved();
            }
        }

        private void Interact(InteractableObject obj)
        {
            if (obj == null || _vm.StateManager.CurrentState is HidingState) return;

            _interactionChain.FirstOrDefault(h => h.CanHandle(obj))?.Handle(obj, _vm);
        }

        private void StopHiding()
        {
            if (_vm.StateManager.CurrentState is HidingState)
            {
                _vm.StateManager.PopState();
                _vm.GameLoop.CheckPlayerMoved();
            }
        }
    }
}