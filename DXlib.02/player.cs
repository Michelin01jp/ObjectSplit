using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game;

namespace Player
{
    class player
    {
        public static player Player = new player();

        public enum Input
        {
            up,
            right,
            down,
            left
        }
        public enum State
        { 
            stand,
            jump,
            squrt,
            move,
            attack
        }
        public struct Content
        {
            public double X;
            public double Y;
            public double Spd;
        }

        public Content content = new Content();

        public void PlayerOperation(Input inputdata)
        {
            float Direction = 0;

            if ((inputdata & Input.up) == Input.up)
                ;
            else if ((inputdata & Input.right) == Input.right)
                Direction = 1;
            else if ((inputdata & Input.left) == Input.left)
                Direction = -1;
            else if ((inputdata & Input.down) == Input.down)
                ;



            return;
        }
    }
}