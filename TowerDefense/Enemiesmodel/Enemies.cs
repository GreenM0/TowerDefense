using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace TowerDefense.EnemiesModel
{
    public class Enemies
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
        }

        public async Task Movement(Point[] _gameWay, Canvas _gameField, Image img)
        {
            for (int i = 0; i < _gameWay.Length - 1; i++)
            {
                Point startPoint = _gameWay[i];
                Point endPoint = _gameWay[i + 1];

                DoubleAnimation animationX = new DoubleAnimation
                {
                    From = Canvas.GetLeft(img),
                    To = endPoint.X,
                    Duration = TimeSpan.FromSeconds(2)
                };

                DoubleAnimation animationY = new DoubleAnimation
                {
                    From = Canvas.GetTop(img),
                    To = endPoint.Y,
                    Duration = TimeSpan.FromSeconds(2)
                };
                // Erstelle eine TaskCompletionSource für das Ende der Animation
                TaskCompletionSource<bool> tcsX = new TaskCompletionSource<bool>();
                TaskCompletionSource<bool> tcsY = new TaskCompletionSource<bool>();

                // Event-Handler für das Ende der X-Animation
                animationX.Completed += (s, e) => tcsX.SetResult(true);
                // Event-Handler für das Ende der Y-Animation
                animationY.Completed += (s, e) => tcsY.SetResult(true);

                img.BeginAnimation(Canvas.LeftProperty, animationX);
                img.BeginAnimation(Canvas.TopProperty, animationY);

                await Task.WhenAll(tcsX.Task, tcsY.Task);
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

        public void GetKilled()
        {
            //GameHandler.AddCoins(Coins);
        }
    }
}
