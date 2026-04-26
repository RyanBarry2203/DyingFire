using System;
using System.IO;
using System.Windows.Media;

namespace DyingFire.Services
{
    public class AudioService
    {
        private MediaPlayer _bgmPlayer = new MediaPlayer();
        private MediaPlayer _sfxPlayer = new MediaPlayer();

        public void PlayBGM(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath.TrimStart('/'));
                _bgmPlayer.Open(new Uri(fullPath, UriKind.Absolute));
                _bgmPlayer.MediaEnded += (s, e) => { _bgmPlayer.Position = TimeSpan.Zero; _bgmPlayer.Play(); }; // Loop
                _bgmPlayer.Play();
            }
            catch { }
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