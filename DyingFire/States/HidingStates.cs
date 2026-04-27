using DyingFire.Models;
using DyingFire.ViewModels;

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
            string spotName = _hidingSpot.Name.ToLower();

            if (spotName.Contains("boiler"))
                _vm.BackgroundImage = "/Assets/Images/insideFurnace.png";
            else if (spotName.Contains("bed"))
                _vm.BackgroundImage = "/Assets/Images/undernuerserybed.png";
            else if (spotName.Contains("chest"))
                _vm.BackgroundImage = "/Assets/Images/insideChest.png";

            _vm.Audio.PlayBGM("/Assets/Audio/heavyscaredbreathing.mp3");
            _vm.IsPopupVisible = false;
            _vm.IsHidingUI = true;
        }

        public void Exit()
        {
            _vm.BackgroundImage = _vm.CurrentLocation.ImagePath;
            _vm.Audio.PlayBGM("/Assets/Audio/horrorAtmosphere.mp3");
            _vm.IsHidingUI = false;
        }

        public void Update()
        {
            if (_vm.Sanity > 10) _vm.Sanity -= 1;
        }
    }
}