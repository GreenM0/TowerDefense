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

namespace TowerDefense.Towers
{
    public class TestTower1 : BaseTower
    {
        public TimeSpan AttackDuration { get; set; }
        public double AttackSlowFactor { get; set; }
        public bool DoDamage { get; set; }
        public TestTower1(Point position)
            : base(
                attackRange: 150,
                attackDamage: 0.5,
                position: position,
                costs: 200,
                size: 100,
                projectileimagePath: @"..\..\..\Projectils\Types\Assets\IceBall.png",
                projectilespeed: 800,
                towerName: "TestTower",
                pathtoImage: @"..\..\..\Towers\Assets\icetower.png",
                towerRadius: 80,
                attackspeed: 1,
                upgradeLevel: 1,
                maxUpgradeLevel: 3,
                upgradeCost: 100,
                towerWorth: 200,
                targetMode: "ALL",
                cooldownTime: 1000
            )
        {
            AttackDuration = TimeSpan.FromSeconds(3);
            AttackSlowFactor = 0.5;
            DoDamage = false;
        }

        public override void Attack(List<Enemies> target, Canvas gameCanvas)
        {
            if (_isCooldownActive) return;

            foreach (Enemies enemies in target)
            {
                target[0].ApplySlowEffect(AttackSlowFactor, AttackDuration, DoDamage, AttackDamage);
            }
        }

        public override void UpgradeTower()
        {
            if (UpgradeLevel < MaxUpgradeLevel)
            {
                if (UpgradeLevel == 1)
                {
                    PathtoImage = @"..\..\..\Towers\Assets\icetower.png";
                    GameHandler.Instance.SetTowerImage(this, Position);

                    UpgradeLevel += 1;
                    AttackRange = 200;
                    AttackSlowFactor = 0.3;
                    TowerWorth = TowerWorth + UpgradeCost;
                    AttackDuration = TimeSpan.FromSeconds(5);
                    AttackSpeed = 2;
                    CooldownTime = 800;

                }
                else if (UpgradeLevel == 2)
                {
                    PathtoImage = @"..\..\..\Towers\Assets\icetower.png";
                    Image newTowerImage = GetEntityPic();
                    GameHandler.Instance.SetTowerImage(this, Position);

                    UpgradeLevel += 1;
                    AttackRange = 220;
                    AttackSlowFactor = 0.2;
                    TowerWorth = TowerWorth + UpgradeCost;
                    AttackDuration = TimeSpan.FromSeconds(7);
                    AttackSpeed = 3;
                    CooldownTime = 500;
                }
            }            
        }

    }
}