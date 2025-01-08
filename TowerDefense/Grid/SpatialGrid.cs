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

    public class SpatialGrid<T> where T : IPositionable
    {
        public int cellSize;
        private Dictionary<(int, int), List<T>> grid = new Dictionary<(int, int), List<T>>();

        public SpatialGrid(int cellSize)
        {
            this.cellSize = cellSize;
        }

        public void AddObject(T obj)
        {
            var cell = GetCell(obj.Position);
            if (!grid.ContainsKey(cell))
            {
                grid[cell] = new List<T>();
            }
            grid[cell].Add(obj);
        }

        public void RemoveObject(T obj)
        {
            var cell = GetCell(obj.Position);
            if (grid.ContainsKey(cell))
            {
                grid[cell].Remove(obj);
            }
        }

        public void UpdateObjectPosition(T obj, Point newPosition)
        {
            var oldCell = GetCell(obj.Position);
            var newCell = GetCell(newPosition);

            if (oldCell != newCell)
            {
                RemoveObject(obj);
                obj.Position = newPosition;
                AddObject(obj);
            }
            else
            {
                obj.Position = newPosition;
            }
        }

        public List<T> GetObjectsInCell((int, int) cell)
        {
            if (grid.ContainsKey(cell))
            {
                return grid[cell];
            }
            return new List<T>();
        }

        public (int, int) GetCell(Point position)
        {
            return ((int)(position.X / cellSize), (int)(position.Y / cellSize));
        }
    }
}
