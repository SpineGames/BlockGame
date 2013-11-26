using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BlockGame.Utilities;

namespace BlockGame.Blocks
{

    /// <summary>
    /// Represents the different flags that a block can use for rendering
    /// </summary>
    [Flags]
    public enum BlockRenderStates
    {
        None = 0,
        Front = 1 << 0,
        Back = 1 << 2,
        Left = 1 << 3,
        Right = 1 << 4,
        Top = 1 << 5,
        Bottom = 1 << 6
    }

    /// <summary>
    /// Represents a cardinal direction
    /// </summary>
    public enum BlockFacing
    {
        Front,
        Back,
        Left,
        Right,
        Top,
        Bottom
    }

    /// <summary>
    /// An extension class for block facings
    /// </summary>
    public static class BlockFacingExt
    {
        /// <summary>
        /// Gets the normal for this block facing
        /// </summary>
        /// <param name="self">The block facing to get the normal from</param>
        /// <returns>The normal for this block facing</returns>
        public static Point3 NormalVector(this BlockFacing self)
        {
            switch (self)
            {
                case BlockFacing.Front:
                    return new Point3(0, 1, 0);
                case BlockFacing.Back:
                    return new Point3(0, -1, 0);
                case BlockFacing.Left:
                    return new Point3(-1, 0, 0);
                case BlockFacing.Right:
                    return new Point3(1, 0, 0);
                case BlockFacing.Top:
                    return new Point3(0, 0, 1);
                case BlockFacing.Bottom:
                    return new Point3(0, 0, -1);
                default: return null;
            }
        }

        /// <summary>
        /// Gets a normal that is 90° to the normal
        /// </summary>
        /// <param name="self">The block facing to get the cross normal from</param>
        /// <returns>A cross normal for this block facing</returns>
        public static Point3 CrossNormalVector(this BlockFacing self)
        {
            switch (self)
            {
                case BlockFacing.Front:
                    return new Point3(1, 0, 0);
                case BlockFacing.Back:
                    return new Point3(-1, 0, 0);
                case BlockFacing.Left:
                    return new Point3(0, 1, 0);
                case BlockFacing.Right:
                    return new Point3(0, -1, 0);
                case BlockFacing.Top:
                    return new Point3(1, 0, 0);
                case BlockFacing.Bottom:
                    return new Point3(1, 0, 0);
                default: return null;
            }
        }

        /// <summary>
        /// Gets a corner that is on a 90° plane to the normal
        /// </summary>
        /// <param name="self">The block facing to get the cross normal from</param>
        /// <returns>A corner vecto for this block facing</returns>
        public static Point3 CornerVector(this BlockFacing self)
        {
            switch (self)
            {
                case BlockFacing.Front:
                    return new Point3(1, 0, 1);
                case BlockFacing.Back:
                    return new Point3(1, 0, 1);
                case BlockFacing.Left:
                    return new Point3(0, 1, 1);
                case BlockFacing.Right:
                    return new Point3(0, 1, 1);
                case BlockFacing.Top:
                    return new Point3(1, 1, 0);
                case BlockFacing.Bottom:
                    return new Point3(1, 1, 0);
                default: return null;
            }
        }

        /// <summary>
        /// Converts this direction in degrees to a block facing (left, right, front, back)
        /// </summary>
        /// <param name="direction">The direction in <b>degrees</b></param>
        /// <returns>The block facing for the given angle</returns>
        public static BlockFacing ToBlockFacing(this float direction)
        {
            direction = direction.Wrap(0, 360);

            if (direction > 45 & direction < 135)
                return BlockFacing.Front;
            if (direction >= 135 & direction <= 225)
                return BlockFacing.Left;
            if (direction > 225 & direction < 315)
                return BlockFacing.Back;
            return BlockFacing.Right;
        }

        /// <summary>
        /// Converts this normal into a block facing
        /// </summary>
        /// <param name="normal">The normal to convert</param>
        /// <returns>The block facing from this normal</returns>
        public static BlockFacing ToBlockFacing(this Vector3 normal)
        {
            normal.Normalize();
            normal.X = (float)Math.Round(normal.X);
            normal.Y = (float)Math.Round(normal.Y);
            normal.Z = (float)Math.Round(normal.Z);

            if (normal.X == -1)
                return BlockFacing.Left;
            if (normal.X == 1)
                return BlockFacing.Right;
            if (normal.Y == -1)
                return BlockFacing.Back;
            if (normal.Y == 1)
                return BlockFacing.Front;
            if (normal.Z == -1)
                return BlockFacing.Bottom;

            return BlockFacing.Top;
        }

        /// <summary>
        /// Checks if this render state has a block facing set
        /// </summary>
        /// <param name="self">The render state to check</param>
        /// <param name="facing">The facing to check for</param>
        /// <returns>True if there is a flag for <i>facing</i></returns>
        public static bool IsFaced(this BlockRenderStates self, BlockFacing facing)
        {
            switch (facing)
            {
                case BlockFacing.Back:
                    return self.HasFlag(BlockRenderStates.Back);
                case BlockFacing.Front:
                    return self.HasFlag(BlockRenderStates.Front);
                case BlockFacing.Left:
                    return self.HasFlag(BlockRenderStates.Left);
                case BlockFacing.Right:
                    return self.HasFlag(BlockRenderStates.Right);
                case BlockFacing.Top:
                    return self.HasFlag(BlockRenderStates.Top);
                case BlockFacing.Bottom:
                    return self.HasFlag(BlockRenderStates.Bottom);
                default:
                    return false;
            }
        }
    }

}
