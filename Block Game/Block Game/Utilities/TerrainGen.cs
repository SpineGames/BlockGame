///The terrain generator for Block Game
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockGame;
using BlockGame.Blocks;

namespace Block_Game.Blocks
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
                return new BlockData(BlockManager.Dirt.ID, 1);

            return new BlockData(BlockManager.Air.ID);

        }
    }
}
