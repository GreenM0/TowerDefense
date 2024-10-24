﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Grid;
using TowerDefense.EnemiesModel;

namespace TowerDefense.Towers
{
    public abstract class BaseTower
    {
        public float AttackRange { get; private set; }
        public float AttackSpeed { get; private set; }
        public Point Position { get; set; }
        public int AttackDamage { get; private set; }
        public int Costs { get; private set; }
        public float Size { get; private set; }

        public BaseTower(float attackRange, int attackDamage, Point position, int costs, float size)
        {
            AttackDamage = attackDamage;
            AttackRange = attackRange;
            AttackSpeed = costs;
            Position = position;
            Costs = costs;
            Size = size;
        }

        public void Attack()
        {
        }
        //public Enemies GetTarget(List<Enemy> enemies)
        //{
        //}

        //public bool IsInRange(Enemy enemy)
        //{
        //    float distance = Point.Subtract(Position, enemy.Position).Length();
        //    return distance <= AttackRange;
        //}
        //public List<EnemiesInRange> EnemiesInRange(Spatial grid, int cellSize)
        //{
        //    var enemiesInRange = new List<Enemy>();

        //    var cellsToCheck = GetCellsInRange(cellSize);

        //    foreach (var cell in cellsToCheck)
        //    {
        //        var enemiesInCell = grid.GetEnemiesInCell(cell);
        //        if (enemiesInCell != null)
        //        {
        //            foreach (var enemy in enemiesInRange)
        //            {
        //                if (IsInRange(enemy))
        //                {
        //                    enemiesInRange.Add(enemy);
        //                }
        //            }
        //        }
        //    }
        //    return enemiesInRange;
        //}

        public List<(int, int)> GetCellsInRange(int cellSize)
        {
            var cellsInRange = new List<(int, int)>();

            (int centerX, int centerY) = ((int)(Position.X / cellSize), (int)(Position.Y / cellSize));

            int rangeInCells = (int)Math.Ceiling(AttackRange / cellSize);

            for (int x = -rangeInCells; x <= rangeInCells; x++)
            {
                for (int y = -rangeInCells; y <= rangeInCells; y++)
                {
                    cellsInRange.Add((centerX, centerY));
                }
            }
                
            return cellsInRange;
        }
    }   
}
