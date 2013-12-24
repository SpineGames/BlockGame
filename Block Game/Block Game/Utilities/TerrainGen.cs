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

        private static float TerrainHeight = 64.0F;
        private static float WaterHeight = 64.0F;

        private static BlockData AirData = new BlockData(BlockManager.Air.ID);
        private static BlockData SolidData = new BlockData(BlockManager.Stone.ID);
        private static BlockData SurfaceData = new BlockData(BlockManager.Dirt.ID, 1);
        private static BlockData FillerData = new BlockData(BlockManager.Dirt.ID);
        private static BlockData WaterData = new BlockData(BlockManager.Water.ID);
        private static BlockData OceanBottomData = new BlockData(BlockManager.Sand.ID);

        private static BlockData OreData = new BlockData(BlockManager.Gravel.ID);

        /// <summary>
        /// The x sampling to use for terrain generation
        /// </summary>
        private static float xSample = 0.01F;
        /// <summary>
        /// The y sampling to use for terrain generation
        /// </summary>
        private static float ySample = 0.01F;
        /// <summary>
        /// The z sampling to use for terrain generation
        /// </summary>
        private static float zSample = 0.01F;

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
            BlockData currentPass =  HeightmapPass(x, y, z);
            currentPass = GrassingPass(x, y, z, currentPass);
            currentPass = WaterPass(x, y, z, currentPass);
            currentPass = CavePass(x, y, z, currentPass, 3, 1.0F);
            currentPass = OrePass(x, y, z, currentPass, 2, 0.8F, 32, OreData, 0.6F);
            return currentPass;
        }

        /// <summary>
        /// The pass that represents the base Heightmap
        /// </summary>
        /// <param name="x">The x co-ord to check</param>
        /// <param name="y">The y co-ord to check</param>
        /// <param name="z">The z co-ord to check</param>
        /// <returns>Either the SolidData or AirData</returns>
        private static BlockData HeightmapPass(int x, int y, int z)
        {
            int heightmap = InitialHeight(x, y);

            if (z < heightmap)
                return SolidData;
            else
                return AirData;
        }

        /// <summary>
        /// Gets the initial terrain height
        /// </summary>
        /// <param name="x">The x co-ord to check</param>
        /// <param name="y">The y co-ord to check</param>
        /// <returns>The initial z for the given position</returns>
        private static int InitialHeight(int x, int y)
        {
            float heightmap = Perlin.GetAtMap(x + 1000, y + 1000, 1000, 2, xSample, ySample, zSample);

            heightmap = ((heightmap + 1.0F) / 2.0F) * (TerrainHeight * 2);

            return (int)heightmap;
        }

        /// <summary>
        /// Generates water in the terrain
        /// </summary>
        /// <param name="x">The x co-ord to get</param>
        /// <param name="y">The y co-ord to get</param>
        /// <param name="z">The z co-ord to get</param>
        /// <param name="currentPass">The current data in the pass</param>
        /// <returns>The post-pass data</returns>
        private static BlockData WaterPass(int x, int y, int z, BlockData currentPass)
        {
            if (z < WaterHeight & currentPass.ID == BlockManager.Air.ID)
                return WaterData;
            else
                return currentPass;
        }

        /// <summary>
        /// The pass that represents the grassing pass
        /// </summary>
        /// <param name="x">The x co-ord to check</param>
        /// <param name="y">The y co-ord to check</param>
        /// <param name="z">The z co-ord to check</param>
        /// <returns>The new pass</returns>
        private static BlockData GrassingPass(int x, int y, int z, BlockData currentPass)
        {
            if (currentPass.ID != BlockManager.Air.ID)
            {
                if (InitialHeight(x,y) > WaterHeight)
                {
                    if (HeightmapPass(x, y, z + 1).ID == BlockManager.Air.ID)
                        return SurfaceData;

                    for (int d = 0; d < 3; d++)
                    {
                        if (HeightmapPass(x, y, z + 1 + d).ID == BlockManager.Air.ID)
                            return FillerData;
                    }
                }
                else
                {
                    for (int d = 0; d < 3; d++)
                    {
                        if (HeightmapPass(x, y, z + d).ID == BlockManager.Air.ID)
                            return OceanBottomData;
                    }
                }
            }
            return currentPass;
        }

        /// <summary>
        /// Generates caves in the terrain
        /// </summary>
        /// <param name="x">The x co-ord to get</param>
        /// <param name="y">The y co-ord to get</param>
        /// <param name="z">The z co-ord to get</param>
        /// <param name="currentPass">The current data in the pass</param>
        /// <param name="caveDensity">The roughness of the caves, should be larger than 0</param>
        /// <param name="caveSize">The size of caves, between 0 and 1. Note that this also changes
        /// the desity of the caves</param>
        /// <returns>the post-pass data</returns>
        private static BlockData CavePass(int x, int y, int z, BlockData currentPass, 
            int caveDensity, float caveSize)
        {
            float sample = 0.11F - caveSize * 0.1F;
            float perlin = Perlin.GetAtMap(x, y, z, caveDensity, sample, sample, sample);

            if (perlin > 0.5F & 
                (currentPass.ID == SolidData.ID || currentPass.ID == FillerData.ID ||
                 currentPass.ID == SurfaceData.ID))
                return AirData;
            else
                return currentPass;
        }

        /// <summary>
        /// Generates caves in the terrain
        /// </summary>
        /// <param name="x">The x co-ord to get</param>
        /// <param name="y">The y co-ord to get</param>
        /// <param name="z">The z co-ord to get</param>
        /// <param name="currentPass">The current data in the pass</param>
        /// <param name="oreRoughness">The roughness/size of deposits</param>
        /// <param name="density">The density of deposits</param>
        /// <param name="falloff">The ore falloff, default 0.5F</param>
        /// <param name="oreDat">The ore data to generate</param>
        /// <param name="maxZ">The maximum z of the ore</param>
        /// <returns>the post-pass data</returns>
        private static BlockData OrePass(int x, int y, int z, BlockData currentPass,
            int oreRoughness, float density, int maxZ, BlockData oreDat, float falloff = 0.5F)
        {
            if (z <= maxZ)
            {
                float sample = 0.5F - density * 0.1F;
                float perlin = Perlin.GetAtMap(x, y, z, oreRoughness, sample, sample, sample);

                if (perlin > falloff &
                    (currentPass.ID == SolidData.ID || currentPass.ID == FillerData.ID ||
                     currentPass.ID == SurfaceData.ID))
                    return oreDat;
            }
                return currentPass;
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
