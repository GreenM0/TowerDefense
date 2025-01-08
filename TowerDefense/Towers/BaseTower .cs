using TowerDefense.Grid;
using TowerDefense.EnemiesModel;
using TowerDefense.Projectils;
using System.Windows;
using TowerDefense.Helper;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TowerDefense.Towers
{
    public abstract class BaseTower : IPositionable
    {
        public float AttackRange { get; private set; }
        public float AttackSpeed { get; private set; }
        public Point Position { get; set; }
        public int AttackDamage { get; private set; }
        public int Costs { get; private set; }
        public float Size { get; private set; }
        public int ProjectileimageId { get; private set; }
        public int ProjectileSpeed { get; private set; }
        public string TowerName { get; private set; }
        public string PathtoImage { get; private set; }

        public BaseTower(float attackRange, int attackDamage, Point position, int costs, float size, int projectileimageId, int projectilespeed, string towerName, string pathtoImage)
        {
            AttackDamage = attackDamage;
            AttackRange = attackRange;
            AttackSpeed = costs;
            Position = position;
            Costs = costs;
            Size = size;
            ProjectileimageId = projectileimageId;
            ProjectileSpeed = projectilespeed;
            TowerName = towerName;
            PathtoImage = pathtoImage;
        }

        public void Attack(Enemies target)
        {
            if (target == null) return;

            Projectile projectile = new Projectile(Position, target.Position, ProjectileSpeed);
            //projectile.Animate(gameCanvas, (proj) =>
            //{
            //    target.GetHit(AttackDamage);
            //});
        }

        public void GetTarget(SpatialGrid<Enemies> grid, int cellSize)
        {
            var enemiesInRange = GetEnemiesInRange(grid, cellSize);

            Enemies closestEnemy = enemiesInRange[0];

            if (enemiesInRange.Count == 0)
            {
                Attack(closestEnemy);
            }

            foreach (var enemy in enemiesInRange)
            {
                double currentDistance = Point.Subtract(Position, enemy.Position).Length;
                double closestDistance = Point.Subtract(Position, closestEnemy.Position).Length;

                if (currentDistance < closestDistance)
                {
                    closestEnemy = enemy;
                }
            }

            Attack(closestEnemy);
        }

        public bool IsInRange(Enemies enemy)
        {
            double distance = Point.Subtract(Position, enemy.Position).Length;
            return distance <= AttackRange;
        }

        public List<Enemies> GetEnemiesInRange(SpatialGrid<Enemies> grid, int cellSize)
        {
            var enemiesInRange = new List<Enemies>();

            var cellsToCheck = GetCellsInRange(cellSize);

            foreach (var cell in cellsToCheck)
            {
                var enemiesInCell = grid.GetObjectsInCell(cell);
                if (enemiesInCell != null)
                {
                    foreach (var enemy in enemiesInRange)
                    {
                        if (IsInRange(enemy))
                        {
                            enemiesInRange.Add(enemy);
                        }
                    }
                }
            }
            return enemiesInRange;
        }

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
        public Image GetEntityPic()
        {
            string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathtoImage);

            ImageHelper imageHelper = new();
            return imageHelper.GetEntityPic(imagePath);
        }
        private double DistanceToLine(Point point, Line line)
        {
            double x1 = line.X1;
            double y1 = line.Y1;
            double x2 = line.X2;
            double y2 = line.Y2;

            double numerator = Math.Abs((y2 - y1) * point.X - (x2 - x1) * point.Y + x2 * y1 - y2 * x1);
            double denominator = Math.Sqrt(Math.Pow(y2 - y1, 2) + Math.Pow(x2 - x1, 2));

            return numerator / denominator;
        }
        //public bool IsPositionValid(Point dropPosition, double towerRadius, List<BaseTower> nearbyTowers, List<Line> lines)
        //{

        //    foreach (var line in lines)
        //    {
        //        if (DistanceToLine(dropPosition, line) + 40 < towerRadius || nearbyTowers != null && nearbyTowers.Any(t => Math.Sqrt(Math.Pow(t.Position.X - dropPosition.X, 2) + Math.Pow(t.Position.Y - dropPosition.Y, 2)) < towerRadius + ((BaseTower)t).Size))
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }
}
