using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using TowerDefense.EnemiesModel;
using TowerDefense.EnemiesModel.Types;
using TowerDefense.Maps;
using TowerDefense.Towers;
using TowerDefense.Waves;

namespace TowerDefense
{
    public partial class GameHandler : UserControl
    {
        private DispatcherTimer? _gameTick;
        private DispatcherTimer? _gridHandler;
        private DispatcherTimer? _towerHandler;
        private Point[] _gameWay = new Point[3];
        private Canvas _mainCanvas = null!;
        public List<Enemies> _enemyList = new List<Enemies>();
        private List<BaseTower> _towers = new List<BaseTower>();
        private List<BaseTower> _deployedTowers = new List<BaseTower>();
        public List<Rectangle> _rectangles = new List<Rectangle>();
        private int cash;
        private Image? ghostTower;
        private bool _gameOver = false;
        private bool _allEnemiesSpawned = false;
        private Ellipse? _rangeIndicator; // Anzeige für die Angriffsreichweite
        private DispatcherTimer? _dragTimer; // Timer für Drag-and-Drop-Überprüfung
        private Point _currentMousePosition; // Aktuelle Mausposition
        public static GameHandler Instance { get; private set; }

        //Spieleinstellungen
        private int _Health = 50;
        private int _waveSpawnInterval = 4000; // Zeit in Millisekunden zwischen Waves
        private int _enemySpawnInterval = 750; // Zeit in Millisekunden zwischen Gegner-Spawns

        public GameHandler()
        {
            InitializeComponent();
            InitializeMap();
            LoadTowers();
            DisplayTowerMenu();
            InitializeSpawner();
            Cashhandler();
            Instance = this;
            _ = SpawnWavesAsync();
            _dragTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1) // Aktualisierungsintervall (50 ms)
            };
            _dragTimer.Tick += DragTimer_Tick;

        }

        private async Task SpawnWavesAsync()
        {
            Wave waves = new Wave();

            //Geht jede Wave durch
            for (int currentWave = 0; currentWave < waves.GetWaveCount(); currentWave++)
            {
                //Prüft ob der Spieler schon verloren hat
                if (_gameOver)
                    break;

				wave.Content = "Wave: " + (currentWave + 1) + "/80";
                
                //Geht für jede Wave die verschiedenen Enemies durch
                for (int currentEnemyType = 0; currentEnemyType < waves.GetTotalEnemyTypes(); currentEnemyType++)
                {
                    //Spawnt den entsprechenden Enemy der Wave
                    for (int EnemyAmount = waves.GetAmountOfEnemies(currentEnemyType, currentWave); EnemyAmount > 0; EnemyAmount--)
                    {
                        Enemies currentEnemy = waves.SpawnEnemy(currentEnemyType, GameField, _gameWay);
						GameField.Children.Add(currentEnemy.Image);
						_ = currentEnemy.Movement(_gameWay, _mainCanvas, currentEnemy.Image);
						_enemyList.Add(currentEnemy);

                        await Task.Delay(_enemySpawnInterval);
                    }
                }
				await Task.Delay(_waveSpawnInterval);

                //Alle 10 Waves die Geschwindigkeit erhöhen
                if (currentWave >= 10 && currentWave % 10 == 0)
                {
					int speed = (int)Math.Round(currentWave * 0.1);
					_waveSpawnInterval = _waveSpawnInterval / speed;
					_enemySpawnInterval = _enemySpawnInterval / speed;
				}
			}

            _allEnemiesSpawned = true;
        }

        private void InitializeMap()
        {
            Map1 Map1 = new Map1();
            GameField.Children.Add(Map1);

            _mainCanvas = Map1.MainCanvas;
            _gameWay = Map1.Way();
            _rectangles = Map1.Rectangles;
            cash = 460;
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

            //Leben abziehen
            foreach (var enemy in _enemyList)
            {
                if (enemy.ReachedEnd)
                {
                    _Health -= enemy.Life;
                    enemy.ReachedEnd = false;
                    enemy.Life = 0;
                }
            }

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

            //Spieler tot
            if (_Health < 0)
            {
                _gameTick.Stop();
                info.Content = "GAME OVER";
                info.Visibility = Visibility.Visible;
                health.Content = "0";
                _gameOver = true;
            }
        }

        private void LoadTowers()
        {
            _towers = new List<BaseTower>
            {
                new TestTower1(new Point(0, 0)),
            };
        }

        private void DisplayTowerMenu()
        {
            foreach (var tower in _towers)
            {
                // Bild des Turms erstellen
                Image towerImage = tower.GetEntityPic();
                towerImage.Width = 50;
                towerImage.Height = 50;
                towerImage.Tag = tower;

                towerImage.MouseMove += TowerImage_MouseMove;

                // Preistext erstellen
                TextBlock priceText = new TextBlock
                {
                    Text = $"{tower.Costs}$",
                    Foreground = Brushes.Black,
                    FontSize = 12,
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

        private void GameField_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ghostTower != null && ghostTower.Tag is BaseTower tower && cash >= tower.Costs)
            {
                Point dropPosition = e.GetPosition(GameField);

                // Überprüfe die Platzierung
                if (!tower.IsPositionValid(dropPosition, _deployedTowers, _rectangles))
                {
                    // Ungültige Platzierung: Entferne das Geisterbild und den Indikator
                    GameField.Children.Remove(ghostTower);
                    ghostTower = null;
                    RemoveRangeIndicator();
                    _dragTimer?.Stop(); // Dragging ebenfalls stoppen
                    return;
                }

                // Gültige Platzierung
                BaseTower newTower = TowerFactory.CreateTower("TestTower1", dropPosition);

                Image towerImage = tower.GetEntityPic();
                towerImage.Width = tower.Size;
                towerImage.Height = tower.Size;

                Canvas.SetLeft(towerImage, dropPosition.X - (towerImage.Width / 2));
                Canvas.SetTop(towerImage, dropPosition.Y - towerImage.Height);

                towerImage.Tag = newTower;
                towerImage.MouseEnter += TowerImage_MouseEnter;
                towerImage.MouseLeave += TowerImage_MouseLeave;
                GameField.Children.Add(towerImage);

                _deployedTowers.Add(newTower);
                newTower.StartAttackTimer(GameField);

                cash -= tower.Costs;
                Cashhandler();

                GameField.Children.Remove(ghostTower);
                ghostTower = null;
                RemoveRangeIndicator();
            }
            else
            {
                // Falls keine gültige Platzierung möglich ist, entferne das Geisterbild und den Indikator
                if (ghostTower != null)
                {
                    GameField.Children.Remove(ghostTower);
                    ghostTower = null;
                    RemoveRangeIndicator();
                    _dragTimer?.Stop(); // Dragging ebenfalls stoppen
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

                ShowTowerRange(selectedTower);
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

        private void Cashhandler()
        {
            Cashbar.Content = Convert.ToString(cash);
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
            // Entferne den Gegner aus der Liste
            if (_enemyList.Contains(enemy))
            {
                _enemyList.Remove(enemy);
                cash += enemy.Coins;
                Cashhandler();
            }

            // Entferne das Bild des Gegners vom Canvas
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
                speedo.Content = "speed x2";
            }
            else
            {
                _waveSpawnInterval = _waveSpawnInterval / 2;
                _enemySpawnInterval = _enemySpawnInterval / 2;
                speedo.Content = "speed";
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
    }
}
