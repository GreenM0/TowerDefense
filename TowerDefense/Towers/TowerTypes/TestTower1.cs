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
using TowerDefense.Helper;

namespace TowerDefense.Towers
{
    public class TestTower1 : BaseTower
    {
        public TimeSpan AttackDuration { get; set; }
        public double AttackSlowFactor { get; set; }
        public bool DoDamage { get; set; }
        private int MaxTargets { get; set; }
        public TestTower1(Point position)
            : base(
                attackRange: 150,
                attackDamage: 0.8,
                position: position,
                costs: 200,
                size: 100,
                projectileimagePath: @"..\..\..\Projectils\Types\Assets\IceBall.png",
                towerName: "TestTower",
                pathtoImage: @"..\..\..\Towers\Assets\icetower.png",
                towerRadius: 80,
                attackspeed: 1,
                upgradeLevel: 1,
                maxUpgradeLevel: 3,
                upgradeCost: 100,
                towerWorth: 200,
                targetMode: "ALL",
                cooldownTime: 10
            )
        {
            AttackDuration = TimeSpan.FromSeconds(3);
            AttackSlowFactor = 0.5;
            DoDamage = false;
            MaxTargets = 5;
        }

        public override void Attack(List<Enemies> target, Canvas gameCanvas)
        {
            if (_isCooldownActive) return;

            // Begrenze die Anzahl der gleichzeitig verlangsamten Gegner
            int targetsApplied = 0;

            foreach (Enemies enemy in target)
            {
                if (targetsApplied >= MaxTargets) break;

                enemy.ApplySlowEffect(AttackSlowFactor, AttackDuration, DoDamage, AttackDamage);
                targetsApplied++;
            }
        }

        public override void UpgradeTower()
        {
            if (UpgradeLevel < MaxUpgradeLevel)
            {
                if (UpgradeLevel == 1)
                {
                    PathtoImage = @"..\..\..\Towers\Assets\icetower2.png";
                    GameHandler.Instance.SetTowerImage(this, Position);

                    UpgradeLevel += 1;
                    AttackRange = 200;
                    AttackSlowFactor = 0.7;
                    TowerWorth = TowerWorth + UpgradeCost;
                    AttackDuration = TimeSpan.FromSeconds(5);
                    AttackSpeed = 2;
                    CooldownTime = 6;
                    MaxTargets = 10;

                }
                else if (UpgradeLevel == 2)
                {
                    PathtoImage = @"..\..\..\Towers\Assets\icetower3.png";
                    Image newTowerImage = GetEntityPic();
                    GameHandler.Instance.SetTowerImage(this, Position);

                    UpgradeLevel += 1;
                    AttackRange = 220;
                    AttackSlowFactor = 0.5;
                    TowerWorth = TowerWorth + UpgradeCost;
                    AttackDuration = TimeSpan.FromSeconds(7);
                    AttackSpeed = 3;
                    CooldownTime = 4;
                    MaxTargets = 15;  
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