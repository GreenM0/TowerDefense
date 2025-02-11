using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace TowerDefense.Maps
{
    public partial class Map1 : UserControl
    {
        public Canvas MainCanvas { get { return this.GameCanvas; } }

        private Point[] _way = new Point[12];
        private Point _startPoint { get; set; }
        private Point _endPoint { get; set; }
        private Point _corner1 { get; set; }

        public List<Rectangle> Rectangles = new List<Rectangle>();
        
        public Map1()
        {
            InitializeComponent();
            SetPoints();
            Rectangles = (new List<Rectangle> { Rectangle1, Rectangle2, Rectangle3, Rectangle4, Rectangle5, Rectangle6, Rectangle7, Rectangle8, Rectangle9, Rectangle10, Rectangle11 });
        }

        private void SetPoints()
        {
            _startPoint = new Point
            {
                X = Convert.ToInt32(Line1.X1),
                Y = Convert.ToInt32(Line1.Y1)
            };

            _way[0] = _startPoint;

            _way[1] = new Point
            {
                X = Convert.ToInt32(Line1.X2),
                Y = Convert.ToInt32(Line1.Y2)
            };

            _way[2] = new Point
            {
                X = Convert.ToInt32(Line2.X2),
                Y = Convert.ToInt32(Line2.Y2)
            };

            _way[3] = new Point
            {
                X = Convert.ToInt32(Line3.X2),
                Y = Convert.ToInt32(Line3.Y2)
            };

            _way[4] = new Point
            {
                X = Convert.ToInt32(Line4.X2),
                Y = Convert.ToInt32(Line4.Y2)
            };

            _way[5] = new Point
            {
                X = Convert.ToInt32(Line5.X2),
                Y = Convert.ToInt32(Line5.Y2)
            };

            _way[6] = new Point
            {
                X = Convert.ToInt32(Line6.X2),
                Y = Convert.ToInt32(Line6.Y2)
            };

            _way[7] = new Point
            {
                X = Convert.ToInt32(Line7.X2),
                Y = Convert.ToInt32(Line7.Y2)
            };

            _way[8] = new Point
            {
                X = Convert.ToInt32(Line8.X2),
                Y = Convert.ToInt32(Line8.Y2)
            };

            _way[9] = new Point
            {
                X = Convert.ToInt32(Line9.X2),
                Y = Convert.ToInt32(Line9.Y2)
            };

            _way[10] = new Point
            {
                X = Convert.ToInt32(Line10.X2),
                Y = Convert.ToInt32(Line10.Y2)
            };

            _way[11] = new Point
            {
                X = Convert.ToInt32(Line11.X2),
                Y = Convert.ToInt32(Line11.Y2)
            };
        }

        public Point[] Way()
        {
            return _way;
        }
    }
}