using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TowerDefense.EnemiesModel.Types
{
    internal class Goblin : Enemies
    {
        public Goblin() : base(speed: 10, life: 2, coins: 2)
        {

        }

        public Image GetEntityPic()
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\EnemiesModel\Types\Assets\goblin.png");

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(imagePath);
            img.DecodePixelHeight = 50;
            img.DecodePixelWidth = 50;
            img.EndInit();

            Image imageControl = new Image
            {
                Source = img,
                Width = 50,
                Height = 50
            };

            return imageControl;
        }
    }
}
