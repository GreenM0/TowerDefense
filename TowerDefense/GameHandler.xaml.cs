using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TowerDefense.EnemiesModel;
using TowerDefense.EnemiesModel.Types;
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
        private int _Health = 10;

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
    }
}