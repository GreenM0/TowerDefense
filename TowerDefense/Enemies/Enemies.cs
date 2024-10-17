using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense.Enemies
{
    internal class Enemies
    {
        public int Speed { get; set; }
        public int Life { get; set; }
        public int Coins { get; set; }

        public Enemies(int speed, int life, int coins)
        {
            Speed = speed;
            Life = life;
            Coins = coins;
        }

        public void Movement()
        {

        }

        public void GetKilled()
        {

        }

    }
}
