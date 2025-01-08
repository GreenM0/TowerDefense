using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TowerDefense.EnemiesModel;
using TowerDefense.EnemiesModel.Types;
using TowerDefense.Grid;
using TowerDefense.Maps;

namespace TowerDefense
{
    public partial class GameHandler : UserControl
    {
        private DispatcherTimer? _gameTick;
        private DispatcherTimer? _gridHandler;
        private Point[] _gameWay = new Point[3];
        private Canvas _mainCanvas = null!;
        private List<Enemies> _enemyList = new List<Enemies>();

        public GameHandler()
        {
            InitializeComponent();

            InitializeMap();
            InitializeSpawner();
            InitializeGridHandler();
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
    }
}