using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Block_Game.Utilities
{
    public class SpineMath
    {
        public static Vector3 GetCrossNormal(Vector3 Normal)
        {
            if (Normal.X != 0)
            {
                Normal.Y = Normal.X;
                Normal.Z = Normal.X;
                Normal.X = 0;
            }

            if (Normal.Y != 0)
            {
                Normal.X = Normal.Y;
                Normal.Z = Normal.Y;
                Normal.Y = 0;
            }

            if (Normal.Z != 0)
            {
                Normal.X = Normal.Z;
                Normal.Y = Normal.Z;
                Normal.Z = 0;
            }

            return Normal;
        }
    }
}
