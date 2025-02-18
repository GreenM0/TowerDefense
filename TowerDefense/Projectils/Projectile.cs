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

        public async Task Shoot(Canvas gameCanvas, Point startPosition, Point targetPosition, double speed, Action<Projectile> onHit)
        {

                
                Canvas.SetLeft(ProjectileImage, startPosition.X);
                Canvas.SetTop(ProjectileImage, startPosition.Y);

                // Berechne die Entfernung und die Dauer der Animation
                double distance = Math.Sqrt(
                    Math.Pow(targetPosition.X - startPosition.X, 2) +
                    Math.Pow(targetPosition.Y - startPosition.Y, 2));

                double duration = distance / speed;

                // Erstelle die Animationen für X- und Y-Richtung
                var animationX = new DoubleAnimation
                {
                    From = startPosition.X,
                    To = targetPosition.X,
                    Duration = TimeSpan.FromSeconds(duration),
                    AutoReverse = false
                };

                var animationY = new DoubleAnimation
                {
                    From = startPosition.Y,
                    To = targetPosition.Y,
                    Duration = TimeSpan.FromSeconds(duration),
                    AutoReverse = false
                };

                // Erstelle das Storyboard
                storyboard = new Storyboard();

                Storyboard.SetTarget(animationX, ProjectileImage);
                Storyboard.SetTarget(animationY, ProjectileImage);

                Storyboard.SetTargetProperty(animationX, new PropertyPath(Canvas.LeftProperty));
                Storyboard.SetTargetProperty(animationY, new PropertyPath(Canvas.TopProperty));

                storyboard.Children.Add(animationX);
                storyboard.Children.Add(animationY);

            // Drehe den Pfeil in Richtung des Ziels
            double angle = Math.Atan2(targetPosition.Y - startPosition.Y, targetPosition.X - startPosition.X) * 180 / Math.PI;
            ProjectileImage.RenderTransform = new RotateTransform(angle);

            // TaskCompletionSource, um das Ende der Animation abzuwarten
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            storyboard.Completed += (s, e) =>
            {
                // Überprüfe Kollision und ob der Pfeil das Ziel getroffen hat
                if (IsCollidingWithAnyTarget(gameCanvas))
                {
                    onHit?.Invoke(this);
                    gameCanvas.Children.Remove(ProjectileImage);
                }
                else
                {
                    // Wenn der Pfeil das Ziel nicht trifft und aus der Karte fliegt
                    if (IsOutOfBounds(gameCanvas))
                    {
                        // Entferne den Pfeil, wenn er aus der Karte fliegt
                        gameCanvas.Children.Remove(ProjectileImage);
                    }
                }

                // Setze das Ergebnis auf true, damit TaskCompletionSource abgeschlossen wird
                tcs.SetResult(true);
            };

            // Starte die Animation
            storyboard.Begin();

            await tcs.Task;

                // Entferne den Pfeil vom Canvas und führe den Treffer-Callback aus
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
