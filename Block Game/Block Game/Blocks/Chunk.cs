///Represents a chunk of blocks
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BlockGame.Utilities;
using Microsoft.Xna.Framework.Graphics;
using BlockGame.Render;
using Block_Game.Render;
using Block_Game;
using Block_Game.Utilities;
using Block_Game.Blocks;

namespace BlockGame.Blocks
{
    /// <summary>
    /// Represents a chunk of block data
    /// </summary>
    public class Chunk
    {
        /// <summary>
        /// The size of chunks in blocks
        /// </summary>
        public const int ChunkSize = 16;

        /// <summary>
        /// Gets the size of a chunk in the world
        /// </summary>
        public static Point3 WorldChunkSize
        {
            get { return new Point3((int)(ChunkSize  * BlockManager.BlockSize)); }
        }
        /// <summary>
        /// The array holding all of the block ID's and metaData
        /// </summary>
        BlockData[, ,] blocks = new BlockData[ChunkSize, ChunkSize, ChunkSize];
        /// <summary>
        /// An array holding all of the render states for easier updating later
        /// </summary>
        BlockRenderer[, ,] renderStates = new BlockRenderer[ChunkSize, ChunkSize, ChunkSize];
        /// <summary>
        /// The chunk's relative position to other chunks
        /// </summary>
        Point3 ChunkPos;
        /// <summary>
        /// The position that block [0,0,0] has in the world
        /// </summary>
        public Point3 WorldPos
        {
            get
            {
                return ChunkPos * (int)(ChunkSize);
            }
        }
        /// <summary>
        /// The position that the block furthest from [0,0,0] has in the world
        /// </summary>
        public Point3 MaxWorldPos { get { return WorldPos + WorldChunkSize; } }
        /// <summary>
        /// The Transformation position
        /// </summary>
        public Point3 TransformedWorldPos
        {
            get
            {
                return ChunkPos * (int)(ChunkSize * BlockManager.BlockSize);
            }
        }
        /// <summary>
        /// The private collision bounding box
        /// </summary>
        Cuboid collision;
        /// <summary>
        /// The world-oriented collision box
        /// </summary>
        public Cuboid Collision { get { return collision; } }
        /// <summary>
        /// The renderer used to render this chunk
        /// </summary>
        PolyRender Renderer;

        /// <summary>
        /// Creates a new chunk with the given chunk position
        /// </summary>
        /// <param name="chunkPos">The position of the chunk relative to other chunks</param>
        public Chunk(Point3 chunkPos)
        {
            this.ChunkPos = chunkPos;
            collision = new Cuboid(WorldPos, MaxWorldPos);
            Renderer = new PolyRender(Matrix.CreateTranslation(TransformedWorldPos));
            InitialRenderStates();
        }

