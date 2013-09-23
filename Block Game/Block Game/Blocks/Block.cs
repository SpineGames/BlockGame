using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BlockGame.Render;
using BlockGame.Utilities;
using System.ComponentModel;
using Block_Game.Utilities;

namespace BlockGame
{
    public abstract class BlockManager
    {
        public const float BlockSize = 2;
        public static readonly float HalfSize = BlockSize / 2F;
        public static readonly IBlock[] Blocks = new IBlock[256];
        public static readonly IBlock Air = new BlockAir();
        public static readonly IBlock Gravel = new BlockGravel();
        public static readonly IBlock Dirt = new BlockDirt();
        public static readonly IBlock Glass = new BlockGlass();
        public static readonly IBlock Log = new BlockLog();

        public static void AddBlock(IBlock block)
        {
            Blocks[block.ID] = block;
        }
    }

    public interface IBlock
    {
        VertexPositionNormalTextureColor[] GetModel(BlockRenderStates facings, Point3 pos, byte Meta);

        byte GetTexIDForFacing(BlockFacing facing);

        bool IsOpaqueOnFace(BlockFacing facing);

        byte ID {get;}
        byte texRef { get; }
        bool IsOpaque { get; }
        string Name { get; }
    }

    public abstract class BaseBlock : IBlock
    {
        public byte ID { get { return 0; } }
        public byte texRef { get { return 2; } }
        public bool IsOpaque { get { return true; } }
        public string Name { get { return "ERROR"; } }

        public BaseBlock()
        {
            BlockManager.AddBlock(this);
        }

        public VertexPositionNormalTextureColor[] GetModel(BlockRenderStates facings, Point3 pos, byte Meta)
        {
            List<VertexPositionNormalTextureColor> temp = new List<VertexPositionNormalTextureColor>();

            foreach (BlockFacing f in (BlockFacing[])Enum.GetValues(typeof(BlockFacing)))
            {
                if (facings.IsFaced(f))
                    temp.AddRange(BlockRenderFaces.GetFacesFromFacing(f, pos, GetTexIDForFacing(f)));
            }

            return temp.ToArray();// BlockRenderFaces.GetFacesFromState(facings, pos, this.texRef);
        }

        public byte GetTexIDForFacing(BlockFacing facing)
        {
            return texRef;
        }

        public bool IsOpaqueOnFace(BlockFacing facing)
        {
            return IsOpaque;
        }
    }

    public class BlockAir : BaseBlock, IBlock
    {
        public byte ID { get { return 0; } }
        public byte texRef { get { return 0; } }
        public bool IsOpaque { get { return false; } }
        public string Name { get { return "Air"; } }

        public VertexPositionNormalTextureColor[] GetModel(BlockRenderStates facings, Point3 pos, byte Meta)
        {
            List<VertexPositionNormalTextureColor> temp = new List<VertexPositionNormalTextureColor>();

            foreach (BlockFacing f in (BlockFacing[])Enum.GetValues(typeof(BlockFacing)))
            {
                if (facings.IsFaced(f))
                    temp.AddRange(BlockRenderFaces.GetInvertedFacesFromFacing(f, pos, GetTexIDForFacing(f), 0.1F));
            }

            return temp.ToArray();// BlockRenderFaces.GetFacesFromState(facings, pos, this.texRef);
        }
    }

    public class BlockDirt : BaseBlock, IBlock
    {
        public byte ID { get { return 1; } }
        public byte texRef { get { return 2; } }
        public bool IsOpaque { get { return true; } }
        public string Name { get { return "Dirt"; } }
    }

    public class BlockGravel : BaseBlock, IBlock
    {
        public byte ID { get { return 2; } }
        public byte texRef { get { return 0; } }
        public bool IsOpaque { get { return true; } }
        public string Name { get { return "Gravel"; } }
    }

    public class BlockGlass : BaseBlock, IBlock
    {
        public byte ID { get { return 3; } }
        public byte texRef { get { return 49; } }
        public bool IsOpaque { get { return false; } }
        public string Name { get { return "Glass"; } }
    }

