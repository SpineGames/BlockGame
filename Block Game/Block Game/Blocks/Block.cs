///Multiple classes to do with blocks from BLock Game
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BlockGame.Render;
using BlockGame.Utilities;
using System.ComponentModel;
using BlockGame.Blocks;
using BlockGame.Blocks.BlockTypes;

namespace BlockGame
{
    /// <summary>
    /// A static class for managing all the block types
    /// </summary>
    public static class BlockManager
    {
        /// <summary>
        /// A list holding all the block types
        /// </summary>
        public static readonly Block[] Blocks = new Block[256];

        public static readonly Block Air = new BlockAir();
        public static readonly Block Stone = new BlockStone();
        public static readonly Block Gravel = new BlockGravel();
        public static readonly Block Sand = new BlockSand();
        public static readonly Block Dirt = new BlockDirt();
        public static readonly Block Glass = new BlockGlass();
        public static readonly Block Log = new BlockLog();
        public static readonly Block Leaves = new BlockLeaves();
        public static readonly Block Water = new BlockWater();
        public static readonly Block IronOre = new BlockIronOre();
        public static readonly Block GoldOre = new BlockGoldOre();
        public static readonly Block DiamondOre = new BlockDiamondOre();

        /// <summary>
        /// Adds the given block to the block list and overwrites the given BlockID
        /// </summary>
        /// <param name="block">The block to add</param>
        public static void AddBlock(Block block)
        {
            Blocks[block.ID] = block;
        }

        /// <summary>
        /// Gets the block with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Block GetBlock(string name)
        {
            foreach (Block b in Blocks)
                if (b != null && b.Name == name)
                    return b;
            return null;
        }
    }

    /// <summary>
    /// The base class for all block types
    /// </summary>
    public abstract class Block
    {
        private static VertexPositionNormalTextureColor[][] models;

        /// <summary>
        /// Gets the block ID for this block
        /// </summary>
        public abstract byte ID { get; }
        /// <summary>
        /// Gets the standard texture refrence
        /// </summary>
        public abstract byte texRef { get; }
        /// <summary>
        /// Gets whether or not this block is opaque
        /// </summary>
        public virtual bool IsOpaque { get { return true; } }
        /// <summary>
        /// Gets the code name for this block
        /// </summary>
        public virtual string Name { get { return this.GetType().Name.Replace("Block", ""); } }
        /// <summary>
        /// Gets the name for this block in an inventory
        /// </summary>
        public virtual string ItemName { get { return this.GetType().Name.Replace("Block", ""); } }

        /// <summary>
        /// Initializes this block and adds it to the block manager
        /// </summary>
        public Block()
        {
            BlockManager.AddBlock(this);
        }
        
        /// <summary>
        /// Gets the model for this block from the given block facings
        /// </summary>
        /// <param name="facings">The facings to generate with</param>
        /// <param name="pos">The world pos to generate at</param>
        /// <param name="Meta">The block's meta-data</param>
        /// <returns>A VPNTC array for this block</returns>
        public virtual VertexPositionNormalTextureColor[] GetModel(BlockRenderStates facings, Point3 pos, byte Meta)
        {
            VertexPositionNormalTextureColor[] model = new VertexPositionNormalTextureColor[facings.FaceCount() * 6];

            int id = 0;

            foreach (BlockFacing f in BlockFacingExt.Facings)
            {
                if (facings.IsFaced(f))
                    BlockRenderFaces.AddFacesFromFacing(f, pos, GetTexIDForFacing(f, Meta), ref model, ref id);
            }

            return model;
        }
       
        /// <summary>
        /// Gets the texture ID for the given facing
        /// </summary>
        /// <param name="facing">The facing to check</param>
        /// <param name="meta">The metadata of the block</param>
        /// <returns>The texture ID for the facing</returns>
        public virtual byte GetTexIDForFacing(BlockFacing facing, byte meta = 0)
        {
            return texRef;
        }

        /// <summary>
        /// Gets whether or not the given face on a block is opaque
        /// </summary>
        /// <param name="facing">The facing to check</param>
        /// <param name="meta">The block's metaData</param>
        /// <returns>True if the face is opaque</returns>
        public virtual bool IsOpaqueOnFace(BlockFacing facing, byte meta = 0)
        {
            return IsOpaque;
        }
    }

    /// <summary>
    /// Represents the data for a block instance
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    public struct BlockData
    {
        /// <summary>
        /// The ID of the block
        /// </summary>
        public byte ID;
        /// <summary>
        /// The MetaData of the block
        /// </summary>
        public byte Meta;

        /// <summary>
        /// Creates a new Block Data
        /// </summary>
        /// <param name="ID">The block's ID</param>
        /// <param name="Meta">The block's MetaData</param>
        public BlockData(byte ID, byte Meta = 0)
        {
            this.ID = ID;
            this.Meta = Meta;
        }

        /// <summary>
        /// Converts this block data to a string
        /// </summary>
        /// <returns>The string version of this block data</returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", ID, Meta);
        }
    }
}
