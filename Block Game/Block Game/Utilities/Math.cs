///Math and math extensions for Block Game
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BlockGame.Utilities
{
    /// <summary>
    /// A class holding miscelaneous extenstions for other classes
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Checks if a given bit is set in this byte
        /// </summary>
        /// <param name="b">The byte to check</param>
        /// <param name="pos">The position to check at</param>
        /// <returns>True if the n'th bit is set</returns>
        public static bool IsBitSet(this byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        /// <summary>
        /// Returns true if either the X or y values are larger than the other
        /// vector's
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <returns>True if x or y in v1 is greater than the same component in
        /// v2</returns>
        public static bool IsGreater(this Vector2 v1, Vector2 v2)
        {
            return (v1.X > v2.X || v1.Y > v2.Y);
        }

        /// <summary>
        /// Wraps this float around a min and a max
        /// </summary>
        /// <param name="val">The value to wrap</param>
        /// <param name="min">The min value to wrap by</param>
        /// <param name="max">The max value to wrap by</param>
        /// <returns>val wrapped to min -> max</returns>
        public static float Wrap(this float val, float min, float max)
        {
            while (val < min)
                val += max - min;
            while (val > max)
                val -= max - min;
            return val;
        }
    }
}
