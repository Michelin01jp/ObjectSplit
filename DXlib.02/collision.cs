using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Box2DX.Common;

namespace Collision
{
    class collision
    {
        /// <summary>
        /// 直線と直線のあたり判定
        /// </summary>
        /// <param name="V1s">線分1の始点</param>
        /// <param name="V1v">線分1の終点</param>
        /// <param name="V2s">線分2の始点</param>
        /// <param name="V2v">線分2の終点</param>
        /// <param name="point">交点</param>
        /// <returns>交差しているか</returns>
        public static bool LineLineIntersect(Vec2 V1s, Vec2 V1v, Vec2 V2s, Vec2 V2v, out Vec2 point)
        {
            point = new Vec2();

            double r, s;
            double denominator = (V1v.X - V1s.X) * (V2v.Y - V2s.Y) - (V1v.Y - V1s.Y) * (V2v.X - V2s.X);

            //分母が０の場合平行
            if (denominator == 0)
            {
                return false;
            }

            double numeratorR = (V1s.Y - V2s.Y) * (V2v.X - V2s.X) - (V1s.X - V2s.X) * (V2v.Y - V2s.Y);
            r = numeratorR / denominator;

            double numeratorS = (V1s.Y - V2s.Y) * (V1v.X - V1s.X) - (V1s.X - V2s.X) * (V1v.Y - V1s.Y);
            s = numeratorS / denominator;

            //交差しない
            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            point.X = (float)(V1s.X + (r * (V1v.X - V1s.X)));
            point.Y = (float)(V1s.Y + (r * (V1v.Y - V1s.Y)));
            return true;
        }
    }
}
