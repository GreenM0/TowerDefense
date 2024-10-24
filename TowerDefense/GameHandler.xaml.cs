using System.Drawing;
using System.Windows.Controls;
using System.Windows.Threading;
using TowerDefense.EnemiesModel.Types;
using TowerDefense.Maps;

namespace TowerDefense
{
    public partial class GameHandler : UserControl
    {
        private DispatcherTimer ?_gameTick;
        private Point[] _gameWay = new Point[3];
        private Canvas _mainCanvas = null!;

        public GameHandler()
        {
            InitializeComponent();

            InitializeMap();
            InitializeSpawner();
        }

        private void InitializeMap()
        {;
            TestMap TestMapUserControl = new TestMap();
            GameField.Children.Add(TestMapUserControl);

            _mainCanvas = TestMapUserControl.MainCanvas;
            _gameWay = TestMapUserControl.Way();
        }

        private void InitializeSpawner()
        {
            _gameTick = new DispatcherTimer();
            _gameTick.Interval = TimeSpan.FromMilliseconds(500);
            _gameTick.Tick += GameTick;
            _gameTick.Start();
        }

        private void GameTick(object? sender, EventArgs e)
        {
            Goblin goblin = new Goblin();
            Image ImageControl = goblin.GetEntityPic();

            Canvas.SetLeft(ImageControl, _gameWay[0].X);
            Canvas.SetTop(ImageControl, _gameWay[0].Y);
            GameField.Children.Add(ImageControl);
            Task movement = goblin.Movement(_gameWay, _mainCanvas, ImageControl);
        }
    }
}
