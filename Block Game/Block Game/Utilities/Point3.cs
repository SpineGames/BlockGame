///REpresents a point in 3D space that has integer co-ordinates
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Block_Game.Utilities;

namespace BlockGame.Utilities
{
    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    public class Point3
    {
        public static Point3 One
        {
            get { return new Point3(1, 1, 1); }
        }
        public int X;
        public int Y;
        public int Z;

        public Point3(int size)
        {
            this.X = size;
            this.Y = size;
            this.Z = size;
        }

        public Point3(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public bool IsInCuboid(Point3 min, Point3 max)
        {
            return IsInCuboid(this, min, max);
        }

        public bool IsInCuboid(Cuboid cuboid)
        {
            return IsInCuboid(this, cuboid);
        }

        public static bool IsInCuboid(Point3 p, Cuboid cuboid)
        {
            return (p.X >= cuboid.Min.X & p.X <= cuboid.Max.X) & (p.Y >= cuboid.Min.Y & p.Y <= cuboid.Max.Y) &
                (p.Z >= cuboid.Min.Z & p.Z <= cuboid.Max.Z);
        }

        public static bool IsInCuboid(Point3 p, Point3 min, Point3 max)
        {
            return (p.X >= min.X & p.X <= max.X) & (p.Y >= min.Y & p.Y <= max.Y) & 
                (p.Z >= min.Z & p.Z <= max.Z);
        }

        public void Clamp(Point3 min, Point3 max)
        {
            Point3 t = Clamp(this, min, max);
            this.X = t.X;
            this.Y = t.Y;
            this.Z = t.Z;
        }

        public static Point3 Clamp(Point3 p, Point3 min, Point3 max)
        {
            return new Point3(
                (int)MathHelper.Clamp(p.X, min.X, max.X),
                (int)MathHelper.Clamp(p.Y, min.Y, max.Y),
                (int)MathHelper.Clamp(p.Z, min.Z, max.Z));
        }

        public static Point3 operator -(Point3 p1, Point3 p2)
        {
            return new Point3(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        public static Point3 operator +(Point3 p1, Point3 p2)
        {
            return new Point3(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        public static Point3 operator *(Point3 p1, Point3 p2)
        {
            return new Point3(p2.X * p1.X, p2.Y * p1.Y, p2.Z * p1.Z);
        }

        public static Point3 operator /(Point3 p1, Point3 p2)
        {
            return new Point3(p2.X / p1.X, p2.Y / p1.Y, p2.Z / p1.Z);
        }

        public static Point3 operator *(Point3 p1, int p2)
        {
            return new Point3(p2* p1.X, p2 * p1.Y, p2 * p1.Z);
        }

        public static Point3 operator /(Point3 p1, int p2)
        {
            return new Point3(p1.X / p2, p1.Y / p2, p1.Z / p2);
        }

        public static implicit operator Vector3(Point3 point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }

        public float Distance(Point3 p2)
        {
            return (float)Math.Sqrt(X * p2.X + Y * p2.Y + Z * p2.Z);
        }

        public static float Distance(Point3 p1, Point3 p2)
        {
            return (float)Math.Sqrt(p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", X, Y, Z);
        }
    }
}
