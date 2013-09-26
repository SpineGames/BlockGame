using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockGame.Blocks;
using BlockGame.Utilities;
using System.Threading;
using Block_Game.Render;
using BlockGame;
using Block_Game.Utilities;
using System.ComponentModel;

namespace Block_Game.Blocks
{
    public static class World
    {
        static Chunk[, ,] CoordChunks = new Chunk[512, 1024, 32]; 
        static Point3[] loaded = new Point3[0];
        public static int ChunkCount { get { return loaded.Length; } }

        static List<Point3> ToBeLoaded = new List<Point3>();
        static BackgroundWorker ChunkThread = new BackgroundWorker();

        public static void Initialize()
        {
            ChunkThread.DoWork += LoadChunk;
            ChunkThread.RunWorkerCompleted += ChunkLoaded;
        }

        public static void AddChunk(Point3 chunkPos)
        {
            ToBeLoaded.Add(chunkPos);

            if (!ChunkThread.IsBusy)
                ChunkThread.RunWorkerAsync(ToBeLoaded[0]);
        }

        public static void LoadChunk(object sender, DoWorkEventArgs e)
        {
            Chunk chunk = new Chunk((Point3)e.Argument);
            chunk.GenChunk();
            e.Result = new object[] { chunk, (Point3)e.Argument };
        }

        public static void ChunkLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            ToBeLoaded.RemoveAt(0);

            Chunk chunk = (Chunk)((object[])e.Result)[0];
            Point3 pos = (Point3)((object[])e.Result)[1];

            CoordChunks[pos.X, pos.Y, pos.Z] = chunk;
            Array.Resize<Point3>(ref loaded, loaded.Length + 1);
            loaded[loaded.Length - 1] = pos;


            if (ToBeLoaded.Count > 0)
                ChunkThread.RunWorkerAsync(ToBeLoaded[0]);
        }

        public static bool ChunkExists(int x, int y, int z)
        {
            return CoordChunks[x / Chunk.ChunkSize, y / Chunk.ChunkSize, z / Chunk.ChunkSize] != null;
        }
        
        public static Chunk GetChunk(int x, int y, int z)
        {
            return CoordChunks[x / Chunk.ChunkSize, y / Chunk.ChunkSize, z / Chunk.ChunkSize];
        }

        public static Chunk GetChunkFromChunkPos(int x, int y, int z)
        {
            return CoordChunks[x, y, z];
        }

        public static void SetBlock(int x, int y, int z, BlockData dat)
        {
            if (ChunkExists(x, y, z))
                GetChunk(x, y, z).SetBlockFromWorld(x, y, z, dat);
        }

        public static void SetBlock(Point3 Pos, BlockData dat)
        {
            if (ChunkExists(Pos.X, Pos.Y, Pos.Z))
                GetChunk(Pos.X, Pos.Y, Pos.Z).SetBlockFromWorld(Pos.X, Pos.Y, Pos.Z, dat);
        }

        public static void SetCuboid(Cuboid cuboid, BlockData dat)
        {
            for (int x = cuboid.Min.X; x < cuboid.Max.X; x++)
                for (int y = cuboid.Min.Y; y < cuboid.Max.Y; y++)
                    for (int z = cuboid.Min.Z; z < cuboid.Max.Z; z++)
                        SetBlock(x, y, z, dat);
        }

        /// <summary>
        /// Get's a block's ID
        /// </summary>
        /// <param name="x">The block's x co-ord (block)</param>
        /// <param name="y">The block's y co-ord (block)</param>
        /// <param name="z">The block's z co-ord (bock)</param>
        /// <returns>True if the block at {x,y,z} is opaque</returns>
        public static byte GetBlockID(int x, int y, int z)
        {
            if (ChunkExists(x, y, z))
                return GetChunk(x, y, z).GetBlockID(x, y, z);
            return 0;
        }

        /// <summary>
        /// Get's a block's ID
        /// </summary>
        /// <param name="x">The block's x co-ord (block)</param>
        /// <param name="y">The block's y co-ord (block)</param>
        /// <param name="z">The block's z co-ord (bock)</param>
        /// <returns>True if the block at {x,y,z} is opaque</returns>
        public static byte GetBlockID(Point3 pos)
        {
            return GetBlockID(pos.X, pos.Y, pos.Z);
        }

        /// <summary>
        /// Checks if a block is opaque or not
        /// </summary>
        /// <param name="x">The block's x co-ord (block)</param>
        /// <param name="y">The block's y co-ord (block)</param>
        /// <param name="z">The block's z co-ord (bock)</param>
        /// <returns>True if the block at {x,y,z} is opaque</returns>
        public static bool IsOpaque(int x, int y, int z)
        {
            if (ChunkExists(x, y, z))
                return GetChunk(x, y, z).IsOpaqueFromWorld(x, y, z);
            return true;
        }

        /// <summary>
        /// Checks if a block is opaque or not
        /// </summary>
        /// <param name="x">The block's x co-ord (block)</param>
        /// <param name="y">The block's y co-ord (block)</param>
        /// <param name="z">The block's z co-ord (bock)</param>
        /// <returns>True if the block at {x,y,z} is opaque</returns>
        public static bool IsOpaque(Point3 pos)
        {
            return IsOpaque(pos.X, pos.Y, pos.Z);
        }

        public static void Render(Camera camera)
        {
            foreach (Point3 point in loaded)
            {
                GetChunkFromChunkPos(point.X, point.Y, point.Z).Render(camera);
            }
        }
    }

    public delegate void ChunkLoadedHandler(ChunkLoadedArgs e);

    public class ChunkLoadedArgs : EventArgs
    {
        public Chunk chunk {get; set;}

        public ChunkLoadedArgs(Chunk chunk)
        {
            this.chunk = chunk;
        }
    }
}
