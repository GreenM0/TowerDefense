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

        public GameHandler()
        {
            InitializeComponent();

            InitializeMap();
            LoadTowers();
            DisplayTowerMenu();
            InitializeSpawner();
            InitializeGridHandler();
        }

        private void InitializeMap()
        {
            TestMap TestMapUserControl = new TestMap();
            GameField.Children.Add(TestMapUserControl);

            _mainCanvas = TestMapUserControl.MainCanvas;
            _gameWay = TestMapUserControl.Way();
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

            Canvas.SetLeft(ImageControl, _gameWay[0].X);
            Canvas.SetTop(ImageControl, _gameWay[0].Y);
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
                Image draggedImage = sender as Image;
                BaseTower selectedTower = draggedImage.Tag as BaseTower;

                DataObject dataObject = new DataObject("Tower", selectedTower);
                DragDrop.DoDragDrop(draggedImage, dataObject, DragDropEffects.Copy);
            }
        }

        private void GameField_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Tower"))
            {
                BaseTower tower = e.Data.GetData("Tower") as BaseTower;

                Point dropPosition = e.GetPosition(GameField);

                Image towerImage = tower.GetEntityPic();
                towerImage.Width = tower.Size + 2000; 

                Canvas.SetLeft(towerImage, dropPosition.X - (towerImage.Width / 2));
                Canvas.SetTop(towerImage, dropPosition.Y - (towerImage.Height / 2));

                GameField.Children.Add(towerImage);
            }
        }
    }
}