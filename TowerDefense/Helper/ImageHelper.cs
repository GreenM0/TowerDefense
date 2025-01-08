using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TowerDefense.Helper
{
    internal class ImageHelper
    {
        public ImageHelper() { }

        //imagepath übergeben (beispiel in Goblin.cs)
        public Image GetEntityPic(string imagePath, int imageWidth = 50, int imageHeight = 50)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(imagePath);
            img.DecodePixelHeight = imageHeight;
            img.DecodePixelWidth = imageWidth;
            img.EndInit();

            Image imageControl = new Image
            {
                Source = img,
                Width = 75,
                Height = 75
            };

            return imageControl;
        }
    }
}