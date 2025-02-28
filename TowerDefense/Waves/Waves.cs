using System.Windows.Controls;
using TowerDefense.EnemiesModel.Types;
using TowerDefense.EnemiesModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace TowerDefense.Waves
{
    public class Wave
    {
        private int[,] _waveData;
        private int[] _singleWave =  new int[4];


        public Wave()
        {
            InitializeWaves();
        }

        public int GetWaveCount() 
        { 
            return _waveData.GetLength(0);
        }

        public int GetTotalEnemyTypes()
        {
            return 4;
		}

        public int GetAmountOfEnemies(int currentEnemy, int currentWave)
        {
            return _waveData[currentWave, currentEnemy];
        }

		public int[] GetSingleWave(int waveNumber)
        {
            _singleWave[0] = _waveData[waveNumber, 0];
            _singleWave[1] = _waveData[waveNumber, 1];
            _singleWave[2] = _waveData[waveNumber, 2];
            _singleWave[3] = _waveData[waveNumber, 3];
            return _singleWave;
        }

        public Enemies SpawnEnemy(int EnemyType, Canvas GameField, PathGeometry path)
        {
            if (EnemyType == 0)
            {
                Slime mage = new Slime();
				return CreateEnemy(mage);
            }
            else if (EnemyType == 1)
            {
                Goblin goblin = new Goblin();
				return CreateEnemy(goblin);
            }
            else if (EnemyType == 2)
            {
                Werwolf werwolf = new Werwolf();
				return CreateEnemy(werwolf);
            }
            else
            {
                Troll troll = new Troll();
                return CreateEnemy(troll);
            }

			Enemies CreateEnemy(Enemies enemy)
            {
                PathGeometry pathCopy = path.Clone();
                enemy.Image = enemy.GetEntityPic();
                Point startPoint = pathCopy.Figures[0].StartPoint;

                Canvas.SetLeft(enemy.Image, startPoint.X - (enemy.Image.Width / 2));
                Canvas.SetTop(enemy.Image, startPoint.Y - enemy.Image.Height / 2);
                enemy.Gamepath = pathCopy;
                return enemy;
			}
        }

        private void InitializeWaves()
        {
            _waveData = new int[,]
            {   {5, 0, 0, 0}, {10, 2, 0, 0}, {0, 5, 0, 0}, {20, 0, 1, 0}, {0, 10, 3, 0},
                {15, 5, 0, 0}, {0, 12, 3, 0}, {25, 0, 0, 0}, {0, 15, 5, 0}, {30, 5, 2, 0},
                {0, 20, 5, 0}, {40, 0, 0, 1}, {15, 10, 5, 0}, {0, 25, 10, 0}, {50, 10, 3, 1},
                {0, 30, 5, 0}, {60, 5, 5, 1}, {0, 35, 10, 0}, {70, 0, 5, 1}, {20, 20, 10, 0},
                {80, 10, 5, 1}, {0, 50, 15, 0}, {90, 0, 10, 1}, {30, 30, 15, 0}, {100, 20, 10, 2},
                {0, 60, 20, 0}, {120, 10, 15, 2}, {0, 80, 20, 1}, {150, 0, 10, 2}, {50, 50, 30, 0},
                {160, 10, 20, 2}, {0, 100, 30, 1}, {170, 20, 25, 2}, {0, 120, 35, 1}, {200, 0, 20, 3},
                {50, 50, 40, 1}, {210, 20, 30, 3}, {0, 140, 40, 1}, {220, 30, 35, 3}, {0, 160, 50, 2},
                {250, 0, 30, 4}, {60, 70, 60, 2}, {260, 20, 45, 4}, {0, 180, 60, 2}, {300, 40, 50, 4},
                {0, 200, 70, 2}, {320, 30, 55, 4}, {0, 220, 75, 2}, {350, 0, 60, 5}, {80, 80, 80, 3},
                {360, 40, 65, 5}, {0, 240, 85, 3}, {380, 50, 70, 5}, {0, 260, 90, 3}, {400, 60, 75, 5},
                {0, 280, 95, 3}, {420, 70, 80, 6}, {440, 0, 85, 6}, {90, 90, 90, 4}, {460, 80, 90, 6},
                {0, 300, 100, 4}, {480, 90, 95, 6}, {0, 320, 105, 4}, {500, 100, 100, 7}, {0, 340, 110, 5},
                {520, 110, 105, 7}, {0, 360, 115, 5}, {550, 120, 110, 7}, {0, 380, 120, 5}, {570, 130, 115, 8},
                {0, 400, 125, 6}, {600, 140, 120, 8}, {0, 420, 130, 6}, {620, 150, 125, 8}, {0, 440, 135, 6},
                {650, 160, 130, 9}, {0, 460, 140, 7}, {670, 170, 135, 9}, {0, 480, 145, 7}, {700, 180, 140, 10}
            };
        }
    }
}
