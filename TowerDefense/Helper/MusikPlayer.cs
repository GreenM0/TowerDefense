using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TowerDefense.Helper
{
    public class MusikPlayer
    {
        private MediaPlayer _mediaPlayer;

        public MusikPlayer()
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.MediaEnded += OnMediaEnded; // Event für Schleife
        }

        public void Play(string filePath)
        {
            _mediaPlayer.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));
            _mediaPlayer.Play();
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
        }

        private void OnMediaEnded(object sender, EventArgs e)
        {
            // Musik wiederholen, wenn sie endet
            _mediaPlayer.Position = TimeSpan.Zero;
            _mediaPlayer.Play();
        }
    }
}
