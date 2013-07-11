using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;

using Game;

namespace Shapes
{
    public abstract class Object
    {
        /// <summary>
        /// オブジェクトの構造体
        /// </summary>
        public struct sObject
        {
            public sObject(byte ini)
            {
                Index = 0;
                Time = 0;
                body = null;
                bodyDef = new BodyDef();
                polygonDef = new PolygonDef();
                centerDis = new double[0];
                centerAng = new double[0];
            }
            public int Index;
            public double Time;
            public Body body;
            public BodyDef bodyDef;
            public PolygonDef polygonDef;
            public double[] centerDis;
            public double[] centerAng;
        }

        public sObject obj;

        /// <summary>
        /// 頂点を時計回りにソートする
        /// </summary>
        /// <param name="Sort">ソートする頂点の配列</param>
        /// <param name="angle">ソートに使用する角度</param>
        /// <param name="avex">中心座標X</param>
        /// <param name="avey">中心座標Y</param>
        /// <returns>ソートした配列</returns>
        public static Vec2[] SortPosition(Vec2[] Sort, out double[] angle, out Vec2 Ave)
        {
            angle = new double[Sort.Length];
            int[] sort = new int[Sort.Length];
            Ave = new Vec2();

            for (int i = 0; i < Sort.Length; i++)
            {
                Ave += Sort[i];
            }
            Ave.X /= Sort.Length;
            Ave.Y /= Sort.Length;

            for (int i = 0; i < Sort.Length; i++)
            {
                angle[i] = System.Math.Atan2((Sort[i].Y -= Ave.Y), (Sort[i].X -= Ave.X));
            }

            for (int i = 0; i < Sort.Length; i++)
            {
                for (int j = i + 1; j < Sort.Length; j++)
                {
                    if (angle[j] < angle[i])
                    {
                        double tmp1 = angle[j];
                        Vec2 tmp2 = Sort[j];

                        for (int k = j; k > i; k--)
                        {
                            angle[k] = angle[k - 1];
                            Sort[k] = Sort[k - 1];
                        }
                        angle[i] = tmp1;
                        Sort[i] = tmp2;
                    }
                }
            }

            return Sort;
        }

        /// <summary>
        /// ベクトルの外積
        /// </summary>
        /// <param name="v1">始点</param>
        /// <param name="v2">終点</param>
        /// <returns>外積</returns>
        protected float Vec2ExtPro(Vec2 v1, Vec2 v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }
    }

    public class ObjectShape : Object
    {
        object Obj;
        static int ObjectMax = 300;
        static List<ObjectShape> ObjectList = new List<ObjectShape>(ObjectMax);

        private void Process()
        {
            obj.Time += OperationGameMain.GameSpeed;

            return;
        }

        /// <summary>
        /// オブジェクトの生成を行う
        /// </summary>
        /// <param name="Index">リスト上のインデックス</param>
        /// <param name="pos">ワールド上に生成するオブジェクトの位置</param>
        /// <param name="angle">ワールド上に生成するオブジェクトの角度</param>
        /// <param name="Vzero">オブジェクトの初速</param>
        /// <param name="type">オブジェクトの種類</param>
        /// <param name="dot">オブジェクトの図形の頂点の位置(index > 2 ^ 時計回り)</param>
        private void Born(int Index, Vec2 pos, double angle, Vec2 Vzero, int type, params Vec2[] dot)
        {
            obj.Index = Index;
            if (dot.Length < 3)
                goto delete;

            obj = new sObject(0);
            Vec2 Ave;
            dot = SortPosition(dot, out obj.centerAng, out Ave);

            obj.bodyDef.Position = pos;
            obj.bodyDef.Angle = (float)angle;
            obj.bodyDef.LinearVelocity = Vzero;
            obj.body = Physics.m_world.CreateBody(obj.bodyDef);

            if (obj.body == null)
            {
                goto delete;
            }

            switch (type)
            {
                case 0:
                    obj.body.IsStatic();
                    break;
                default:
                    obj.body.IsDynamic();
                    break;
            }

            obj.polygonDef.VertexCount = dot.Length;
            obj.polygonDef.Vertices = dot;
            obj.polygonDef.Density = 1.0f;
            obj.polygonDef.Friction = 0.5f;

            if (obj.body.CreateFixture(obj.polygonDef) == null)
                goto delete;
            obj.body.SetMassFromShapes();

            obj.centerDis = new double[dot.Length];
            for (int i = 0; i < dot.Length; i++)
                obj.centerDis[i] = System.Math.Sqrt(main.Distance(obj.polygonDef.Vertices[i], obj.body.GetLocalCenter()));

            return;

        delete:
            {
                Delete(Index);
                return;
            }
        }

