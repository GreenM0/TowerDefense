using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace TowerDefense.Maps
{
    public partial class Map2 : UserControl
    {
        public Canvas MainCanvas { get { return this.GameCanvas; } }

        public List<Rectangle> Rectangles = new List<Rectangle>();

        public Map2()
        {
            InitializeComponent();
            Rectangles = new List<Rectangle>
            {
                Rectangle1, Rectangle2, Rectangle3, Rectangle4, Rectangle5,
                Rectangle6, Rectangle7, Rectangle8, Rectangle9, Rectangle10,
                Rectangle11, Rectangle12
            };
        }

        public PathGeometry GetPathGeometry()
        {
            return Path;
        }
    }
}