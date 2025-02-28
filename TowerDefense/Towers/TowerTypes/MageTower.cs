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
    public class MageTower : BaseTower
    {
        private Enemies currentTarget;
        private Point Positionoffset;
        private TimeSpan ShockDuration;
        private int ShockRadius;
        public MageTower(Point position)
            : base(
                attackRange: 250,
                attackDamage: 2,
                position: position,
                costs: 300,
                size: 100,
                projectileimagePath: @"..\..\..\Projectils\Types\Assets\Blitzball.png",
                towerName: "ArcherTower",
                pathtoImage: @"..\..\..\Towers\Assets\Mage.png",
                towerRadius: 80,
                attackspeed: 150,
                upgradeLevel: 1,
                maxUpgradeLevel: 3,
                upgradeCost: 100,
                towerWorth: 200,
                targetMode: "ALL",
                cooldownTime: 7
            )
        {
            Positionoffset = new Point(Position.X + 55, Position.Y - 60);
            ShockDuration = TimeSpan.FromSeconds(1);
            ShockRadius = 30;
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
            Blitzball pfeil1 = new(Positionoffset, currentTarget.Position, AttackSpeed, AttackDamage, GameHandler.Instance._enemyList, ProjectileimagePath, ShockDuration, ShockRadius);
            pfeil1.Shoot(gameCanvas, Positionoffset, currentTarget, AttackSpeed, (projectile) =>
            {
            });
        }

        public void UpdateTowerDirection()
        {
            bool isTargetOnRight = currentTarget.Position.X > this.Position.X;

            if (Image.RenderTransform is ScaleTransform flipTransform)
            {
                flipTransform.ScaleX = isTargetOnRight ? -1 : 1;
            }
            else
            {
                Positionoffset.X -= 40;
                flipTransform = new ScaleTransform(isTargetOnRight ? -1 : 1, 1);
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
                    PathtoImage = @"..\..\..\Towers\Assets\Mage2.png";
                    GameHandler.Instance.SetTowerImage(this, Position);

                    UpgradeLevel += 1;
                    AttackRange = 270;
                    TowerWorth = TowerWorth + UpgradeCost;
                    AttackSpeed = 150;
                    CooldownTime = 5;
                    ShockDuration = TimeSpan.FromSeconds(5);
                    ShockRadius = 80;

                }
                else if (UpgradeLevel == 2)
                {
                    PathtoImage = @"..\..\..\Towers\Assets\Mage3.png";
                    Image newTowerImage = GetEntityPic();
                    GameHandler.Instance.SetTowerImage(this, Position);

                    UpgradeLevel += 1;
                    AttackRange = 300;
                    TowerWorth = TowerWorth + UpgradeCost;
                    AttackSpeed = 200;
                    CooldownTime = 3;
                    ShockDuration = TimeSpan.FromSeconds(7);
                    ShockRadius = 150;
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