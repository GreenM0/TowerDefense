using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TowerDefense.Towers.TowerTypes
{
    public class TestTower1 : BaseTower
    {
        public TestTower1(Point postion)
            : base(attackRange: 10, attackDamage: 2, position: postion, costs: 10, size: 5)
        {
                
        }
    }
}
