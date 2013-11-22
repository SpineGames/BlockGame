///Represents a cuboid in 3D space
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockGame.Utilities;

namespace BlockGame.Utilities
{
    /// <summary>
    /// Represents an axis aligned, integer co-ord cuboid
    /// </summary>
    public class Cuboid
    {
        /// <summary>
        /// The minimum value of this cuboid
        /// </summary>
        public Point3 Min;
        /// <summary>
        /// The maximum value of this cuboid
        /// </summary>
        public Point3 Max;

        /// <summary>
        /// Creates a new cuboid with the given min/max
        /// </summary>
        /// <param name="min">The minimum value of the cuboid</param>
        /// <param name="max">The maximum value of the cuboid</param>
        public Cuboid(Point3 min, Point3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Checks if the given co-ordinates intersect with this cuboid
        /// </summary>
        /// <param name="x">The x co-ord</param>
        /// <param name="y">The y co-ord</param>
        /// <param name="z">The z co-ord</param>
        /// <returns>True if {x,y,z} intersects this cuboid</returns>
        public bool Contains(int x, int y, int z)
        {
            return (x >= Min.X & x <= Max.X) & (y >= Min.Y & y <= Max.Y) &
                   (z >= Min.Z & z <= Max.Z);
        }

        /// <summary>
        /// Checks if this cuboid intersects another cuboid
        /// </summary>
        /// <param name="other">The cuboid to check against</param>
        /// <returns>True if this cuboid intersects another</returns>
        public bool Intersects(Cuboid other)
        {
            return ! (Min.X > other.Max.X || Min.Y > other.Max.Y || Min.Z > other.Max.Z);
        }
    }
}
