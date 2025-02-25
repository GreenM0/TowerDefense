using System.Windows;
using System.Windows.Media;

namespace TowerDefense.Helper
{
    public static class PathGeometryExtensions
    {
        public static double GetTotalLength(this PathGeometry path)
        {
            double totalLength = 0;
            foreach (var figure in path.Figures)
            {
                Point start = figure.StartPoint;
                foreach (var segment in figure.Segments)
                {
                    if (segment is LineSegment lineSegment)
                    {
                        totalLength += (lineSegment.Point - start).Length;
                        start = lineSegment.Point;
                    }
                    // Füge hier weitere Segmenttypen hinzu, falls nötig
                }
            }
            return totalLength;
        }
    }
}