        /// <summary>
        /// オブジェクトを削除する
        /// </summary>
        public static void Delete(int Index)
        {
            if (ObjectList[Index].obj.body != null)
            {
                if (ObjectList[Index].obj.body.GetFixtureList() != null)
                    ObjectList[Index].obj.body.DestroyFixture(ObjectList[Index].obj.body.GetFixtureList());
                Physics.m_world.DestroyBody(ObjectList[Index].obj.body);
            }
            ObjectList.RemoveAt(Index);

            for (int i = 0; i < ObjectList.Count; i++)
                ObjectList[i].obj.Index = i;

            return;
        }

        public Vec2[] GetDrawPos(double scale)
        {
            Vec2[] Return = new Vec2[obj.polygonDef.VertexCount];

            scale *= DrawGameMain.Zooming;
            for (int i = 0; i < Return.Length; i++)
            {
                double Angle = obj.body.GetAngle() + obj.centerAng[i];
                double AngleX = System.Math.Cos(Angle);
                double AngleY = System.Math.Sin(Angle);
                Return[i] = new Vec2((float)((obj.body.GetPosition().X + obj.centerDis[i] * AngleX) * scale - DrawGameMain.GetScrollX()), (float)((obj.body.GetPosition().Y + obj.centerDis[i] * AngleY) * scale) - DrawGameMain.GetScrollY());
            }

            return Return;
        }

        public Vec2[] GetLocalPos()
        {
            Vec2[] Return = new Vec2[obj.polygonDef.VertexCount];

            for (int i = 0; i < Return.Length; i++)
            {
                double Angle = obj.body.GetAngle() + obj.centerAng[i];
                double AngleX = System.Math.Cos(Angle);
                double AngleY = System.Math.Sin(Angle);
                Return[i] = new Vec2((float)(obj.centerDis[i] * AngleX), (float)(obj.centerDis[i] * AngleY));
            }

            return Return;
        }

