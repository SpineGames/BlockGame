///The terrain generator for Block Game
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockGame;
using BlockGame.Blocks;
using BlockGame.Utilities;

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
            if (z < GroundLevel - 3)
            {
                if (rand.Next(100) < 20)
                    return new BlockData(BlockManager.Gravel.ID);
                else
                    return new BlockData(BlockManager.Stone.ID);
            }
            else if (z < GroundLevel)
                return new BlockData(BlockManager.Dirt.ID);
            else if (z == GroundLevel)
            {
                //if ((x + y) % 2 != 1)
                    return new BlockData(BlockManager.Dirt.ID, 1);
               // else
                   // return new BlockData(BlockManager.Log.ID, 0);
            }

            return new BlockData(BlockManager.Air.ID);

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
