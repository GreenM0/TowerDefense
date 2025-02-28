using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Input;
using TowerDefense.Helper;

namespace TowerDefense
{
    public partial class MainWindow : Window
    {
        private int currentMapIndex = 0;
        private string[] maps = { "Map1", "Map2" }; // Liste der verfügbaren Karten
        private GameHandler gameHandler;

        public MainWindow()
        {
            InitializeComponent();

            this.KeyDown += MainWindow_KeyDown; 

            gameHandler = new GameHandler(maps[currentMapIndex]); // Initialisiere mit der ersten Karte
            gameHandler.GameOver += OnGameOver;
            gameHandler.Opacity = 0.5;
            MainGrid.Children.Add(gameHandler);
            BackgroundMusic.Play();
        }

        private void BackgroundMusic_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Musik wiederholen, wenn sie endet
            BackgroundMusic.Position = TimeSpan.Zero;
            BackgroundMusic.Play();
        }

        private void LeftArrow_Click(object sender, RoutedEventArgs e)
        {
            if (currentMapIndex > 0)
            {
                currentMapIndex--;
                SwitchMap(maps[currentMapIndex]);
            }
            else
            {
                currentMapIndex++;
                SwitchMap(maps[currentMapIndex]);
            }
        }

        private void RightArrow_Click(object sender, RoutedEventArgs e)
        {
            if (currentMapIndex < maps.Length - 1)
            {
                currentMapIndex++;
                SwitchMap(maps[currentMapIndex]);
            }
            else
            {
                currentMapIndex--;
                SwitchMap(maps[currentMapIndex]);
            }
        }

        private void SwitchMap(string mapName)
        {
            MainGrid.Children.Remove(gameHandler);
            gameHandler = new GameHandler(mapName); // Initialisiere den GameHandler mit der neuen Karte
            gameHandler.GameOver += OnGameOver;
            gameHandler.Opacity = 0.5;
            MainGrid.Children.Add(gameHandler);
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            _ = Task.WhenAll(
                FadeOutButton(Start),
                FadeOutButton(Tutorial),
                FadeOutButton(Credits),
                FadeOutButton(Leave),
                FadeOutButton(LeftArrow),
                FadeOutButton(RightArrow),
                FadeOutButton(Restart)
            );
            
            await FadeInElement(gameHandler);
            Menu.Visibility = Visibility.Hidden;
            BackgroundMusic.Stop();
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

        private Task FadeInElement2(UIElement element)
        {
            var tcs = new TaskCompletionSource<bool>();
            element.Opacity = 0;
            var anim = new DoubleAnimation(0, 0.5, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new QuadraticEase()
            };
            anim.Completed += (_, __) => tcs.SetResult(true);
            element.BeginAnimation(UIElement.OpacityProperty, anim);
            return tcs.Task;
        }

        private void Tutorial_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "SPIELANLEITUNG\n\n" +
                "1. Ziel des Spiels:\n" +
                "   - Verhindern, dass Gegner das Ziel erreichen.\n\n" +
                "2. Türme platzieren:\n" +
                "   - Wähle verschiedene Türme mit einzigartigen Eigenschaften.\n" +
                "   - Platziere sie strategisch entlang des Weges.\n\n" +
                "3. Gegnerwellen:\n" +
                "   - Gegner erscheinen in Wellen.\n" +
                "   - Jede Welle wird stärker und schneller.\n\n" +
                "4. Upgrades:\n" +
                "   - Verdiene Geld und verbessere deine Türme.\n\n" +
                "5. Spielende:\n" +
                "   - Das Spiel endet, wenn alle Wellen durch sind\n",
                "Spielanleitung", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "PDM-Projekt: Tower Defense\n\n" +
                "Entwickler: Timon, Lunis, Moritz und Lisa\n",
                "Credits", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private Task FadeOutElement(UIElement element)
        {
            var tcs = new TaskCompletionSource<bool>();
            var anim = new DoubleAnimation(0, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new QuadraticEase()
            };
            anim.Completed += (_, __) => tcs.SetResult(true);
            element.BeginAnimation(UIElement.OpacityProperty, anim);
            return tcs.Task;
        }

        private async void OnGameOver()
        {
            _ = Dispatcher.Invoke(async () =>
            {
                // Spielfeld ausblenden
                await FadeOutElement(gameHandler);

                // Alten GameHandler entfernen
                MainGrid.Children.Remove(gameHandler);
                gameHandler.GameOver -= OnGameOver;
                gameHandler = null;

                // Neuen GameHandler erstellen
                gameHandler = new GameHandler(new(maps[currentMapIndex]));
                gameHandler.GameOver += OnGameOver;
                gameHandler.Opacity = 0;
                MainGrid.Children.Add(gameHandler);


                // Spielfeld einblenden
                await FadeInElement2(gameHandler);

                // Menü wieder anzeigen
                Menu.Visibility = Visibility.Visible;

                // Buttons reinfaden lassen
                await Task.WhenAll(
                    FadeInButton(Start),
                    FadeInButton(Tutorial),
                    FadeInButton(Credits),
                    FadeInButton(Leave),
                    FadeInButton(LeftArrow),
                    FadeInButton(RightArrow),
                    FadeInButton(Restart)
                );
            });
        }


        private Task FadeInButton(UIElement button)
        {
            var tcs = new TaskCompletionSource<bool>();
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new QuadraticEase()
            };
            anim.Completed += (_, __) => tcs.SetResult(true);
            button.BeginAnimation(UIElement.OpacityProperty, anim);
            return tcs.Task;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Überprüfe, ob die ESC-Taste gedrückt wurde
            if (e.Key == Key.Escape)
            {
                if (gameHandler.isPaused)
                {
                    gameHandler.ResumeGame(); // Fortsetzen, wenn das Spiel pausiert ist
                    CloseMenu();
                }
                else
                {
                    gameHandler.PauseGame(); // Pausieren, wenn das Spiel läuft
                    OpenMenu();
                }
            }
        }

        private void OpenMenu()
        {
            BackgroundMusic.Play();
            // Verstecke das Spielfeld
            gameHandler.Opacity = 0.5;
            gameHandler.IsEnabled = false;

            // Zeige das Menü an
            Menu.Visibility = Visibility.Visible;

            // Fade-In-Animation für die Menü-Buttons
            FadeInButtons();
        }

        private void CloseMenu()
        {
            // Verstecke das Menü
            Menu.Visibility = Visibility.Collapsed;

            // Zeige das Spielfeld an
            gameHandler.Opacity = 1;
            gameHandler.IsEnabled = true;
            BackgroundMusic.Stop();
        }

        private async void FadeInButtons()
        {
            await Task.WhenAll(
                FadeInButton(Start),
                FadeInButton(Tutorial),
                FadeInButton(Credits),
                FadeInButton(Leave),
                FadeInButton(LeftArrow),
                FadeInButton(RightArrow),
                FadeInButton(Restart)
            );
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            // Schließe das Menü
            CloseMenu();

            // Setze das Spiel zurück
            gameHandler.ResetGame();
        }
    }
}