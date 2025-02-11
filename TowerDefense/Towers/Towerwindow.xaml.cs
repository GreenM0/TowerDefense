using System.Windows;
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

            UpgradeButton.IsEnabled = GameHandler.Instance.cash >= tower.UpgradeCost;
        }

        private void UpgradeTower_Click(object sender, RoutedEventArgs e)
        {
            if (GameHandler.Instance.cash >= _tower.UpgradeCost)
            {
                GameHandler.Instance.cash -= _tower.UpgradeCost;
                _tower.UpgradeTower();
                IsUpgraded = true;
                MessageBox.Show("Turm erfolgreich aufgerüstet!", "Upgrade", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Nicht genug Geld für ein Upgrade!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SellTower_Click(object sender, RoutedEventArgs e)
        {
            GameHandler.Instance.SellTower(_tower);
            this.Close();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
