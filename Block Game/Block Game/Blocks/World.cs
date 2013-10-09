///Represents a Block Game world
///© 2013 Spine Games

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
    /// <summary>
    /// Represents the world that can be edited
    /// </summary>
    public static class World
    {
        /// <summary>
        /// The chunks in this world
        /// </summary>
        static Chunk[, ,] CoordChunks = new Chunk[512, 1024, 32]; 
        /// <summary>
        /// Holds all of the points where chunks were loaded
        /// </summary>
        static Point3[] loaded = new Point3[0];
        /// <summary>
        /// Gets the number of chunks
        /// </summary>
        public static int ChunkCount { get { return loaded.Length; } }

        /// <summary>
        /// The list of all points where chunks should be loaded
        /// </summary>
        static List<Point3> ToBeLoaded = new List<Point3>();
        /// <summary>
        /// The thread used to load chunks
        /// </summary>
        static BackgroundWorker ChunkThread = new BackgroundWorker();

        /// <summary>
        /// Initializes the world
        /// </summary>
        public static void Initialize()
        {
            ChunkThread.DoWork += LoadChunk;
            ChunkThread.RunWorkerCompleted += ChunkLoaded;
        }

        /// <summary>
        /// Registered a chunk t be loaded at the specified chunk co-ords
        /// </summary>
        /// <param name="chunkPos">The chunk co-ords to load</param>
        public static void AddChunk(Point3 chunkPos)
        {
            ToBeLoaded.Add(chunkPos);

            if (!ChunkThread.IsBusy)
                ChunkThread.RunWorkerAsync(ToBeLoaded[0]);
        }

        /// <summary>
        /// Performs the chunk loading/generating event
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">The work event args containing the chunk co-ords to be loaded</param>
        private static void LoadChunk(object sender, DoWorkEventArgs e)
        {
            Chunk chunk = new Chunk((Point3)e.Argument);
            chunk.GenChunk();
            e.Result = new object[] { chunk, (Point3)e.Argument };
        }

        /// <summary>
        /// Finalizes the chunk loading by adding it back to the map, and starts loading the next chunk
        /// if need be
        /// </summary>
        /// <param name="sender">The object that raised this event (should be this world)</param>
        /// <param name="e">The WorkCompleted containing the chunk that was loaded</param>
        private static void ChunkLoaded(object sender, RunWorkerCompletedEventArgs e)
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

        /// <summary>
        /// Gets if the chunk at {x,y,z} is loaded
        /// </summary>
        /// <param name="x">The x co-ord (chunk)</param>
        /// <param name="y">The y co-ord (chunk)</param>
        /// <param name="z">The z co-ord (chunk)</param>
        /// <returns>True if a chunk exists at {x,y,z}</returns>
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

        /// <summary>
        /// Renders all the currently loaded chunks
        /// </summary>
        /// <param name="camera"></param>
        public static void Render(Camera camera)
        {
            foreach (Point3 point in loaded)
                GetChunkFromChunkPos(point.X, point.Y, point.Z).RenderOpaque(camera);

            foreach (Point3 point in loaded)
                GetChunkFromChunkPos(point.X, point.Y, point.Z).RenderNonOPaque(camera);
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
