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
using BlockGame;
using BlockGame.Blocks;
using System.Timers;
using System.Threading;

namespace BlockGame.Blocks
{
    /// <summary>
    /// Represents a chunk of block data
    /// </summary>
    public class Chunk
    {
        protected readonly Point3 _worldPos;

        /// <summary>
        /// The size of chunks in blocks
        /// </summary>
        public const int ChunkSize = 32;
        /// <summary>
        /// Gets the size of a chunk in a point3
        /// </summary>
        public static readonly Point3 ChunkSizeP = new Point3(ChunkSize);

        /// <summary>
        /// Gets the size of a chunk in the world
        /// </summary>
        public static Point3 WorldChunkSize
        {
            get { return new Point3((int)(ChunkSize * BlockRenderer.BlockSize)); }
        }
        /// <summary>
        /// The array holding all of the block ID's and metaData
        /// </summary>
        byte[, ,] _ids = new byte[ChunkSize, ChunkSize, ChunkSize];
        /// <summary>
        /// The array holding all of the block ID's and metaData
        /// </summary>
        byte[, ,] _metas = new byte[ChunkSize, ChunkSize, ChunkSize];
        /// <summary>
        /// A list of all the block render faces
        /// </summary>
        BlockRenderStates[,,] _renderStates = new BlockRenderStates[ChunkSize, ChunkSize, ChunkSize];
        /// <summary>
        /// An array holding all of the render states for easier updating later
        /// </summary>
        /// BlockRenderer[, ,] renderStates = new BlockRenderer[ChunkSize, ChunkSize, ChunkSize];
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
                return _worldPos;
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
                return ChunkPos * (int)(ChunkSize * BlockRenderer.BlockSize);
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
        BoundingBox Bounding;
        /// <summary>
        /// The renderer used to render this chunk
        /// </summary>
        PolyRender Renderer;

        VertexPositionNormalTextureColor[] _vertices;
        
        /// <summary>
        /// Creates a new chunk with the given chunk position
        /// </summary>
        /// <param name="chunkPos">The position of the chunk relative to other chunks</param>
        public Chunk(Point3 chunkPos)
        {
            this.ChunkPos = chunkPos;

            _worldPos = ChunkPos * (int)(ChunkSize);

            Bounding = new BoundingBox(TransformedWorldPos, TransformedWorldPos + WorldChunkSize);

            collision = new Cuboid(WorldPos, MaxWorldPos);
            Renderer = new PolyRender(Matrix.CreateTranslation(TransformedWorldPos));

            GenChunk();
            UpdateVoxelData();
        }

