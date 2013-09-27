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

    public static class Extensions
    {
        /// <summary>
        /// Returns true if either the X or y values are larger than the other
        /// vector's
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool IsGreater(this Vector2 v1, Vector2 v2)
        {
            return (v1.X > v2.X || v1.Y > v2.Y);
        }

        public static float Wrap(this float val, float min, float max)
        {
            while (val < min)
                val += max - min;
            while (val > max)
                val -= max - min;
            return val;
        }
    }

    public class TrackableVariable
    {
        object val;
        public object Value
        {
            get{return val;}
            set
            {
                val = value;
                if (valueChanged != null)
                    valueChanged.Invoke(new ObjectiveEventArgs(val));
            }
        }

        public ObjectiveEventHandler valueChanged;
    }
    
    public delegate void ObjectiveEventHandler(ObjectiveEventArgs e);

    public class ObjectiveEventArgs : EventArgs
    {
        public object Value { get; set; }

        public ObjectiveEventArgs(object value)
        {
            this.Value = value;
        }
    }
}
