using TowerDefense.EnemiesModel;
using TowerDefense.EnemiesModel.Types;

namespace TowerDefense.Waves
{
    public class Wave
    {
        public int TotalWaveCount = 5;
        private int maxEnemies = 3;
        public int[,] WaveList { get; set; }
        // [ X, Y] X = Count of Wave, Y = Amount of Enemies of that type
        //
        // [ X, 0] = Mage
        // [ X, 1] = Goblin
        // [ X, 2] = Werwolf,

        public Wave()
        {
            WaveList = new int[TotalWaveCount, maxEnemies];

            FillWaveListWithInfo();
        }

        private void FillWaveListWithInfo()
        {
            CreateWave1();
            CreateWave2();
            CreateWave3();
            CreateWave4();
            CreateWave5();
        }

        public int[] GetWave(int waveNumber)
        {
            int[] SingleWave = new int[maxEnemies];

            for (int i = 0; i < maxEnemies; i++)
            {
                SingleWave[i] = WaveList[waveNumber, i];
            }

            return SingleWave;
        }

        public void CreateWave1()
        {
            WaveList[0, 0] = 5;
            WaveList[0, 1] = 0;
            WaveList[0, 2] = 0;
        }

        public void CreateWave2()
        {
            WaveList[1, 0] = 10;
            WaveList[1, 1] = 2;
            WaveList[1, 2] = 0;
        }

        public void CreateWave3()
        {
            WaveList[2, 0] = 0;
            WaveList[2, 1] = 8;
            WaveList[2, 2] = 0;
        }

        public void CreateWave4()
        {
            WaveList[3, 0] = 20;
            WaveList[3, 1] = 0;
            WaveList[3, 2] = 1;
        }

        public void CreateWave5()
        {
            WaveList[4, 0] = 0;
            WaveList[4, 1] = 10;
            WaveList[4, 2] = 5;
        }
    }
}
