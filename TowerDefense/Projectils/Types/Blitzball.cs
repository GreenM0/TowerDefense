using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TowerDefense.EnemiesModel;

namespace TowerDefense.Projectils
{
    public class Blitzball : Projectile
    {
        protected TimeSpan ShockDuraion;
        protected int ShockRadius;
        public Blitzball(Point startPosition, Point targetPosition, float speed, double damage, List<Enemies> targets, string imagePath, TimeSpan shockduration, int shockRadius)
            : base(startPosition, targetPosition, speed, damage, targets, imagePath)
        {
            ShockDuraion = shockduration;
            ShockRadius = shockRadius;
        }

        public override bool IsCollidingWithAnyTarget(Canvas GameCanvas)
        {
            Rect arrowRect = new Rect(Canvas.GetLeft(ProjectileImage), Canvas.GetTop(ProjectileImage), ProjectileImage.Width /2, ProjectileImage.Height /2);

            Point impactPoint = new Point(Canvas.GetLeft(ProjectileImage) + ProjectileImage.Width / 2, Canvas.GetTop(ProjectileImage) + ProjectileImage.Height / 2);

            foreach (var enemy in GameHandler.Instance._enemyList)
            {
                Rect targetRect = new Rect(enemy.ImageCenterPosition.X, enemy.ImageCenterPosition.Y + 50, enemy.Image.Width, enemy.Image.Height);

                if (arrowRect.IntersectsWith(targetRect))
                {
                    if (ShockRadius > 1)
                    {
                        foreach (var enemy2 in GameHandler.Instance._enemyList)
                        {
                            double distance = Math.Sqrt(Math.Pow(impactPoint.X - enemy2.Position.X, 2) + Math.Pow(impactPoint.Y - enemy2.Position.Y, 2));
                            if (distance <= ShockRadius)
                            {
                                // Setze den Gegner in Brand
                                enemy2.HandleBitzBallAtack(ShockDuraion, Damage);
                            }
                        }
                    }
                    enemy.HandleBitzBallAtack(ShockDuraion, Damage);
                    GameCanvas.Children.Remove(ProjectileImage);
                    return true;
                }
            }
            return false;
        }
    }
}
