using TowerDefense.EnemiesModel;
using TowerDefense.EnemiesModel.Types;

namespace TowerDefense.Waves
{
    public class Wave
    {
        public int TotalWaveCount = 80;
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
            CreateWave6();
            CreateWave7();
            CreateWave8();
            CreateWave9();
            CreateWave10();
            CreateWave11();
            CreateWave12();
            CreateWave13();
            CreateWave14();
            CreateWave15();
            CreateWave16();
            CreateWave17();
            CreateWave18();
            CreateWave19();
            CreateWave20();
            CreateWave21();
            CreateWave22();
            CreateWave23();
            CreateWave24();
            CreateWave25();
            CreateWave26();
            CreateWave27();
            CreateWave28();
            CreateWave29();
            CreateWave30();
            CreateWave31();
            CreateWave32();
            CreateWave33();
            CreateWave34();
            CreateWave35();
            CreateWave36();
            CreateWave37();
            CreateWave38();
            CreateWave39();
            CreateWave40();
            CreateWave41();
            CreateWave42();
            CreateWave43();
            CreateWave44();
            CreateWave45();
            CreateWave46();
            CreateWave47();
            CreateWave48();
            CreateWave49();
            CreateWave50();
            CreateWave51();
            CreateWave52();
            CreateWave53();
            CreateWave54();
            CreateWave55();
            CreateWave56();
            CreateWave57();
            CreateWave58();
            CreateWave59();
            CreateWave60();
            CreateWave61();
            CreateWave62();
            CreateWave63();
            CreateWave64();
            CreateWave65();
            CreateWave66();
            CreateWave67();
            CreateWave68();
            CreateWave69();
            CreateWave70();
            CreateWave71();
            CreateWave72();
            CreateWave73();
            CreateWave74();
            CreateWave75();
            CreateWave76();
            CreateWave77();
            CreateWave78();
            CreateWave79();
            CreateWave80();
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
            WaveList[2, 1] = 5;
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
            WaveList[4, 2] = 3;
        }

        public void CreateWave6()
        {
            WaveList[5, 0] = 15;
            WaveList[5, 1] = 5;
            WaveList[5, 2] = 0;
        }

        public void CreateWave7()
        {
            WaveList[6, 0] = 0;
            WaveList[6, 1] = 12;
            WaveList[6, 2] = 3;
        }

        public void CreateWave8()
        {
            WaveList[7, 0] = 25;
            WaveList[7, 1] = 0;
            WaveList[7, 2] = 0;
        }

        public void CreateWave9()
        {
            WaveList[8, 0] = 0;
            WaveList[8, 1] = 15;
            WaveList[8, 2] = 5;
        }

        public void CreateWave10()
        {
            WaveList[9, 0] = 30;
            WaveList[9, 1] = 5;
            WaveList[9, 2] = 2;
        }

        public void CreateWave11()
        {
            WaveList[10, 0] = 0;
            WaveList[10, 1] = 20;
            WaveList[10, 2] = 5;
        }

        public void CreateWave12()
        {
            WaveList[11, 0] = 40;
            WaveList[11, 1] = 0;
            WaveList[11, 2] = 0;
        }

        public void CreateWave13()
        {
            WaveList[12, 0] = 15;
            WaveList[12, 1] = 10;
            WaveList[12, 2] = 5;
        }

        public void CreateWave14()
        {
            WaveList[13, 0] = 0;
            WaveList[13, 1] = 25;
            WaveList[13, 2] = 10;
        }

        public void CreateWave15()
        {
            WaveList[14, 0] = 50;
            WaveList[14, 1] = 10;
            WaveList[14, 2] = 3;
        }

        public void CreateWave16()
        {
            WaveList[15, 0] = 0;
            WaveList[15, 1] = 30;
            WaveList[15, 2] = 5;
        }

        public void CreateWave17()
        {
            WaveList[16, 0] = 60;
            WaveList[16, 1] = 5;
            WaveList[16, 2] = 5;
        }

        public void CreateWave18()
        {
            WaveList[17, 0] = 0;
            WaveList[17, 1] = 35;
            WaveList[17, 2] = 10;
        }

        public void CreateWave19()
        {
            WaveList[18, 0] = 70;
            WaveList[18, 1] = 0;
            WaveList[18, 2] = 5;
        }

        public void CreateWave20()
        {
            WaveList[19, 0] = 20;
            WaveList[19, 1] = 20;
            WaveList[19, 2] = 10;
        }

        public void CreateWave21()
        {
            WaveList[20, 0] = 80;
            WaveList[20, 1] = 10;
            WaveList[20, 2] = 5;
        }

        public void CreateWave22()
        {
            WaveList[21, 0] = 0;
            WaveList[21, 1] = 50;
            WaveList[21, 2] = 15;
        }

