using DyingFire.Models;
using DyingFire.States;
using DyingFire.Strategies.Interaction;
using DyingFire.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DyingFire.Systems
{
    // This class groups player actions into one place.
    // It is given the main view model so it can read and change the game state
    // and call other systems through that view model.
    public class ActionSystem
    {
        // The main view model is the central place that holds game state,
        // references to other services, and UI properties.
        // use it here to update location, images, popup visibility,
        // and to call other systems like the game loop.
        private MainViewModel _vm;

        // A list of interaction handlers forms a simple chain of responsibility.
        // Each handler can say if it can handle a clicked object, and then execute the logic.
        // The order in this list determines which handler gets the first chance to handle an object.
        private List<IInteractionHandler> _interactionChain;

        // These ICommand properties are created here and intended to be used by the UI.
        // The UI or the MainViewModel will bind buttons or input to these commands.
        public ICommand MoveCommand { get; }
        public ICommand InteractCommand { get; }
        public ICommand StopHidingCommand { get; }

        public ActionSystem(MainViewModel vm)
        {
            // Keep a reference to the main view model so we can update shared state.
            _vm = vm;

            // Build the ordered list of interaction handlers.
            // Handlers check if they can handle an interaction and then run the behavior.
            _interactionChain = new List<IInteractionHandler>
            {
                new HideInteraction(),
                new LockedDoorInteraction(),
                new TransitionInteraction(),
                new LootInteraction(),
                new EmptySearchInteraction()
            };

            // Wire UI commands to the local methods.
            // RelayCommand will call the given method when the command is executed by the UI.
            MoveCommand = new RelayCommand<string>(Move);
            InteractCommand = new RelayCommand<InteractableObject>(Interact);
            StopHidingCommand = new RelayCommand<object>(_ => StopHiding());
        }

        // Move is called when the player requests movement.
        // The direction string comes from the UI binding.
        // This method changes the current location on the main view model and updates UI properties.
        private void Move(string direction)
        {
            // If the player is hiding, do not allow movement.
            if (_vm.StateManager.CurrentState is HidingState) return;

            int newLocID = -1;

            // Convert the direction name into the target location id.
            // The location object stores neighboring location IDs.
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

            // If a valid location id was found, update the main view model.
            if (newLocID != -1)
            {
                // Find the location object by id and set it as the current location.
                // If not found, keep the previous location.
                _vm.CurrentLocation = _vm.AllLocations.FirstOrDefault(x => x.ID == newLocID) ?? _vm.CurrentLocation;

                // Update the background image path so the UI shows the new room.
                _vm.BackgroundImage = _vm.CurrentLocation.ImagePath;

                // Close any open popup when moving.
                _vm.IsPopupVisible = false;

                // Tell the game loop that the player moved so it can run monster or game logic.
                // The game loop is accessed through the main view model.
                _vm.GameLoop.CheckPlayerMoved();
            }
        }

        // Interact is called when the player clicks on an interactable object in the UI.
        // The object model represents things like doors, loot, hiding spots, etc.
        private void Interact(InteractableObject obj)
        {
            // Ignore null clicks and interactions while hiding.
            if (obj == null || _vm.StateManager.CurrentState is HidingState) return;

            // Walk the interaction chain and find the first handler that can handle this object.
            // Then call its Handle method, passing the object and the main view model so the handler
            // can change state, play audio, add items to inventory, push game states, etc.
            _interactionChain.FirstOrDefault(h => h.CanHandle(obj))?.Handle(obj, _vm);
        }

        // Stop hiding is used to exit the hiding state.
        // It manipulates the game state stack and tells the game loop about the state change.
        private void StopHiding()
        {
            // Only act if the current state is the hiding state.
            if (_vm.StateManager.CurrentState is HidingState)
            {
                // Pop the hiding state to return to the previous state.
                _vm.StateManager.PopState();

                // Notify the game loop that the player is no longer hiding.
                // This can cause monsters to move or other world reactions to happen.
                _vm.GameLoop.CheckPlayerMoved();
            }
        }
    }
}