using System.Windows;

namespace TowerDefense
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            GameHandler gameHandler = new();
            gameHandler.Width = 1920;
            gameHandler.Height = 1080;
            MainGrid.Children.Add(gameHandler);
        }
    }
}