        public void CreateWave23()
        {
            WaveList[22, 0] = 90;
            WaveList[22, 1] = 0;
            WaveList[22, 2] = 10;
        }

        public void CreateWave24()
        {
            WaveList[23, 0] = 30;
            WaveList[23, 1] = 30;
            WaveList[23, 2] = 15;
        }

        public void CreateWave25()
        {
            WaveList[24, 0] = 100;
            WaveList[24, 1] = 20;
            WaveList[24, 2] = 10;
        }

        public void CreateWave26()
        {
            WaveList[25, 0] = 0;
            WaveList[25, 1] = 60;
            WaveList[25, 2] = 20;
        }

        public void CreateWave27()
        {
            WaveList[26, 0] = 120;
            WaveList[26, 1] = 10;
            WaveList[26, 2] = 15;
        }

        public void CreateWave28()
        {
            WaveList[27, 0] = 0;
            WaveList[27, 1] = 80;
            WaveList[27, 2] = 20;
        }

        public void CreateWave29()
        {
            WaveList[28, 0] = 150;
            WaveList[28, 1] = 0;
            WaveList[28, 2] = 10;
        }

        public void CreateWave30()
        {
            WaveList[29, 0] = 50;
            WaveList[29, 1] = 50;
            WaveList[29, 2] = 30;
        }

        public void CreateWave31()
        {
            WaveList[30, 0] = 160;
            WaveList[30, 1] = 10;
            WaveList[30, 2] = 20;
        }

        public void CreateWave32()
        {
            WaveList[31, 0] = 0;
            WaveList[31, 1] = 100;
            WaveList[31, 2] = 30;
        }

        public void CreateWave33()
        {
            WaveList[32, 0] = 170;
            WaveList[32, 1] = 20;
            WaveList[32, 2] = 25;
        }

        public void CreateWave34()
        {
            WaveList[33, 0] = 0;
            WaveList[33, 1] = 120;
            WaveList[33, 2] = 35;
        }

        public void CreateWave35()
        {
            WaveList[34, 0] = 200;
            WaveList[34, 1] = 0;
            WaveList[34, 2] = 20;
        }

        public void CreateWave36()
        {
            WaveList[35, 0] = 50;
            WaveList[35, 1] = 50;
            WaveList[35, 2] = 40;
        }

        public void CreateWave37()
        {
            WaveList[36, 0] = 210;
            WaveList[36, 1] = 20;
            WaveList[36, 2] = 30;
        }

        public void CreateWave38()
        {
            WaveList[37, 0] = 0;
            WaveList[37, 1] = 140;
            WaveList[37, 2] = 40;
        }

        public void CreateWave39()
        {
            WaveList[38, 0] = 220;
            WaveList[38, 1] = 30;
            WaveList[38, 2] = 35;
        }

        public void CreateWave40()
        {
            WaveList[39, 0] = 0;
            WaveList[39, 1] = 160;
            WaveList[39, 2] = 50;
        }

        public void CreateWave41()
        {
            WaveList[40, 0] = 250;
            WaveList[40, 1] = 0;
            WaveList[40, 2] = 30;
        }

        public void CreateWave42()
        {
            WaveList[41, 0] = 60;
            WaveList[41, 1] = 70;
            WaveList[41, 2] = 60;
        }

        public void CreateWave43()
        {
            WaveList[42, 0] = 260;
            WaveList[42, 1] = 20;
            WaveList[42, 2] = 45;
        }

        public void CreateWave44()
        {
            WaveList[43, 0] = 0;
            WaveList[43, 1] = 180;
            WaveList[43, 2] = 60;
        }

        public void CreateWave45()
        {
            WaveList[44, 0] = 300;
            WaveList[44, 1] = 40;
            WaveList[44, 2] = 50;
        }

        public void CreateWave46()
        {
            WaveList[45, 0] = 0;
            WaveList[45, 1] = 200;
            WaveList[45, 2] = 70;
        }

        public void CreateWave47()
        {
            WaveList[46, 0] = 320;
            WaveList[46, 1] = 30;
            WaveList[46, 2] = 55;
        }

        public void CreateWave48()
        {
            WaveList[47, 0] = 0;
            WaveList[47, 1] = 220;
            WaveList[47, 2] = 75;
        }

        public void CreateWave49()
        {
            WaveList[48, 0] = 350;
            WaveList[48, 1] = 0;
            WaveList[48, 2] = 60;
        }

        public void CreateWave50()
        {
            WaveList[49, 0] = 80;
            WaveList[49, 1] = 80;
            WaveList[49, 2] = 80;
        }

        public void CreateWave51()
        {
            WaveList[50, 0] = 360;
            WaveList[50, 1] = 40;
            WaveList[50, 2] = 65;
        }

