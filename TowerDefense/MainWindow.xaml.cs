using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace TowerDefense
{
    public partial class MainWindow : Window
    {
        GameHandler gameHandler;

        public MainWindow()
        {
            InitializeComponent();
            gameHandler = new();
            gameHandler.Opacity = 0.5;
            MainGrid.Children.Add(gameHandler);
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            _ = Task.WhenAll(
                FadeOutButton(Start),
                FadeOutButton(Tutorial),
                FadeOutButton(Credits),
                FadeOutButton(Leave)
            );

            Menu.Visibility = Visibility.Hidden;

            await FadeInElement(gameHandler);
            Thread.Sleep(500);
            gameHandler.StartGame();
        }

        private Task FadeOutButton(UIElement button)
        {
            var tcs = new TaskCompletionSource<bool>();
            var anim = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new QuadraticEase()
            };
            anim.Completed += (_, __) => tcs.SetResult(true);
            button.BeginAnimation(UIElement.OpacityProperty, anim);
            return tcs.Task;
        }

        private Task FadeInElement(UIElement element)
        {
            var tcs = new TaskCompletionSource<bool>();
            element.Opacity = 0.5;
            var anim = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new QuadraticEase()
            };
            anim.Completed += (_, __) => tcs.SetResult(true);
            element.BeginAnimation(UIElement.OpacityProperty, anim);
            return tcs.Task;
        }

    }
}