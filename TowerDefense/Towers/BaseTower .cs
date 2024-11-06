using TowerDefense.Grid;
using TowerDefense.EnemiesModel;
using TowerDefense.Projectils;
using System.Windows;

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
        public int ProjectileimageId {  get; private set; }
        public int ProjectileSpeed { get; private set; }

        public BaseTower(float attackRange, int attackDamage, Point position, int costs, float size, int projectileimageId, int projectilespeed)
        {
            AttackDamage = attackDamage;
            AttackRange = attackRange;
            AttackSpeed = costs;
            Position = position;
            Costs = costs;
            Size = size;
            ProjectileimageId = projectileimageId;
            ProjectileSpeed = projectilespeed;
        }

        public void Attack(Enemies target)
        {
            if (target == null) return;
            
            Projectile projectile = new Projectile(Position, target.Position, ProjectileSpeed);
            projectile.Animate(gameCanvas, (proj) =>
            {
                target.GetHit(AttackDamage);
            });
        }

        public void GetTarget(SpatialGrid grid, int cellSize)
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

        public List<Enemies> GetEnemiesInRange(SpatialGrid grid, int cellSize)
        {
            var enemiesInRange = new List<Enemies>();

            var cellsToCheck = GetCellsInRange(cellSize);

            foreach (var cell in cellsToCheck)
            {
                var enemiesInCell = grid.GetEnemiesInCell(cell);
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
    }   
}