        public void CreateWave52()
        {
            WaveList[51, 0] = 0;
            WaveList[51, 1] = 240;
            WaveList[51, 2] = 85;
        }

        public void CreateWave53()
        {
            WaveList[52, 0] = 380;
            WaveList[52, 1] = 50;
            WaveList[52, 2] = 70;
        }

        public void CreateWave54()
        {
            WaveList[53, 0] = 0;
            WaveList[53, 1] = 260;
            WaveList[53, 2] = 90;
        }

        public void CreateWave55()
        {
            WaveList[54, 0] = 400;
            WaveList[54, 1] = 60;
            WaveList[54, 2] = 75;
        }

        public void CreateWave56()
        {
            WaveList[55, 0] = 0;
            WaveList[55, 1] = 280;
            WaveList[55, 2] = 95;
        }

        public void CreateWave57()
        {
            WaveList[56, 0] = 420;
            WaveList[56, 1] = 70;
            WaveList[56, 2] = 80;
        }

        public void CreateWave58()
        {
            WaveList[57, 0] = 0;
            WaveList[57, 1] = 300;
            WaveList[57, 2] = 100;
        }

        public void CreateWave59()
        {
            WaveList[58, 0] = 450;
            WaveList[58, 1] = 80;
            WaveList[58, 2] = 85;
        }

        public void CreateWave60()
        {
            WaveList[59, 0] = 0;
            WaveList[59, 1] = 320;
            WaveList[59, 2] = 110;
        }

        public void CreateWave61()
        {
            WaveList[60, 0] = 480;
            WaveList[60, 1] = 90;
            WaveList[60, 2] = 90;
        }

        public void CreateWave62()
        {
            WaveList[61, 0] = 0;
            WaveList[61, 1] = 340;
            WaveList[61, 2] = 120;
        }

        public void CreateWave63()
        {
            WaveList[62, 0] = 500;
            WaveList[62, 1] = 100;
            WaveList[62, 2] = 95;
        }

        public void CreateWave64()
        {
            WaveList[63, 0] = 0;
            WaveList[63, 1] = 360;
            WaveList[63, 2] = 130;
        }

        public void CreateWave65()
        {
            WaveList[64, 0] = 520;
            WaveList[64, 1] = 110;
            WaveList[64, 2] = 100;
        }

        public void CreateWave66()
        {
            WaveList[65, 0] = 0;
            WaveList[65, 1] = 380;
            WaveList[65, 2] = 140;
        }

        public void CreateWave67()
        {
            WaveList[66, 0] = 540;
            WaveList[66, 1] = 120;
            WaveList[66, 2] = 105;
        }

        public void CreateWave68()
        {
            WaveList[67, 0] = 0;
            WaveList[67, 1] = 400;
            WaveList[67, 2] = 150;
        }

        public void CreateWave69()
        {
            WaveList[68, 0] = 560;
            WaveList[68, 1] = 130;
            WaveList[68, 2] = 110;
        }

        public void CreateWave70()
        {
            WaveList[69, 0] = 0;
            WaveList[69, 1] = 420;
            WaveList[69, 2] = 160;
        }

        public void CreateWave71()
        {
            WaveList[70, 0] = 580;
            WaveList[70, 1] = 140;
            WaveList[70, 2] = 115;
        }

        public void CreateWave72()
        {
            WaveList[71, 0] = 0;
            WaveList[71, 1] = 440;
            WaveList[71, 2] = 170;
        }

        public void CreateWave73()
        {
            WaveList[72, 0] = 600;
            WaveList[72, 1] = 150;
            WaveList[72, 2] = 120;
        }

        public void CreateWave74()
        {
            WaveList[73, 0] = 0;
            WaveList[73, 1] = 460;
            WaveList[73, 2] = 180;
        }

        public void CreateWave75()
        {
            WaveList[74, 0] = 620;
            WaveList[74, 1] = 160;
            WaveList[74, 2] = 125;
        }

        public void CreateWave76()
        {
            WaveList[75, 0] = 0;
            WaveList[75, 1] = 480;
            WaveList[75, 2] = 190;
        }

        public void CreateWave77()
        {
            WaveList[76, 0] = 640;
            WaveList[76, 1] = 170;
            WaveList[76, 2] = 130;
        }

        public void CreateWave78()
        {
            WaveList[77, 0] = 0;
            WaveList[77, 1] = 500;
            WaveList[77, 2] = 200;
        }

        public void CreateWave79()
        {
            WaveList[78, 0] = 660;
            WaveList[78, 1] = 180;
            WaveList[78, 2] = 135;
        }

        public void CreateWave80()
        {
            WaveList[79, 0] = 0;
            WaveList[79, 1] = 520;
            WaveList[79, 2] = 210;
        }
    }
}