        /// <summary>
        /// 図形を二つに分割する
        /// </summary>
        /// <param name="Line">分割する直線状の点の位置(V1の始点, V1の終点, V2の始点, V2の終点)</param>
        public bool Split(int index, params Vec2[] Line)
        {
            // 例外
            if (Line.Length != 2 || obj.Time < 20)
                return false;

            // 線分を図形との相対座標に変換
            for (int i = 0; i < Line.Length; i++)
                Line[i] -= obj.body.GetPosition();

            var j = 0;
            var CrsMax = 2;
            var Index = new int[CrsMax];
            var NewPos = new Vec2[CrsMax];
            var Ave = new Vec2[2];
            var Shapes = false;
            
            var LineAngle = System.Math.Atan2(Line[1].X - Line[0].X, Line[1].Y - Line[0].Y);

            var ShapeDot = GetLocalPos();

            List<Vec2>[] NewShape = new List<Vec2>[2];

            // 交点求める
            for (int i = 0; i < obj.polygonDef.Vertices.Length; i++)
            {
                var CrsPos = new Vec2();
                var p1 = i;
                var p2 = i + 1;
                if (p2 == obj.polygonDef.Vertices.Length)
                    p2 = 0;

                // 衝突判定から交点を得る
                if (Collision.collision.LineLineIntersect(Line[0], Line[1], ShapeDot[p1], ShapeDot[p2], out CrsPos))
                {
                    double Dis = System.Math.Sqrt(main.Distance(CrsPos, new Vec2()));
                    double Angle = System.Math.Atan2(CrsPos.Y, CrsPos.X) - obj.body.GetAngle();

                    Index[j] = i;
                    NewPos[j] = new Vec2((float)(Dis * System.Math.Cos(Angle)), (float)(Dis * System.Math.Sin(Angle)));

                    for (int k = 0; k < 2; k++)
                    {
                        if (j == 0)
                            NewShape[k] = new List<Vec2>();
                        NewShape[k].Add(NewPos[j]);
                        Ave[k] += NewPos[j];
                    }
                    j++;

                    if (j == CrsMax)
                    {
                        Shapes = true;
                        break;
                    }
                }
            }

            if (Shapes)
            {
                for (int i = 0; i < obj.polygonDef.Vertices.Length; i++)
                {
                    for (int k = 0; k < CrsMax; k++)
                        if (main.Distance(NewPos[k], obj.polygonDef.Vertices[i]) < 0.00125)
                        {
                            Console.WriteLine("{0} :{1}", operation.GetFrame(), i);
                            goto Jump;
                        }

                    if (i > Index[0] && i <= Index[1])
                    {
                        Console.WriteLine("{0} Add 1 :{1}", operation.GetFrame(), i);
                        NewShape[0].Add(obj.polygonDef.Vertices[i]);
                        Ave[0] += obj.polygonDef.Vertices[i];
                    }
                    else
                    {
                        Console.WriteLine("{0} Add 2 :{1}", operation.GetFrame(), i);
                        NewShape[1].Add(obj.polygonDef.Vertices[i]);
                        Ave[1] += obj.polygonDef.Vertices[i];
                    }

                Jump: ;
                }

                for (int i = 0; i < 2; i++)
                {
                    Ave[i] = new Vec2(Ave[i].X / NewShape[i].Count, Ave[i].Y / NewShape[i].Count);

                    double Angle = 30;

                    if (i == 1)
                        Angle += 90;

                    Angle = LineAngle + main.Angle(Angle);

                    var Velocity = obj.body.GetLinearVelocity() + new Vec2(
                            (float)(System.Math.Cos(Angle) * 0.7071),
                            (float)(System.Math.Sin(Angle) * 0.7071));

                    BornObject(obj.body.GetPosition() + Ave[i], obj.body.GetAngle(), Velocity, 1, NewShape[i].ToArray());
                }

                Delete(index);
            }

            return Shapes;
        }

        public static void Init()
        {
            while (ObjectList.Count != 0)
            {
                Delete(0);
            }

            return;
        }

        public static void Operation()
        {
            for (int i = 0; i < ObjectList.Count; i++)
            {
                ObjectList[i].Process();
                if (ObjectList[i].Split(i, new Vec2[] { new Vec2(6.5f, -2.5f), new Vec2(6.5f, 120.0f) }))
                    i--;
            }
        }

        /// <summary>
        /// オブジェクトを生成する
        /// </summary>
        /// <param name="pos">ワールド上のオブジェクトの位置</param>
        /// <param name="angle">ワールド上のオブジェクトの角度</param>
        /// <param name="Vzero">オブジェクトの初速</param>
        /// <param name="type">オブジェクトの種類</param>
        /// <param name="dot">オブジェクトの図形の頂点の位置(index > 2 ^ 時計回り)</param>
        public static void BornObject(Vec2 pos, double angle, Vec2 Vzero, int type, params Vec2[] dot)
        {
            ObjectList.Add(new ObjectShape());

            int Index = ObjectList.Count - 1;
            ObjectList[Index].Born(Index, pos, angle, Vzero, type, dot);
        }

        /// <summary>
        /// リストの特定のインデックスを取得する
        /// </summary>
        /// <param name="index">取得するインデックス</param>
        /// <returns>取得したオブジェクト</returns>
        public static ObjectShape GetObject(int index)
        {
            if (index > ObjectList.Count || ObjectList.Count == 0)
                return null;

            return ObjectList[index];
        }

        public static int GetCount()
        {
            return ObjectList.Count;
        }
    }
}