    public class BlockLog : BaseBlock, IBlock
    {
        public byte ID { get { return 4; } }
        public byte texRef { get { return 20; } }
        public bool IsOpaque { get { return true; } }
        public string Name { get { return "Log"; } }

        public byte GetTexIDForFacing(BlockFacing facing)
        {
            switch (facing)
            {
                case BlockFacing.Top | BlockFacing.Bottom:
                    return 21;
                default:
                    return 20;
            }
        }
    }

    public static class BlockRenderFaces
    {
        static Color DefaultColor { get { return Color.Black; } }
        #region Front
        public static readonly VertexPositionNormalTextureColor[] FrontFace = new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), new Vector2(0,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), new Vector2(0,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), new Vector2(1,0)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), new Vector2(0,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), new Vector2(1,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), new Vector2(1,0))
        };
        #endregion

        #region Back
        public static readonly VertexPositionNormalTextureColor[] BackFace = new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), new Vector2(0,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), new Vector2(1,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), new Vector2(0,1)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), new Vector2(0,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), new Vector2(1,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), new Vector2(1,1))
        };
        #endregion

        #region Top
        public static readonly VertexPositionNormalTextureColor[] TopFace = new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), new Vector2(0,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), new Vector2(1,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), new Vector2(0,1)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), new Vector2(0,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), new Vector2(1,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), new Vector2(1,1))
        };
        #endregion

        #region Bottom
        public static readonly VertexPositionNormalTextureColor[] BottomFace = new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), new Vector2(0,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), new Vector2(0,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), new Vector2(1,0)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), new Vector2(0,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), new Vector2(1,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), new Vector2(1,0))
        };
        #endregion

        #region Left
        public static readonly VertexPositionNormalTextureColor[] LeftFace = new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), new Vector2(0,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), new Vector2(0,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), new Vector2(1,0)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), new Vector2(0,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), new Vector2(1,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), new Vector2(1,0))
        };
        #endregion

        #region Right
        public static readonly VertexPositionNormalTextureColor[] RightFace = new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), new Vector2(0,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), new Vector2(1,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), new Vector2(0,1)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), new Vector2(0,1)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), new Vector2(1,0)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), new Vector2(1,1))
        };
        #endregion

        public static VertexPositionNormalTextureColor[] GetFacesFromState(BlockRenderStates state, Point3 point)
        {
            List<VertexPositionNormalTextureColor> temp = new List<VertexPositionNormalTextureColor>();

            if (state.HasFlag(BlockRenderStates.Front))
                temp.AddRange(FrontFace);

            if (state.HasFlag(BlockRenderStates.Back))
                temp.AddRange(BackFace);

            if (state.HasFlag(BlockRenderStates.Top))
                temp.AddRange(TopFace);

            if (state.HasFlag(BlockRenderStates.Bottom))
                temp.AddRange(BottomFace);

            if (state.HasFlag(BlockRenderStates.Left))
                temp.AddRange(LeftFace);

            if (state.HasFlag(BlockRenderStates.Right))
                temp.AddRange(RightFace);

            VertexPositionNormalTextureColor[] Temp2 = temp.ToArray();

            Matrix trans = Matrix.CreateTranslation(point.X * BlockManager.BlockSize, point.Y * BlockManager.BlockSize, point.Z * BlockManager.BlockSize);

            for (int i = 0; i < temp.Count; i++)
            {
                Temp2[i].Position = Vector3.Transform(temp[i].Position, trans);
            }

            return Temp2;
        }

        public static VertexPositionNormalTextureColor[] GetFacesFromState(BlockRenderStates state, Point3 point, byte texID)
        {
            List<VertexPositionNormalTextureColor> temp = new List<VertexPositionNormalTextureColor>();

            #region Front Face
            if (state.HasFlag(BlockRenderStates.Front))
                temp.AddRange(new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.TR(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.BR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.TR(texID))
                });
            #endregion

            #region Back Face
            if (state.HasFlag(BlockRenderStates.Back))
                temp.AddRange(new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.BL(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.BR(texID))
                });
            #endregion

            #region Top Face
            if (state.HasFlag(BlockRenderStates.Top))
                temp.AddRange(new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.BL(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.BR(texID))
                });
            #endregion

            #region Bottom Face
            if (state.HasFlag(BlockRenderStates.Bottom))
                temp.AddRange(new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.TR(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.BR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.TR(texID))
                });
            #endregion

            #region Left Face
            if (state.HasFlag(BlockRenderStates.Left))
                temp.AddRange(new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.TR(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.BR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.TR(texID))
                });
            #endregion

            #region Right Face
            if (state.HasFlag(BlockRenderStates.Right))
                temp.AddRange(new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.BL(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.BR(texID))
                });
            #endregion

            VertexPositionNormalTextureColor[] Temp2 = temp.ToArray();

            Matrix trans = Matrix.CreateTranslation(point.X * BlockManager.BlockSize, point.Y * BlockManager.BlockSize, point.Z * BlockManager.BlockSize);

            for (int i = 0; i < temp.Count; i++)
            {
                Temp2[i].Position = Vector3.Transform(temp[i].Position, trans);
            }

            return Temp2;
        }

        public static VertexPositionNormalTextureColor[] GetFacesFromFacing(BlockFacing facing, Point3 point, byte texID)
        {
            VertexPositionNormalTextureColor[] temp = new VertexPositionNormalTextureColor[6];

            switch (facing)
            {
                #region Front
                case BlockFacing.Front:
                    temp = (new VertexPositionNormalTextureColor[]{
                new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.TR(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.BR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.TR(texID))
                });
                    break;
                #endregion

                #region Back Face
                case BlockFacing.Back:
                    temp = (new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.BL(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.BR(texID))
                });
                    break;
                #endregion

                #region Top Face
                case BlockFacing.Top:
                    temp = (new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.BL(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.BR(texID))
                });
                    break;
                #endregion

                #region Bottom Face
                case BlockFacing.Bottom:
                    temp = (new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.TR(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.BR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.TR(texID))
                });
                    break;
                #endregion

                #region Left Face
                case BlockFacing.Left:
                    temp = (new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.TR(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.BR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.TR(texID))
                });
                    break;
                #endregion

                #region Right Face
                case BlockFacing.Right:
                    temp = (new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.BL(texID)),
                
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.TR(texID)),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.BR(texID))
                });
                    break;
                #endregion
            }

            Matrix trans = Matrix.CreateTranslation(point.X * BlockManager.BlockSize, point.Y * BlockManager.BlockSize, point.Z * BlockManager.BlockSize);

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].Position = Vector3.Transform(temp[i].Position, trans);
            }

            return temp;
        }

        public static VertexPositionNormalTextureColor[] GetInvertedFacesFromFacing(BlockFacing facing, Point3 point, byte texID, float alpha)
        {
            Color tColor = new Color(1, 1, 1, alpha);
            VertexPositionNormalTextureColor[] temp = new VertexPositionNormalTextureColor[6];

            switch (facing)
            {
                #region Front
                case BlockFacing.Front:
                    temp = (new VertexPositionNormalTextureColor[]{
                new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.TL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.TR(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.BL(texID), tColor),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.BL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.TR(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,1,0), TextureManager.BR(texID), tColor)
                });
                    break;
                #endregion

                #region Back Face
                case BlockFacing.Back:
                    temp = (new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.TL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.BL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.TR(texID), tColor),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.BL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.BR(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,-1,0), TextureManager.TR(texID), tColor)
                });
                    break;
                #endregion

                #region Top Face
                case BlockFacing.Top:
                    temp = (new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.TL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.BL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.TR(texID), tColor),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.BL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.BR(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(0,0,1), TextureManager.TR(texID), tColor)
                });
                    break;
                #endregion

                #region Bottom Face
                case BlockFacing.Bottom:
                    temp = (new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.TL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.TR(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.BL(texID), tColor),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.BL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.TR(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(0,0,-1), TextureManager.BR(texID), tColor)
                });
                    break;
                #endregion

                #region Left Face
                case BlockFacing.Left:
                    temp = (new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.TL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.TR(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.BL(texID), tColor),
                
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.BL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.TR(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(-BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(-1,0,0), TextureManager.BR(texID), tColor)
                });
                    break;
                #endregion

                #region Right Face
                case BlockFacing.Right:
                    temp = (new VertexPositionNormalTextureColor[]{
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.TL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.BL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.TR(texID), tColor),
                
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, -BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.BL(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.BR(texID), tColor),
            new VertexPositionNormalTextureColor(
                new Vector3(BlockManager.HalfSize, BlockManager.HalfSize, -BlockManager.HalfSize), new Vector3(1,0,0), TextureManager.TR(texID), tColor)
                });
                    break;
                #endregion
            }

            Matrix trans = Matrix.CreateTranslation(point.X * BlockManager.BlockSize, point.Y * BlockManager.BlockSize, point.Z * BlockManager.BlockSize);

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].Position = Vector3.Transform(temp[i].Position, trans);
            }

            return temp;
        }
    }

    [Flags]
    public enum BlockRenderStates
    {
        None = 0,
        Front = 1 << 0,
        Back = 1 << 2,
        Left = 1 << 3,
        Right = 1 << 4,
        Top = 1 << 5,
        Bottom = 1 << 6
    }

    public class BlockRenderer
    {
        public VertexPositionNormalTextureColor[] verts = new VertexPositionNormalTextureColor[0];
    }

    public enum BlockFacing
    {
        Front,
        Back,
        Left,
        Right,
        Top,
        Bottom
    }

    public static class BlockFacingExt
    {
        private static Dictionary<BlockFacing, Point3> list = new Dictionary<BlockFacing, Point3>()
        {
            {BlockFacing.Front, new Point3(0,1,0)},
            {BlockFacing.Back, new Point3(0,-1,0)},
            {BlockFacing.Left, new Point3(-1,0,0)},
            {BlockFacing.Right, new Point3(1,0,0)},
            {BlockFacing.Top, new Point3(0,0,1)},
            {BlockFacing.Bottom, new Point3(0,0,-1)}
        };

        public static Point3 NormalVector(this BlockFacing self)
        {
            switch (self)
            {
                case BlockFacing.Front:
                    return new Point3(0, 1, 0);
                case BlockFacing.Back:
                    return new Point3(0, -1, 0);
                case BlockFacing.Left:
                    return new Point3(-1, 0, 0);
                case BlockFacing.Right:
                    return new Point3(1, 0, 0);
                case BlockFacing.Top:
                    return new Point3(0, 0, 1);
                case BlockFacing.Bottom:
                    return new Point3(0, 0, -1);
                default: return null;
            }
        }

        public static bool IsFaced(this BlockRenderStates self, BlockFacing facing)
        {
            switch (facing)
            {
                case BlockFacing.Back:
                    return self.HasFlag(BlockRenderStates.Back);
                case BlockFacing.Front:
                    return self.HasFlag(BlockRenderStates.Front);
                case BlockFacing.Left:
                    return self.HasFlag(BlockRenderStates.Left);
                case BlockFacing.Right:
                    return self.HasFlag(BlockRenderStates.Right);
                case BlockFacing.Top:
                    return self.HasFlag(BlockRenderStates.Top);
                case BlockFacing.Bottom:
                    return self.HasFlag(BlockRenderStates.Bottom);
                default :
                    return false;
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    public struct BlockData
    {
        public byte ID;
        public byte Meta;

        public BlockData(byte ID, byte Meta = 0)
        {
            this.ID = ID;
            this.Meta = 0;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", ID, Meta);
        }
    }
}
