using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TowerDefense.Grid;
using TowerDefense.Helper;

namespace TowerDefense.EnemiesModel
{
    public class Enemies : IPositionable
    {
        public double BaseSpeed { get; set; }
        public double CurrentSpeed { get; set; }
        public double Life { get; set; }
        public int Coins { get; set; }
        public Point Position { get; set; }
        public Point ImageCenterPosition { get; set; }
        public (int, int) CurrentCell { get; set; }
        public Image Image { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public bool ReachedEnd { get; set; }
        private bool _isSlowed = false;
        private Storyboard storyboard;
        private bool slowactive = false;
        public bool _isBurning = false;
        private bool burnactive = false;
        public PathGeometry Gamepath { get; set; }
        public Image FlameOverlay { get; set; }
        private Storyboard _flameStoryboard;
        private Canvas EnemyCanvas;
        public double length { get; set; }

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
            EnemyCanvas = _gameField;
            length = Gamepath.GetTotalLength(); // Neue Methode zur Berechnung der Pfadlänge

            // Berechne die Dauer basierend auf dem aktuellen Speed
            double adjustedDuration = length / CurrentSpeed;

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

            storyboard.CurrentTimeInvalidated += async (s, e) =>
            {
                UpdatePositionFromCanvas(Image);
                enemyGrid.UpdateObjectPosition(this, Position);
                RemovePassedSegments(GetEnemyPosition(), tolerance: 10.0);

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
                    Movement(EnemyCanvas, enemyGrid);
                }

                if (!_isSlowed && slowactive)
                {
                    slowactive = false;
                    Movement(EnemyCanvas, enemyGrid);
                }

                if (_isBurning && !burnactive)
                {
                    burnactive = true;
                    FlameMovement();
                }

                if (!_isBurning && burnactive)
                {
                    burnactive = false;
                    StopBurning(_gameField);
                }
            };

            await tcs.Task; // Wait for the animation to complete

            // Check if the enemy has reached the end
            if (Life > 0)
            {
                ReachedEnd = true;
                GetKilled();
            }
            else
            {
                ReachedEnd = false;
            }

            Image.Visibility = Visibility.Collapsed;
        }

        private void RemovePassedSegments(Point newStart, double tolerance = 10.0)
        {
            var segments = Gamepath.Figures[0].Segments;

            if (segments.Count == 0) return; // Falls keine Segmente vorhanden sind, nichts tun

            // Berechne die Länge des Pfads bis zum neuen Startpunkt
            double pathLengthToNewStart = CalculatePathLength(Gamepath.Figures[0].StartPoint, newStart);

            // Liste für zu löschende Segmente
            List<int> segmentsToRemove = new List<int>();

            // Aktueller Startpunkt des Pfads
            Point currentStart = Gamepath.Figures[0].StartPoint;

            // Gehe alle Segmente durch
            for (int i = 0; i < segments.Count; i++)
            {
                if (segments[i] is LineSegment lineSegment)
                {
                    Point segmentEnd = lineSegment.Point;

                    // Berechne die Länge des aktuellen Segments
                    double segmentLength = CalculateDistance(currentStart, segmentEnd);

                    // Prüfe, ob die kumulierte Länge kleiner ist als die Länge bis zum neuen Startpunkt (mit Toleranz)
                    if (pathLengthToNewStart + tolerance > segmentLength)
                    {
                        segmentsToRemove.Add(i);
                        pathLengthToNewStart -= segmentLength; // Reduziere die verbleibende Länge
                    }
                    else
                    {
                        // Wenn das Segment nicht passiert wurde, breche die Schleife ab
                        break;
                    }

                    // Aktualisiere den Startpunkt für die nächste Iteration
                    currentStart = segmentEnd;
                }
            }

            // Entferne Segmente in umgekehrter Reihenfolge (damit die Indizes korrekt bleiben)
            for (int i = segmentsToRemove.Count - 1; i >= 0; i--)
            {
                segments.RemoveAt(segmentsToRemove[i]);
            }

            // Aktualisiere den Startpunkt des Pfads
            if (segments.Count > 0)
            {
                Gamepath.Figures[0].StartPoint = newStart;
            }
        }

