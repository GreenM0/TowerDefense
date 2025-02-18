using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TowerDefense.Helper;
using System;
using System.Windows.Media.Animation;
using TowerDefense.EnemiesModel;
using System.IO;
using System.Windows.Threading;

namespace TowerDefense.Projectils
{
    public class Projectile
    {
        private Point StartPosition { get; }
        private Point TargetPosition { get; }
        private int Speed { get; }
        private Image ProjectileImage { get; set; }
        private Enemies Target { get; }
        private Double Damage { get; }
        public int ImageWidth { get; } = 50;
        public int ImageHeight { get; } = 50;


        public Projectile(Point startPosition, Point targetPosition, int speed, Double damage, string imagePath, Enemies target)
        {
            StartPosition = startPosition;
            TargetPosition = targetPosition;
            Speed = speed;
            Damage = damage;
            Target = target;


            // Positioniere das Projektil auf der Leinwand
            Canvas.SetLeft(GetEntityPic(), StartPosition.X);
            Canvas.SetTop(GetEntityPic(), StartPosition.Y);
        }

        public Image GetEntityPic()
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Projectils\Types\Assets\IceBall.png");

            ImageHelper imageHelper = new();
            ProjectileImage = imageHelper.GetEntityPic(imagePath, this.ImageWidth, this.ImageHeight);
            return imageHelper.GetEntityPic(imagePath, this.ImageWidth, this.ImageHeight);
        }

        public void Animate(Canvas gameCanvas, Action<Projectile> onHit)
        {
            gameCanvas.Children.Add(ProjectileImage);

            // Berechne die Anfangsdistanz und die Richtung
            double distance = Math.Sqrt(
                Math.Pow(Target.Position.X - StartPosition.X, 2) +
                Math.Pow(Target.Position.Y - StartPosition.Y, 2));

            double duration = distance / Speed; // Berechne die Dauer der Animation basierend auf der Geschwindigkeit

            // Berechne die Schritte in X- und Y-Richtung
            double stepX = (Target.Position.X - StartPosition.X) / duration;
            double stepY = (Target.Position.Y - StartPosition.Y) / duration;

            // Setze das Projektil an die Startposition
            Canvas.SetLeft(ProjectileImage, StartPosition.X);
            Canvas.SetTop(ProjectileImage, StartPosition.Y);

            // Erstelle das Storyboard und die DoubleAnimation für die X- und Y-Richtung
            var animationX = new DoubleAnimation
            {
                From = StartPosition.X,
                To = Target.Position.X,
                Duration = TimeSpan.FromSeconds(duration),
                AutoReverse = false
            };

            var animationY = new DoubleAnimation
            {
                From = StartPosition.Y,
                To = Target.Position.Y,
                Duration = TimeSpan.FromSeconds(duration),
                AutoReverse = false
            };

            // Erstelle das Storyboard
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animationX);
            storyboard.Children.Add(animationY);

            Storyboard.SetTarget(animationX, ProjectileImage);
            Storyboard.SetTarget(animationY, ProjectileImage);

            Storyboard.SetTargetProperty(animationX, new PropertyPath("(Canvas.Left)"));
            Storyboard.SetTargetProperty(animationY, new PropertyPath("(Canvas.Top)"));

            // Wenn die Animation abgeschlossen ist, entferne das Projektil und führe den Treffer-Callback aus
            storyboard.Completed += (sender, e) =>
            {
                gameCanvas.Children.Remove(ProjectileImage);
                onHit(this);
            };

            // Starte das Storyboard
            storyboard.Begin();

        }

        public void Hit()
        {
            Target.GetHit(Damage);
        }
    }
}
