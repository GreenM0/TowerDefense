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

namespace TowerDefense.Towers
{
    public class ArcherTower : BaseTower
    {
        private Enemies currentTarget;
        private int ArrowCount;
        public ArcherTower(Point position)
            : base(
                attackRange: 250,
                attackDamage: 5,
                position: position,
                costs: 200,
                size: 100,
                projectileimagePath: @"..\..\..\Projectils\Types\Assets\Pfeil.png",
                projectilespeed: 800,
                towerName: "ArcherTower",
                pathtoImage: @"..\..\..\Towers\Assets\Archer.png",
                towerRadius: 80,
                attackspeed: 100,
                upgradeLevel: 1,
                maxUpgradeLevel: 3,
                upgradeCost: 100,
                towerWorth: 200,
                targetMode: "ALL",
                cooldownTime: 1000
            )
        {
            ArrowCount = 1;
        }

        public override void Attack(List<Enemies> target, Canvas gameCanvas)
        {
            // Überprüfe, ob das Ziel noch existiert und in Reichweite ist
            if (target == null || target.Count == 0 || !IsInRange(target[0]))
            {
                currentTarget = null; // Setze das aktuelle Ziel zurück
                return; // Beende die Methode, wenn kein gültiges Ziel vorhanden ist
            }

            UpdateTowerDirection();
            // Erstelle Pfeil-Image
            if (UpgradeLevel == 3)
            {
                Point SecondArrowstart = new Point();
                SecondArrowstart.X = Position.X;
                SecondArrowstart.Y = Position.Y - 500;
                Pfeil pfeil2 = new(SecondArrowstart, target[0].Position, AttackSpeed, AttackDamage, GameHandler.Instance._enemyList, ProjectileimagePath);
                pfeil2.Shoot(gameCanvas, Position, target[0], AttackSpeed, (projectile) =>
                {
                    target[0].GetHit(AttackDamage);
                });
            }
            Pfeil pfeil = new(Position, target[0].Position, AttackSpeed, AttackDamage, GameHandler.Instance._enemyList, ProjectileimagePath);
            pfeil.Shoot(gameCanvas, Position, target[0], AttackSpeed, (projectile) =>
            {
                target[0].GetHit(AttackDamage);
            });
        }

        public void UpdateTowerDirection()
        {
            // Überprüfe, ob das aktuelle Ziel noch existiert und in Reichweite ist
            if (currentTarget != null && !GameHandler.Instance._enemyList.Contains(currentTarget))
            {
                currentTarget = null; // Setze das aktuelle Ziel zurück, wenn der Gegner nicht mehr existiert
            }

            if (currentTarget != null)
            {
                // Berechne den Winkel zum Ziel
                double deltaX = currentTarget.Position.X - this.Position.X;
                double deltaY = currentTarget.Position.Y - this.Position.Y;
                double angle = Math.Atan2(deltaY, deltaX);

                // Umrechnung von Radians zu Grad
                double angleInDegrees = angle * (90.0 / Math.PI);

                // Begrenze den Winkel auf einen bestimmten Bereich (z. B. -90° bis 90°)
                double minAngle = -180.0; // Minimaler Winkel
                double maxAngle = 180.0;  // Maximaler Winkel
                angleInDegrees = Math.Max(minAngle, Math.Min(maxAngle, angleInDegrees));

                // Setze den Drehpunkt auf die Hand des Schützen
                Image.RenderTransformOrigin = new Point(0.5, 0.8); // Beispielwerte, anpassen je nach Grafik

                // Drehe den Turm
                RotateTransform rotateTransform = new RotateTransform(angleInDegrees);
                Image.RenderTransform = rotateTransform;
            }
        }

        public override void UpgradeTower()
        {
            if (UpgradeLevel < MaxUpgradeLevel)
            {
                if (UpgradeLevel == 1)
                {
                    PathtoImage = @"..\..\..\Towers\Assets\Archer.png";
                    GameHandler.Instance.SetTowerImage(this, Position);

                    UpgradeLevel += 1;
                    AttackRange = 270;
                    TowerWorth = TowerWorth + UpgradeCost;
                    AttackSpeed = 150;
                    CooldownTime = 800;

                }
                if (UpgradeLevel == 2)
                {
                    PathtoImage = @"..\..\..\Towers\Assets\Archer.png";
                    Image newTowerImage = GetEntityPic();
                    GameHandler.Instance.SetTowerImage(this, Position);

                    UpgradeLevel += 1;
                    AttackRange = 300;
                    TowerWorth = TowerWorth + UpgradeCost;
                    AttackSpeed = 200;
                    CooldownTime = 500;
                }
            }
        }

    }
}