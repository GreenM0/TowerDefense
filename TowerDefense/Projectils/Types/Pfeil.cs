using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TowerDefense.EnemiesModel;

namespace TowerDefense.Projectils
{
    public class Pfeil : Projectile
    {

        public Pfeil(Point startPosition, Point targetPosition, float speed, double damage, List<Enemies> targets, string imagePath)
            : base(startPosition, targetPosition, speed, damage, targets, imagePath)
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
