using System.Windows;
using System.Windows.Controls;

namespace TowerDefense.Settings
{
    public partial class SoundSettingsWindow : Window
    {
        private MediaElement _menuMusic;
        private MediaElement _ingameMusic;

        public SoundSettingsWindow(MediaElement menuMusic, MediaElement ingameMusic)
        {
            InitializeComponent();

            _menuMusic = menuMusic;
            _ingameMusic = ingameMusic;

            // Initialisiere die Slider mit den aktuellen Lautstärken
            MenuVolumeSlider.Value = _menuMusic.Volume;
            IngameVolumeSlider.Value = _ingameMusic.Volume;
        }

        private void MenuVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Aktualisiere die Menü-Musik-Lautstärke
            if (_menuMusic != null)
            {
                _menuMusic.Volume = MenuVolumeSlider.Value;
            }

            // Speichere die Einstellung
            Properties.Settings.Default.MenuVolume = MenuVolumeSlider.Value;
        }

        private void IngameVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Aktualisiere die Ingame-Musik-Lautstärke
            if (_ingameMusic != null)
            {
                _ingameMusic.Volume = IngameVolumeSlider.Value;
            }

            // Speichere die Einstellung
            Properties.Settings.Default.IngameVolume = IngameVolumeSlider.Value;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Speichere alle Einstellungen
            Properties.Settings.Default.Save();

            // Schließe das Fenster
            this.Close();
        }
    }
}