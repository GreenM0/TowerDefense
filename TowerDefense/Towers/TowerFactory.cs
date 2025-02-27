using TowerDefense.Grid;
using TowerDefense.EnemiesModel;
using TowerDefense.Projectils;
using System.Windows;
using TowerDefense.Towers;

public class TowerFactory
{
    public static BaseTower CreateTower(string towerType, Point position)
    {
        switch (towerType)
        {
            case "TestTower1":
                return new TestTower1(position);
            case "ArcherTower":
                return new ArcherTower(position);
            case "FireTower":
                return new FireTower(position);
            case "MageTower":
                return new MageTower(position);
            default:
                throw new ArgumentException("Unknown tower type");
        }
    }
}
