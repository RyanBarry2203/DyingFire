using DyingFire.Models;
using DyingFire.ViewModels;
using System.Collections.Generic;

namespace DyingFire.States
{
    public class HidingState : IGameState
    {
        private MainViewModel _vm;
        private InteractableObject _hidingSpot;

        public HidingState(MainViewModel vm, InteractableObject hidingSpot)
        {
            _vm = vm;
            _hidingSpot = hidingSpot;
        }

        public void Enter()
        {
            _vm.BackgroundImage = _hidingSpot.HidingImagePath ?? "/Assets/Images/hallway.png";

            _vm.Audio.PlayBGM("/Assets/Audio/heavyscaredbreathing.mp3");
            _vm.IsPopupVisible = false;
            _vm.IsHidingUI = true;
        }
        public void Exit()
        {
            _vm.BackgroundImage = _vm.CurrentLocation.ImagePath;
            _vm.Audio.PlayBGM("/Assets/Audio/dyingFireTrack.mp3"); // Return to main track
            _vm.IsHidingUI = false;
        }

        public void Update()
        {
            if (_vm.Sanity > 10) _vm.Sanity -= 1;
        }
    }
}