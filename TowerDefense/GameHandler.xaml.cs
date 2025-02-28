using System;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using TowerDefense.EnemiesModel;
using TowerDefense.EnemiesModel.Types;
using TowerDefense.Helper;
using TowerDefense.Maps;
using TowerDefense.Towers;
using TowerDefense.Waves;

namespace TowerDefense
{
    public partial class GameHandler : UserControl
    {
        private DispatcherTimer? _gameTick;
        public PathGeometry _gameWay;
        public Canvas _mainCanvas = null!;
        public List<Enemies> _enemyList = new List<Enemies>();
        private List<BaseTower> _towers = new List<BaseTower>();
        private List<BaseTower> _deployedTowers = new List<BaseTower>();
        public List<Rectangle> _rectangles = new List<Rectangle>();
        public int cash;
        private int startchash = 800;
        private Image? ghostTower;
        private bool _gameOver = false;
        private bool _allEnemiesSpawned = false;
        private Ellipse? _rangeIndicator; // Anzeige für die Angriffsreichweite
        private DispatcherTimer? _dragTimer; // Timer für Drag-and-Drop-Überprüfung
        private Point _currentMousePosition; // Aktuelle Mausposition
        public SpatialGrid<Enemies> _enemyGrid;
        public SpatialGrid<BaseTower> _towerGrid;
        public static GameHandler Instance { get; private set; }
        public event Action GameOver;
        private string currentMap;
        public bool isPaused = false;

        //Spieleinstellungen
        private double _Health = 50;
        private int _waveSpawnInterval = 4000; // Zeit in Millisekunden zwischen Waves
        private int _enemySpawnInterval = 2000; // Zeit in Millisekunden zwischen Gegner-Spawns

        public GameHandler(string mapName)
        {
            InitializeComponent();
            currentMap = mapName;
            InitializeMap();

        }

