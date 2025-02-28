using System;
using System.Collections.Generic;
using System.Windows;
using TowerDefense.Grid;
using TowerDefense.EnemiesModel;
using TowerDefense.Projectils;
using System.Windows.Controls;
using System.Configuration;
using System.Windows.Media;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TowerDefense.Helper;

namespace TowerDefense.Towers
{
    public class FireTower : BaseTower
    {
        private Enemies currentTarget = null;
        private Point Positionoffset;   
        public TimeSpan AttackDuration { get; set; }
        private int FlameRadius; 
        public FireTower(Point position)
            : base(
                attackRange: 250,
                attackDamage: 1,
                position: position,
                costs: 200,
                size: 100,
                projectileimagePath: @"..\..\..\Projectils\Types\Assets\Flamme.png",
                projectilespeed: 800,
                towerName: "FireTower",
                pathtoImage: @"..\..\..\Towers\Assets\Firetower.png",
                towerRadius: 80,
                attackspeed: 100,
                upgradeLevel: 1,
                maxUpgradeLevel: 3,
                upgradeCost: 100,
                towerWorth: 200,
                targetMode: "CLOSE",
                cooldownTime: 5
            )
        {
            AttackDuration = TimeSpan.FromSeconds(3);
            Positionoffset = new Point(Position.X, Position.Y - 70);
            FlameRadius = 50; 
        }

        public override void Attack(List<Enemies> targets, Canvas gameCanvas)
        {
            // Überprüfe, ob das Ziel noch existiert und in Reichweite ist
            if (targets == null || targets.Count == 0)
            {
                currentTarget = null; // Setze das aktuelle Ziel zurück
                return; // Beende die Methode, wenn kein gültiges Ziel vorhanden ist
            }

            // Filtere nur Gegner, die in Reichweite sind und nicht brennen
            var validTargets = targets.Where(t => IsInRange(t) && !t._isBurning).ToList();

            if (validTargets.Count == 0)
            {
                currentTarget = null;
                return;
            }

            currentTarget = validTargets[0]; // Wähle das erste Ziel aus der Liste
            // Wenn kein gültiges Ziel gefunden wurde, beende die Methode
            if (currentTarget == null)
                return;

            // Angriffslogik (Flamme schießen)
            Flamme flamme = new(Positionoffset, currentTarget.Position, AttackSpeed, AttackDamage, GameHandler.Instance._enemyList, ProjectileimagePath, AttackDuration, FlameRadius);
            flamme.Shoot(gameCanvas, Positionoffset, currentTarget, AttackSpeed, (projectile) =>
            {
                // Callback nach dem Schießen (optional)
            });
        }

        public override void UpgradeTower()
        {
            if (UpgradeLevel < MaxUpgradeLevel)
            {
                if (UpgradeLevel == 1)
                {
                    PathtoImage = @"..\..\..\Towers\Assets\Firetower2.png";
                    ProjectileimagePath = @"..\..\..\Projectils\Types\Assets\Flamme.png";
                    GameHandler.Instance.SetTowerImage(this, Position);

                    UpgradeLevel += 1;
                    AttackRange = 270;
                    TowerWorth = TowerWorth + UpgradeCost;
                    AttackSpeed = 150;
                    CooldownTime = 3;
                    AttackDuration = TimeSpan.FromSeconds(4); 
                    AttackDamage = 2;
                    FlameRadius = 70;

                }
                else if (UpgradeLevel == 2)
                {
                    PathtoImage = @"..\..\..\Towers\Assets\Firetower3.png";
                    ProjectileimagePath = @"..\..\..\Projectils\Types\Assets\Flamme.png";
                    Image newTowerImage = GetEntityPic();
                    GameHandler.Instance.SetTowerImage(this, Position);

                    UpgradeLevel += 1;
                    AttackRange = 300;
                    TowerWorth = TowerWorth + UpgradeCost;
                    AttackSpeed = 200;
                    CooldownTime = 2;
                    AttackDuration = TimeSpan.FromSeconds(5);
                    AttackDamage = 3;
                    FlameRadius = 150;
                }
            }
        }

        public override Image GetEntityPic()
        {
            string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathtoImage);

            ImageHelper imageHelper = new();
            return imageHelper.GetEntityPic(imagePath, 120, 130);
        }

    }
}