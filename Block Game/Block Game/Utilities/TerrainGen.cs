///The terrain generator for Block Game
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockGame;
using BlockGame.Blocks;
using BlockGame.Utilities;
using Microsoft.Xna.Framework;

namespace BlockGame.Blocks
{
    /// <summary>
    /// The terrain generator to use
    /// </summary>
    public static class TerrainGen
    {
        /// <summary>
        /// The random number generator for this terrain
        /// </summary>
        static Random rand = new Random();
        public static Random Random
        {
            get { return rand; }
        }
        /// <summary>
        /// The base z that the ground is at
        /// </summary>
        public const int GroundLevel = 52;

        /// <summary>
        /// Sets the seed for the RNG
        /// </summary>
        /// <param name="seed">The seed to use for this level</param>
        public static void SetSeed(int seed)
        {
            rand = new Random(seed);
        }

        /// <summary>
        /// Get the BlockData at {x,y,z}
        /// </summary>
        /// <param name="x">The x co-ords of the block to get</param>
        /// <param name="y">The y co-ords of the block to get</param>
        /// <param name="z">The z co-ords of the block to get</param>
        /// <returns>The BlockData for position {x,y,z}</returns>
        public static BlockData GetBlockAtPos(int x, int y, int z)
        {
            BlockData SolidData = new BlockData(BlockManager.Stone.ID);
            BlockData SurfaceData = new BlockData(BlockManager.Dirt.ID, 1);
            BlockData FillerData = new BlockData(BlockManager.Dirt.ID);

            bool solid = IsSolid(x, y, z);

            if (!solid)
                return new BlockData(BlockManager.Air.ID);
            else
            {
                if (!IsSolid(x, y, z + 1))
                    return SurfaceData;

                else if (!IsSolid(x, y, z + 2) | !IsSolid(x, y, z + 3) | !IsSolid(x, y, z + 4))
                    return FillerData;

                return SolidData;
            }
        }

        /// <summary>
        /// Checks if a given position is solid
        /// </summary>
        /// <param name="x">The x co-ord to check</param>
        /// <param name="y">The y co-ord to check</param>
        /// <param name="z">The z co-ord to check</param>
        /// <returns>True if the position at {x,y,z} is solid</returns>
        private static bool IsSolid(int x, int y, int z)
        {
            float txSample = 0.01F;
            float tySample = 0.01F;
            float tzSample = 0.01F;

            int terrainOctaves = 1;

            float tHeight = 64;

            float perlin = Perlin.GetAtMap(x + 1000, y + 1000, z + 1000, terrainOctaves, txSample, tySample, tzSample);

            float height = ((Perlin.GetAtMap(x + 1000, y + 1000, 1000) + 1.0F) / 2.0F) * tHeight;
            float density = 1 - (MathHelper.Clamp(z / height, 0, 1));

            bool solid = false;

            perlin = perlin.Wrap(-1, 0);

            if (perlin + density > 0F)
                solid = true;

            return solid;
        }

        /// <summary>
        /// Generates a tree in the world
        /// </summary>
        /// <param name="x">The x co-ord of the tree</param>
        /// <param name="y">The y co-ord of the tree</param>
        /// <param name="z">The z co-ord of the tree</param>
        /// <param name="height">The height of the tree</param>
        public static void GenTree(int x, int y, int z, int height)
        {
            for (int sX = - 3; sX < + 3; sX++)
                for (int sY = - 3; sY < + 3; sY++)
                    for (int sZ = - 3; sZ < + 3; sZ++)
                    {
                        if (Math.Pow(1.5, 3) > Math.Pow(sX, 2) + Math.Pow(sY, 2) + Math.Pow(sZ, 2))
                            World.SetBlock(x + sX, y + sY, z + sZ + height, new BlockData(BlockManager.Leaves.ID, 0));

                    }
                for (int zP = 0; zP <= height; zP++)
                {
                    World.SetBlock(x, y, z + zP, new BlockData(BlockManager.Log.ID, 1));
                }
        }

        /// <summary>
        /// Generates a tree in the chunk
        /// </summary>
        /// <param name="x">The x co-ord of the tree</param>
        /// <param name="y">The y co-ord of the tree</param>
        /// <param name="z">The z co-ord of the tree</param>
        /// <param name="height">The height of the tree</param>
        public static void GenTree(int x, int y, int z, int height, Chunk chunk)
        {
            for (int sX = -3; sX < +3; sX++)
                for (int sY = -3; sY < +3; sY++)
                    for (int sZ = -3; sZ < +3; sZ++)
                    {
                        if (Math.Pow(2, 3) > Math.Pow(sX, 2) + Math.Pow(sY, 2) + Math.Pow(sZ, 2))
                            chunk.SetBlockWithoutNotify(x + sX, y + sY, z + sZ + height, new BlockData(BlockManager.Leaves.ID, 0));

                    }
            for (int zP = 0; zP <= height; zP++)
            {
                chunk.SetBlockWithoutNotify(x, y, z + zP, new BlockData(BlockManager.Log.ID, 1));
            }
        }
    }
}
