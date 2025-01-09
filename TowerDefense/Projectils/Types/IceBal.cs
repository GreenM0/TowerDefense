using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TowerDefense.EnemiesModel;

namespace TowerDefense.Projectils
{
    public class IceBall : Projectile
    {
        private const int SlowEffectDuration = 3; 
        private const double SlowFactor = 0.5; 

        public IceBall(Point startPosition, Point targetPosition, int speed, int damage, string imagePath, Enemies target)
            : base(startPosition, targetPosition, speed, damage, imagePath, target)
        {
        }

        //public override void Hit()
        //{
        //    base.Hit();

        //    // Füge die Logik für den Verlangsamungseffekt hinzu
        //    SlowTarget();
        //}

        //private void SlowTarget()
        //{
        //    // Reduziere die Bewegungsgeschwindigkeit des Ziels (z. B. für eine bestimmte Zeit)
        //    if (Target != null)
        //    {
        //        Target.ApplySlow(SlowEffectDuration, SlowFactor);
        //    }
        //}
    }
}
