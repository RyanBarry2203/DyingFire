using System;
using System.IO;
using System.Windows.Media;

namespace DyingFire.Services
{
    public class AudioService
    {
        private MediaPlayer _bgmPlayer = new MediaPlayer();
        private MediaPlayer _tensionPlayer = new MediaPlayer();
        private MediaPlayer _sfxPlayer = new MediaPlayer();

        public AudioService()
        {
            // Set volumes
            _bgmPlayer.Volume = 0.01;
            _tensionPlayer.Volume = 0.6;
            _sfxPlayer.Volume = 0.8;
        }

        public void PlayBGM(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath.TrimStart('/'));
                _bgmPlayer.Open(new Uri(fullPath, UriKind.Absolute));
                _bgmPlayer.MediaEnded += (s, e) => { _bgmPlayer.Position = TimeSpan.Zero; _bgmPlayer.Play(); };
                _bgmPlayer.Play();
            }
            catch { }
        }

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
            catch { }
        }

        public void StopTension()
        {
            _tensionPlayer.Stop();
            _tensionPlayer.Close();
        }

        public void PlaySFX(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath.TrimStart('/'));
                _sfxPlayer.Open(new Uri(fullPath, UriKind.Absolute));
                _sfxPlayer.Play();
            }
            catch { }
        }
    }
}