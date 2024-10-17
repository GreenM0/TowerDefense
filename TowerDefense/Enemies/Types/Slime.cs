using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense.Enemies.Types
{
    internal class Slime : Enemies
    {
        public Slime() : base(speed: 4, life: 5, coins: 5)
        {

        }
    }
}
