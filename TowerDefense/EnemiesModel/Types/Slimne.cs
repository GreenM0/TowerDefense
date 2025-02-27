using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TowerDefense.Helper;

namespace TowerDefense.EnemiesModel.Types
{
    internal class Slimne : Enemies
    {
        public Slimne() : base(speed: 20, life: 5, coins: 3, imagewidth: 50, imageheight: 55)
        {

        }

        public override Image GetEntityPic()
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\EnemiesModel\Types\Assets\slime.png");

            ImageHelper imageHelper = new();
            return imageHelper.GetEntityPic(imagePath, ImageWidth, ImageHeight);
        }
    }
}
