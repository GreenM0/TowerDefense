using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TowerDefense.Helper;

namespace TowerDefense.EnemiesModel.Types
{
    internal class Werwolf : Enemies
    {
        public Werwolf() : base(speed: 5, life: 30, coins: 10, imagewidth: 75, imageheight: 100)
        {

        }

        public Image GetEntityPic()
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\EnemiesModel\Types\Assets\werwolf.png");

            ImageHelper imageHelper = new();
            return imageHelper.GetEntityPic(imagePath, this.ImageWidth, this.ImageHeight);
        }
    }
}
