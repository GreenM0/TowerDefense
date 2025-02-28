using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TowerDefense.Helper;

namespace TowerDefense.EnemiesModel.Types
{
    internal class Troll : Enemies
    {
        public Troll() : base(speed: 30, life: 50, coins: 14, imagewidth: 60, imageheight: 95)
        {

        }

        public override Image GetEntityPic()
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\EnemiesModel\Types\Assets\Bergtroll.png");

            ImageHelper imageHelper = new();
            return imageHelper.GetEntityPic(imagePath, ImageWidth, ImageHeight);
        }
    }
}
