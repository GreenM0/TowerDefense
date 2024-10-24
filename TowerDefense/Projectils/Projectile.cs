using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TowerDefense.Projectils
{
    public class Projectile
    {
        public Point StartPosition { get; private set; }
        public Point TargetPosition { get; private set; }
        public double Speed { get; private set; }
        public UIElement UIElement { get; private set; } 

        public Projectile(Point startPosition, Point targetPosition, double speed, double damage)
        {
            StartPosition = startPosition;
            TargetPosition = targetPosition;

            // Erstelle eine grafische Darstellung des Projektils (z.B. ein kleiner Kreis)
            Ellipse projectileVisual = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.Black
            };

            UIElement = projectileVisual;

            Canvas.SetLeft(UIElement, StartPosition.X);
            Canvas.SetTop(UIElement, StartPosition.Y);
        }

        public void Animate(Canvas canvas, Action<Projectile> onComplete = null)
        {
            double distance = (TargetPosition - StartPosition).Length;
            double durationInSeconds = distance / Speed;

            DoubleAnimation moveXAnimation = new DoubleAnimation
            {
                From = StartPosition.X,
                To = TargetPosition.X,
                Duration = TimeSpan.FromSeconds(durationInSeconds)
            };

            DoubleAnimation moveYAnimation = new DoubleAnimation
            {
                From = StartPosition.Y,
                To = TargetPosition.Y,
                Duration = TimeSpan.FromSeconds(durationInSeconds)
            };

            Storyboard.SetTarget(moveXAnimation, UIElement);
            Storyboard.SetTargetProperty(moveXAnimation, new PropertyPath("(Canvas.Left)"));

            Storyboard.SetTarget(moveYAnimation, UIElement);
            Storyboard.SetTargetProperty(moveYAnimation, new PropertyPath("(Canvas.Top)"));

            Storyboard movementStoryboard = new Storyboard();
            movementStoryboard.Children.Add(moveXAnimation);
            movementStoryboard.Children.Add(moveYAnimation);

            movementStoryboard.Completed += (s, e) =>
            {
                onComplete?.Invoke(this);
            };

            canvas.Children.Add(UIElement);
            movementStoryboard.Begin();
        }
    }
}