        public void StartGame()
        {
            LoadTowers();
            DisplayTowerMenu();
            InitializeSpawner();
            Cashhandler(startchash);
            Instance = this;
           
            _enemyGrid = new SpatialGrid<Enemies>(150);
            _towerGrid = new SpatialGrid<BaseTower>(50);

            IngameMusic.Play();
            _ = SpawnWavesAsync();
            _dragTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50) // Aktualisierungsintervall (50 ms)
            };
            _dragTimer.Tick += DragTimer_Tick;
        }

        public async void ResetGame()
        {
            // Pausiere das Spiel
            isPaused = true;

            // Stoppe alle Timer
            _gameTick?.Stop();
            _dragTimer?.Stop();

            // Stoppe das Spawnen von Gegnern
            _spawnCancellationTokenSource.Cancel();
            _spawnCancellationTokenSource = new CancellationTokenSource(); // Reset für den Neustart

            // Entferne alle Gegner und Türme
            foreach (var enemy in _enemyList)
            {
                if (GameField.Children.Contains(enemy.Image))
                {
                    GameField.Children.Remove(enemy.Image);
                }
            }
            _enemyList.Clear();

            foreach (var tower in _deployedTowers)
            {
                if (GameField.Children.Contains(tower.Image))
                {
                    GameField.Children.Remove(tower.Image);
                }
            }
            _deployedTowers.Clear();

            // Setze alle Spielvariablen zurück
            cash = 0;
            _Health = 50;
            _gameOver = false;
            _allEnemiesSpawned = false;

            // Zeige die Nachricht an
            info.Visibility = Visibility.Visible;

            // Timer für den Neustart
            int countdown = 5;
            while (countdown > 0)
            {
                info.Content = $"Neustart in {countdown}";
                await Task.Delay(1000); // Warte 1 Sekunde
                countdown--;
            }

            // Verstecke die Nachricht
            info.Visibility = Visibility.Collapsed;

            // Initialisiere die Karte neu
            InitializeMap();

            // Setze das Tower-Menü zurück
            TowerMenu.Children.Clear();
            LoadTowers();
            DisplayTowerMenu();

            // Starte das Spiel neu
            isPaused = false;
            StartGame();
        }

        public void PauseGame()
        {
            IngameMusic.Pause();
            // Pausiere das Spiel
            isPaused = true;

            // Stoppe alle Timer
            _gameTick?.Stop();
            _dragTimer?.Stop();

            // Stoppe das Spawnen von Gegnern
            _spawnCancellationTokenSource.Cancel();

            // Pausiere die Gegneranimationen
            PauseEnemyAnimations();

            // Pausiere die Turmlogik
            foreach (var tower in _deployedTowers)
            {
                tower.PauseTowerLogic();
            }

            // Pausiere die Projektile
            PauseProjectiles();
        }

        public void ResumeGame()
        {
            IngameMusic.Play();
            // Setze das Spiel fort
            isPaused = false;

            // Starte alle Timer neu
            _gameTick?.Start();
            _dragTimer?.Start();

            // Starte das Spawnen von Gegnern neu
            _spawnCancellationTokenSource = new CancellationTokenSource();
            _ = SpawnWavesAsync();

            // Setze die Gegneranimationen fort
            ResumeEnemyAnimations();

            // Setze die Turmlogik fort
            foreach (var tower in _deployedTowers)
            {
                tower.ResumeTowerLogic();
            }

            // Setze die Projektile fort
            ResumeProjectiles();

            // Verstecke die Pause-Nachricht
            info.Visibility = Visibility.Collapsed;
        }

        private CancellationTokenSource _spawnCancellationTokenSource = new CancellationTokenSource();

        private async Task SpawnWavesAsync()
        {
            Wave waves = new Wave();

            for (int currentWave = 0; currentWave < waves.GetWaveCount(); currentWave++)
            {
                if (_gameOver || _spawnCancellationTokenSource.Token.IsCancellationRequested || isPaused)
                    break;

                wave.Content = "Wave: " + (currentWave + 1) + "/80";

                for (int currentEnemyType = 0; currentEnemyType < waves.GetTotalEnemyTypes(); currentEnemyType++)
                {
                    int enemyAmount = waves.GetAmountOfEnemies(currentEnemyType, currentWave);
                    for (int EnemyAmount = 0; EnemyAmount < enemyAmount; EnemyAmount++)
                    {
                        if (_spawnCancellationTokenSource.Token.IsCancellationRequested || isPaused)
                            break;

                        Enemies currentEnemy = waves.SpawnEnemy(currentEnemyType, GameField, _gameWay);

                        _enemyGrid.AddObject(currentEnemy);
                        GameField.Children.Add(currentEnemy.Image);
                        _ = currentEnemy.Movement(_mainCanvas, _enemyGrid);
                        _enemyList.Add(currentEnemy);

                        await Task.Delay(_enemySpawnInterval, _spawnCancellationTokenSource.Token);
                    }
                }

                await Task.Delay(_waveSpawnInterval, _spawnCancellationTokenSource.Token);

                if (currentWave >= 10 && currentWave % 10 == 0)
                {
                    int speed = (int)Math.Round(currentWave * 0.1);
                    _waveSpawnInterval = Math.Max(500, _waveSpawnInterval / speed);
                    _enemySpawnInterval = Math.Max(250, _enemySpawnInterval / speed);
                }
            }

            _allEnemiesSpawned = true;
        }

        private void InitializeMap()
        {
            // Entferne vorhandene Karte, falls vorhanden
            GameField.Children.Clear();

            switch (currentMap)
            {
                case "Map1":
                    Map1 map1 = new Map1();
                    GameField.Children.Add(map1);
                    _mainCanvas = map1.MainCanvas;
                    _gameWay = map1.GetPathGeometry();
                    _rectangles = map1.Rectangles;
                    break;
                case "Map2":
                    Map2 map2 = new Map2();
                    GameField.Children.Add(map2);
                    _mainCanvas = map2.MainCanvas;
                    _gameWay = map2.GetPathGeometry();
                    _rectangles = map2.Rectangles;
                    break;
                default:
                    throw new ArgumentException("Ungültige Karte: " + currentMap);
            }
        }


        private void InitializeSpawner()
        {
            _gameTick = new DispatcherTimer();
            _gameTick.Interval = TimeSpan.FromSeconds(0.2);
            _gameTick.Tick += GameTick;
            _gameTick.Start();
        }

        private void GameTick(object? sender, EventArgs e)
        {
            health.Content = _Health;       

            bool won = true;
            foreach (var enemy in _enemyList)
            {
                if (enemy.Life > 0)
                    won = false;       
            }

            if (won && _allEnemiesSpawned)
            {
                _gameTick.Stop();
                info.Content = "GAME WON";
                info.Visibility = Visibility.Visible;
                health.Content = "0";
                _gameOver = true;
            }

            if (_Health <= 0) // <= statt <, um 0 einzuschließen
            {
                _gameTick.Stop();
                info.Content = "GAME OVER";
                info.Visibility = Visibility.Visible;
                health.Content = "0";
                _gameOver = true;

                Task.Delay(5000).ContinueWith(_ =>
                {
                    GameOver?.Invoke(); // Event auslösen
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void LoadTowers()
        {
            _towers = new List<BaseTower>
            {
                new IceTower(new Point(0, 0)),
                new ArcherTower(new Point(0, 0)),
                new FireTower(new Point(0, 0)),
                new MageTower(new Point(0, 0))
            };
        }

        private void DisplayTowerMenu()
        {
            // Leere das Tower-Menü
            TowerMenu.Children.Clear();

            foreach (var tower in _towers)
            {
                // Bild des Turms erstellen
                Image towerImage = tower.GetEntityPic();
                towerImage.Width = 100;
                towerImage.Height = 100;
                towerImage.Tag = tower;

                towerImage.MouseMove += TowerImage_MouseMove;

                // Preistext erstellen
                TextBlock priceText = new TextBlock
                {
                    Text = $"{tower.Costs}$",
                    Foreground = Brushes.Black,
                    FontSize = 20,
                    TextAlignment = TextAlignment.Center,
                };

                // Container für Bild und Preis erstellen
                StackPanel towerPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                towerPanel.Children.Add(towerImage);
                towerPanel.Children.Add(priceText);

                // Füge das Panel ins Menü ein
                TowerMenu.Children.Add(towerPanel);
            }
        }

        private void TowerImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Image draggedImage && draggedImage.Tag is BaseTower selectedTower)
            {
                // Starte den Drag-Timer
                if (_dragTimer?.IsEnabled == false)
                {
                    _dragTimer.Start();
                }

                // Erstelle den Geistertower, falls er nicht existiert
                if (ghostTower == null)
                {
                    ghostTower = new Image
                    {
                        Source = draggedImage.Source,
                        Width = selectedTower.Size,
                        Height = selectedTower.Size,
                        Opacity = 0.5,
                        IsHitTestVisible = false,
                        Tag = selectedTower
                    };
                    GameField.Children.Add(ghostTower);
                }

                // Aktualisiere die aktuelle Mausposition
                _currentMousePosition = e.GetPosition(GameField);
            }
        }

        private void UpdateGhostTowerColor(bool isValid)
        {
            if (ghostTower != null)
            {
                ghostTower.Opacity = 0.5;
                ghostTower.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = isValid ? Colors.Green : Colors.Red, // Grün für gültig, Rot für ungültig
                    BlurRadius = 10,
                    ShadowDepth = 0,
                    Opacity = 1
                };
            }
        }

        public void SetTowerImage(BaseTower tower, Point Position)
        {
            Image towerImage = tower.GetEntityPic();
            towerImage.Width = tower.Size;
            towerImage.Height = tower.Size;

            Canvas.SetLeft(towerImage, Position.X - (towerImage.Width / 2));
            Canvas.SetTop(towerImage, Position.Y - towerImage.Height);

            towerImage.Tag = tower;
            towerImage.MouseEnter += TowerImage_MouseEnter;
            towerImage.MouseLeave += TowerImage_MouseLeave;
            towerImage.MouseDown += GameField_MouseDown;
            tower.Image = towerImage;
            GameField.Children.Add(towerImage);
        }

        private void GameField_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ghostTower != null && ghostTower.Tag is BaseTower tower && cash >= tower.Costs)
            {
                Point dropPosition = e.GetPosition(GameField);

                if (!tower.IsPositionValid(dropPosition, _deployedTowers, _rectangles))
                {
                    GameField.Children.Remove(ghostTower);
                    ghostTower = null;
                    RemoveRangeIndicator();
                    _dragTimer?.Stop();
                    return;
                }

                BaseTower newTower = TowerFactory.CreateTower(ghostTower.Tag.GetType().Name, dropPosition);

                SetTowerImage(newTower, dropPosition);

                _deployedTowers.Add(newTower);
                _towerGrid.AddObject(newTower); // Füge den Turm dem Spatial Grid hinzu
                newTower.StartAttackTimer(GameField, _enemyGrid); // Übergebe das Spatial Grid

                var cashchange = -1 * tower.Costs;
                Cashhandler(cashchange);

                GameField.Children.Remove(ghostTower);
                ghostTower = null;
                RemoveRangeIndicator();
            }
            else
            {
                if (ghostTower != null)
                {
                    GameField.Children.Remove(ghostTower);
                    ghostTower = null;
                    RemoveRangeIndicator();
                    _dragTimer?.Stop();
                }
            }
        }

        private void GameField_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ghostTower != null)
            {
                GameField.Children.Remove(ghostTower);
                ghostTower = null;
                RemoveRangeIndicator();
                _dragTimer?.Stop();
            }
        }

        private void TowerImage_MouseLeave(object sender, MouseEventArgs e)
        {
            RemoveRangeIndicator();
        }

        private void TowerImage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Image Image && Image.Tag is BaseTower selectedTower)
            {
                ShowTowerRange(selectedTower);
            }
        }

        private void GameField_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Image Image && Image.Tag is BaseTower selectedTower)
            {
                Point clickPosition = e.GetPosition(GameField);

                // Zeige die Reichweite des Turms
                ShowTowerRange(selectedTower);

                // Upgrade-Menü oder direkte Upgrade-Option
                Towerwindow upgradeWindow = new Towerwindow(selectedTower);
                upgradeWindow.ShowDialog();

                if (upgradeWindow.IsUpgraded)
                {
                    RemoveTowerImage(selectedTower);
                }
            }
        }

        private void ShowTowerRange(BaseTower tower)
        {
            RemoveRangeIndicator();

            _rangeIndicator = new Ellipse
            {
                Width = tower.AttackRange * 2,
                Height = tower.AttackRange * 2,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Opacity = 0.5,
                IsHitTestVisible = false
            };

            Canvas.SetLeft(_rangeIndicator, tower.Position.X - tower.AttackRange);
            Canvas.SetTop(_rangeIndicator, tower.Position.Y - tower.AttackRange);

            GameField.Children.Add(_rangeIndicator);
        }

        private void RemoveRangeIndicator()
        {
            if (_rangeIndicator != null)
            {
                GameField.Children.Remove(_rangeIndicator);
                _rangeIndicator = null;
            }
        }

        public void Cashhandler(int change)
        {
            cash += change;
            Cashbar.Content = Convert.ToString(cash);
            ShowCashChange(change);
            UpdateTowerMenuState(); // Aktualisiere den Zustand des Menü

        }

        private void GameField_MouseMove(object sender, MouseEventArgs e)
        {
            if (ghostTower != null)
            {
                _currentMousePosition = e.GetPosition(GameField);
            }
        }

        public void RemoveEnemy(Enemies enemy)
        {
            if (_enemyList.Contains(enemy))
            {
                enemy.storyboard.Stop();
                _enemyList.Remove(enemy);
                if (enemy.Life <= 0)
                {
                    var cashchange = enemy.Coins;
                    Cashhandler(cashchange);
                }
                else
                {
                    var _Health = -1 * enemy.Life;
                    ShowHealthChange(_Health);
                }
                // Entferne den Gegner aus dem Spatial Grid
                _enemyGrid.RemoveObject(enemy);
            }

            if (GameField.Children.Contains(enemy.Image))
            {
                GameField.Children.Remove(enemy.Image);
            }
        }

        private void speedo_Click(object sender, RoutedEventArgs e)
        {
            if (_waveSpawnInterval == 3500)
            {
                _waveSpawnInterval = _waveSpawnInterval * 2;
                _enemySpawnInterval = _enemySpawnInterval * 2;
            }
            else
            {
                _waveSpawnInterval = _waveSpawnInterval / 2;
                _enemySpawnInterval = _enemySpawnInterval / 2;
            }
        }

        private void DragTimer_Tick(object? sender, EventArgs e)
        {
            if (ghostTower != null && ghostTower.Tag is BaseTower selectedTower)
            {
                // Aktualisiere die Position des Geistertowers
                Canvas.SetLeft(ghostTower, _currentMousePosition.X - (ghostTower.Width / 2));
                Canvas.SetTop(ghostTower, _currentMousePosition.Y - ghostTower.Height);

                // Überprüfe die Gültigkeit der Position
                bool isValid = selectedTower.IsPositionValid(_currentMousePosition, _deployedTowers, _rectangles);

                // Aktualisiere die visuelle Darstellung (Farbe)
                UpdateGhostTowerColor(isValid);

                // Zeige die Reichweite des Geistertowers an
                UpdateRangeIndicator(selectedTower, _currentMousePosition);
            }
        }
        private void UpdateRangeIndicator(BaseTower tower, Point position)
        {
            // Wenn der Reichweitenindikator noch nicht existiert, erstelle ihn
            if (_rangeIndicator == null)
            {
                _rangeIndicator = new Ellipse
                {
                    Width = tower.AttackRange * 2, // Durchmesser = 2 * AttackRange
                    Height = tower.AttackRange * 2, // Durchmesser = 2 * AttackRange
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Opacity = 0.5,
                    IsHitTestVisible = false
                };
                GameField.Children.Add(_rangeIndicator);
            }

            // Aktualisiere die Position des Indikators
            double indicatorLeft = position.X - tower.AttackRange;
            double indicatorTop = position.Y - tower.AttackRange;

            Canvas.SetLeft(_rangeIndicator, indicatorLeft);
            Canvas.SetTop(_rangeIndicator, indicatorTop);
        }
        private void UpdateTowerMenuState()
        {
            foreach (StackPanel towerPanel in TowerMenu.Children)
            {
                if (towerPanel.Children[0] is Image towerImage && towerImage.Tag is BaseTower tower)
                {
                    if (cash < tower.Costs)
                    {
                        // Deaktivieren, wenn nicht genug Geld
                        towerImage.Opacity = 0.5; // Setze den Turm halbtransparent
                        towerImage.IsEnabled = false; // Deaktiviere die Interaktion
                        (towerPanel.Children[1] as TextBlock).Foreground = Brushes.Red; // Preis rot
                    }
                    else
                    {
                        // Aktivieren, wenn genug Geld
                        towerImage.Opacity = 1.0;
                        towerImage.IsEnabled = true;
                        (towerPanel.Children[1] as TextBlock).Foreground = Brushes.Black; // Preis schwarz
                    }
                }
            }
        }

        public void RemoveTowerImage(BaseTower tower)
        {
            Dispatcher.Invoke(() =>
            {
                Image? imageToRemove = null;
                foreach (var child in GameField.Children)
                {
                    if (child is Image img && img.Tag == tower)
                    {
                        imageToRemove = img;
                        break;
                    }
                }

                if (imageToRemove != null)
                {
                    GameField.Children.Remove(imageToRemove);
                }
            });
        }

        public void SellTower(BaseTower tower)
        {
            if (_deployedTowers.Contains(tower))
            {
                _deployedTowers.Remove(tower);
                if(tower._cooldownTimer != null)
                    tower._cooldownTimer.Stop();
                tower.StopAttackTimer();
                var cashchange = tower.TowerWorth / 2;
                Cashhandler(cashchange);
            }

            RemoveTowerImage(tower);
        }

        private void ShowHealthChange(double change)
        {
            if (change < 0)
            {
                healthChange.Foreground = Brushes.Red;
                healthChange.Content = $"{change}";
            }
            else
            {
                healthChange.Foreground = Brushes.Green;
                healthChange.Content = $"+{change}";
            }

            _Health += change;
            healthChange.Visibility = Visibility.Visible;

            // Animation: Nach 1 Sekunde ausblenden
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (sender, args) =>
            {
                healthChange.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
        }

        // Methode zur Anzeige von Geldänderungen
        private void ShowCashChange(int change)
        {
            if (change < 0)
            {
                cashChange.Foreground = Brushes.Red;
                cashChange.Content = $"{change}";
            }
            else
            {
                cashChange.Foreground = Brushes.Green;
                cashChange.Content = $"+{change}";
            }

            cashChange.Visibility = Visibility.Visible;

            // Animation: Nach 1 Sekunde ausblenden
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (sender, args) =>
            {
                cashChange.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
        }

        public void ResumeTowerLogic()
        {
            foreach (var tower in _deployedTowers)
            {
                tower.CooldownTime = 100000;
                tower.StartAttackTimer(GameField, _enemyGrid);
            }
        }

        public void PauseTowerLogic()
        {
            foreach (var tower in _deployedTowers)
            {
                tower.CooldownTime = 100000;
                tower.StartCooldown();
            }
        }

        public void ResumeEnemyAnimations()
        {
            foreach (var enemy in _enemyList)
            {
                if (enemy.storyboard != null)
                {
                    enemy.storyboard.Resume();
                }
            }
        }

        public void PauseEnemyAnimations()
        {
            foreach (var enemy in _enemyList)
            {
                if (enemy.storyboard != null)
                {
                    enemy.storyboard.Pause();
                }
            }
        }

        public void PauseProjectiles()
        {
            foreach (var tower in _deployedTowers)
            {
                foreach (var projectile in tower.ActiveProjectiles)
                {
                    projectile.PauseAnimation();
                }
            }
        }

        public void ResumeProjectiles()
        {
            foreach (var tower in _deployedTowers)
            {
                foreach (var projectile in tower.ActiveProjectiles)
                {
                    projectile.ResumeAnimation();
                }
            }
        }

        public void IngameMusic_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Ingame-Musik in einer Schleife abspielen
            IngameMusic.Position = TimeSpan.Zero;
            IngameMusic.Play();
        }
    }
}
