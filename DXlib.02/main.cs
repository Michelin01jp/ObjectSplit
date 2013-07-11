using System;
using System.IO;
using System.Reflection;
using System.Collections;
using DxLibDLL;

using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;

using Game;

namespace Game
{
    class main
    {
        public static double Scale = 1.0;
        public static byte WindowFlag = 1;
        public static int WindowSizeX = 640;
        public static int WindowSizeY = 480;
        public static int FuncState = 0;
        public static readonly double aRadi = System.Math.PI / 180;
        public static readonly string ExePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public const int GAMEINIT = 0;
        public const int GAMEMAIN = 100;

        public static void Main()
        {
            while (true)
            {
                if (operation.Operation() == -1)
                    break;
                if (draw.Draw() == -1)
                    break;
            }

            return;
        }

        public static void Init()
        {
            Setting.LoadSettingData();
            DX.SetWindowSize((int)(WindowSizeX * Scale), (int)(WindowSizeY * Scale));
            DX.ChangeWindowMode(WindowFlag);
            if (DX.DxLib_Init() == -1)
                return;
        }

        public static double Angle(double angle)
        {
            return aRadi * angle;
        }
        public static double Distance(double X1, double Y1, double X2, double Y2)
        {
            return (X1 - X2) * (X1 - X2) + (Y1 - Y2) * (Y1 - Y2);
        }
        public static double Distance(Vec2 V1, Vec2 V2)
        {
            return (V2.X - V1.X) * (V2.X - V1.X) + (V2.Y - V1.Y) * (V2.Y - V1.Y);
        }
    }
}

class FileInput
{
    public static ArrayList Input(string FilePath)
    {
        ArrayList list = new ArrayList();

        // ファイルの存在を確認する
        if (File.Exists(FilePath))
        {
            using (var r = new StreamReader(FilePath))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    list.Add(line);
                }

                if (list.Count == 0)
                    list = null;
            }
        }
        else
        {
            return null;
        }

        return list;
    }
}

static class Setting
{
    // ファイルの書式
    /*
     windowsize\tfullscreenflag
     */

    static string FilePath = main.ExePath + "\\Setting.ini";

    public struct setting
    {
        public setting(int a)
        {
            main.Scale = scale = 1.0;
            main.WindowFlag = fullscreen = 1;
        }

        public setting(double size,byte fullscreenflag)
        {
            main.Scale = scale = size;
            main.WindowFlag = fullscreen = fullscreenflag;
        }

        public double scale;
        public byte fullscreen;
    }
    static setting SettingData;

    public static void LoadSettingData()
    {
        var list = FileInput.Input(FilePath);

        // ファイルの存在を確認する
        if (list != null)
        {
            // foreachを使いファイルをデータ化する
            foreach (string line in list)
            {
                var str = line.Split('\t');

                switch (list.IndexOf(line))
                {
                    case 0: // 一行目
                        if (str.Length < 2)
                            goto write; // 書式が不正だったとき
                        double.TryParse(str[0],out SettingData.scale);
                        byte.TryParse(str[1], out SettingData.fullscreen);
                        break;
                }
            }

            return;
        }

        write:
        // 存在しなかった場合新しくファイルを出力する
        SettingData = new setting(0);
        WriteSettingData(SettingData, true);

        return;
    }

    public static void WriteSettingData(setting data, bool OverWriteFlag)
    {
        // OverWriteFlag = true → 上書き
        // データの書き込みを行う
        using (var w = new StreamWriter(FilePath, !OverWriteFlag))
        {
            w.WriteLine("{0}\t{1}", data.scale, data.fullscreen);
        }
    }
}