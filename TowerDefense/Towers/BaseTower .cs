using TowerDefense.Grid;
using TowerDefense.EnemiesModel;
using TowerDefense.Projectils;
using System.Windows;
using TowerDefense.Helper;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System;

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
        public string ProjectileimagePath { get; private set; }
        public int ProjectileSpeed { get; private set; }
        public string TowerName { get; private set; }
        public string PathtoImage { get; private set; }
        public double TowerRadius { get; private set; }
        private DispatcherTimer? _attackTimer;

        public BaseTower(float attackRange, int attackDamage, Point position,float attackspeed, int costs, float size, string projectileimagePath, int projectilespeed, string towerName, string pathtoImage, double towerRadius)
        {
            AttackDamage = attackDamage;
            AttackRange = attackRange;
            AttackSpeed = attackspeed;
            Position = position;
            Costs = costs;
            Size = size;
            ProjectileimagePath = projectileimagePath;
            ProjectileSpeed = projectilespeed;
            TowerName = towerName;
            PathtoImage = pathtoImage;
            TowerRadius = towerRadius;
        }

        public void StartAttackTimer(Canvas gameCanvas)
        {
            _attackTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(AttackSpeed) // AttackSpeed gibt die Angriffe pro Sekunde an
            };

            _attackTimer.Tick += (sender, e) =>
            {
                List<Enemies> enemiesInRange = new List<Enemies>();
                foreach (var enemy in GameHandler.Instance._enemyList)
                {
                    // Berechne, ob der Gegner im Angriffsradius ist
                    double distance = Math.Sqrt(Math.Pow(Position.X - enemy.Position.X, 2) + Math.Pow(Position.Y - enemy.Position.Y, 2));

                    if (distance <= AttackRange)
                    {
                        // Gegner angreifen
                        enemiesInRange.Add(enemy);
                    }
                }

                if (enemiesInRange.Count == 0) return;

                Enemies closestEnemy = enemiesInRange[0];

                foreach (var enemy in enemiesInRange)
                {
                    double currentDistance = Point.Subtract(Position, enemy.Position).Length;
                    double closestDistance = Point.Subtract(Position, closestEnemy.Position).Length;

                    if (currentDistance < closestDistance)
                    {
                        closestEnemy = enemy;
                    }
                }

                Attack(closestEnemy, gameCanvas);
            };

            _attackTimer.Start();
        }

        public virtual void Attack(Enemies target, Canvas gameCanvas)
        {
            if (target == null || !IsInRange(target)) return;

            Vector targetVelocity = target.Velocity;  // Annahme: Velocity ist die Geschwindigkeit des Ziels

            // Berechne den Abstand zwischen Turm und Ziel
            double distance = Math.Sqrt(Math.Pow(Position.X - target.Position.X, 2) + Math.Pow(Position.Y - target.Position.Y, 2));

            // Berechne die Zeit, die das Projektil braucht, um das Ziel zu erreichen
            double timeToTarget = distance / ProjectileSpeed;

            // Berechne den Vorhersagepunkt des Ziels
            Point predictedTargetPosition = new Point(
                target.Position.X + targetVelocity.X * timeToTarget,
                target.Position.Y + targetVelocity.Y * timeToTarget
            );

            Projectile projectile = new Projectile(Position, target.Position, ProjectileSpeed, AttackDamage, ProjectileimagePath, target);

            projectile.Animate(gameCanvas, (proj) =>
            {
                proj.Hit();
            });
        }

        public bool IsInRange(Enemies enemy)
        {
            double distance = Point.Subtract(Position, enemy.Position).Length;
            return distance <= AttackRange;
        }

        //public List<Enemies> GetEnemiesInRange(SpatialGrid<Enemies> grid, int cellSize)
        //{
        //    var enemiesInRange = new List<Enemies>();

        //    var cellsToCheck = GetCellsInRange(cellSize);

        //    foreach (var cell in cellsToCheck)
        //    {
        //        var enemiesInCell = grid.GetObjectsInCell(cell);
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
        public bool IsPositionValid(Point dropPosition, List<BaseTower> depolyedTowers, List<Rectangle> gameWayBounds)
        {
            // Prüfen, ob der Turm zu nah an anderen Türmen platziert wird
            if (depolyedTowers != null)
            {
                foreach (var t in depolyedTowers)
                {
                    if (Math.Sqrt(Math.Pow(t.Position.X - dropPosition.X, 2) + Math.Pow(t.Position.Y - dropPosition.Y, 2)) < TowerRadius)
                    {
                        return false;
                    }
                }
            }

            // Prüfen, ob der Punkt innerhalb eines Rechtecks (Bounding Box des Wegs) liegt
            foreach (var rect in gameWayBounds)
            {
                if (IsPointInsideRectangle(dropPosition, rect))
                {
                    return false;
                }
            }

            return true;
        }

        // Hilfsmethode: Prüfen, ob ein Punkt innerhalb eines Rechtecks liegt
        private bool IsPointInsideRectangle(Point point, Rectangle rect)
        {
            // Berechnung der Grenzen des Rechtecks
            double rectLeft = Canvas.GetLeft(rect);
            double rectTop = Canvas.GetTop(rect);
            double rectRight = rectLeft + rect.Width;
            double rectBottom = rectTop + rect.Height;

            // Prüfen, ob der Punkt innerhalb der Grenzen liegt
            return (point.X >= rectLeft && point.X <= rectRight &&
                    point.Y >= rectTop && point.Y <= rectBottom);
        }

    }
}
