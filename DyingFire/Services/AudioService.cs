using System;
using System.IO;
using System.Windows.Media;

namespace DyingFire.Services
{
    public class AudioService
    {
        // MediaPlayer used for continuous background music.
        // The MainViewModel or app startup code will call PlayBGM to start it.
        private MediaPlayer _bgmPlayer = new MediaPlayer();

        // MediaPlayer used for a tension/stress loop track.
        // Game logic (for example a monster AI or game loop) starts and stops this.
        private MediaPlayer _tensionPlayer = new MediaPlayer();

        // MediaPlayer used for one-shot sound effects.
        // Interaction handlers and UI code call PlaySFX for clicks, item use, etc.
        private MediaPlayer _sfxPlayer = new MediaPlayer();

        public AudioService()
        {
            // Set default volumes for each channel.
            // These low-level volume settings control how loud each type is relative to others.
            _bgmPlayer.Volume = 0.01;
            _tensionPlayer.Volume = 0.6;
            _sfxPlayer.Volume = 0.8;
        }

        // PlayBGM opens and plays a looping background track.
        // The parameter is a relative path from the application base directory.
        // The method wires MediaEnded to restart the track so it loops indefinitely.
        // Callers simply ask for a track to play and do not need to manage the MediaPlayer directly.
        public void PlayBGM(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath.TrimStart('/'));
                _bgmPlayer.Open(new Uri(fullPath, UriKind.Absolute));
                _bgmPlayer.MediaEnded += (s, e) => { _bgmPlayer.Position = TimeSpan.Zero; _bgmPlayer.Play(); };
                _bgmPlayer.Play();
            }
            catch { } // Swallow exceptions so missing audio files do not crash the game.
        }

        // PlayTension opens and plays a looping tension track.
        // If the requested track is already playing, the method returns early.
        // This is used by gameplay systems to signal tension music without affecting BGM.
        public void PlayTension(string relativePath)
        {
            try
            {
                if (_tensionPlayer.Source != null && _tensionPlayer.Source.AbsolutePath.Contains(relativePath)) return;

                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath.TrimStart('/'));
                _tensionPlayer.Open(new Uri(fullPath, UriKind.Absolute));
                _tensionPlayer.MediaEnded += (s, e) => { _tensionPlayer.Position = TimeSpan.Zero; _tensionPlayer.Play(); };
                _tensionPlayer.Play();
            }
            catch { } // Ignore audio load/play errors.
        }

        // StopTension stops and closes the tension player.
        // Call this when the game should return to normal audio state.
        public void StopTension()
        {
            _tensionPlayer.Stop();
            _tensionPlayer.Close();
        }

        // PlaySFX opens and plays a single sound effect.
        // This is for short sounds like item pickup, door creaks, or UI clicks.
        public void PlaySFX(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath.TrimStart('/'));
                _sfxPlayer.Open(new Uri(fullPath, UriKind.Absolute));
                _sfxPlayer.Play();
            }
            catch { } // Do not throw on missing or invalid SFX files.
        }
    }
}