using System.Windows.Controls;
using TowerDefense.EnemiesModel.Types;
using TowerDefense.EnemiesModel;
using System.Windows;

namespace TowerDefense.Waves
{
    public class Wave
    {
        private int[,] _waveData;
        private int[] _singleWave =  new int[3];


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
            return 3;
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
            return _singleWave;
        }

        public Enemies SpawnEnemy(int EnemyType, Canvas GameField, Point[] _gameWay)
        {
            if (EnemyType == 0)
            {
                Mage mage = new Mage();
				return CreateEnemy(mage);
            }
            else if (EnemyType == 1)
            {
                Goblin goblin = new Goblin();
				return CreateEnemy(goblin);
            }
            else
            {
                Werwolf werwolf = new Werwolf();
				return CreateEnemy(werwolf);
            }

			Enemies CreateEnemy(Enemies enemy)
            {
				enemy.Image = enemy.GetEntityPic();
                Canvas.SetLeft(enemy.Image, _gameWay[0].X - enemy.Image.Width / 2);
                Canvas.SetTop(enemy.Image, _gameWay[0].Y - enemy.Image.Height / 2);
				return enemy;
			}
        }

        private void InitializeWaves()
        {
            _waveData = new int[,]
            {
                {5, 0, 0}, {10, 2, 0}, {0, 5, 0}, {20, 0, 1}, {0, 10, 3},
                {15, 5, 0}, {0, 12, 3}, {25, 0, 0}, {0, 15, 5}, {30, 5, 2},
                {0, 20, 5}, {40, 0, 0}, {15, 10, 5}, {0, 25, 10}, {50, 10, 3},
                {0, 30, 5}, {60, 5, 5}, {0, 35, 10}, {70, 0, 5}, {20, 20, 10},
                {80, 10, 5}, {0, 50, 15}, {90, 0, 10}, {30, 30, 15}, {100, 20, 10},
                {0, 60, 20}, {120, 10, 15}, {0, 80, 20}, {150, 0, 10}, {50, 50, 30},
                {160, 10, 20}, {0, 100, 30}, {170, 20, 25}, {0, 120, 35}, {200, 0, 20},
                {50, 50, 40}, {210, 20, 30}, {0, 140, 40}, {220, 30, 35}, {0, 160, 50},
                {250, 0, 30}, {60, 70, 60}, {260, 20, 45}, {0, 180, 60}, {300, 40, 50},
                {0, 200, 70}, {320, 30, 55}, {0, 220, 75}, {350, 0, 60}, {80, 80, 80},
                {360, 40, 65}, {0, 240, 85}, {380, 50, 70}, {0, 260, 90}, {400, 60, 75},
                {0, 280, 95}, {420, 70, 80}, {440, 0, 85}, {90, 90, 90}, {460, 80, 90},
                {0, 300, 100}, {480, 90, 95}, {0, 320, 105}, {500, 100, 100}, {0, 340, 110},
                {520, 110, 105}, {0, 360, 115}, {550, 120, 110}, {0, 380, 120}, {570, 130, 115},
                {0, 400, 125}, {600, 140, 120}, {0, 420, 130}, {620, 150, 125}, {0, 440, 135},
                {650, 160, 130}, {0, 460, 140}, {670, 170, 135}, {0, 480, 145}, {700, 180, 140}
            };
        }
    }
}
