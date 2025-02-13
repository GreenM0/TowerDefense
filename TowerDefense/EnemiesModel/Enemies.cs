using System;
using System.Net.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TowerDefense.Grid;

namespace TowerDefense.EnemiesModel
{
    public class Enemies : IPositionable
    {
        public double BaseSpeed { get; set; }
        public double CurrentSpeed { get; set; }
        public double Life { get; set; }
        public int Coins { get; set; }
        public Point Position { get; set; }
        public (int, int) CurrentCell { get; set; }
        public Image Image { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public bool ReachedEnd { get; set; }
        private double _totalLength = 0;
        private double _lineLength = 0;
        private double _lineDuration = 0;
        private bool _movingRight = false;
        private Storyboard _currentStoryboard; // Speichert das aktuelle Storyboard
        private CancellationTokenSource _slowEffectCancellationTokenSource; // Für die Steuerung des Slow-Effekts
        public Vector Velocity { get; set; }
        public virtual Image? GetEntityPic() => null;

        public Enemies(int speed, int life, int coins, int imagewidth = 0, int imageheight = 0)
        {
            CurrentSpeed = speed;
            BaseSpeed = speed;
            Life = life;
            Coins = coins;
            ImageWidth = imagewidth;
            ImageHeight = imageheight;
            Velocity = new Vector(0, 0);
        }

        public async Task Movement(Point[] _gameWay, Canvas _gameField, Image img, SpatialGrid<Enemies> enemyGrid)
        {
            // Gesamtlänge vom Weg berechnen
            for (int i = 0; i < _gameWay.Length - 1; i++)
            {
                double dx = _gameWay[i + 1].X - _gameWay[i].X;
                double dy = _gameWay[i + 1].Y - _gameWay[i].Y;
                double segmentLength = Math.Sqrt(dx * dx + dy * dy);
                _totalLength += segmentLength;
            }

            // Bild von Punkt zu Punkt animieren
            for (int i = 0; i < _gameWay.Length - 1; i++)
            {
                Point startPoint = _gameWay[i];
                Point endPoint = _gameWay[i + 1];

                bool movingRight = endPoint.X >= startPoint.X;

                _lineLength = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
                _lineDuration = (_lineLength / _totalLength) * this.CurrentSpeed;

                // Erstelle ein Storyboard für die Animation
                Storyboard storyboard = new Storyboard();

                DoubleAnimation animationX = new DoubleAnimation
                {
                    From = Canvas.GetLeft(img),
                    To = endPoint.X - img.Width / 2,
                    Duration = TimeSpan.FromSeconds(_lineDuration)
                };

                DoubleAnimation animationY = new DoubleAnimation
                {
                    From = Canvas.GetTop(img),
                    To = endPoint.Y - img.Height / 2,
                    Duration = TimeSpan.FromSeconds(_lineDuration)
                };

                Storyboard.SetTarget(animationX, img);
                Storyboard.SetTargetProperty(animationX, new PropertyPath(Canvas.LeftProperty));
                Storyboard.SetTarget(animationY, img);
                Storyboard.SetTargetProperty(animationY, new PropertyPath(Canvas.TopProperty));

                storyboard.Children.Add(animationX);
                storyboard.Children.Add(animationY);

                // Speichere das aktuelle Storyboard
                _currentStoryboard = storyboard;

                // TaskCompletionSource, um auf das Ende der Animation zu warten
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

                // Ereignis für das Ende der Animation
                storyboard.Completed += (s, e) => tcs.SetResult(true);

                // Starte das Storyboard
                storyboard.Begin();

                // Aktualisiere die Position während der Animation
                storyboard.CurrentTimeInvalidated += (s, e) =>
                {
                    UpdatePositionFromCanvas(img);
                    enemyGrid.UpdateObjectPosition(this, Position);
                };

                FlipImageDirection(img, movingRight);

                await tcs.Task; // Warte auf das Ende der Animation
            }

            // Überprüfe, ob der Gegner das Ende des Weges erreicht hat
            if (Life > 0)
            {
                ReachedEnd = true;
                GameHandler.Instance.RemoveEnemy(this); // Entferne den Gegner
            }
            else
            {
                ReachedEnd = false;
            }

            img.Visibility = Visibility.Collapsed; // Verstecke das Bild des Gegners
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

        public void GetHit(double damage)
        {
            Life -= damage;

            if (Life <= 0)
            {
                GetKilled();
            }
        }

        public void GetKilled()
        {
            GameHandler.Instance.RemoveEnemy(this);
        }

        public Point GetEnemyPosition()
        {
            Point currentPosition;
            if(Image != null)
            {
                currentPosition.X = Canvas.GetLeft(Image);
                currentPosition.Y = Canvas.GetTop(Image);

                Position = new Point(currentPosition.X, currentPosition.Y);
            }
            return currentPosition;
        }

        private void UpdatePositionFromCanvas(Image img)
        {
            double x = Canvas.GetLeft(img);
            double y = Canvas.GetTop(img);

            Position = new Point(x, y);

            // Debug-Ausgabe, um die Position zu überprüfen
            Console.WriteLine($"Enemy Position: X={Position.X}, Y={Position.Y}");

            // Aktualisiere die Position im Spatial Grid
            GameHandler.Instance._enemyGrid.UpdateObjectPosition(this, Position);
        }

        private bool _isSlowed = false;

        public void ApplySlowEffect(double slowFactor, TimeSpan duration, bool dodamage, double attackDamage)
        {
            if (_isSlowed) return; // Verhindere mehrfache Anwendung

            _isSlowed = true;
            CurrentSpeed = BaseSpeed * slowFactor;

            // Falls eine Bewegung läuft, passe die Geschwindigkeit an
            if (_currentStoryboard != null)
            {
                _currentStoryboard.SetSpeedRatio(slowFactor);
            }

            // Setze die normale Geschwindigkeit nach Ablauf des Slow-Effekts
            Task.Delay(duration).ContinueWith(_ =>
            {
                if (dodamage)
                {
                    Life -= attackDamage * slowFactor;
                }

                // Setze die normale Geschwindigkeit zurück
                CurrentSpeed = BaseSpeed;
                _isSlowed = false;

                // Falls die Bewegung noch läuft, setze die normale Geschwindigkeit wiederher
                if (_currentStoryboard != null)
                {
                    _currentStoryboard.SetSpeedRatio(1.0);
                }

            }, TaskScheduler.FromCurrentSynchronizationContext()); // UI-Thread verwenden
        }
        private void StartSlowMovement(TimeSpan duration)
        {
            // Erstelle ein neues Storyboard für die verlangsamte Bewegung
            Storyboard slowStoryboard = new Storyboard();

            DoubleAnimation slowAnimationX = new DoubleAnimation
            {
                From = Canvas.GetLeft(Image),
                To = Canvas.GetLeft(Image) + (Velocity.X * duration.TotalSeconds),
                Duration = duration
            };

            DoubleAnimation slowAnimationY = new DoubleAnimation
            {
                From = Canvas.GetTop(Image),
                To = Canvas.GetTop(Image) + (Velocity.Y * duration.TotalSeconds),
                Duration = duration
            };

            Storyboard.SetTarget(slowAnimationX, Image);
            Storyboard.SetTargetProperty(slowAnimationX, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTarget(slowAnimationY, Image);
            Storyboard.SetTargetProperty(slowAnimationY, new PropertyPath(Canvas.TopProperty));

            slowStoryboard.Children.Add(slowAnimationX);
            slowStoryboard.Children.Add(slowAnimationY);

            // Speichere das aktuelle Storyboard
            _currentStoryboard = slowStoryboard;

            // Starte die verlangsamte Animation
            slowStoryboard.Begin();
        }
        private void StartNormalMovement()
        {
            // Stoppe die aktuelle Animation (falls vorhanden)
            if (_currentStoryboard != null)
            {
                _currentStoryboard.Stop();
            }

            // Starte die normale Movement-Methode erneut
            _ = Movement(GameHandler.Instance._gameWay, GameHandler.Instance._mainCanvas, Image, GameHandler.Instance._enemyGrid);
        }
    }
}

