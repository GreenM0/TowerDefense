using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TowerDefense.Helper;
using System;
using System.Windows.Media.Animation;
using TowerDefense.EnemiesModel;
using System.IO;
using System.Windows.Threading;
using System.Windows.Media;

namespace TowerDefense.Projectils
{
    public class Projectile
    {
        private Point StartPosition { get; }
        private Point TargetPosition { get; }
        private float Speed { get; }
        private Image ProjectileImage { get; set; }
        private List<Enemies> Targets { get; }
        private double Damage { get; }
        public int ImageWidth { get; } = 20;
        public int ImageHeight { get; } = 10;
        public string ImagePath { get; set; }
        public Action<Enemies> OnHit { get; set; }
        private Storyboard storyboard;



        public Projectile(Point startPosition, Point targetPosition, float speed, double damage, List<Enemies> targets, string imagePath)
        {
            StartPosition = startPosition;
            TargetPosition = targetPosition;
            Speed = speed;
            Damage = damage;
            Targets = targets;
            ImagePath = imagePath;

            // Positioniere das Projektil auf der Leinwand
            Canvas.SetLeft(GetEntityPic(), StartPosition.X);
            Canvas.SetTop(GetEntityPic(), StartPosition.Y);
        }

        public Image GetEntityPic()
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImagePath);

            ImageHelper imageHelper = new();
            ProjectileImage = imageHelper.GetEntityPic(imagePath, ImageWidth, ImageHeight);
            return ProjectileImage;
        }

        public async Task Shoot(Canvas gameCanvas, Point startPosition, Enemies target, double speed, Action<Projectile> onHit)
        {
            // Füge den Pfeil zum Canvas hinzu
            gameCanvas.Children.Add(ProjectileImage);
            ProjectileImage.Visibility = Visibility.Visible;
            ProjectileImage.Opacity = 1.0;

            Canvas.SetLeft(ProjectileImage, startPosition.X);
            Canvas.SetTop(ProjectileImage, startPosition.Y);

            // Berechne die Richtung zum Ziel
            double directionX = target.Position.X - startPosition.X;
            double directionY = target.Position.Y - startPosition.Y;

            // Normalisiere die Richtung (Einheitsvektor)
            double distanceToTarget = Math.Sqrt(directionX * directionX + directionY * directionY);
            if (distanceToTarget > 0)
            {
                directionX /= distanceToTarget;
                directionY /= distanceToTarget;
            }

            // Lege einen Punkt weit hinter dem Ziel fest (z. B. 1000 Einheiten entfernt)
            double extendedDistance = 5000; // Entfernung hinter dem Ziel
            Point extendedTarget = new Point(
                target.Position.X + directionX * extendedDistance,
                target.Position.Y + directionY * extendedDistance
            );

            // Berechne die Entfernung und die Dauer der Animation
            double totalDistance = Math.Sqrt(
                Math.Pow(extendedTarget.X - startPosition.X, 2) +
                Math.Pow(extendedTarget.Y - startPosition.Y, 2)
            );
            double duration = totalDistance / speed;

            // Erstelle die Animationen für X- und Y-Richtung
            var animationX = new DoubleAnimation
            {
                From = startPosition.X,
                To = extendedTarget.X,
                Duration = TimeSpan.FromSeconds(duration),
                AutoReverse = false
            };

            var animationY = new DoubleAnimation
            {
                From = startPosition.Y,
                To = extendedTarget.Y,
                Duration = TimeSpan.FromSeconds(duration),
                AutoReverse = false
            };

            // Erstelle das Storyboard
            storyboard = new Storyboard();
            storyboard.Children.Add(animationX);
            storyboard.Children.Add(animationY);

            Storyboard.SetTarget(animationX, ProjectileImage);
            Storyboard.SetTarget(animationY, ProjectileImage);

            Storyboard.SetTargetProperty(animationX, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(animationY, new PropertyPath(Canvas.TopProperty));

            // Drehe den Pfeil in Richtung des Ziels
            double angle = Math.Atan2(directionY, directionX) * 180 / Math.PI;
            ProjectileImage.RenderTransform = new RotateTransform(angle);

            // TaskCompletionSource, um das Ende der Animation abzuwarten
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            // Event, um die Animation zu überwachen
            storyboard.CurrentTimeInvalidated += (s, e) => 
            {
                // Überprüfe, ob der Pfeil einen Gegner getroffen hat
                if (IsCollidingWithAnyTarget(gameCanvas))
                {
                    // Animation stoppen
                    storyboard.Stop();
                    gameCanvas.Children.Remove(ProjectileImage);
                    onHit?.Invoke(this);
                }
                // Überprüfe, ob der Pfeil die Map verlassen hat
                else if (IsOutOfBounds(gameCanvas))
                {
                    // Animation stoppen
                    storyboard.Stop();
                    gameCanvas.Children.Remove(ProjectileImage);
                }
            };

            // Starte die Animation
            storyboard.Begin();

            // Warte auf das Ende der Animation (entweder durch Treffer oder Verlassen der Map)
            await tcs.Task;
        }

        private bool IsCollidingWithAnyTarget(Canvas GameCanvas)
        {
            Rect arrowRect = new Rect(Canvas.GetLeft(ProjectileImage), Canvas.GetTop(ProjectileImage), ProjectileImage.Width, ProjectileImage.Height);

            foreach (var enemy in Targets)
            {
                Rect targetRect = new Rect(enemy.Position.X, enemy.Position.Y, enemy.Image.Width, enemy.Image.Height);

                // Prüfe, ob der Pfeil mit einem Gegner kollidiert
                if (arrowRect.IntersectsWith(targetRect))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsOutOfBounds(Canvas GameCanvas)
        {
            double x = Canvas.GetLeft(ProjectileImage);
            double y = Canvas.GetTop(ProjectileImage);

            var Position = new Point(x, y);

            if (Position.X < -100 || Position.X > 3000 || Position.Y < -100 || Position.Y > 3000)
            {
                return true;
            }
            else
                return false;

        }
    }
}
