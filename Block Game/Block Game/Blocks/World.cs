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

namespace Block_Game.Blocks
{
    public static class World
    {
        static List<Chunk> chunks = new List<Chunk>();
        static ChunkLoadedHandler LoadedHandler = new ChunkLoadedHandler(ChunkLoaded);

        public static void AddChunk(Point3 chunkPos)
        {
            Thread t = new Thread(LoadChunk);
            t.Start(chunkPos);
        }

        public static void LoadChunk(object pos)
        {
            Chunk chunk = new Chunk((Point3)pos);
            chunk.SetCuboid(new Point3(0), new Point3(Chunk.ChunkSize - 1), 1);
            LoadedHandler.Invoke(new ChunkLoadedArgs(chunk));
        }

        public static void ChunkLoaded(ChunkLoadedArgs e)
        {
            chunks.Add(e.chunk);
        }

        public static void SetBlock(Point3 Pos, BlockData dat)
        {
            foreach (Chunk c in chunks)
            {
                if (Pos.IsInCuboid(c.Collision))
                    c.SetBlockWithUpdate(c.ToChunkCoord(Pos), dat);
            }
        }

        public static void SetCuboid(Cuboid cuboid, BlockData dat)
        {
            foreach (Chunk c in chunks)
            {
                if (c.Collision.Intersects(cuboid))
                    c.SetCuboid(cuboid, dat);
            }
        }

        public static void Render(Camera camera)
        {
            for (int i = 0; i < chunks.Count; i ++)
            {
                chunks[i].Render(camera);
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
