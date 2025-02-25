using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            Rectangles = (new List<Rectangle> { Rectangle1, Rectangle2, Rectangle3, Rectangle4, Rectangle5, Rectangle6, Rectangle7, Rectangle8, Rectangle9, Rectangle10, Rectangle11 });
        }

        public PathGeometry GetPathGeometry()
        {
            return Path;
        }

        public Point[] Way()
        {
            return _way;
        }
    }
}