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
        public TestTower1(Point position)
            : base(
                attackRange: 150,
                attackDamage: 5,
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
                towerWorth:  200,
                targetMode: "ALL"
            )
        {
            AttackDuration = TimeSpan.FromSeconds(3);
        }
        public override void Attack(List<Enemies> target, Canvas gameCanvas)
        {
            foreach (Enemies enemies in target)
            {
                target[0].ApplySlowEffect(AttackDamage, AttackDuration);
            }
        }
    }
}