        private double CalculatePathLength(Point start, Point end)
        {
            // Berechne die Länge des Pfads zwischen zwei Punkten
            return CalculateDistance(start, end);
        }

        private double CalculateDistance(Point p1, Point p2)
        {
            // Berechne den euklidischen Abstand zwischen zwei Punkten
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
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
            if (_isBurning)
            {
                StopBurning(GameHandler.Instance._mainCanvas);
            }
            GameHandler.Instance.RemoveEnemy(this);
        }

        public Point GetEnemyPosition()
        {
            Point currentPosition;
            if (Image != null)
            {
                currentPosition.X = Canvas.GetLeft(Image);
                currentPosition.Y = Canvas.GetTop(Image);

                ImageCenterPosition = new Point(currentPosition.X, currentPosition.Y);
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

        public void ApplySlowEffect(double slowFactor, TimeSpan duration, bool doDamage, double attackDamage)
        {
            if (_isSlowed) return; // Verhindere mehrfache Anwendung

            _isSlowed = true;
            CurrentSpeed = BaseSpeed * slowFactor;

            // Ändere die Farbe des Gegners (z. B. Blauton)
            Image.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Colors.Blue,
                Opacity = 0.7,
                ShadowDepth = 0
            };

            // Timer für die Dauer des Slow-Effekts
            DispatcherTimer slowTimer = new DispatcherTimer
            {
                Interval = duration
            };

            slowTimer.Tick += (s, e) =>
            {
                slowTimer.Stop();
                CurrentSpeed = BaseSpeed;
                Image.Effect = null; // Entferne den Farbfilter
                _isSlowed = false;
            };

            slowTimer.Start();

            // Füge Schaden hinzu, falls erforderlich
            if (doDamage)
            {
                Life -= attackDamage * slowFactor;
                if (Life <= 0)
                {
                    GetKilled();
                }
            }
        }

        private void StopBurning(Canvas gameCanvas)
        {
            // Stoppe die Flamme-Animation
            _flameStoryboard?.Stop();

            // Überprüfen, ob die Flamme einem Canvas zugeordnet ist
            if (FlameOverlay.Parent != null)
            {
                // Entferne die Flamme aus dem Canvas
                var parentCanvas = FlameOverlay.Parent as Canvas;
                parentCanvas?.Children.Remove(FlameOverlay);
            }

            FlameOverlay.Visibility = Visibility.Collapsed;

            _isBurning = false;

        }

        public void InitializeFlameOverlay()
        {
            string flamePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Projectils\Types\Assets\Flamme.png");

            BitmapImage flameImage = new BitmapImage(new Uri(flamePath));
            FlameOverlay = new Image
            {
                Source = flameImage,
                Width = ImageWidth,  // Passen Sie die Größe an das Gegner-Bild an
                Height = ImageHeight,
                Opacity = 0.7,       // Leicht transparent
                Visibility = Visibility.Collapsed, // Standardmäßig unsichtbar
                RenderTransform = new TranslateTransform() // Initialisiere das TranslateTransform
            };
        }

        public async void SetOnFire(TimeSpan _burnDuration, double damage)
        {
            if (FlameOverlay == null)
            {
                InitializeFlameOverlay();
            }
            _isBurning = true;

            // Überprüfen, ob die Flamme bereits einem Canvas zugeordnet ist
            if (FlameOverlay.Parent != null)
            {
                // Entferne die Flamme aus dem vorherigen Canvas
                var parentCanvas = FlameOverlay.Parent as Canvas;
                parentCanvas?.Children.Remove(FlameOverlay);
            }

            // Positioniere die Flamme über dem Gegner-Bild
            Canvas.SetLeft(FlameOverlay, Position.X);
            Canvas.SetTop(FlameOverlay, Position.Y - ImageHeight / 2); // Leicht über dem Gegner

            // Zeige die Flamme an
            FlameOverlay.Visibility = Visibility.Visible;
            EnemyCanvas.Children.Add(FlameOverlay);

            AnimateFlameFlicker();

            // Starte den Timer für die Brenndauer
            DispatcherTimer burnTimer = new DispatcherTimer
            {
                Interval = _burnDuration // Dauer der Attacke
            };

            burnTimer.Tick += (s, e) =>
            {
                GetHit(damage);
                // Stoppe den Timer
                burnTimer.Stop();
                _isBurning = false;
            };

            burnTimer.Start();
        }

        public async Task FlameMovement()
        {

            double totalPathLength = Gamepath.GetTotalLength();

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
            _flameStoryboard = new Storyboard();
            _flameStoryboard.Children.Add(animationX);
            _flameStoryboard.Children.Add(animationY);

            Storyboard.SetTarget(animationX, FlameOverlay);
            Storyboard.SetTarget(animationY, FlameOverlay);

            Storyboard.SetTargetProperty(animationX, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(animationY, new PropertyPath(Canvas.TopProperty));

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            _flameStoryboard.Completed += (s, e) => tcs.SetResult(true);
            _flameStoryboard.Begin();

            // Warte auf das Ende der Animation
            await tcs.Task;
        }

        private void AnimateFlameFlicker()
        {
            // Erstelle ein Storyboard für die Flamme-Animation
            Storyboard flickerStoryboard = new Storyboard();

            // Erstelle eine zufällige Opacity-Animation
            Random random = new Random();
            double targetOpacity = random.NextDouble() * 0.5 + 0.5; // Zufällige Opacity zwischen 0.5 und 1.0

            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                To = targetOpacity,
                Duration = TimeSpan.FromMilliseconds(random.Next(50, 200)), // Zufällige Dauer zwischen 50ms und 200ms
                AutoReverse = true, // Animation kehrt sich um
                RepeatBehavior = RepeatBehavior.Forever // Animation wiederholt sich unendlich
            };

            // Füge die Animation dem Storyboard hinzu
            Storyboard.SetTarget(opacityAnimation, FlameOverlay);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
            flickerStoryboard.Children.Add(opacityAnimation);

            // Starte die Animation
            flickerStoryboard.Begin();
        }

        public void HandleBitzBallAtack(TimeSpan _burnDuration, double damage)
        {
            ApplySlowEffect(0.1, _burnDuration, true, damage);
            ApplyLightningEffect();
        }

        public void ApplyLightningEffect()
        {
            // Erstelle das Storyboard
            Storyboard lightningStoryboard = new Storyboard();

            // 1. Glow-Effekt
            DropShadowEffect glowEffect = new DropShadowEffect
            {
                Color = Colors.Cyan,
                BlurRadius = 20,
                Opacity = 0.8,
                ShadowDepth = 0
            };
            Image.Effect = glowEffect;

            // 2. Schüttel-Effekt (kleine Bewegung links/rechts)
            DoubleAnimation shakeAnimation = new DoubleAnimation
            {
                From = -5,
                To = 5,
                Duration = TimeSpan.FromMilliseconds(50),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(5) // 5-mal wiederholen
            };
            TranslateTransform shakeTransform = new TranslateTransform();
            Image.RenderTransform = shakeTransform;
            Storyboard.SetTarget(shakeAnimation, Image);
            Storyboard.SetTargetProperty(shakeAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            lightningStoryboard.Children.Add(shakeAnimation);

            // Animation starten
            lightningStoryboard.Begin();

            // Entferne den Effekt nach der Animation
            lightningStoryboard.Completed += (s, e) =>
            {
                Image.Effect = null;
            };
        }
    }
}

