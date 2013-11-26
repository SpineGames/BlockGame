///Represents a Block Game world
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockGame.Blocks;
using BlockGame.Utilities;
using System.Threading;
using BlockGame.Render;
using BlockGame;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace BlockGame.Blocks
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
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "ChunkLoadThread";

            Point3 t = (Point3)e.Argument;
            Chunk chunk = new Chunk(t);
            chunk.GenChunk();
            chunk.SetBlockFromWorld(t.X * Chunk.ChunkSize, t.Y * Chunk.ChunkSize, t.Z * Chunk.ChunkSize, new BlockData(0));
            
            e.Result = new object[] { chunk, t};
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

            InvalidateChunkFaces(pos);

            if (ToBeLoaded.Count > 0)
                ChunkThread.RunWorkerAsync(ToBeLoaded[0]);
        }

        /// <summary>
        /// Invalidates the chunks that share a face with the given chunk position
        /// </summary>
        /// <param name="CentreChunk">The centre chunk to invalidate faces towards</param>
        private static void InvalidateChunkFaces(Point3 CentreChunk)
        {
            if (ChunkExistsChunkPos(CentreChunk.X - 1,CentreChunk.Y, CentreChunk.Z))
                GetChunkFromChunkPos(CentreChunk.X - 1, CentreChunk.Y, CentreChunk.Z).InvalidateChunkFace(BlockFacing.Right);

            if (ChunkExistsChunkPos(CentreChunk.X + 1, CentreChunk.Y, CentreChunk.Z))
                GetChunkFromChunkPos(CentreChunk.X + 1, CentreChunk.Y, CentreChunk.Z).InvalidateChunkFace(BlockFacing.Left);

            if (ChunkExistsChunkPos(CentreChunk.X, CentreChunk.Y - 1, CentreChunk.Z))
                GetChunkFromChunkPos(CentreChunk.X, CentreChunk.Y - 1, CentreChunk.Z).InvalidateChunkFace(BlockFacing.Front);

            if (ChunkExistsChunkPos(CentreChunk.X, CentreChunk.Y + 1, CentreChunk.Z))
                GetChunkFromChunkPos(CentreChunk.X, CentreChunk.Y + 1, CentreChunk.Z).InvalidateChunkFace(BlockFacing.Back);

            if (ChunkExistsChunkPos(CentreChunk.X, CentreChunk.Y, CentreChunk.Z - 1))
                GetChunkFromChunkPos(CentreChunk.X, CentreChunk.Y, CentreChunk.Z - 1).InvalidateChunkFace(BlockFacing.Top);

            if (ChunkExistsChunkPos(CentreChunk.X, CentreChunk.Y, CentreChunk.Z + 1))
                GetChunkFromChunkPos(CentreChunk.X, CentreChunk.Y, CentreChunk.Z + 1).InvalidateChunkFace(BlockFacing.Bottom);
        }

        /// <summary>
        /// Gets a string containing all the loaded chunks
        /// </summary>
        /// <returns></returns>
        private static string LoadedStrings()
        {
            string s = "";
            foreach (Point3 p in loaded)
                s += " \n" + p;
            return s;
        }

        /// <summary>
        /// Gets if the chunk at {x,y,z} is loaded
        /// </summary>
        /// <param name="x">The x co-ord (world)</param>
        /// <param name="y">The y co-ord (world)</param>
        /// <param name="z">The z co-ord (world)</param>
        /// <returns>True if a chunk exists at {x,y,z}</returns>
        public static bool ChunkExistsCoords(int x, int y, int z)
        {
            return GetChunkFromCoords(x, y, z) != null;
        }

        /// <summary>
        /// Gets if the chunk at {x,y,z} is loaded
        /// </summary>
        /// <param name="x">The x co-ord (chunk pos)</param>
        /// <param name="y">The y co-ord (chunk pos)</param>
        /// <param name="z">The z co-ord (chunk pos)</param>
        /// <returns>True if a chunk exists at {x,y,z}</returns>
        public static bool ChunkExistsChunkPos(int x, int y, int z)
        {
            return loaded.Contains(new Point3(x,y,z));
        }
        
        /// <summary>
        /// Gets the chunks from the given world position
        /// </summary>
        /// <param name="x">The x co-ord (world)</param>
        /// <param name="y">The y co-ord (world)</param>
        /// <param name="z">The z co-ord (world)</param>
        /// <returns>The chunk that contains the given world co-ord</returns>
        public static Chunk GetChunkFromCoords(int x, int y, int z)
        {
            return CoordChunks[x / Chunk.ChunkSize, y / Chunk.ChunkSize, z / Chunk.ChunkSize];
        }
        
        /// <summary>
        /// Gets the chunks from the given chunk position
        /// </summary>
        /// <param name="x">The x co-ord (chunk ref)</param>
        /// <param name="y">The y co-ord (chunk ref)</param>
        /// <param name="z">The z co-ord (chunk ref)</param>
        /// <returns>The chunk at data slot {x,y,z}</returns>
        public static Chunk GetChunkFromChunkPos(int x, int y, int z)
        {
            return CoordChunks[x, y, z];
        }

        /// <summary>
        /// Sets a block in the world to the given ID
        /// </summary>
        /// <param name="x">The x co-ord (world)</param>
        /// <param name="y">The y co-ord (world)</param>
        /// <param name="z">The z co-ord (world)</param>
        /// <param name="dat">The new block data to set to</param>
        public static void SetBlock(int x, int y, int z, BlockData dat)
        {
            if (ChunkExistsCoords(x, y, z))
                GetChunkFromCoords(x, y, z).SetBlockFromWorld(x, y, z, dat);
        }

        /// <summary>
        /// Sets a block in the world to the given ID without pushing the render state
        /// </summary>
        /// <param name="x">The x co-ord (world)</param>
        /// <param name="y">The y co-ord (world)</param>
        /// <param name="z">The z co-ord (world)</param>
        /// <param name="dat">The new block data to set to</param>
        public static void SetBlockNoNotify(int x, int y, int z, BlockData dat)
        {
            if (ChunkExistsCoords(x, y, z))
                GetChunkFromCoords(x, y, z).SetBlockFromWorldNoNotify(x, y, z, dat);
        }

        /// <summary>
        /// Sets a block in the world to the given ID
        /// </summary>
        /// <param name="Pos">The co-ords (world)</param>
        /// <param name="dat">The new block data to set to</param>
        public static void SetBlock(Point3 Pos, BlockData dat)
        {
            if (ChunkExistsCoords(Pos.X, Pos.Y, Pos.Z))
                GetChunkFromCoords(Pos.X, Pos.Y, Pos.Z).SetBlockFromWorld(Pos.X, Pos.Y, Pos.Z, dat);
        }

        /// <summary>
        /// Sets a cuboid in the world
        /// </summary>
        /// <param name="cuboid">The cuboid to set</param>
        /// <param name="dat">The block data to set</param>
        public static void SetCuboid(Cuboid cuboid, BlockData dat)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            for (int x = cuboid.Min.X; x < cuboid.Max.X; x++)
                for (int y = cuboid.Min.Y; y < cuboid.Max.Y; y++)
                    for (int z = cuboid.Min.Z; z < cuboid.Max.Z; z++)
                        SetBlockNoNotify(x, y, z, dat);

            Debug.WriteLine("Time to set {0} blocks: {1}s\n", cuboid.Volume, time.Elapsed.TotalSeconds);
            time.Restart();

            foreach (Point3 cPos in loaded)
                    CoordChunks[cPos.X, cPos.Y, cPos.Z].ForceUpdate(cuboid);
            Debug.WriteLine("Time to check @ update {0} chunks: {1}s\n", loaded.Length, time.Elapsed.TotalSeconds);
        }

        /// <summary>
        /// Sets a sphere in the world
        /// </summary>
        /// <param name="cuboid">The bounds of the sphere to set</param>
        /// <param name="dat">The block data to set</param>
        public static void SetSphere(Cuboid cuboid, BlockData dat)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            Point3 centre = (cuboid.Max - cuboid.Min) / 2;
            float radius = ((Vector3)centre).Length();

            for (int x = cuboid.Min.X; x < cuboid.Max.X; x++)
                for (int y = cuboid.Min.Y; y < cuboid.Max.Y; y++)
                    for (int z = cuboid.Min.Z; z < cuboid.Max.Z; z++)
                    {
                        if ((new Vector3(x, y, z) - (Vector3)centre).Length() < radius)
                            SetBlock(x, y, z, dat);
                    }


            Debug.WriteLine("Time to set {0} blocks: {1}s\n", cuboid.Volume, time.Elapsed.TotalSeconds);
            time.Restart();


            foreach (Point3 cPos in loaded)
                CoordChunks[cPos.X, cPos.Y, cPos.Z].ForceUpdate(cuboid);

            Debug.WriteLine("Time to check & update {0} chunks: {1}s\n", loaded.Length, time.Elapsed.TotalSeconds);
        }

        /// <summary>
        /// Sets a sphere in the world
        /// </summary>
        /// <param name="cuboid">The bounds of the sphere to set</param>
        /// <param name="dat">The block data to set</param>
        public static void SetSphere(Point3 centre, float radius, BlockData dat)
        {
            Cuboid cuboid = new Cuboid(centre - (int)radius, centre + (int)radius);
            SetSphere(cuboid, dat);
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
            if (ChunkExistsCoords(x, y, z))
                return GetChunkFromCoords(x, y, z).GetBlockIDFromWorld(x, y, z);
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
            if (ChunkExistsCoords(x, y, z))
                return GetChunkFromCoords(x, y, z).IsOpaqueFromWorld(x, y, z);
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

    /// <summary>
    /// Invoked when a chunk is loaded
    /// </summary>
    /// <param name="e">The ChunkLoadedArgs to use</param>
    public delegate void ChunkLoadedHandler(ChunkLoadedArgs e);

    /// <summary>
    /// Represents the agruments for a chunk loaded event
    /// </summary>
    public class ChunkLoadedArgs : EventArgs
    {
        /// <summary>
        /// The chunk that was generated
        /// </summary>
        public readonly Chunk chunk;

        /// <summary>
        /// Creates a new event args for a chunk load
        /// </summary>
        /// <param name="chunk">The chunk that has been loaded</param>
        public ChunkLoadedArgs(Chunk chunk)
        {
            this.chunk = chunk;
        }
    }
}
