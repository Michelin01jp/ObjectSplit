using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;

using Shapes;

class Physics
{
    public static World m_world;      // Box2D World

    static float timeStep = 1.0f / 60.0f;
    static int velocityIterations = 6;
    static int positionIterations = 2;

    /// <summary>
    /// 物理エンジンの初期化を行う
    /// </summary>
    public static void Init()
    {
        // World生成
        AABB worldAABB = new AABB();
        worldAABB.LowerBound.Set(-500.0f, -500.0f);
        worldAABB.UpperBound.Set(500.0f, 500.0f);

        Vec2 gravity = new Vec2(0.0f, 10.0f);
        bool doSleep = true;
        m_world = new World(worldAABB, gravity, doSleep);

        // Ground作成
        BodyDef groundBodyDef = new BodyDef();
        groundBodyDef.Position.Set(0.0f, 19.2f);
        Body groundBody = m_world.CreateBody(groundBodyDef);

        PolygonDef groundShapeDef = new PolygonDef();
        groundShapeDef.SetAsBox(50.0f, 10.0f);
        groundShapeDef.Density = 1.0f;
        groundShapeDef.Friction = 0.3f;

        groundBody.CreateFixture(groundShapeDef);

        return;
    }

    /// <summary>
    /// 物理エンジンの処理を行う
    /// </summary>
    /// <returns>状態(-1:何らかの不具合)</returns>
    public static int MainOperation(double spd)
    {
        m_world.Step((float)(timeStep * spd), velocityIterations, positionIterations);
        ObjectShape.Operation();

        return 0;
    }
}