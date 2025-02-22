using System;
using System.IO;
using System.Net.NetworkInformation;
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
        private bool _isSlowed = false;
        private Storyboard storyboard;
        private bool slowactive = false;
        public PathGeometry Gamepath { get; set; }
        public virtual Image? GetEntityPic() => null;

        public Enemies(int speed, int life, int coins, int imagewidth = 0, int imageheight = 0)
        {
            CurrentSpeed = speed;
            BaseSpeed = speed;
            Life = life;
            Coins = coins;
            ImageWidth = imagewidth;
            ImageHeight = imageheight;
        }

        public async Task Movement(Canvas _gameField, SpatialGrid<Enemies> enemyGrid)
        {
            double totalPathLength = Gamepath.GetRenderBounds(null).Width + Gamepath.GetRenderBounds(null).Height;

            // Berechne die Dauer basierend auf dem aktuellen Speed
            double adjustedDuration = totalPathLength / CurrentSpeed;

            // Animationsobjekte erstellen
            DoubleAnimationUsingPath animationX = new DoubleAnimationUsingPath
            {
                PathGeometry = Gamepath,
                Duration = TimeSpan.FromSeconds(adjustedDuration),
                Source = PathAnimationSource.X 
            };

            DoubleAnimationUsingPath animationY = new DoubleAnimationUsingPath
            {
                PathGeometry = Gamepath,
                Duration = TimeSpan.FromSeconds(adjustedDuration),
                Source = PathAnimationSource.Y
            };

            // Storyboard erstellen und Animationsobjekte hinzufügen
            storyboard = new Storyboard();
            Storyboard.SetTarget(animationX, Image);
            Storyboard.SetTargetProperty(animationX, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTarget(animationY, Image);
            Storyboard.SetTargetProperty(animationY, new PropertyPath(Canvas.TopProperty));

            storyboard.Children.Add(animationX);
            storyboard.Children.Add(animationY);

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            storyboard.Completed += (s, e) => tcs.SetResult(true);
            storyboard.Begin();

            // Track the previous position to detect direction change
            Point previousPosition = Gamepath.Figures[0].StartPoint;
            bool isFlipped = false;

            storyboard.CurrentTimeInvalidated += (s, e) =>
            {

                UpdatePositionFromCanvas(Image);
                enemyGrid.UpdateObjectPosition(this, Position);
                // Get the current position of the image

                // Compare current position with the previous position to detect direction change
                if (Position.X < previousPosition.X)
                {
                    // If moving left, flip the image
                    if (!isFlipped)
                    {
                        FlipImageDirection(Image, false);
                        isFlipped = true;
                    }
                }
                else if (Position.X > previousPosition.X)
                {
                    // If moving right, reset flip
                    if (isFlipped)
                    {
                        FlipImageDirection(Image, true);
                        isFlipped = false;
                    }
                }

                // Update the previous position for the next frame
                previousPosition = Position;

                // Wenn der Slow-Effekt aktiv ist, ändere die Geschwindigkeit und starte die Animation mit der aktuellen Position
                if (_isSlowed && !slowactive)
                {
                    slowactive = true;

                    // Erstelle eine neue PathGeometry, basierend auf der aktuellen Position
                    var bufferdstart = Gamepath.Figures[0].StartPoint;
                    Gamepath.Figures[0].StartPoint = GetEnemyPosition(); // Setze den Startpunkt auf die aktuelle Position
                    RemovePassedSegments(bufferdstart);

                    // Starte die Animation mit dem neuen Pfad und der neuen Dauer
                    Movement(_gameField, enemyGrid);
                }

                if (!_isSlowed && slowactive)
                {
                    slowactive = false;

                    // Erstelle eine neue PathGeometry, basierend auf der aktuellen Position
                    var bufferdstart = Gamepath.Figures[0].StartPoint;
                    Gamepath.Figures[0].StartPoint = GetEnemyPosition(); // Setze den Startpunkt auf die aktuelle Position
                    RemovePassedSegments(bufferdstart);

                    // Starte die Animation mit dem neuen Pfad und der neuen Dauer
                    Movement(_gameField, enemyGrid);
                }
     
            };

            await tcs.Task; // Wait for the animation to complete

            // Check if the enemy has reached the end
            if (Life > 0)
            {
                ReachedEnd = true;
                GameHandler.Instance.RemoveEnemy(this);
            }
            else
            {
                ReachedEnd = false;
            }

            Image.Visibility = Visibility.Collapsed;
        }

        private void RemovePassedSegments(Point originalStart)
        {
            var segments = Gamepath.Figures[0].Segments;

            if (segments.Count == 0) return; // Falls keine Segmente vorhanden sind, nichts tun

            // Liste für zu löschende Segmente
            List<int> segmentsToRemove = new List<int>();

            for (int i = 0; i < segments.Count; i++)
            {
                if (segments[i] is LineSegment lineSegment)
                {
                    Point segmentEnd = lineSegment.Point;

                    // Prüfe, ob das Segment zwischen originalStart und newStart liegt
                    if (IsPointBetween(originalStart, Gamepath.Figures[0].StartPoint, segmentEnd))
                    {
                        segmentsToRemove.Add(i);
                    }
                }
            }

            // Entferne Segmente in umgekehrter Reihenfolge (damit die Indizes korrekt bleiben)
            for (int i = segmentsToRemove.Count - 1; i >= 0; i--)
            {
                segments.RemoveAt(segmentsToRemove[i]);
            }
        }

        private bool IsPointBetween(Point originalStart, Point newStart, Point segmentEnd)
        {
            double minX = Math.Min(originalStart.X, newStart.X);
            double maxX = Math.Max(originalStart.X, newStart.X);
            double minY = Math.Min(originalStart.Y, newStart.Y);
            double maxY = Math.Max(originalStart.Y, newStart.Y);

            return (segmentEnd.X >= minX && segmentEnd.X <= maxX) &&
                   (segmentEnd.Y >= minY && segmentEnd.Y <= maxY);
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

            double centerX = x + (ImageWidth / 2);
            double centerY = y + (ImageHeight / 2);
            Position = new Point(centerX, centerY);

            // Aktualisiere die Position im Spatial Grid
            GameHandler.Instance._enemyGrid.UpdateObjectPosition(this, Position);
        }       

        public void ApplySlowEffect(double slowFactor, TimeSpan duration, bool dodamage, double attackDamage)
        {
            if (_isSlowed) return; // Verhindere mehrfache Anwendung

            _isSlowed = true;
            CurrentSpeed = BaseSpeed * slowFactor;
            storyboard.Stop();

            Task.Delay(duration).ContinueWith(_ =>
            {   
                if (dodamage)
                {
                    Life -= attackDamage * slowFactor;
                }
                CurrentSpeed = BaseSpeed;
                _isSlowed = false;
            });
        }
    }
}

