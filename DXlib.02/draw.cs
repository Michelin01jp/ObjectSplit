using System;
using DxLibDLL;

using Shapes;

using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;

using Game;

class draw
{
    static int time2 = 0;
    static int time1 = 0;
    static int fps = 0;
    static int frame;

    public static int Draw()
    {
        int ReturnNum = 0;

        DX.SetDrawScreen(DX.DX_SCREEN_BACK);
        DX.ClearDrawScreen();

        switch (main.FuncState)
        {
            case main.GAMEMAIN:
                DrawGameMain.Draw();
                break;
        }

        PutFPS();

        DX.ScreenFlip();

        return ReturnNum;
    }

    private static void PutFPS()
    {
        time2 = System.Environment.TickCount;

        if (time2 - time1 >= 1000)
        {
            time1 = System.Environment.TickCount;
            fps = frame;
            frame = 0;
        }

        DX.DrawString(0, 0, "FPS:" + fps, 0xffffff);

        frame++;

        return;
    }
}

class DrawGameMain
{
    static int ScrollX;
    static int ScrollY;
    static double scale;
    static double zooming;

    public static void Init()
    {
        DefaultScale();
        DefaultZooming();
        DefaultScreen();

        return;
    }

    /// <summary>
    /// プレイ中の主な描画を行う
    /// </summary>
    public static void Draw()
    {
        DrawShape();
        Zooming = 0.25;

        return;
    }

    private static void DrawShape()
    {
        for (int i = 0; i < ObjectShape.GetCount(); i++)
        {
            ObjectShape obj = ObjectShape.GetObject(i);
            if (obj != null)
            {
                Vec2[] vec = obj.GetDrawPos(Scale);
                for (int j = 0; j < vec.Length; j++)
                {
                    int p1 = j + 1;
                    if (j == vec.Length - 1)
                        p1 = 0;

                    DX.DrawLine((int)vec[j].X, (int)vec[j].Y, (int)vec[p1].X, (int)vec[p1].Y, 0xffffff);
                    DX.DrawString((int)vec[j].X, (int)vec[j].Y, "" + j, 0xffffff);
                }
            }
        }

        return;
    }

    /// <summary>
    /// 描画位置を移動させる
    /// </summary>
    /// <param name="x">X要素</param>
    /// <param name="y">Y要素</param>
    public static void PanningScreen(int x, int y)
    {
        ScrollX += x;
        ScrollY += y;

        return;
    }

    public static void DefaultScreen()
    {
        ScrollX = 0;
        ScrollY = 0;

        return;
    }

    public static int GetScrollX()
    {
        return ScrollX;
    }
    public static int GetScrollY()
    {
        return ScrollY;
    }

    /// <summary>
    /// 画面の拡大率を変更する(描画する範囲)
    /// </summary>
    public static double Zooming
    {
        set
        {
            zooming = value;
            if (value < 0.5)
                zooming = 0.5;
            if (value > 2.0)
                zooming = 2.0;
            return;
        }
        get
        {
            return zooming;
        }
    }
    /// <summary>
    /// 拡大率をデフォルトに戻す
    /// </summary>
    public static void DefaultZooming()
    {
        Zooming = 1.0;

        return;
    }

    public static double Scale
    {
        set
        {
            scale = value;
            if (scale <= 0)
                DefaultScale();
        }

        get
        {
            return scale;
        }
    }

    public static void DefaultScale()
    {
        Scale = 100;
    }
}