using System.Windows;
using System.Windows.Media.Animation;

namespace TowerDefense.Enemies
{
    internal class Enemies
    {
        public int Speed { get; set; }
        public int Life { get; set; }
        public int Coins { get; set; }
        public Point Position { get; set; }

        public Enemies(int speed, int life, int coins)
        {
            Speed = speed;
            Life = life;
            Coins = coins;

            Movement();
        }

        public void Movement()
        {
            for(int i = 1; i < GameHandler.CornerPoints; i++)
            {
                moveAnimation(Position, GameHandler.CornerPoints[i]);
                Position = GameHandler.CornerPoints[i];
            }
        }

        public void GetHit(int damage)
        {
            Life -= damage;

            if(Life <= 0)
            {
                GetKilled();
            }
        }

        public void MoveAnimation(Point PositionA, Point PosotionB)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = PositionA,
                To = PosotionB,
                Duration = 2
            };
        }

        public void GetKilled()
        {
            GameHandler.AddCoins(Coins);
        }
    }
}
