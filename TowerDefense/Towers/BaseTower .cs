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
using System.Windows.Media;

namespace TowerDefense.Towers
{
    public abstract class BaseTower : IPositionable
    {
        public double AttackRange { get; protected set; }
        public Image Image { get; set; }
        public float AttackSpeed { get; protected set; }
        public Point Position { get; set; }
        public (int, int) CurrentCell { get; set; }
        public double AttackDamage { get; protected set; }
        public int Costs { get; protected set; }
        public float Size { get; protected set; }
        public string ProjectileimagePath { get; protected set; }
        public int ProjectileSpeed { get; protected set; }
        public string TowerName { get; protected set; }
        public string PathtoImage { get; protected set; }
        public double TowerRadius { get; protected set; }
        public int UpgradeLevel { get; protected set; }
        public int MaxUpgradeLevel { get; protected set; }
        public int UpgradeCost { get; protected set; }
        public int TowerWorth {  get; protected set; }
        public string TargetMode { get; set; }
        public int CooldownTime { get; protected set; }
        public Enemies? currentTarget { get; protected set; }

        private DispatcherTimer? _attackTimer;
        private DispatcherTimer? _cooldownTimer;  // Neu: Cooldown-Timer
        protected bool _isCooldownActive = false;  // Flag, um zu prüfen, ob der Cooldown läuft

        public BaseTower(float attackRange, double attackDamage, Point position,float attackspeed, int costs, float size, string projectileimagePath, int projectilespeed, string towerName, string pathtoImage, double towerRadius, int upgradeLevel, int maxUpgradeLevel, int upgradeCost, int towerWorth, string targetMode, int cooldownTime)
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
            UpgradeLevel = upgradeLevel;
            MaxUpgradeLevel = maxUpgradeLevel;
            UpgradeCost = upgradeCost;
            TowerWorth = towerWorth;
            TargetMode = targetMode;
            CooldownTime = cooldownTime;
        }
        private EventHandler _renderingHandler;

        public void StartAttackTimer(Canvas gameCanvas, SpatialGrid<Enemies> enemyGrid)
        {
            _renderingHandler = (s, e) =>
            {
                if (_isCooldownActive) return;

                var enemiesInRange = GetEnemiesInRange(enemyGrid);

                // Überprüfe, ob es gültige Ziele gibt
                if (enemiesInRange.Count == 0)
                {
                    currentTarget = null; // Setze das aktuelle Ziel zurück
                    return;
                }

                var targets = GetTarget(enemiesInRange);

                // Überprüfe, ob die Ziele noch existieren
                targets = targets.Where(target => GameHandler.Instance._enemyList.Contains(target)).ToList();

                if (targets.Count == 0)
                {
                    currentTarget = null; // Setze das aktuelle Ziel zurück
                    return;
                }

                Attack(targets, gameCanvas);

                StartCooldown();
            };

            CompositionTarget.Rendering += _renderingHandler;
        }

        public void StopAttackTimer()
        {
            if (_renderingHandler != null)
            {
                CompositionTarget.Rendering -= _renderingHandler;
                _renderingHandler = null;
            }
        }

        public void StartCooldown()
        {
            if (_isCooldownActive) return;  // Wenn der Cooldown bereits läuft, nichts tun

            _isCooldownActive = true;

            // Cooldown-Timer
            _cooldownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(CooldownTime)
            };

            _cooldownTimer.Tick += (s, e) =>
            {
                _cooldownTimer.Stop();
                _cooldownTimer = null;
                _isCooldownActive = false;  // Cooldown beendet
            };

            _cooldownTimer.Start();
        }

        public abstract void Attack(List<Enemies> target, Canvas gameCanvas);

        public bool IsInRange(Enemies enemy)
        {
            double distance = Point.Subtract(Position, enemy.Position).Length;
            return distance <= AttackRange;
        }

        public List<Enemies> GetEnemiesInRange(SpatialGrid<Enemies> grid)
        {
            var enemiesInRange = grid.GetObjectsInRange(Position, (float)AttackRange);

            // Filtere nur die Gegner, die tatsächlich in Reichweite sind
            return enemiesInRange.Where(enemy => IsInRange(enemy)).ToList();
        }

        public virtual Image GetEntityPic()
        {
            string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathtoImage);

            ImageHelper imageHelper = new();
            return imageHelper.GetEntityPic(imagePath);
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

        public virtual void UpgradeTower()
        {
            if (UpgradeLevel < MaxUpgradeLevel)
            {
                UpgradeLevel += 1;
                TowerWorth = TowerWorth + UpgradeCost;
            }
        }

        public List<Enemies> GetTarget(List<Enemies> enemiesInRange)
        {
            List<Enemies> targets = new List<Enemies>();

            if (enemiesInRange == null || enemiesInRange.Count == 0)
                return targets; // Keine Gegner in Reichweite

            switch (TargetMode)
            {
                case "CLOSE": // Nächster Gegner (geringste Entfernung zum Turm)
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
                        if (closestEnemy != null)
                        {
                            targets.Add(closestEnemy);
                            return targets;
                        }
                    break;
                case "STRONG": // Stärkster Gegner (höchstes Leben)
                    Enemies strongestEnemy = enemiesInRange[0];
                    double maxLife = strongestEnemy.Life;

                    foreach (var enemy in enemiesInRange)
                    {
                        if (enemy.Life > maxLife)
                        {
                            strongestEnemy = enemy;
                            maxLife = enemy.Life;
                        }
                    }
                    targets.Add(strongestEnemy);
                    break;

                case "NEAR_END": // Gegner, der am nächsten am Ziel ist (höchste Weglänge)
                    Enemies nearestToEndEnemy = enemiesInRange[0];
                    double maxPathLength = nearestToEndEnemy.length;

                    foreach (var enemy in enemiesInRange)
                    {
                        if (enemy.length > maxPathLength)
                        {
                            nearestToEndEnemy = enemy;
                            maxPathLength = enemy.length;
                        }
                    }
                    targets.Add(nearestToEndEnemy);
                    break;

                default: // Standard: Alle Gegner in Reichweite
                    targets = enemiesInRange;
                    break;
            }

            return targets;
        }
    }
}
