using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TowerDefense.Helper;

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

            ImageHelper imageHelper = new();
            return imageHelper.GetEntityPic(imagePath);
        }
    }
}
