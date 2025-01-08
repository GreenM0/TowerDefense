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


namespace TowerDefense
{
    public partial class GameHandler : UserControl
    {
        private DispatcherTimer? _gameTick;
        private DispatcherTimer? _gridHandler;
        private Point[] _gameWay = new Point[3];
        private Canvas _mainCanvas = null!;
        private List<Enemies> _enemyList = new List<Enemies>();
        private List<BaseTower> _towers = new List<BaseTower>();
        private List<BaseTower> _deployedTowers = new List<BaseTower>();
        private int cash;
        private SpatialGrid<BaseTower> _towerGrid = new SpatialGrid<BaseTower>(100);
        private SpatialGrid<Enemies> _enemyGrid = new SpatialGrid<Enemies>(50);
        private List<Line> _lineList = new List<Line>();
        private Image? ghostTower;
        private int _Health = 10;

        public GameHandler()
        {
            InitializeComponent();

            InitializeMap();
            LoadTowers();
            DisplayTowerMenu();
            InitializeSpawner();
            InitializeGridHandler();
            Cashhandler();
        }

        private void InitializeMap()
        {
            Map1 Map1 = new Map1();
            GameField.Children.Add(Map1);

            _mainCanvas = Map1.MainCanvas;
            _gameWay = Map1.Way();
            cash = 1000;
        }

        private void InitializeGridHandler()
        {
            _gridHandler = new DispatcherTimer();
            _gridHandler.Interval = TimeSpan.FromSeconds(0.1);
            _gridHandler.Tick += GridHandlerTick;
            _gridHandler.Start();
        }

        private void InitializeSpawner()
        {
            _gameTick = new DispatcherTimer();
            _gameTick.Interval = TimeSpan.FromSeconds(1);
            _gameTick.Tick += GameTick;
            _gameTick.Start();
        }

        private void GameTick(object? sender, EventArgs e)
        {
            health.Content = _Health;

            SpawnEnemy();

            //Leben abziehen
            foreach (var enemy in _enemyList)
            {
                if (enemy.ReachedEnd)
                {
                    _Health -= enemy.Life;
                    enemy.ReachedEnd = false;
                }
            }

            //Spieler tot
            if (_Health < 0)
            {
                _gameTick.Stop();
                lost.Content = "GAME OVER";
                lost.Visibility = Visibility.Visible;
                health.Content = "0";
            }
        }

        private void SpawnEnemy()
        {
            Werwolf mage = new Werwolf();
            _enemyList.Add(mage);

            Image ImageControl = mage.GetEntityPic();

            Canvas.SetLeft(ImageControl, _gameWay[0].X - ImageControl.Width / 2);
            Canvas.SetTop(ImageControl, _gameWay[0].Y - ImageControl.Height / 2);
            GameField.Children.Add(ImageControl);
            _ = mage.Movement(_gameWay, _mainCanvas, ImageControl);
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
                Image towerImage = tower.GetEntityPic();
                towerImage.Width = 100;
                towerImage.Margin = new Thickness(10);
                towerImage.Tag = tower;

                towerImage.MouseMove += TowerImage_MouseMove;

                TowerMenu.Children.Add(towerImage);
            }
        }

        private void TowerImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Image draggedImage && draggedImage.Tag is BaseTower selectedTower)
            {
                if (ghostTower == null)
                {
                    // Erstelle das Geistermodell
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

                // Bewege das Geistermodell mit der Maus
                Point mousePosition = e.GetPosition(GameField);
                Canvas.SetLeft(ghostTower, mousePosition.X - (ghostTower.Width / 2));
                Canvas.SetTop(ghostTower, mousePosition.Y - (ghostTower.Height / 2));
            }
        }

        private void GameField_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ghostTower != null && ghostTower.Tag is BaseTower tower && cash >= tower.Costs)
            {
                Point dropPosition = e.GetPosition(GameField);

                if (!tower.IsPositionValid(dropPosition, _deployedTowers))
                {
                    return;
                }

                BaseTower newTower = TowerFactory.CreateTower("TestTower1", dropPosition);

                // Platziere den Turm
                Image towerImage = tower.GetEntityPic();
                towerImage.Width = tower.Size;
                towerImage.Height = tower.Size;

                Ellipse towerRadiusVisual = new Ellipse
                {
                    Width = tower.TowerRadius,
                    Height = tower.TowerRadius,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Opacity = 0.5,
                    IsHitTestVisible = false
                };

                Canvas.SetLeft(towerImage, dropPosition.X - (towerImage.Width / 2));
                Canvas.SetTop(towerImage, dropPosition.Y - (towerImage.Height / 2));

                Canvas.SetLeft(towerRadiusVisual, dropPosition.X - (towerRadiusVisual.Width / 2));
                Canvas.SetTop(towerRadiusVisual, dropPosition.Y - (towerRadiusVisual.Height / 2));

                GameField.Children.Add(towerImage);
                GameField.Children.Add(towerRadiusVisual);

                tower.Position = dropPosition;
                _towerGrid.AddObject(newTower);
                _deployedTowers.Add(newTower);

                cash -= tower.Costs;
                Cashhandler();

                // Entferne das Geistermodell
                GameField.Children.Remove(ghostTower);
                ghostTower = null;
            }
        }

        private void GridHandlerTick(object? sender, EventArgs e)
        {
            foreach (var tower in _deployedTowers)
            {
                var cell = _towerGrid.GetCell(tower.Position);
                var nearbyEnemies = _enemyGrid.GetObjectsInCell(cell);

                if (nearbyEnemies != null)
                {
                    foreach (var enemy in nearbyEnemies)
                    {
                        double distance = Math.Sqrt(Math.Pow(tower.Position.X - enemy.Position.X, 2) + Math.Pow(tower.Position.Y - enemy.Position.Y, 2));
                        if (distance <= tower.AttackRange)
                        {
                            tower.Attack(enemy);
                        }
                    }
                }
            }
        }

        private void GameField_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ghostTower != null)
            {
                GameField.Children.Remove(ghostTower);
                ghostTower = null;
            }
        }

        private void Cashhandler()
        {
            Cashbar.Text = Convert.ToString(cash);
        }

        private void GameField_MouseMove(object sender, MouseEventArgs e)
        {
            if (ghostTower != null)
            {
                Point mousePosition = e.GetPosition(GameField);
                Canvas.SetLeft(ghostTower, mousePosition.X - (ghostTower.Width / 2));
                Canvas.SetTop(ghostTower, mousePosition.Y - (ghostTower.Height / 2));
            }
        }
    }
}