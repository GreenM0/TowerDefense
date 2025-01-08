using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private Point[] _gameWay = new Point[3];
        private Canvas _mainCanvas = null!;
        private List<Enemies> _enemyList = new List<Enemies>();
        private List<BaseTower> _towers = new List<BaseTower>();
        private int cash;

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

        private void GridHandlerTick(object? sender, EventArgs e)
        {
            //SpatialGrid spatialGrid = new();
            //gridobject = spatialGrid.GetGrid(_mainCanvas, _enemyList);
        }

        private void GameTick(object? sender, EventArgs e)
        {
            Goblin goblin = new Goblin();
            _enemyList.Add(goblin);

            Image ImageControl = goblin.GetEntityPic();

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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Image draggedImage && draggedImage.Tag is BaseTower selectedTower)
                {
                    DataObject dataObject = new DataObject("Tower", selectedTower);
                    DragDrop.DoDragDrop(draggedImage, dataObject, DragDropEffects.Copy);
                }
            }
        }

        private void GameField_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Tower"))
            {
                if (e.Data.GetData("Tower") is BaseTower tower)
                {
                    Point dropPosition = e.GetPosition(GameField);

                    Image towerImage = tower.GetEntityPic();
                    towerImage.Width = tower.Size + 2000;

                    Canvas.SetLeft(towerImage, dropPosition.X - (towerImage.Width / 2));
                    Canvas.SetTop(towerImage, dropPosition.Y - (towerImage.Height / 2));

                    GameField.Children.Add(towerImage);
                    cash -= tower.Costs;
                }
            }
        }

        private void Cashhandler()
        {
            cash = 1000;
            Cashbar.Text = Convert.ToString(cash);
        }
    }
}