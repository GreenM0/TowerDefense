using System;
using System.Collections.Generic;
using System.Windows;
using TowerDefense.Grid;
using TowerDefense.EnemiesModel;
using TowerDefense.Projectils;
using System.Windows.Controls;

namespace TowerDefense.Towers
{
    public class TestTower1 : BaseTower
    {
        public TestTower1(Point position)
            : base(
                attackRange: 220,
                attackDamage: 3,
                position: position,
                costs: 200,
                size: 100,
                projectileimagePath: @"..\..\..\Projectils\Types\Assets\IceBall.png",
                projectilespeed: 800,
                towerName: "TestTower",
                pathtoImage: @"..\..\..\Towers\Assets\icetower.png",
                towerRadius: 100,
                attackspeed: 1
            )
        {

        }
    }
}