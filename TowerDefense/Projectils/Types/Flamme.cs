using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TowerDefense.EnemiesModel;

namespace TowerDefense.Projectils
{
    public class Flamme : Projectile
    {
        protected TimeSpan BurnDuration;
        protected int FlameRadius;
        public Flamme(Point startPosition, Point targetPosition, float speed, double damage, List<Enemies> targets, string imagePath, TimeSpan burnDuration, int flameRadius)
            : base(startPosition, targetPosition, speed, damage, targets, imagePath)
        {
            BurnDuration = burnDuration;
            FlameRadius = flameRadius;
        }

        public override bool IsCollidingWithAnyTarget(Canvas GameCanvas)
        {
            Rect arrowRect = new Rect(Canvas.GetLeft(ProjectileImage), Canvas.GetTop(ProjectileImage), ProjectileImage.Width /2, ProjectileImage.Height /2);

            Point impactPoint = new Point(Canvas.GetLeft(ProjectileImage) + ProjectileImage.Width / 2, Canvas.GetTop(ProjectileImage) + ProjectileImage.Height / 2);

            foreach (var enemy in GameHandler.Instance._enemyList)
            { 
                Rect targetRect = new Rect(enemy.ImageCenterPosition.X, enemy.ImageCenterPosition.Y, enemy.Image.Width , enemy.Image.Height);

                if (arrowRect.IntersectsWith(targetRect))
                {
                    if (FlameRadius > 1)
                    {
                        foreach (var enemy2 in GameHandler.Instance._enemyList)
                        {
                            double distance = Math.Sqrt(Math.Pow(impactPoint.X - enemy2.Position.X, 2) + Math.Pow(impactPoint.Y - enemy2.Position.Y, 2));
                            if (distance <= FlameRadius)
                            {
                                // Setze den Gegner in Brand
                                enemy2.SetOnFire(BurnDuration, Damage);
                            }
                        }
                    }      
                    enemy.SetOnFire(BurnDuration, Damage);
                    GameCanvas.Children.Remove(ProjectileImage);
                    return true;   
                }
            }
            return false;
        }
    }
}
