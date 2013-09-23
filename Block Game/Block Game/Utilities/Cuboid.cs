using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockGame.Utilities;

namespace Block_Game.Utilities
{
    public class Cuboid
    {
        public Point3 Min;
        public Point3 Max;

        public Cuboid(Point3 min, Point3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        public bool Intersects(Cuboid other)
        {
            if (other.Min.IsInCuboid(this) | other.Max.IsInCuboid(this))
                return true;
            if (Min.IsInCuboid(other) | Max.IsInCuboid(other))
                return true;
            return false;
        }
    }
}
