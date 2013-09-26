using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockGame;
using BlockGame.Blocks;

namespace Block_Game.Blocks
{
    public static class TerrainGen
    {
        static Random rand = new Random();

        public static void SetSeed(int seed)
        {
            rand = new Random(seed);
        }

        public static BlockData GetBlockAtPos(int x, int y, int z)
        {
            if (z < 12)
            {
                if (rand.Next(100) < 20)
                    return new BlockData(BlockManager.Gravel.ID);
                else
                    return new BlockData(BlockManager.Stone.ID);
            }
            else if (z < 14)
                return new BlockData(BlockManager.Dirt.ID);
            else if (z == 14)
                return new BlockData(BlockManager.Dirt.ID, 1);
            return new BlockData(BlockManager.Air.ID);

        }
    }
}
