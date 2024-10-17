using TowerDefense.EnemiesModel;
using TowerDefense.EnemiesModel.Types;

namespace TowerDefense.Waves
{
    public class Wave
    {
        private int maxEnemies = 3;
        private int maxWaves = 5;
        public int[,] WaveList { get; set; }
        // [ X, Y] X = Count of Wave, Y = Amount of Enemies of that type
        //
        // [ X, 0] = Slimes
        // [ X, 1] = Goblins
        // [ X, 2] = Dragons,

        public Wave()
        {
            WaveList = new int[maxWaves - 1, maxEnemies - 1];

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

        public Array GetWave(int waveNumber)
        {
            int[] SingleWave = new int[maxEnemies - 1];

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
            WaveList[0, 0] = 10;
            WaveList[0, 1] = 2;
            WaveList[0, 2] = 0;
        }

        public void CreateWave3()
        {
            WaveList[0, 0] = 0;
            WaveList[0, 1] = 8;
            WaveList[0, 2] = 0;
        }

        public void CreateWave4()
        {
            WaveList[0, 0] = 20;
            WaveList[0, 1] = 0;
            WaveList[0, 2] = 1;
        }

        public void CreateWave5()
        {
            WaveList[0, 0] = 0;
            WaveList[0, 1] = 10;
            WaveList[0, 2] = 5;
        }
    }
}
