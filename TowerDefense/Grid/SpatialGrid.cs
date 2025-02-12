using System;
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
            obj.CurrentCell = cell; // Speichere die aktuelle Zelle

            if (!grid.ContainsKey(cell))
            {
                grid[cell] = new List<T>();
            }
            grid[cell].Add(obj);
        }

        public List<T> GetObjectsInRange(Point position, float range)
        {
            var objectsInRange = new List<T>();

            // Bestimme die Zelle des Turms
            (int centerX, int centerY) = GetCell(position);

            // Berechne, wie viele Zellen im Umkreis des Turms liegen
            int rangeInCells = (int)Math.Ceiling(range / cellSize);

            // Iteriere über alle Zellen im Umkreis
            for (int x = -rangeInCells; x <= rangeInCells; x++)
            {
                for (int y = -rangeInCells; y <= rangeInCells; y++)
                {
                    int neighborX = centerX + x;
                    int neighborY = centerY + y;

                    var cell = (neighborX, neighborY);
                    if (grid.ContainsKey(cell))
                    {
                        objectsInRange.AddRange(grid[cell]);
                    }
                }
            }
            
            return objectsInRange.Where(obj => Math.Sqrt(Math.Pow(obj.Position.X - position.X, 2) + Math.Pow(obj.Position.Y - position.Y, 2)) <= range).ToList();
            
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
            var newCell = GetCell(newPosition);

            // Überprüfe, ob sich die Zelle geändert hat
            if (obj.CurrentCell != newCell)
            {
                // Entferne das Objekt aus der alten Zelle
                if (grid.ContainsKey(obj.CurrentCell))
                {
                    grid[obj.CurrentCell].Remove(obj);
                }

                // Füge das Objekt der neuen Zelle hinzu
                if (!grid.ContainsKey(newCell))
                {
                    grid[newCell] = new List<T>();
                }
                grid[newCell].Add(obj);

                // Aktualisiere die gespeicherte Zelle des Objekts
                obj.CurrentCell = newCell;
            }

            // Aktualisiere die Position des Objekts
            obj.Position = newPosition;
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
