using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TowerDefense.Helper
{
    internal class ImageHelper
    {
        public ImageHelper() { }

        //imagepath übergeben (beispiel in Goblin.cs)
        public Image GetEntityPic(string imagePath)
        {
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