        #region Rendering
        /// <summary>
        /// Rebuild the whole chunk's render states
        /// </summary>
        public void InitialRenderStates()
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        renderStates[x, y, z] = new BlockRenderer();
                    }
                }
            }
        }

        /// <summary>
        /// Updates all render states in a cuboid
        /// </summary>
        /// <param name="min">The minimum of the cube to update (chunk)</param>
        /// <param name="max">The maximum of the cube to update (chunk)</param>
        private void UpdateRenderStates(Point3 min, Point3 max)
        {
            min.Clamp(new Point3(0), new Point3(ChunkSize));
            max.Clamp(new Point3(0), new Point3(ChunkSize));

            for (int x = min.X; x <= max.X; x++)
            {
                for (int y = min.Y; y <= max.Y; y++)
                {
                    for (int z = min.Z; z <= max.Z; z++)
                    {
                        UpdateRenderState(x, y, z);
                    }
                }
            }
            PushRenderState();
        }

        /// <summary>
        /// Updates the render state at pos
        /// </summary>
        /// <param name="pos">The chunk co-ords of the render state to update</param>
        private void UpdateRenderState(Point3 pos)
        {
            UpdateRenderState(pos.X, pos.Y, pos.Z);
        }

        /// <summary>
        /// Updates the render state at {x,y,z}
        /// </summary>
        /// <param name="x">The x co-ord of the block (chunk)</param>
        /// <param name="y">The y co-ord of the block (chunk)</param>
        /// <param name="z">The z co-ord of the block (chunk)</param>
        private void UpdateRenderState(int x, int y, int z)
        {
            if (IsinRange(x, y, z))
            {
                renderStates[x, y, z].verts =
                        BlockManager.Blocks[GetBlockID(x, y, z)].GetModel(
                        GetRenderStateForBlock(x, y, z),
                        new Point3(x, y, z), 
                        blocks[x, y, z].Meta);
            }
        }

        /// <summary>
        /// Pushes all changes to the render states to the rendered
        /// </summary>
        public void PushRenderState()
        {
            Renderer.Clear();

            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        if (!BlockManager.Blocks[GetBlockID(x, y, z)].IsOpaque)
                        {
                            Renderer.AddNonOpaquePolys(
                                renderStates[x, y, z].verts);
                        }
                        else
                        {
                            Renderer.AddOpaquePolys(
                                renderStates[x, y, z].verts);
                        }
                    }
                }
            }

            Renderer.FinalizePolys();
        }

        /// <summary>
        /// Returns the BlockRenderState at {x,y,z}
        /// </summary>
        /// <param name="x">The x co-ord of the block (chunk)</param>
        /// <param name="y">The y co-ord of the block (chunk)</param>
        /// <param name="z">The z co-ord of the block (chunk)</param>
        /// <returns>The BlockRenderState for {x,y,z}</returns>
        public BlockRenderStates GetRenderStateForBlock(int x, int y, int z)
        {
            BlockRenderStates ret = BlockRenderStates.None; // initially no render

            if (IsinRange(x, y, z) && GetBlockID(x,y,z) != 0) // make sure block is in range
            {
                if (ShouldRenderFace(BlockFacing.Left, x, y, z))//left face
                    ret = ret | BlockRenderStates.Left;
                if (ShouldRenderFace(BlockFacing.Right, x, y, z))//right face
                    ret = ret | BlockRenderStates.Right;
                if (ShouldRenderFace(BlockFacing.Back, x, y, z))//back face
                    ret = ret | BlockRenderStates.Back;
                if (ShouldRenderFace(BlockFacing.Front, x, y, z))//front face
                    ret = ret | BlockRenderStates.Front;
                if (ShouldRenderFace(BlockFacing.Bottom, x, y, z))//bottom face
                    ret = ret | BlockRenderStates.Bottom;
                if (ShouldRenderFace(BlockFacing.Top, x, y, z))//top face
                    ret = ret | BlockRenderStates.Top;
            }
            return ret;
        }

        /// <summary>
        /// Returns true if the face shuld be rendered
        /// </summary>
        /// <param name="facing">The facing normal to check</param>
        /// <param name="position">The block's chunk co-ords</param>
        /// <returns>True if the face should be rendered</returns>
        private bool ShouldRenderFace(BlockFacing facing, Point3 position)
        {
            if (GetBlockID(position + facing.NormalVector()) == 0)
                return true; //main cause to render the face is if it faces air

            if (World.IsOpaque(position) & !World.IsOpaque(position + facing.NormalVector()))
            {
                return true; //only other case is if there is a non-opaque and an opaque
            }

            return false;
        }

        /// <summary>
        /// Returns true if the face shuld be rendered
        /// </summary>
        /// <param name="facing">The facing normal to check</param>
        /// <param name="x">The x co-ord of the block (chunk)</param>
        /// <param name="y">The y co-ord of the block (chunk)</param>
        /// <param name="z">The z co-ord of the block (chunk)</param>
        /// <returns>True if the face should be rendered</returns>
        private bool ShouldRenderFace(BlockFacing facing, int x, int y, int z)
        {
            return ShouldRenderFace(facing, new Point3(x, y, z));
        }

        /// <summary>
        /// Checks if a block is opaque or not
        /// </summary>
        /// <param name="x">The block's x co-ord (chunk)</param>
        /// <param name="y">The block's y co-ord (chunk)</param>
        /// <param name="z">The block's z co-ord (chunk)</param>
        /// <returns>True if the block at {x,y,z} is opaque</returns>
        private bool IsOpaque(int x, int y, int z)
        {
            if (IsinRange(x, y, z)) //if the pos is in range
            {
                return BlockManager.Blocks[GetBlockID(x, y, z)].IsOpaque; //return opacity                   
            }
            return false; //otherwise assume it's air (TO BE FIXED WITH WORLD!!!)
        }

        /// <summary>
        /// Checks if a block is opaque or not
        /// </summary>
        /// <param name="pos">The block's chunk co-ordinates</param>
        /// <returns>True if the block at pos is opaque</returns>
        private bool IsOpaque(Point3 pos)
        {
            return IsOpaque(pos.X, pos.Y, pos.Z);
        }

        /// <summary>
        /// Checks if a block is opaque or not
        /// </summary>
        /// <param name="x">The block's x co-ord (chunk)</param>
        /// <param name="y">The block's y co-ord (chunk)</param>
        /// <param name="z">The block's z co-ord (chunk)</param>
        /// <returns>True if the block at {x,y,z} is opaque</returns>
        public bool IsOpaqueFromWorld(int x, int y, int z)
        {
            x -= WorldPos.X;
            y -= WorldPos.Y;
            z -= WorldPos.Z;

            return IsOpaque(x, y, z);
        }

        /// <summary>
        /// Renders this chunk
        /// </summary>
        /// <param name="view">The camera to render with</param>
        public void Render(Camera camera)
        {
            Renderer.Render(camera.View);
        }

        /// <summary>
        /// Renders this chunk's Opaque surfaces
        /// </summary>
        /// <param name="view">The camera to render with</param>
        public void RenderOpaque(Camera camera)
        {
            Renderer.RenderOpaque(camera.View);
        }

        /// <summary>
        /// Renders this chunk's non-opaque surfaces
        /// </summary>
        /// <param name="view">The camera to render with</param>
        public void RenderNonOPaque(Camera camera)
        {
            Renderer.RenderNonOpaque(camera.View);
        }
        #endregion

        #region Get Data
        /// <summary>
        /// Return the ID of the block at {x,y,z}
        /// </summary>
        /// <param name="x">The x co-ord of the block (chunk)</param>
        /// <param name="y">The y co-ord of the block (chunk)</param>
        /// <param name="z">The z co-ord of the block (chunk)</param>
        /// <returns>The block's ID at {x,y,z}</returns>
        public byte GetBlockID(int x, int y, int z)
        {
            if (IsinRange(x, y, z))
            {
                return blocks[x, y, z].ID;
            }
            return 0;
        }

        /// <summary>
        /// Gets the block ID at the specified chunk co-ordinates
        /// </summary>
        /// <param name="Pos">The chunk co-ords of the block to check</param>
        /// <returns>The block ID at Pos</returns>
        public byte GetBlockID(Point3 Pos)
        {
            return GetBlockID(Pos.X, Pos.Y, Pos.Z);
        }
        
        /// <summary>
        /// Return the Meta of the block at {x,y,z}
        /// </summary>
        /// <param name="x">The x co-ord of the block (chunk)</param>
        /// <param name="y">The y co-ord of the block (chunk)</param>
        /// <param name="z">The z co-ord of the block (chunk)</param>
        /// <returns>The block's Meta at {x,y,z}</returns>
        public byte GetBlockMeta(int x, int y, int z)
        {
            if (IsinRange(x, y, z))
            {
                return blocks[x, y, z].Meta;
            }
            return 0;
        }

        /// <summary>
        /// Gets the block Meta at the specified chunk co-ordinates
        /// </summary>
        /// <param name="Pos">The chunk co-ords of the block to check</param>
        /// <returns>The block Meta at Pos</returns>
        public byte GetBlockMeta(Point3 Pos)
        {
            return GetBlockMeta(Pos.X, Pos.Y, Pos.Z);
        }

        /// <summary>
        /// Return the Block Data of the block at {x,y,z}
        /// </summary>
        /// <param name="x">The x co-ord of the block (chunk)</param>
        /// <param name="y">The y co-ord of the block (chunk)</param>
        /// <param name="z">The z co-ord of the block (chunk)</param>
        /// <returns>The block's data at {x,y,z}</returns>
        public BlockData GetBlockData(int x, int y, int z)
        {
            if (IsinRange(x, y, z))
            {
                return blocks[x, y, z];
            }
            return new BlockData() { ID = 0, Meta = 0 };
        }

        /// <summary>
        /// Gets the block data at the specified chunk co-ordinates
        /// </summary>
        /// <param name="Pos">The chunk co-ords of the block to check</param>
        /// <returns>The block's data at Pos</returns>
        public BlockData GetBlockData(Point3 Pos)
        {
            return GetBlockData(Pos.X, Pos.Y, Pos.Z);
        }
        #endregion

        #region Set Block
        /// <summary>
        /// Generates this chunk using TerrainGen
        /// </summary>
        public void GenChunk()
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        SetBlock(x,y,z, 
                            TerrainGen.GetBlockAtPos(x + WorldPos.X, y + WorldPos.Y, z + WorldPos.Z));
                    }
                }
            }
            UpdateRenderStates(new Point3(0), new Point3(ChunkSize));
        }

        /// <summary>
        /// Sets the block at {x,y,z}
        /// </summary>
        /// <param name="x">The x co-ords of the block (chunk)</param>
        /// <param name="y">The y co-ords of the block (chunk)</param>
        /// <param name="z">The z co-ords of the block (chunk)</param>
        /// <param name="ID">The ID to set the block to</param>
        private void SetBlock(int x, int y, int z, byte ID)
        {
            SetBlock(x, y, z, new BlockData { ID = ID });
        }

        /// <summary>
        /// Sets the block at pos
        /// </summary>
        /// <param name="pos">The chunk co-ords for this block</param>
        /// <param name="ID">The ID to set the block to</param>
        private void SetBlock(Point3 pos, byte ID)
        {
            SetBlock(pos.X, pos.Y, pos.Z, new BlockData { ID = ID });
        }
        
        /// <summary>
        /// Sets the block at {x,y,z} to dat
        /// </summary>
        /// <param name="pos">The chunk co-ords for the block to set</param>
        /// <param name="dat">The block data to set at {x,y,z}</param>
        private void SetBlock(Point3 pos, BlockData dat)
        {
            SetBlock(pos.X, pos.Y, pos.Z, dat);
        }

        /// <summary>
        /// Sets the block at {x,y,z} to dat
        /// </summary>
        /// <param name="x">The x co-ords of the block (chunk)</param>
        /// <param name="y">The y co-ords of the block (chunk)</param>
        /// <param name="z">The z co-ords of the block (chunk)</param>
        /// <param name="dat">The block data to set at {x,y,z}</param>
        private void SetBlock(int x, int y, int z, BlockData dat)
        {
            if (IsinRange(x, y, z))
            {
                blocks[x, y, z] = dat;
            }
        } //base function

        /// <summary>
        /// Sets the block at {x,y,z} to dat
        /// </summary>
        /// <param name="x">The x co-ords of the block (world)</param>
        /// <param name="y">The y co-ords of the block (world)</param>
        /// <param name="z">The z co-ords of the block (world)</param>
        /// <param name="dat">The block data to set at {x,y,z}</param>
        public void SetBlockFromWorld(int x, int y, int z, BlockData dat)
        {
            x -= WorldPos.X;
            y -= WorldPos.Y;
            z -= WorldPos.Z;

            SetBlockWithUpdate(x, y, z, dat);

            UpdateRenderState(x, y, z);
        }
        
        /// <summary>
        /// Sets the block at {x,y,z} to dat and updates the renderer
        /// </summary>
        /// <param name="x">The x co-ords of the block (chunk)</param>
        /// <param name="y">The y co-ords of the block (chunk)</param>
        /// <param name="z">The z co-ords of the block (chunk)</param>
        /// <param name="dat">The block data to set at {x,y,z}</param>
        private void SetBlockWithUpdate(int x, int y, int z, BlockData dat)
        {
            SetBlock(x, y, z, dat);
            UpdateRenderState(x, y, z);
            PushRenderState();
        }

        /// <summary>
        /// Sets the block at {x,y,z} to dat and updates the renderer
        /// </summary>
        /// <param name="x">The x co-ords of the block (chunk)</param>
        /// <param name="y">The y co-ords of the block (chunk)</param>
        /// <param name="z">The z co-ords of the block (chunk)</param>
        /// <param name="dat">The block data to set at {x,y,z}</param>
        private void SetBlockWithUpdate(Point3 pos, BlockData dat)
        {
            SetBlockWithUpdate(pos.X, pos.Y, pos.Z, dat);
        }

        /// <summary>
        /// Sets a block of blocks to a single ID
        /// </summary>
        /// <param name="min">The minimum position to start from</param>
        /// <param name="max">The max position to start from</param>
        /// <param name="ID">The block ID to set the region to</param>
        private void SetCuboid(Point3 min, Point3 max, byte ID)
        {
            SetCuboid(min, max, new BlockData(ID));
        }

        /// <summary>
        /// Sets a block of blocks to a single block data
        /// </summary>
        /// <param name="min">The minimum position to start from</param>
        /// <param name="max">The max position to start from</param>
        /// <param name="dat">The block data to set the region to</param>
        private void SetCuboid(Point3 min, Point3 max, BlockData dat)
        {
            for (int x = min.X; x <= max.X; x++)
            {
                for (int y = min.Y; y <= max.Y; y++)
                {
                    for (int z = min.Z; z <= max.Z; z++)
                    {
                        SetBlock(x, y, z, dat);
                    }
                }
            }

            UpdateRenderStates(min - new Point3(1), max + Point3.One);
        }

        /// <summary>
        /// Sets a block of blocks to a single block data
        /// </summary>
        /// <param name="cuboid">The cuboid to set</param>
        /// <param name="dat">The block data to set the region to</param>
        private void SetCuboid(Cuboid cuboid, BlockData dat)
        {
            SetCuboid(cuboid.Min - WorldPos, cuboid.Max - WorldPos, dat);

            UpdateRenderStates(cuboid.Min - new Point3(1), cuboid.Max + Point3.One);
        }
        
        /// <summary>
        /// Sets a sphere of blocks to a single block ID
        /// </summary>
        /// <param name="centre">The centre of the sphere</param>
        /// <param name="radius">The radius of the sphere</param>
        public void SetSphere(Point3 centre, float radius, byte ID)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        if (Vector3.Distance(centre, new Vector3(x,y,z)) <= radius)
                            SetBlock(x, y, z, ID);
                    }
                }
            }
            Point3 size = new Point3((int)Math.Ceiling(radius + BlockManager.BlockSize));
            UpdateRenderStates(centre - size, centre + size);      
        }

        /// <summary>
        /// Sets a sphere of blocks to a single block ID
        /// </summary>
        /// <param name="centre">The centre of the sphere</param>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="dat">The data to set to region to</param>
        public void SetSphere(Point3 centre, float radius, BlockData dat)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        if (Vector3.Distance(centre, new Vector3(x, y, z)) <= radius)
                            SetBlock(x, y, z, dat);
                    }
                }
            }
            Point3 size = new Point3((int)Math.Ceiling(radius + BlockManager.BlockSize));
            UpdateRenderStates(centre - size, centre + size);
        }
        #endregion

        /// <summary>
        /// Checks if the position is within this chunk's range
        /// </summary>
        /// <param name="pos">The position to check for (chunk co-ords)</param>
        /// <returns>True if pos is within this chunk's range</returns>
        private bool IsinRange(Point3 pos)
        {
            return IsinRange(pos.X, pos.Y, pos.Z);
        }

        /// <summary>
        /// Returns true if the given point is within range of this chunk
        /// </summary>
        /// <param name="x">The X co-ord (Chunk)</param>
        /// <param name="y">The Y co-ord (Chunk)</param>
        /// <param name="z">The Z co-ord (Chunk)</param>
        /// <returns>Returns true if point {x,y,z} can exist in this chunk</returns>
        private bool IsinRange(int x, int y, int z)
        {
            return (x >= 0 & x < ChunkSize) & (y >= 0 & y < ChunkSize) & (z >= 0 & z < ChunkSize);
        }

        /// <summary>
        /// Converts world co-ords to chunk co-ords
        /// </summary>
        /// <param name="worldPos">The world point to convert</param>
        /// <returns>A point relative to this chunk</returns>
        public Point3 ToChunkCoord(Point3 worldPos)
        {
            return new Point3(
                worldPos.X - (int)(ChunkPos.X * ChunkSize * BlockManager.BlockSize),
                worldPos.Y - (int)(ChunkPos.Y * ChunkSize * BlockManager.BlockSize),
                worldPos.Z - (int)(ChunkPos.Z * ChunkSize * BlockManager.BlockSize));
        }

        /// <summary>
        /// Converts a chunk to a Point3
        /// </summary>
        /// <param name="c">The chunk to be converted</param>
        /// <returns>c cast to a Point3</returns>
        public static implicit operator Point3(Chunk c)
        {
            return c.ChunkPos;
        }

        /// <summary>
        /// Converts a point to a chunk
        /// </summary>
        /// <param name="p">The Point3 to convert</param>
        /// <returns>p cast to a Chunk</returns>
        public static implicit operator Chunk(Point3 p)
        {
            return new Chunk(p);
        }
    }
}
