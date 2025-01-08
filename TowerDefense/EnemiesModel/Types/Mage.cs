using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TowerDefense.Helper;

namespace TowerDefense.EnemiesModel.Types
{
    internal class Mage : Enemies
    {
        public Mage() : base(speed: 30, life: 5, coins: 5, imagewidth: 50, imageheight: 75)
        {

        }

        public override Image GetEntityPic()
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\EnemiesModel\Types\Assets\mage.png");

            ImageHelper imageHelper = new();
            return imageHelper.GetEntityPic(imagePath, this.ImageWidth, this.ImageHeight);
        }
    }
}
