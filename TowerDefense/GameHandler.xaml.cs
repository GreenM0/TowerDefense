using System.Drawing;
using System.Windows.Controls;
using System.Windows.Threading;
using TowerDefense.Maps;

namespace TowerDefense
{
    public partial class GameHandler : UserControl
    {
        private DispatcherTimer ?_gameTick;
        private Point[] _gameWay = new Point[3];

        public GameHandler()
        {
            InitializeComponent();

            InitializeMap();
            InitializeSpawner();
        }

        private void InitializeMap()
        {;
            TestMap testMap = new TestMap();
            testMap.Width = GameField.Width;
            testMap.Height = GameField.Height;
            GameField.Children.Add(testMap);
            _gameWay = testMap.Way();
        }

        private void InitializeSpawner()
        {
            _gameTick = new DispatcherTimer();
            _gameTick.Interval = TimeSpan.FromMilliseconds(500);
            _gameTick.Tick += GameTick;
        }

        private void GameTick(object? sender, EventArgs e)
        {

        }
    }
}
