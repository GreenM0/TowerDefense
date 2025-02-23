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
        public Flamme(Point startPosition, Point targetPosition, float speed, double damage, List<Enemies> targets, string imagePath, TimeSpan burnDuration)
            : base(startPosition, targetPosition, speed, damage, targets, imagePath)
        {
            BurnDuration = burnDuration;
        }

        public override bool IsCollidingWithAnyTarget(Canvas GameCanvas)
        {
            Rect arrowRect = new Rect(Canvas.GetLeft(ProjectileImage), Canvas.GetTop(ProjectileImage), ProjectileImage.Width, ProjectileImage.Height);

            foreach (var enemy in GameHandler.Instance._enemyList)
            { 
                Rect targetRect = new Rect(enemy.Position.X, enemy.Position.Y, enemy.Image.Width / 2, enemy.Image.Height / 2);

                // Prüfe, ob der Pfeil mit einem Gegner kollidiert
                if (arrowRect.IntersectsWith(targetRect))
                {
                    enemy.SetOnFire(GameCanvas, BurnDuration, Damage);
                    GameCanvas.Children.Remove(ProjectileImage);
                    return true;
                }
            }
            return false;
        }
    }
}
