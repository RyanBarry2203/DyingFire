using DyingFire.ViewModels;

namespace DyingFire.States
{
    public class HidingState : IGameState
    {
        private MainViewModel _vm;

        public HidingState(MainViewModel vm)
        {
            _vm = vm;
        }

        public void Enter()
        {
            // Scene Change & Audio
            _vm.BackgroundImage = "/Assets/Images/insideFurnace.png";
            _vm.Audio.PlayBGM("/Assets/Audio/heavyScaredBreathing.mp3");

            // UI Update
            _vm.IsPopupVisible = false;
            _vm.IsHidingUI = true;
        }

        public void Exit()
        {
            // Revert Scene & Audio
            _vm.BackgroundImage = _vm.CurrentLocation.ImagePath;
            _vm.Audio.PlayBGM("/Assets/Audio/horrorAtmosphere.mp3");

            // UI Update
            _vm.IsHidingUI = false;
        }

        public void Update()
        {
            if (_vm.Sanity > 10) _vm.Sanity -= 1;
        }
    }
}