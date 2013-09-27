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
        public const float BlockSize = 1;
        public static readonly float HalfSize = BlockSize / 2F;
        public static readonly IBlock[] Blocks = new IBlock[256];
        public static readonly IBlock Air = new BlockAir();
        public static readonly IBlock Stone = new BlockStone();
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

        byte GetTexIDForFacing(BlockFacing facing, byte meta = 0);

        bool IsOpaqueOnFace(BlockFacing facing, byte meta = 0);

        byte ID {get;}
        byte texRef { get; }
        bool IsOpaque { get; }
        string Name { get; }
    }

    public abstract class BaseBlock : IBlock
    {
        public abstract byte ID { get; }
        public abstract byte texRef { get; }
        public abstract bool IsOpaque { get; }
        public abstract string Name { get; }

        public BaseBlock()
        {
            BlockManager.AddBlock(this);
        }

        public virtual VertexPositionNormalTextureColor[] GetModel(BlockRenderStates facings, Point3 pos, byte Meta)
        {
            List<VertexPositionNormalTextureColor> temp = new List<VertexPositionNormalTextureColor>();

            foreach (BlockFacing f in (BlockFacing[])Enum.GetValues(typeof(BlockFacing)))
            {
                if (facings.IsFaced(f))
                    temp.AddRange(BlockRenderFaces.GetFacesFromFacing(f, pos, GetTexIDForFacing(f, Meta)));
            }

            return temp.ToArray();// BlockRenderFaces.GetFacesFromState(facings, pos, this.texRef);
        }

        public virtual byte GetTexIDForFacing(BlockFacing facing, byte meta = 0)
        {
            return texRef;
        }

        public virtual bool IsOpaqueOnFace(BlockFacing facing, byte meta = 0)
        {
            return IsOpaque;
        }
    }

    public class BlockAir : BaseBlock, IBlock
    {
        public override byte ID { get { return 0; } }
        public override byte texRef { get { return 0; } }
        public override bool IsOpaque { get { return false; } }
        public override string Name { get { return "Air"; } }

        public override VertexPositionNormalTextureColor[] GetModel(BlockRenderStates facings, Point3 pos, byte Meta)
        {
            return new VertexPositionNormalTextureColor[0];
            //List<VertexPositionNormalTextureColor> temp = new List<VertexPositionNormalTextureColor>();

            //foreach (BlockFacing f in (BlockFacing[])Enum.GetValues(typeof(BlockFacing)))
            //{
            //    if (facings.IsFaced(f))
            //        temp.AddRange(BlockRenderFaces.GetInvertedFacesFromFacing(f, pos, GetTexIDForFacing(f), 0.1F));
            //}

            //return temp.ToArray();// BlockRenderFaces.GetFacesFromState(facings, pos, this.texRef);
        }
    }

    public class BlockStone : BaseBlock, IBlock
    {
        public override byte ID { get { return 1; } }
        public override byte texRef { get { return 0; } }
        public override bool IsOpaque { get { return true; } }
        public override string Name { get { return "Stone"; } }
    }

    public class BlockDirt : BaseBlock, IBlock
    {
        public override byte ID { get { return 2; } }
        public override byte texRef { get { return 1; } }
        public override bool IsOpaque { get { return true; } }
        public override string Name { get { return "Dirt"; } }

        public override byte GetTexIDForFacing(BlockFacing facing, byte meta = 0)
        {
            if (meta != 0)
            {
                switch (facing)
                {
                    case BlockFacing.Top:
                        return 7;
                    case BlockFacing.Bottom:
                        return 1;
                    default:
                        return 8;
                }
            }
            else
                return 1;
        }
    }

    public class BlockGravel : BaseBlock, IBlock
    {
        public override byte ID { get { return 3; } }
        public override byte texRef { get { return 2; } }
        public override bool IsOpaque { get { return true; } }
        public override string Name { get { return "Gravel"; } }
    }

    public class BlockGlass : BaseBlock, IBlock
    {
        public override byte ID { get { return 4; } }
        public override byte texRef { get { return 3; } }
        public override bool IsOpaque { get { return false; } }
        public override string Name { get { return "Glass"; } }
    }

    public class BlockLog : BaseBlock, IBlock
    {
        public override byte ID { get { return 5; } }
        public override byte texRef { get { return 4; } }
        public override bool IsOpaque { get { return true; } }
        public override string Name { get { return "Log"; } }

        public override byte GetTexIDForFacing(BlockFacing facing, byte meta = 0)
        {
            switch (facing)
            {
                case BlockFacing.Top:
                    return 5;
                case BlockFacing.Bottom:
                    return 5;
                default:
                    return 4;
            }
        }
    }

    public class BlockSand : BaseBlock, IBlock
    {
        public override byte ID { get { return 6; } }
        public override byte texRef { get { return 6; } }
        public override bool IsOpaque { get { return true; } }
        public override string Name { get { return "Sand"; } }
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

        public static VertexPositionNormalTextureColor[] GetFacesFromNormal(Vector3 Centre, Vector3 Normal, Vector3 CrossNormal, byte texID, float size)
        {
            Normal.Normalize();
            CrossNormal.Normalize();

            Vector3 UpDown = Vector3.Normalize(Vector3.Cross(Normal, CrossNormal));
            Vector3 TL = (Centre + UpDown - CrossNormal + Normal) * size;
            Vector3 TR = (Centre + UpDown + CrossNormal + Normal) * size;
            Vector3 BL = (Centre - UpDown - CrossNormal + Normal) * size;
            Vector3 BR = (Centre - UpDown + CrossNormal + Normal) * size;

            VertexPositionNormalTextureColor[] temp = new VertexPositionNormalTextureColor[]
            {
            new VertexPositionNormalTextureColor(
                TL, Normal, TextureManager.TL(texID)),
            new VertexPositionNormalTextureColor(
                BL, Normal, TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                TR, Normal, TextureManager.TR(texID)),
                
            new VertexPositionNormalTextureColor(
                BL, Normal, TextureManager.BL(texID)),
            new VertexPositionNormalTextureColor(
                BR, Normal, TextureManager.BR(texID)),
            new VertexPositionNormalTextureColor(
                TR, Normal, TextureManager.TR(texID))
            };
            
            Matrix trans = Matrix.CreateTranslation(Centre);

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].Position = Vector3.Transform(temp[i].Position, trans);
            }

            return temp;
        }

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

            return temp.ToArray();
        }

        public static VertexPositionNormalTextureColor[] GetFacesFromState(BlockRenderStates state, Point3 point, byte texID)
        {
            List<VertexPositionNormalTextureColor> temp = new List<VertexPositionNormalTextureColor>();

            if (state.HasFlag(BlockRenderStates.Front))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Front.NormalVector(), BlockFacing.Front.CrossNormalVector(), texID, BlockManager.BlockSize));

            if (state.HasFlag(BlockRenderStates.Back))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Back.NormalVector(), BlockFacing.Back.CrossNormalVector(), texID, BlockManager.BlockSize));

            if (state.HasFlag(BlockRenderStates.Top))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Top.NormalVector(), BlockFacing.Top.CrossNormalVector(), texID, BlockManager.BlockSize)); ;

            if (state.HasFlag(BlockRenderStates.Bottom))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Bottom.NormalVector(), BlockFacing.Bottom.CrossNormalVector(), texID, BlockManager.BlockSize));

            if (state.HasFlag(BlockRenderStates.Left))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Left.NormalVector(), BlockFacing.Right.CrossNormalVector(), texID, BlockManager.BlockSize));

            if (state.HasFlag(BlockRenderStates.Right))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Right.NormalVector(), BlockFacing.Right.CrossNormalVector(), texID, BlockManager.BlockSize));

            return temp.ToArray();
        }

        public static VertexPositionNormalTextureColor[] GetFacesFromFacing(BlockFacing facing, Point3 point, byte texID)
        {
            VertexPositionNormalTextureColor[] temp = new VertexPositionNormalTextureColor[6];

            switch (facing)
            {
                case BlockFacing.Front:
                    temp = (GetFacesFromNormal(point, BlockFacing.Front.NormalVector(), BlockFacing.Front.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;

                case BlockFacing.Back:
                    temp = (GetFacesFromNormal(point, BlockFacing.Back.NormalVector(), BlockFacing.Back.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;

                case BlockFacing.Top:
                    temp = (GetFacesFromNormal(point, BlockFacing.Top.NormalVector(), BlockFacing.Top.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;

                case BlockFacing.Bottom:
                    temp = (GetFacesFromNormal(point, BlockFacing.Bottom.NormalVector(), BlockFacing.Bottom.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;

                case BlockFacing.Left:
                    temp = (GetFacesFromNormal(point, BlockFacing.Left.NormalVector(), BlockFacing.Right.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;

                case BlockFacing.Right:
                    temp = (GetFacesFromNormal(point, BlockFacing.Right.NormalVector(), BlockFacing.Right.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;
            }

            return temp;
        }

        public static VertexPositionNormalTextureColor[] GetInvertedFacesFromFacing(BlockFacing facing, Point3 point, byte texID, float alpha)
        {
            Color tColor = new Color(1, 1, 1, alpha);
            VertexPositionNormalTextureColor[] temp = new VertexPositionNormalTextureColor[6];

            switch (facing)
            {
                case BlockFacing.Front:
                    temp = (GetFacesFromNormal(point - BlockFacing.Back.NormalVector(), BlockFacing.Back.NormalVector(), BlockFacing.Back.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;

                case BlockFacing.Back:
                    temp = (GetFacesFromNormal(point - BlockFacing.Front.NormalVector(), BlockFacing.Back.NormalVector(), BlockFacing.Back.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;

                case BlockFacing.Top:
                    temp = (GetFacesFromNormal(point, BlockFacing.Top.NormalVector(), BlockFacing.Top.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;

                case BlockFacing.Bottom:
                    temp = (GetFacesFromNormal(point, BlockFacing.Bottom.NormalVector(), BlockFacing.Bottom.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;

                case BlockFacing.Left:
                    temp = (GetFacesFromNormal(point, BlockFacing.Left.NormalVector(), BlockFacing.Right.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;

                case BlockFacing.Right:
                    temp = (GetFacesFromNormal(point, BlockFacing.Right.NormalVector(), BlockFacing.Right.CrossNormalVector(), texID, BlockManager.BlockSize));
                    break;
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

        public static Point3 CrossNormalVector(this BlockFacing self)
        {
            switch (self)
            {
                case BlockFacing.Front:
                    return new Point3(1, 0, 0);
                case BlockFacing.Back:
                    return new Point3(-1, 0, 0);
                case BlockFacing.Left:
                    return new Point3(0, 1, 0);
                case BlockFacing.Right:
                    return new Point3(0, -1, 0);
                case BlockFacing.Top:
                    return new Point3(1, 0, 0);
                case BlockFacing.Bottom:
                    return new Point3(1, 0, 0);
                default: return null;
            }
        }

        public static BlockFacing ToBlockFacing(this float direction)
        {
            direction = direction.Wrap(0, 360);

            if (direction > 45 & direction < 135)
                return BlockFacing.Front;
            if (direction >= 135 & direction <= 225)
                return BlockFacing.Left;
            if (direction > 225 & direction < 315)
                return BlockFacing.Back;
            return BlockFacing.Right;
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
            this.Meta = Meta;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", ID, Meta);
        }
    }
}
