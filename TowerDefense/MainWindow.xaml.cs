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
            gameHandler.Width = MainGrid.ActualWidth;
            gameHandler.Height = MainGrid.ActualHeight;
            MainGrid.Children.Add(gameHandler);
        }
    }
}