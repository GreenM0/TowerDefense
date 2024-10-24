using System.Drawing;
using System.Windows.Controls;

namespace TowerDefense.Maps
{
    public partial class TestMap : UserControl
    {
        public Canvas MainCanvas { get { return this.GameCanvas; } }

        private Point[] _way = new Point[3];
        private Point _startPoint { get; set; }
        private Point _endPoint { get; set; }
        private Point _corner1 { get; set; }

        public TestMap()
        {
            InitializeComponent();
            SetPoints();
        }

        private void SetPoints()
        {
            _startPoint = new Point
            {
                X = Convert.ToInt32(Line1.X1),
                Y = Convert.ToInt32(Line1.Y1)
            };

            _corner1 = new Point
            {
                X = Convert.ToInt32(Line2.X2),
                Y = Convert.ToInt32(Line2.Y2)
            };

            _endPoint = new Point
            {
                X = Convert.ToInt32(Line2.X2),
                Y = Convert.ToInt32(Line2.Y2)
            };

            _way[0] = _startPoint;
            _way[1] = _corner1;
            _way[2] = _endPoint;
        }

        public Point[] Way()
        {
            return _way;
        }
    }
}
