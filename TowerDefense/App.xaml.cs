using System.Configuration;
using System.Data;
using System.Windows;

namespace TowerDefense
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public double MenuVolume { get; set; } = 0.5;
        public double IngameVolume { get; set; } = 0.5;
    }

}
