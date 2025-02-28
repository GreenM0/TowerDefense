using System;
using System.Windows;
using System.Windows.Controls;
using TowerDefense.Towers;
using TowerDefense;

namespace TowerDefense.Towers
{
    public partial class Towerwindow : Window
    {
        public bool IsUpgraded { get; private set; } = false;
        private BaseTower _tower;

        public Towerwindow(BaseTower tower)
        {
            InitializeComponent();
            _tower = tower;

            // Aktiviere den Upgrade-Button nur, wenn genug Geld vorhanden ist und der Turm nicht maximal aufgerüstet ist
            UpgradeButton.IsEnabled = (GameHandler.Instance.cash >= tower.UpgradeCost && _tower.UpgradeLevel < _tower.MaxUpgradeLevel);
            // Deaktiviere den ComboBox, wenn der Turm vom Typ AllTargetTower ist
            if (_tower is IceTower)
            {
                TargetModeComboBox.IsEnabled = false; // Deaktiviere den ComboBox
                TargetModeComboBox.Visibility = Visibility.Collapsed; // Verstecke den ComboBox
            }
            else
            {
                // Setze den ausgewählten Zielmodus im ComboBox
                SetSelectedTargetMode();
            }
            UpdateButtonLabels();
        }

        private void UpdateButtonLabels()
        {
            // Upgrade-Button: Zeige die Upgrade-Kosten an
            UpgradeButton.Content = $"🛡️ Upgrade ({_tower.UpgradeCost}$)";

            // Verkaufen-Button: Zeige die Einnahmen an (z. B. 50% des Turmwertes)
            int sellValue = _tower.TowerWorth / 2;
            SellButton.Content = $"💰 Verkaufen ({sellValue}$)";
        }

        private void SetSelectedTargetMode()
        {
            // Setze den ausgewählten Modus im ComboBox basierend auf dem aktuellen Zielmodus des Turms
            foreach (ComboBoxItem item in TargetModeComboBox.Items)
            {
                if (item.Tag.ToString() == _tower.TargetMode)
                {
                    TargetModeComboBox.SelectedItem = item;
                    break;
                }
            }

            // Falls kein Zielmodus gesetzt ist, setze den Standardwert auf "Nächster Gegner"
            if (TargetModeComboBox.SelectedItem == null)
            {
                TargetModeComboBox.SelectedIndex = 0; // Erster Eintrag ("Nächster Gegner")
            }
        }

        private void UpgradeTower_Click(object sender, RoutedEventArgs e)
        {
            if (GameHandler.Instance.cash >= _tower.UpgradeCost)
            {
                var change = -1 * _tower.UpgradeCost;
                GameHandler.Instance.Cashhandler(change);
                _tower.UpgradeTower();
                IsUpgraded = true;
                UpdateButtonLabels();
                Close(); // Schließe das Fenster ohne Nachricht
            }
        }

        private void SellTower_Click(object sender, RoutedEventArgs e)
        {
            GameHandler.Instance.SellTower(_tower);
            Close();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TargetModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ändere den Zielmodus des Turms basierend auf der Auswahl im ComboBox
            if (TargetModeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                _tower.TargetMode = selectedItem.Tag.ToString();
            }
        }
    }
}