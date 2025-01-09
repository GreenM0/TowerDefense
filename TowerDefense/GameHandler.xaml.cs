using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using TowerDefense.EnemiesModel;
using TowerDefense.EnemiesModel.Types;
using TowerDefense.Grid;
using TowerDefense.Maps;
using TowerDefense.Towers;

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
        private int cash;
        private SpatialGrid<BaseTower> _towerGrid = new SpatialGrid<BaseTower>(100);
        private SpatialGrid<Enemies> _enemyGrid = new SpatialGrid<Enemies>(100);
        private List<Line> _lineList = new List<Line>();
        private Image? ghostTower;
        private Ellipse? _rangeIndicator; // Anzeige für die Angriffsreichweite
        public static GameHandler Instance { get; private set; }

        public GameHandler()
        {
            InitializeComponent();

            InitializeMap();
            LoadTowers();
            DisplayTowerMenu();
            InitializeSpawner();
            //InitializeGridHandler();
            //InitializeTowerHandler();
            Cashhandler();
            Instance = this;
        }

        private void InitializeMap()
        {
            Map1 Map1 = new Map1();
            GameField.Children.Add(Map1);

            _mainCanvas = Map1.MainCanvas;
            _gameWay = Map1.Way();
            cash = 1000;
        }

        //private void InitializeGridHandler()
        //{
        //    _gridHandler = new DispatcherTimer();
        //    _gridHandler.Interval = TimeSpan.FromSeconds(0.000001);
        //    _gridHandler.Tick += GridHandlerTick;
        //    _gridHandler.Start();
        //}

        //private void InitializeTowerHandler()
        //{
        //    _towerHandler = new DispatcherTimer();
        //    _towerHandler.Interval = TimeSpan.FromSeconds(0.000001);
        //    _towerHandler.Tick += TowerHandlerTick;
        //    _towerHandler.Start();
        //}

        private void InitializeSpawner()
        {
            _gameTick = new DispatcherTimer();
            _gameTick.Interval = TimeSpan.FromSeconds(1);
            _gameTick.Tick += GameTick;
            _gameTick.Start();
        }

        private void GameTick(object? sender, EventArgs e)
        {
            Goblin goblin = new Goblin();          
            _enemyGrid.AddObject(goblin);

            Image ImageControl = goblin.GetEntityPic();
            goblin.Image = ImageControl;
            _enemyList.Add(goblin);

            Canvas.SetLeft(ImageControl, _gameWay[0].X - ImageControl.Width / 2);
            Canvas.SetTop(ImageControl, _gameWay[0].Y - ImageControl.Height / 2);
            GameField.Children.Add(ImageControl);
            _ = goblin.Movement(_gameWay, _mainCanvas, ImageControl);
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

                Image towerImage = tower.GetEntityPic();
                towerImage.Width = tower.Size;
                towerImage.Height = tower.Size;

                Ellipse towerRadiusVisual = new Ellipse
                {
                    Width = tower.AttackRange,
                    Height = tower.AttackRange,
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
                newTower.StartAttackTimer(GameField);

                cash -= tower.Costs;
                Cashhandler();

                GameField.Children.Remove(ghostTower);
                ghostTower = null;

                RemoveRangeIndicator();
            }
        }

        //private void GridHandlerTick(object? sender, EventArgs e)
        //{
        //    foreach (var enemi in _enemyList)
        //    {
        //        var newposition = enemi.GetEnemyPosition();
        //        enemi.Position = newposition;
        //        _enemyGrid.UpdateObjectPosition(enemi, enemi.Position);
        //    }
        //}

        //private void TowerHandlerTick(object? sender, EventArgs e)
        //{
        //    //foreach (var tower in _deployedTowers)
        //    //{
        //    //}
        //}

        private void GameField_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ghostTower != null)
            {
                GameField.Children.Remove(ghostTower);
                ghostTower = null;
            }
        }

        private void GameField_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point clickPosition = e.GetPosition(GameField);

                foreach (var tower in _deployedTowers)
                {
                    double towerX = Canvas.GetLeft(tower.GetEntityPic()) + tower.Size / 2;
                    double towerY = Canvas.GetTop(tower.GetEntityPic()) + tower.Size / 2;
                    double distanceToClick = Math.Sqrt(Math.Pow(clickPosition.X - towerX, 2) + Math.Pow(clickPosition.Y - towerY, 2));

                    if (distanceToClick <= tower.Size / 2)
                    {
                        ShowTowerRange(tower);
                        return;
                    }
                }

                RemoveRangeIndicator();
            }
        }

        private void ShowTowerRange(BaseTower tower)
        {
            RemoveRangeIndicator();

            _rangeIndicator = new Ellipse
            {
                Width = tower.AttackRange * 2,
                Height = tower.AttackRange * 2,
                Stroke = Brushes.Red,
                StrokeThickness = 2,
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
        public void RemoveEnemy(Enemies enemy)
        {
            // Entferne den Gegner aus der Liste
            if (_enemyList.Contains(enemy))
            {
                _enemyList.Remove(enemy);
            }

            // Entferne das Bild des Gegners vom Canvas
            if (GameField.Children.Contains(enemy.Image))
            {
                GameField.Children.Remove(enemy.Image);
            }
        }
    }
}
