using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.EnemiesModel;

namespace TowerDefense.Grid
{
    public class SpatialGrid
    {
    //    private int cellSize;
    //    private Dictionary<(int, int), List<Enemies>> grid = new Dictionary<(int, int), List<Enemies>>();

    //    public SpatialGrid(int cellSize) 
    //    {
    //        this.cellSize = cellSize;
    //    }
    //    public void AddEnemy(Enemies enemy)
    //    {
    //        var cell = GetCell(enemy.Position);
    //        if (!grid.ContainsKey(cell))
    //        {
    //            grid[cell] = new List<Enemies>();
    //        }
    //        grid[cell].Add(enemy);
    //    }

    //    public void RemoveEnemy(Enemies enemy)
    //    {
    //        var cell = GetCell(enemy.Position);
    //        if (!grid.ContainsKey(cell))
    //        {
    //            grid[cell].Remove(enemy);
    //        }
    //    }

    //    public void UpdateEnemyPosition(Enemies enemy, Point newPosition)
    //    {
    //        var oldCell = GetCell(enemy.Position);
    //        var newCell = GetCell(newPosition);

    //        if (oldCell != newCell)
    //        {
    //            RemoveEnemy(enemy);
    //            enemy.Position = newPosition;
    //            AddEnemy(enemy);
    //        }
    //        else
    //        {
    //            enemy.Position = newPosition;
    //        }
    //    }
    //    public List<Enemies> GetEnemiesInCell((int, int) cell)
    //    {
    //        if (grid.ContainsKey(cell))
    //        {
    //            return grid[cell];
    //        }
    //        return null;
    //    }

    //    private (int, int) GetCell(Point postion)
    //    {
    //        return ((int)(postion.X / cellSize), (int)(postion.Y / cellSize));
    //    }
    }
}
