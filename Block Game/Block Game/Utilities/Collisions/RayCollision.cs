using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Block_Game.Utilities.Collisions
{
    /// <summary>
    /// Represents a collision caused by a ray
    /// </summary>
    public class RayCollision
    {
        /// <summary>
        /// The point of collision
        /// </summary>
        public readonly Vector3 Position;
        /// <summary>
        /// The distance from the position to the ray caster
        /// </summary>
        public readonly float Length;
        /// <summary>
        /// The normal facing towards the ray caster
        /// </summary>
        public readonly Vector3 Normal;

        /// <summary>
        /// Creates a new instance of a ray collision
        /// </summary>
        /// <param name="ray">The ray that invoked the collision</param>
        /// <param name="length">The length of the ray</param>
        public RayCollision(Ray ray, float length)
        {
            this.Position = ray.Position + (ray.Direction * length);
            this.Normal = -ray.Direction;
            this.Normal.Normalize();
            this.Length = length;
        }
    }
}
