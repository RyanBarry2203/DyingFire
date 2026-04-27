using DyingFire.Models;
using DyingFire.Services;
using DyingFire.States;
using DyingFire.ViewModels;
using System;
using System.Windows.Threading;

namespace DyingFire.Systems
{
    public class GameLoopSystem
    {
        private MainViewModel _vm;
        private DispatcherTimer _gameTimer;

        public GameLoopSystem(MainViewModel vm)
        {
            _vm = vm;
            _vm.MonsterAI.OnJumpscareTriggered += HandleJumpscare;
            _vm.MonsterAI.OnMonsterMoved += HandleMonsterAudioCues;
        }

        public void Start()
        {
            _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            _vm.StateManager.Update();

            if (_vm.CurrentLocation != null && _vm.CurrentLocation.IsDark) _vm.Sanity = Math.Max(0, _vm.Sanity - 5);
            else _vm.Sanity = Math.Min(100, _vm.Sanity + 2);

            if (_vm.Sanity <= 0)
            {
                HandleSanityDeath();
                return;
            }

            if (_vm.IsMonsterEnabled)
            {
                var state = _vm.StateManager.CurrentState is HidingState ? GameState.Hiding : GameState.Exploring;
                if (state == GameState.Exploring) _vm.IsParanormalActivityPresent = new Random().Next(100) < 20;

                _vm.MonsterAI.MoveMonster(_vm.AllLocations, _vm.CurrentLocation.ID, state);
            }
        }

        public void CheckPlayerMoved()
        {
            if (_vm.IsMonsterEnabled && _vm.CurrentLocation.ID == _vm.MonsterAI.MonsterLocationID)
                _vm.MonsterAI.TriggerJumpscare();
            else if (_vm.IsMonsterEnabled)
                UpdateTensionMusic();
        }

        private void HandleMonsterAudioCues(int monsterLocId)
        {
            if (_vm.CurrentLocation == null || _vm.StateManager.CurrentState is HidingState) return;

            int distance = _vm.MonsterAI.GetDistance(_vm.AllLocations, _vm.CurrentLocation.ID, monsterLocId);

            if (distance == 1) _vm.Audio.PlaySFX(_vm.Config["SFX_Footstep_Heavy"]);
            else if (distance == 2) _vm.Audio.PlaySFX(_vm.Config["SFX_Footstep_Medium"]);
            else if (distance >= 3 && distance < 999) _vm.Audio.PlaySFX(_vm.Config["SFX_Footstep_Light"]);

            UpdateTensionMusic();
        }

        private void UpdateTensionMusic()
        {
            if (_vm.CurrentLocation == null) return;
            int distance = _vm.MonsterAI.GetDistance(_vm.AllLocations, _vm.CurrentLocation.ID, _vm.MonsterAI.MonsterLocationID);

            if (distance == 1 && !(_vm.StateManager.CurrentState is HidingState))
                _vm.Audio.PlayTension(_vm.Config["BGM_Tension"]);
            else
                _vm.Audio.StopTension();
        }

        private void HandleJumpscare()
        {
            _gameTimer.Stop();
            _vm.Audio.StopTension();
            _vm.BackgroundImage = _vm.Config["Img_Jumpscare_Monster"];
            _vm.Audio.PlaySFX(_vm.Config["BGM_Tension"]);
            _vm.ShowMessage("GAME OVER", "It found you...");
        }

        private void HandleSanityDeath()
        {
            _gameTimer.Stop();
            _vm.Audio.StopTension();
            _vm.BackgroundImage = _vm.Config["Img_Jumpscare_Sanity"];
            _vm.Audio.PlaySFX(_vm.Config["BGM_Tension"]);
            _vm.ShowMessage("GAME OVER", "You succumbed to the darkness and lost your mind...");
        }
    }
}