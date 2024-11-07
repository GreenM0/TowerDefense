using System;
using System.Collections.Generic;
using System.Windows;
using TowerDefense.Grid;
using TowerDefense.EnemiesModel;
using TowerDefense.Projectils;

namespace TowerDefense.Towers
{
    public class TestTower1 : BaseTower
    {
        public TestTower1(Point position)
            : base(
                attackRange: 100,
                attackDamage: 50, 
                position: position,
                costs: 200,
                size: 1.0f, 
                projectileimageId: 1,
                projectilespeed: 5,   
                towerName: "TestTower",
                pathtoImage: @"..\..\..\Towers\Assets\icetower.png"
            )
        {

        }
    } 
}