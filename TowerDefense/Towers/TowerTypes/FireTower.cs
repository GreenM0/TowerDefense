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
        public FireTower(Point position)
            : base(
                attackRange: 250,
                attackDamage: 2,
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
                targetMode: "ALL",
                cooldownTime: 1000
            )
        {
            AttackDuration = TimeSpan.FromSeconds(3);
            Positionoffset = new Point(Position.X + 40, Position.Y - 60);
        }

        public override void Attack(List<Enemies> target, Canvas gameCanvas)
        {
            // Überprüfe, ob das Ziel noch existiert und in Reichweite ist
            if (target == null || target.Count == 0 || !IsInRange(target[0]))
            {
                currentTarget = null; // Setze das aktuelle Ziel zurück
                return; // Beende die Methode, wenn kein gültiges Ziel vorhanden ist
            }
            else
            {
                currentTarget = target[0];
            }

            UpdateTowerDirection();
            Flamme flamme = new(Positionoffset, currentTarget.Position, AttackSpeed, AttackDamage, GameHandler.Instance._enemyList, ProjectileimagePath, AttackDuration);
            flamme.Shoot(gameCanvas, Positionoffset, currentTarget, AttackSpeed, (projectile) =>
            {
            });

            // Wenn der Turm auf Level 3 ist, schieße einen zweiten Pfeil
            if (UpgradeLevel == 3)
            {
                // Positioniere den zweiten Pfeil unterhalb des ersten Pfeils
                Point secondArrowStart = new Point(Positionoffset.X, Positionoffset.Y + 20); // 20 Einheiten unterhalb des Turms

                Pfeil pfeil2 = new(secondArrowStart, currentTarget.Position, AttackSpeed, AttackDamage, GameHandler.Instance._enemyList, ProjectileimagePath);
                pfeil2.Shoot(gameCanvas, secondArrowStart, currentTarget, AttackSpeed, (projectile) =>
                {
                });
            }
        }

        public void UpdateTowerDirection()
        {
            bool isTargetOnRight = currentTarget.Position.X > this.Position.X;

            if (Image.RenderTransform is ScaleTransform flipTransform)
            {
                flipTransform.ScaleX = isTargetOnRight ? 1 : -1;
            }
            else
            {
                Positionoffset.X -= 40;
                flipTransform = new ScaleTransform(isTargetOnRight ? 1 : -1, 1);
                Image.RenderTransform = flipTransform;
                Image.RenderTransformOrigin = new Point(0.5, 0.5);
            }
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
                    CooldownTime = 800;

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
                    CooldownTime = 500;
                }
            }
        }

        public override Image GetEntityPic()
        {
            string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathtoImage);

            ImageHelper imageHelper = new();
            return imageHelper.GetEntityPic(imagePath, 25, 35);
        }

    }
}