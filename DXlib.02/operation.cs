using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using DxLibDLL;

using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;

using Shapes;
using Player;

using Game;

static class operation
{
    private static int Frame = 0;

    private static byte[] KeyState = new byte[256];
    private static int[] KeyFrame = new int[256];

    public static int Operation()
    {
        int ReturnNum = 0;

        if (DX.ProcessMessage() == -1)
            return -1;

        if (main.FuncState != main.GAMEINIT)
            GetKeyInput();

        switch (main.FuncState)
        {
            case main.GAMEINIT:
                main.Init();
                OperationGameMain.Init();
                ResetFrame();
                main.FuncState = 100;
                break;
            case main.GAMEMAIN:
                OperationGameMain.Operation();
                break;
        }

        Frame++;

        return ReturnNum;
    }

    public static void ResetFrame()
    {
        Frame = 0;

        return;
    }
    public static int GetFrame()
    {
        return Frame;
    }

    private static int GetKeyInput()
    {
        for (int i = 0; i < 256; i++)
        {
            if (KeyState[i] == 1)
            {
                KeyFrame[i]++;
            }
            else
            {
                KeyFrame[i]--;
            }
        }

        DX.GetHitKeyStateAll(out KeyState[0]);

        for (int i = 0; i < 256; i++)
        {
            if (KeyState[i] == 1)
            {
                if (KeyFrame[i] < 0)
                    KeyFrame[i] = 0;
            }
            else
            {
                if (KeyFrame[i] > 0)
                    KeyFrame[i] = 0;
            }
        }

        return 0;
    }

    public static int GetKeyState(int key)
    {
        return KeyState[key];
    }
    public static int GetKeyFrame(int key)
    {
        return KeyFrame[key];
    }
}

static class OperationGameMain
{
    private static double gamespeed = 1.0;
    private static double speed = 1.0;

    public static void Operation()
    {
        if (operation.GetKeyFrame(DX.KEY_INPUT_Z) == 1)
            main.FuncState = main.GAMEINIT;

        if (operation.GetKeyFrame(DX.KEY_INPUT_W) % 2 == 1)
        {
            DrawGameMain.PanningScreen(0, -5);
        }
        if (operation.GetKeyFrame(DX.KEY_INPUT_S) % 2 == 1)
        {
            DrawGameMain.PanningScreen(0, 5);
        }
        if (operation.GetKeyFrame(DX.KEY_INPUT_D) % 2 == 1)
        {
            DrawGameMain.PanningScreen(5, 0);
        }
        if (operation.GetKeyFrame(DX.KEY_INPUT_A) % 2 == 1)
        {
            DrawGameMain.PanningScreen(-5, 0);
        }

        if (operation.GetKeyState(DX.KEY_INPUT_C) == 1)
                GameSpeed = 0.25;
        else
            GameSpeed = 1;

        if (operation.GetKeyFrame(DX.KEY_INPUT_X) == 1)
        {
            Vec2[] vec = new Vec2[3 + DX.GetRand(3)];

            //vec = new Vec2[4];

            for (int i = 0; i < vec.Length; i++)
            {
                vec[i] = new Vec2((float)System.Math.Cos(main.Angle(360 / vec.Length * i)), (float)System.Math.Sin(main.Angle(360 / vec.Length * i)));
            }

            ObjectShape.BornObject(new Vec2(6.5f, 0.5f), main.Angle(0), new Vec2(), 1, vec);

            //if (operation.GetFrame() % 60 == 0)
            //    ObjectShape.BornObject(new Vec2(6.5f, 0.5f), main.Angle(45), 1, new Vec2[] { new Vec2(-1, 0), new Vec2(0, -1), new Vec2(1, 0), new Vec2(0, 1) });
            //else
            //    ObjectShape.BornObject(new Vec2(6.5f, 0.5f), main.Angle(-90), 1, new Vec2[] { new Vec2(0, (float)-System.Math.Sqrt(3)), new Vec2(0, 0), new Vec2(-1, 0) });
        }

        player.Input input = new player.Input();

        player.Player.PlayerOperation(input);
        Physics.MainOperation(gamespeed);
    }

    public static void Init()
    {
        ObjectShape.Init();
        Physics.Init();
        DrawGameMain.Init();
        DX.SRand(DateTime.Now.Millisecond);
    }

    public static double GameSpeed
    {
        set
        {
            if (value < 0)
                return;
            gamespeed = value;
        }
        get
        {
            return gamespeed;
        }
    }
    public static double Speed
    {
        set
        {
            if (value < 0)
                return;
            speed = value;
        }
        get
        {
            return speed;
        }
    }
}