using DyingFire.Models;
using DyingFire.ViewModels;
using System.Collections.Generic;

namespace DyingFire.States
{
    // HidingState is pushed onto the game's state stack when the player hides.
    // The GameStateManager (via ActionSystem or interaction handlers) creates and pushes this state.
    public class HidingState : IGameState
    {
        // Reference to the main view model so the state can update UI-bound properties and call services.
        // The MainViewModel owns the StateManager and the Audio service, plus game properties like Sanity.
        private MainViewModel _vm;

        // The interactable object the player hid inside.
        // Its HidingImagePath is used to change the background while hiding.
        private InteractableObject _hidingSpot;

        // Constructor stores the view model and the specific hiding spot.
        // The code that creates this state supplies the correct InteractableObject instance.
        public HidingState(MainViewModel vm, InteractableObject hidingSpot)
        {
            _vm = vm;
            _hidingSpot = hidingSpot;
        }

        // Enter is called when the state becomes active.
        // It changes the background image to the hiding spot's image (or a default),
        // starts the hiding audio, hides any popup, and turns on the hiding UI.
        // Other parts of the app listen to the view model properties and react accordingly.
        public void Enter()
        {
            _vm.BackgroundImage = _hidingSpot.HidingImagePath ?? "/Assets/Images/hallway.png";

            // Play a breathing or tension BGM while hiding.
            // The Audio service is accessed from the view model.
            _vm.Audio.PlayBGM("/Assets/Audio/heavyscaredbreathing.mp3");
            _vm.IsPopupVisible = false;
            // IsHidingUI tells the view to show the hiding screen and hide normal HUD.
            _vm.IsHidingUI = true;
        }

        // Exit is called when the state is popped.
        // It restores the normal room background and main background music.
        // It also turns off the hiding UI so normal UI returns.
        public void Exit()
        {
            _vm.BackgroundImage = _vm.CurrentLocation.ImagePath;
            _vm.Audio.PlayBGM("/Assets/Audio/dyingFireTrack.mp3"); // Return to main track
            _vm.IsHidingUI = false;
        }

        // Update is called regularly by the game loop while this state is active.
        // It reduces the player's sanity over time while hiding.
        // The GameLoopSystem or GameStateManager is responsible for calling Update each tick.
        public void Update()
        {
            if (_vm.Sanity > 10) _vm.Sanity -= 1;
        }
    }
}