        void UpdateVoxelData()
        {
            VertexPositionNormalTextureColor[] vertices = new VertexPositionNormalTextureColor[GetTotalFaces() * 6];

            int id = 0;

            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        if (_ids[x, y, z] != 0)
                        BlockManager.Blocks[_ids[x, y, z]].AddModel(
                        _renderStates[x, y, z], new Point3(x, y, z), _metas[x, y, z], ref vertices, ref id);
                    }
                }
            }

            _vertices = vertices;
        }

        int GetTotalFaces()
        {
            int count = 0;

            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        count += _renderStates[x, y, z].FaceCount();
                    }
                }
            }

            return count;
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
                        _renderStates[x, y, z] = BlockRenderStates.None;
                    }
                }
            }
        }

        /// <summary>
        /// Invalidates all the renderstates along one fae of the chunk
        /// </summary>
        /// <param name="facing">The facing to invalidate</param>
        public void InvalidateChunkFace(BlockFacing facing)
        {
            switch (facing)
            {
                case BlockFacing.Back:
                    UpdateRenderStates(new Point3(0, 31, 0), new Point3(31,31,31));
                    break;
                case BlockFacing.Front:
                    UpdateRenderStates(new Point3(0, 0, 0), new Point3(31, 0, 31));
                    break;
                case BlockFacing.Left:
                    UpdateRenderStates(new Point3(0, 0, 0), new Point3(0, 31, 31));
                    break;
                case BlockFacing.Right:
                    UpdateRenderStates(new Point3(31, 0, 0), new Point3(31, 31, 31));
                    break;
                case BlockFacing.Top:
                    UpdateRenderStates(new Point3(0, 0, 31), new Point3(31, 31, 31));
                    break;
                case BlockFacing.Bottom:
                    UpdateRenderStates(new Point3(0, 0, 0), new Point3(31, 31, 0));
                    break;
            }
        }

        /// <summary>
        /// Updates all render states in a cuboid
        /// </summary>
        /// <param name="min">The minimum of the cube to update (chunk)</param>
        /// <param name="max">The maximum of the cube to update (chunk)</param>
        private void UpdateRenderStates(Point3 min, Point3 max)
        {     
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
        }

        /// <summary>
        /// Updates all render states in a cuboid
        /// </summary>
        /// <param name="min">The minimum of the cube to update (world)</param>
        /// <param name="max">The maximum of the cube to update (worl)</param>
        public void ForceUpdate(Cuboid cuboid)
        {
            UpdateRenderStates(cuboid.Min, cuboid.Max);
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
                if (_ids[x, y, z] == 0)
                    _renderStates[x, y, z] = BlockRenderStates.None;
                else
                    _renderStates[x, y, z] = GetRenderStateForBlock(x, y, z);
            }
        }
        
        /// <summary>
        /// Returns the BlockRenderState at {x,y,z}
        /// </summary>
        /// <param name="x">The x co-ord of the block (chunk)</param>
        /// <param name="y">The y co-ord of the block (chunk)</param>
        /// <param name="z">The z co-ord of the block (chunk)</param>
        /// <returns>The BlockRenderState for {x,y,z}</returns>
        private BlockRenderStates GetRenderStateForBlock(int x, int y, int z)
        {
            BlockRenderStates ret = BlockRenderStates.None; // initially no render

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
            Point3 facePos = position + facing.NormalVector();

            if (facePos.X >= 0 & facePos.X < ChunkSize &
                facePos.Y >= 0 & facePos.Y < ChunkSize &
                facePos.Z >= 0 & facePos.Z < ChunkSize)
            {
                if (GetBlockID(facePos) == 0)
                    return true; //main cause to render the face is if it faces air

                if (IsOpaque(position) & !IsOpaque(facePos))
                {
                    return true; //second case is if there is a non-opaque and an opaque
                }

                if (!IsOpaque(position) & !IsOpaque(facePos) & (GetBlockID(position) != GetBlockID(facePos)))
                    return true; //final case is two different types of transparent blocks
            }
            else
            {
                position += WorldPos;
                facePos += WorldPos;

                if (World.GetBlockID(facePos) == 0)
                    return true; //main cause to render the face is if it faces air

                if (World.IsOpaque(position) & !World.IsOpaque(facePos))
                {
                    return true; //second case is if there is a non-opaque and an opaque
                }


                if (!World.IsOpaque(position) & !World.IsOpaque(facePos) & (World.GetBlockID(position) != World.GetBlockID(facePos)))
                    return true; //final case is two different types of transparent blocks
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
            else
                return World.IsOpaque(x + WorldPos.X, y + WorldPos.Y, z + WorldPos.Z); //otherwise assume it's air (TO BE FIXED WITH WORLD!!!)
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
        /// <param name="x">The block's x co-ord (world)</param>
        /// <param name="y">The block's y co-ord (world)</param>
        /// <param name="z">The block's z co-ord (world)</param>
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
            if (Game1.IsBebugging)
                RenderBounds(camera);

            if (camera.ViewFrustum.Contains(Bounding) != ContainmentType.Disjoint && _vertices.Length > 0)
            {
                Game1.worldEffect.World = Matrix.CreateTranslation(TransformedWorldPos);

                Game1.worldEffect.CurrentTechnique.Passes[0].Apply();

                Game1.worldEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTextureColor>(
                    PrimitiveType.TriangleList, _vertices, 0, _vertices.Length / 3);
            }
        }

        private void RenderBounds(Camera camera)
        {
            //Utils.ApplyCamera(camera);
            //Utils.DrawBoundingBox(Bounding, Color.Red, camera.GraphicsDevice);
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
                return _ids[x, y, z];
            }
            return 0;
        }

        /// <summary>
        /// Return the ID of the block at {x,y,z}
        /// </summary>
        /// <param name="x">The x co-ord of the block (world)</param>
        /// <param name="y">The y co-ord of the block (world)</param>
        /// <param name="z">The z co-ord of the block (world)</param>
        /// <returns>The block's ID at {x,y,z}</returns>
        public byte GetBlockIDFromWorld(int x, int y, int z)
        {
            x -= WorldPos.X;
            y -= WorldPos.Y;
            z -= WorldPos.Z;

            if (IsinRange(x, y, z))
            {
                return _ids[x, y, z];
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
                return _metas[x, y, z];
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
        public byte GetBlockData(int x, int y, int z)
        {
            if (IsinRange(x, y, z))
            {
                return _ids[x, y, z];
            }
            return 0;
        }

        /// <summary>
        /// Gets the block data at the specified chunk co-ordinates
        /// </summary>
        /// <param name="Pos">The chunk co-ords of the block to check</param>
        /// <returns>The block's data at Pos</returns>
        public byte GetBlockData(Point3 Pos)
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
            //Stopwatch w = new Stopwatch();
            //w.Start();

            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        SetBlockWithoutNotify(x, y, z,
                            TerrainGen.GetBlockAtPos(x + WorldPos.X, y + WorldPos.Y, z + WorldPos.Z));
                    }
                }
            }

            int treeCount = TerrainGen.Random.Next(5, 20);

            for (int i = 0; i < treeCount; i++)
            {
                int cx = TerrainGen.Random.Next(2, ChunkSize - 2);
                int cy = TerrainGen.Random.Next(2, ChunkSize - 2);
                int wz = World.GetTopZ(cx + WorldPos.X, cy + WorldPos.Y, 500);
                int cz = wz - WorldPos.Z;

                TerrainGen.GenTree(cx, cy, cz,
                    4 + TerrainGen.Random.Next(0, 3), this);
            }

            //w.Stop();
            //Debug.WriteLine("Took {0} milliseconds to generate chunk data", w.ElapsedMilliseconds);

            //w.Start();
            UpdateRenderStates(new Point3(0), new Point3(ChunkSize));
            //w.Stop();
            //Debug.WriteLine("Took {0} milliseconds to generate chunk render data", w.ElapsedMilliseconds);
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
            SetBlockWithoutNotify(x, y, z, ID);
        }

        /// <summary>
        /// Sets the block at pos
        /// </summary>
        /// <param name="pos">The chunk co-ords for this block</param>
        /// <param name="ID">The ID to set the block to</param>
        private void SetBlock(Point3 pos, byte ID)
        {
            SetBlockWithoutNotify(pos.X, pos.Y, pos.Z, ID);
        }

        /// <summary>
        /// Sets the block at {x,y,z} to dat
        /// </summary>
        /// <param name="x">The x co-ords of the block (chunk)</param>
        /// <param name="y">The y co-ords of the block (chunk)</param>
        /// <param name="z">The z co-ords of the block (chunk)</param>
        /// <param name="ID">The ID of the block to set at {x,y,z}</param>
        /// <param name="meta">The meta data of the block to set at {x,y,z}</param>
        public void SetBlockWithoutNotify(int x, int y, int z, byte ID, byte meta = 0)
        {
            if (IsinRange(x, y, z))
            {
                _ids[x, y, z] = ID;
                _metas[x, y, z] = meta;
                _renderStates[x, y, z] = GetRenderStateForBlock(x, y, z);
            }
        } //base function

        /// <summary>
        /// Sets the block at {x,y,z} to dat
        /// </summary>
        /// <param name="x">The x co-ords of the block (chunk)</param>
        /// <param name="y">The y co-ords of the block (chunk)</param>
        /// <param name="z">The z co-ords of the block (chunk)</param>
        /// <param name="data">The block data to set at {x,y,z}</param>
        public void SetBlockWithoutNotify(int x, int y, int z, BlockData data)
        {
            if (IsinRange(x, y, z))
            {
                _ids[x, y, z] = data.ID;
                _metas[x, y, z] = data.Meta;
                _renderStates[x, y, z] = GetRenderStateForBlock(x, y, z);
            }
        } //base function

        /// <summary>
        /// Sets the block at {x,y,z} to dat
        /// </summary>
        /// <param name="x">The x co-ords of the block (world)</param>
        /// <param name="y">The y co-ords of the block (world)</param>
        /// <param name="z">The z co-ords of the block (world)</param>
        /// <param name="dat">The block data to set at {x,y,z}</param>
        public void SetBlockFromWorld(int x, int y, int z, byte id)
        {
            x -= WorldPos.X;
            y -= WorldPos.Y;
            z -= WorldPos.Z;

            SetBlockWithUpdate(x, y, z, id);
        }

        /// <summary>
        /// Sets the block at {x,y,z} to dat
        /// </summary>
        /// <param name="x">The x co-ords of the block (world)</param>
        /// <param name="y">The y co-ords of the block (world)</param>
        /// <param name="z">The z co-ords of the block (world)</param>
        /// <param name="dat">The block data to set at {x,y,z}</param>
        public void SetBlockFromWorldNoNotify(int x, int y, int z, byte ID)
        {
            x -= WorldPos.X;
            y -= WorldPos.Y;
            z -= WorldPos.Z;

            SetBlockWithoutNotify(x, y, z, ID);
        }

        /// <summary>
        /// Sets the block at {x,y,z} to dat and updates the renderer
        /// </summary>
        /// <param name="x">The x co-ords of the block (chunk)</param>
        /// <param name="y">The y co-ords of the block (chunk)</param>
        /// <param name="z">The z co-ords of the block (chunk)</param>
        /// <param name="dat">The block data to set at {x,y,z}</param>
        private void SetBlockWithUpdate(int x, int y, int z, byte id)
        {
            SetBlockWithoutNotify(x, y, z, id);
            UpdateVoxelData();
        }

        /// <summary>
        /// Sets the block at {x,y,z} to dat and updates the renderer
        /// </summary>
        /// <param name="x">The x co-ords of the block (chunk)</param>
        /// <param name="y">The y co-ords of the block (chunk)</param>
        /// <param name="z">The z co-ords of the block (chunk)</param>
        /// <param name="dat">The block data to set at {x,y,z}</param>
        private void SetBlockWithUpdate(Point3 pos, byte id)
        {
            SetBlockWithUpdate(pos.X, pos.Y, pos.Z, id);
        }

        /// <summary>
        /// Sets a block of blocks to a single block data
        /// </summary>
        /// <param name="min">The minimum position to start from</param>
        /// <param name="max">The max position to start from</param>
        /// <param name="dat">The block data to set the region to</param>
        private void SetCuboid(Point3 min, Point3 max, byte id)
        {
            for (int x = min.X; x <= max.X; x++)
            {
                for (int y = min.Y; y <= max.Y; y++)
                {
                    for (int z = min.Z; z <= max.Z; z++)
                    {
                        SetBlockWithoutNotify(x, y, z, id);
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
        private void SetCuboid(Cuboid cuboid, byte id)
        {
            SetCuboid(cuboid.Min - WorldPos, cuboid.Max - WorldPos, id);

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
                        if (Vector3.Distance(centre, new Vector3(x, y, z)) <= radius)
                            SetBlock(x, y, z, ID);
                    }
                }
            }
            Point3 size = new Point3((int)Math.Ceiling(radius + BlockRenderer.BlockSize));
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
                worldPos.X - (int)(ChunkPos.X * ChunkSize * BlockRenderer.BlockSize),
                worldPos.Y - (int)(ChunkPos.Y * ChunkSize * BlockRenderer.BlockSize),
                worldPos.Z - (int)(ChunkPos.Z * ChunkSize * BlockRenderer.BlockSize));
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

        private class RegionEventArgs
        {
            public Point3 Min;
            public Point3 Max;

            public RegionEventArgs(Point3 min, Point3 max)
            {
                Min = min;
                Max = max;
            }
        }
    }
}
