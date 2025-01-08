using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace TowerDefense.EnemiesModel
{
    public class Enemies
    {
        public int Speed { get; set; }
        public int Life { get; set; }
        public int Coins { get; set; }
        public Point Position { get; set; }
        public Image Image { get; set; }
        private double _totalLength = 0;
        private double _lineLength = 0;
        private double _lineDuration = 0;

        public Enemies(int speed, int life, int coins)
        {
            Speed = speed;
            Life = life;
            Coins = coins;
        }

        public async Task Movement(Point[] _gameWay, Canvas _gameField, Image img)
        {
            Image = img;

            for (int i = 0; i < _gameWay.Length - 1; i++)
            {
                double dx = _gameWay[i + 1].X - _gameWay[i].X;
                double dy = _gameWay[i + 1].Y - _gameWay[i].Y;
                double segmentLength = Math.Sqrt(dx * dx + dy * dy);

                _totalLength += segmentLength;
            }

            for (int i = 0; i < _gameWay.Length - 1; i++)
            {
                Point startPoint = _gameWay[i];
                Point endPoint = _gameWay[i + 1];

                _lineLength = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));

                _lineDuration = (_lineLength / _totalLength) * this.Speed;

                DoubleAnimation animationX = new DoubleAnimation
                {
                    From = Canvas.GetLeft(Image),
                    To = endPoint.X - Image.Height / 2,
                    Duration = TimeSpan.FromSeconds(_lineDuration)
                };

                DoubleAnimation animationY = new DoubleAnimation
                {
                    From = Canvas.GetTop(Image),
                    To = endPoint.Y - Image.Height / 2,
                    Duration = TimeSpan.FromSeconds(_lineDuration)
                };

                // Erstelle eine TaskCompletionSource für das Ende der Animation
                TaskCompletionSource<bool> tcsX = new TaskCompletionSource<bool>();
                TaskCompletionSource<bool> tcsY = new TaskCompletionSource<bool>();

                // Event-Handler für das Ende der X-Animation
                animationX.Completed += (s, e) => tcsX.SetResult(true);
                // Event-Handler für das Ende der Y-Animation
                animationY.Completed += (s, e) => tcsY.SetResult(true);

                Image.BeginAnimation(Canvas.LeftProperty, animationX);
                Image.BeginAnimation(Canvas.TopProperty, animationY);

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

        public Point GetEnemyPosition()
        {
            Point currentPosition;
            currentPosition.X = Canvas.GetLeft(Image);
            currentPosition.Y = Canvas.GetTop(Image);

            return currentPosition;
        }
    }
}
