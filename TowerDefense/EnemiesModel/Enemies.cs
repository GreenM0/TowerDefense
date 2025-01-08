using System;
using System.Net.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TowerDefense.Grid;

namespace TowerDefense.EnemiesModel
{
    public class Enemies : IPositionable
    {
        public int Speed { get; set; }
        public int Life { get; set; }
        public int Coins { get; set; }
        public Point Position { get; set; }
        public Image Image { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public bool ReachedEnd { get; set; }
        private double _totalLength = 0;
        private double _lineLength = 0;
        private double _lineDuration = 0;
        private bool _movingRight = false;

        public Enemies(int speed, int life, int coins, int imagewidth = 0, int imageheight = 0)
        {
            Speed = speed;
            Life = life;
            Coins = coins;
            ImageWidth = imagewidth;
            ImageHeight = imageheight;
        }

        public async Task Movement(Point[] _gameWay, Canvas _gameField, Image img)
        {
            //Gesamtlänge vom Weg berechnen
            for (int i = 0; i < _gameWay.Length - 1; i++)
            {
                double dx = _gameWay[i + 1].X - _gameWay[i].X;
                double dy = _gameWay[i + 1].Y - _gameWay[i].Y;
                double segmentLength = Math.Sqrt(dx * dx + dy * dy);

                _totalLength += segmentLength;
            }

            //Bild von Punkt zu Punkt animieren
            for (int i = 0; i < _gameWay.Length - 1; i++)
            {
                Point startPoint = _gameWay[i];
                Point endPoint = _gameWay[i + 1];

                bool movingRight = endPoint.X >= startPoint.X;

                _lineLength = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
                _lineDuration = (_lineLength / _totalLength) * this.Speed;

                DoubleAnimation animationX = new DoubleAnimation
                {
                    From = Canvas.GetLeft(img),
                    To = endPoint.X - img.Height / 2,
                    Duration = TimeSpan.FromSeconds(_lineDuration)
                };

                DoubleAnimation animationY = new DoubleAnimation
                {
                    From = Canvas.GetTop(img),
                    To = endPoint.Y - img.Height / 2,
                    Duration = TimeSpan.FromSeconds(_lineDuration)
                };
                
                // Erstelle eine TaskCompletionSource für das Ende der Animation
                TaskCompletionSource<bool> tcsX = new TaskCompletionSource<bool>();
                TaskCompletionSource<bool> tcsY = new TaskCompletionSource<bool>();

                // Event-Handler für das Ende der Animationen
                animationX.Completed += (s, e) => tcsX.SetResult(true);
                animationY.Completed += (s, e) => tcsY.SetResult(true);

                img.BeginAnimation(Canvas.LeftProperty, animationX);
                img.BeginAnimation(Canvas.TopProperty, animationY);

                FlipImageDirection(img, movingRight);

                await Task.WhenAll(tcsX.Task, tcsY.Task);
            }

            if (Life > 0)
            {
                ReachedEnd = true;
            }
            else
            {
                ReachedEnd = false;
            }

            img.Visibility = Visibility.Collapsed;
        }

        private void FlipImageDirection(Image img, bool movingRight)
        {
            if (img.RenderTransform is ScaleTransform flipTransform)
            {
                flipTransform.ScaleX = movingRight ? 1 : -1;
            }
            else
            {
                flipTransform = new ScaleTransform(movingRight ? 1 : -1, 1);
                img.RenderTransform = flipTransform;
                img.RenderTransformOrigin = new Point(0.5, 0.5);
            }
        }

        public void GetHit(int damage)
        {
            Life -= damage;

            if (Life <= 0)
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
