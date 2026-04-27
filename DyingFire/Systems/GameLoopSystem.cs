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
        // Reference to the central view model so the loop can read and update game state and call services.
        private MainViewModel _vm;

        // Timer used to call the game tick on a regular interval on the UI thread.
        private DispatcherTimer _gameTimer;

        // Constructor stores the view model and subscribes to monster events.
        // The constructor wires monster events so the loop reacts to monster actions.
        public GameLoopSystem(MainViewModel vm)
        {
            _vm = vm;
            // When the monster triggers a jumpscare, handle it here.
            _vm.MonsterAI.OnJumpscareTriggered += HandleJumpscare;
            // When the monster moves, play audio cues based on distance.
            _vm.MonsterAI.OnMonsterMoved += HandleMonsterAudioCues;
        }

        // Start creates and starts the DispatcherTimer used for the main game loop.
        public void Start()
        {
            // Run a tick every 3 seconds.
            _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            // Attach the tick handler.
            _gameTimer.Tick += GameTimer_Tick;
            // Start the timer so ticks begin.
            _gameTimer.Start();
        }

        // This method runs each timer tick and advances game state.
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Update any active game state (for example HidingState.Update).
            _vm.StateManager.Update();

            // If the current room is dark, decrease sanity faster.
            if (_vm.CurrentLocation != null && _vm.CurrentLocation.IsDark) _vm.Sanity = Math.Max(0, _vm.Sanity - 5);
            else // Otherwise slowly recover sanity.
                _vm.Sanity = Math.Min(100, _vm.Sanity + 2);

            // If sanity reached zero handle player death by sanity.
            if (_vm.Sanity <= 0)
            {
                HandleSanityDeath();
                return;
            }

            // If the monster feature is enabled run monster logic.
            if (_vm.IsMonsterEnabled)
            {
                // Determine whether the player is hiding or exploring.
                var state = _vm.StateManager.CurrentState is HidingState ? GameState.Hiding : GameState.Exploring;

                // While exploring occasionally mark paranormal activity so some items respond.
                if (state == GameState.Exploring) _vm.IsParanormalActivityPresent = new Random().Next(100) < 20;

                // Tell the monster AI to pick its next move using the full map and the player's location.
                _vm.MonsterAI.MoveMonster(_vm.AllLocations, _vm.CurrentLocation.ID, state);
            }
        }

        // Called after the player moves to update monster reactions or trigger a jumpscare if in same room.
        public void CheckPlayerMoved()
        {
            // If monster is enabled and is in the same room as the player trigger jumpscare immediately.
            if (_vm.IsMonsterEnabled && _vm.CurrentLocation.ID == _vm.MonsterAI.MonsterLocationID)
                _vm.MonsterAI.TriggerJumpscare();
            else if (_vm.IsMonsterEnabled)
                // Otherwise update the tension music based on current distance.
                UpdateTensionMusic();
        }

        // Play footstep SFX based on how far the monster is from the player.
        private void HandleMonsterAudioCues(int monsterLocId)
        {
            // If there is no current location or the player is hiding do nothing.
            if (_vm.CurrentLocation == null || _vm.StateManager.CurrentState is HidingState) return;

            // Calculate distance in rooms between player and monster.
            int distance = _vm.MonsterAI.GetDistance(_vm.AllLocations, _vm.CurrentLocation.ID, monsterLocId);

            // Play different step sounds for different distances.
            if (distance == 1) _vm.Audio.PlaySFX(_vm.Config["SFX_Footstep_Heavy"]);
            else if (distance == 2) _vm.Audio.PlaySFX(_vm.Config["SFX_Footstep_Medium"]);
            else if (distance >= 3 && distance < 999) _vm.Audio.PlaySFX(_vm.Config["SFX_Footstep_Light"]);

            // After playing step sounds update tension music to match distance.
            UpdateTensionMusic();
        }

        // Start or stop the tension music depending on how close the monster is.
        private void UpdateTensionMusic()
        {
            // If no current location we cannot decide, so return.
            if (_vm.CurrentLocation == null) return;

            // Get distance from player to monster.
            int distance = _vm.MonsterAI.GetDistance(_vm.AllLocations, _vm.CurrentLocation.ID, _vm.MonsterAI.MonsterLocationID);

            // If monster is adjacent and player is not hiding play tension track.
            if (distance == 1 && !(_vm.StateManager.CurrentState is HidingState))
                _vm.Audio.PlayTension(_vm.Config["BGM_Tension"]);
            else
                // Otherwise stop tension music so normal BGM can continue.
                _vm.Audio.StopTension();
        }

        // Handle a full jumpscare event fired by the MonsterAI.
        private void HandleJumpscare()
        {
            // Stop the game timer so no more ticks run while jumpscare is shown.
            _gameTimer.Stop();
            // Stop any tension music.
            _vm.Audio.StopTension();
            // Change the background to the jumpscare image from config.
            _vm.BackgroundImage = _vm.Config["Img_Jumpscare_Monster"];
            // Play a loud SFX or music to emphasize the jumpscare.
            _vm.Audio.PlaySFX(_vm.Config["BGM_Tension"]);
            // Show the game over popup to the player.
            _vm.ShowMessage("GAME OVER", "It found you...");
        }

        // Handle the player dying from sanity loss.
        private void HandleSanityDeath()
        {
            // Stop ticking while the game over screen is shown.
            _gameTimer.Stop();
            // Stop tension music.
            _vm.Audio.StopTension();
            // Show the sanity death image from config.
            _vm.BackgroundImage = _vm.Config["Img_Jumpscare_Sanity"];
            // Play the tension SFX/music for dramatic effect.
            _vm.Audio.PlaySFX(_vm.Config["BGM_Tension"]);
            // Show the game over popup with a sanity-specific message.
            _vm.ShowMessage("GAME OVER", "You succumbed to the darkness and lost your mind...");
        }
    }
}