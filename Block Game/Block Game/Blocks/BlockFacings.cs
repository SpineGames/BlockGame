﻿using System;
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
    public enum BlockRenderStates : byte
    {
        None = 0,
        Front = 1,
        Back = 2,
        Left = 4,
        Right = 8,
        Top = 16,
        Bottom = 32
    }

    /// <summary>
    /// Represents a cardinal direction
    /// </summary>
    public enum BlockFacing : byte
    {
        Front = 1,
        Back = 2,
        Left = 4,
        Right = 8,
        Top = 16,
        Bottom = 32
    }

    /// <summary>
    /// An extension class for block facings
    /// </summary>
    public static class BlockFacingExt
    {
        private static BlockFacing[] facings;
        public static BlockFacing[] Facings
        {
            get { return facings; }
        }

        private static Point3 Front = new Point3(0, 1, 0);
        private static Point3 Back = new Point3(0, -1, 0);
        private static Point3 Left = new Point3(-1, 0, 0);
        private static Point3 Right = new Point3(1, 0, 0);
        private static Point3 Top = new Point3(0, 0, 1);
        private static Point3 Bottom = new Point3(0, 0, -1);

        private static Point3 FrontCorner = new Point3(1, 0, 1);
        private static Point3 BackCorner = new Point3(1, 0, 1);
        private static Point3 RightCorner = new Point3(0, 1, 1);
        private static Point3 LeftCorner = new Point3(0, 1, 1);
        private static Point3 TopCorner = new Point3(1, 1, 0);
        private static Point3 BottomCorner = new Point3(1, 1, 0);

        static BlockFacingExt()
        {
            facings = (BlockFacing[])Enum.GetValues(typeof(BlockFacing));
        }

        public static int FaceCount(this BlockRenderStates self)
        {
            return ((byte)self).BitCount();
        }

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
                    return Front;
                case BlockFacing.Back:
                    return Back;
                case BlockFacing.Left:
                    return Left;
                case BlockFacing.Right:
                    return Right;
                case BlockFacing.Top:
                    return Top;
                case BlockFacing.Bottom:
                    return Bottom;
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
                    return Right;
                case BlockFacing.Back:
                    return Left;
                case BlockFacing.Left:
                    return Front;
                case BlockFacing.Right:
                    return Back;
                case BlockFacing.Top:
                    return Right;
                case BlockFacing.Bottom:
                    return Left;
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
                    return FrontCorner;
                case BlockFacing.Back:
                    return BackCorner;
                case BlockFacing.Left:
                    return LeftCorner;
                case BlockFacing.Right:
                    return RightCorner;
                case BlockFacing.Top:
                    return TopCorner;
                case BlockFacing.Bottom:
                    return BottomCorner;
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
            return ((byte)self & (byte)facing) != 0;
        }
    }

}
