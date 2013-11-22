using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockGame.Utilities;
using Microsoft.Xna.Framework;
using BlockGame.Render;

namespace BlockGame.Blocks
{
    /// <summary>
    /// Used to contruct block models from block facings
    /// </summary>
    public static class BlockRenderFaces
    {
        /// <summary>
        /// Creates a quad from the given parameters
        /// </summary>
        /// <param name="Centre">The centre point of the quad</param>
        /// <param name="Normal">The normal vector of the quad</param>
        /// <param name="CrossNormal">The cross normal of the quad</param>
        /// <param name="texID">The texture ID for the quad</param>
        /// <param name="size">The size of the quad</param>
        /// <returns>A VPNTC array representing the quad</returns>
        public static VertexPositionNormalTextureColor[] GetFacesFromNormal(Vector3 Centre, Vector3 Normal, Vector3 CrossNormal, byte texID, float size)
        {
            Normal.Normalize();
            CrossNormal.Normalize();
            Centre *= size;

            Vector3 Cross = Vector3.Normalize(Vector3.Cross(Normal, CrossNormal));
            Vector3 TL = (Centre + Cross - CrossNormal + Normal);
            Vector3 TR = (Centre + Cross + CrossNormal + Normal);
            Vector3 BL = (Centre - Cross - CrossNormal + Normal);
            Vector3 BR = (Centre - Cross + CrossNormal + Normal);

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

            //Matrix trans = Matrix.CreateTranslation(Centre);

            //for (int i = 0; i < temp.Length; i++)
            //{
            //    temp[i].Position = Vector3.Transform(temp[i].Position, trans);
            //}

            return temp;
        }

        /// <summary>
        /// Returns all the faces from the given render state
        /// </summary>
        /// <param name="state">The state to generate for</param>
        /// <param name="point">The centre of the cube</param>
        /// <param name="texID">The texture ID for the cube</param>
        /// <returns>A VPNTC array representing the block</returns>
        public static VertexPositionNormalTextureColor[] GetFacesFromState(BlockRenderStates state, Point3 point, byte texID)
        {
            List<VertexPositionNormalTextureColor> temp = new List<VertexPositionNormalTextureColor>();

            if (state.HasFlag(BlockRenderStates.Front))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Front.NormalVector(), BlockFacing.Front.CrossNormalVector(), texID, BlockRenderer.BlockSize));

            if (state.HasFlag(BlockRenderStates.Back))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Back.NormalVector(), BlockFacing.Back.CrossNormalVector(), texID, BlockRenderer.BlockSize));

            if (state.HasFlag(BlockRenderStates.Top))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Top.NormalVector(), BlockFacing.Top.CrossNormalVector(), texID, BlockRenderer.BlockSize)); ;

            if (state.HasFlag(BlockRenderStates.Bottom))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Bottom.NormalVector(), BlockFacing.Bottom.CrossNormalVector(), texID, BlockRenderer.BlockSize));

            if (state.HasFlag(BlockRenderStates.Left))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Left.NormalVector(), BlockFacing.Left.CrossNormalVector(), texID, BlockRenderer.BlockSize));

            if (state.HasFlag(BlockRenderStates.Right))
                temp.AddRange(GetFacesFromNormal(point, BlockFacing.Right.NormalVector(), BlockFacing.Right.CrossNormalVector(), texID, BlockRenderer.BlockSize));

            return temp.ToArray();
        }

        /// <summary>
        /// Gets the vertices for the given face
        /// </summary>
        /// <param name="facing">The facing to get the vertices for</param>
        /// <param name="point">The cntre of the face</param>
        /// <param name="texID">The texture ID to use</param>
        /// <returns>A VPNTC array representing the face</returns>
        public static VertexPositionNormalTextureColor[] GetFacesFromFacing(BlockFacing facing, Point3 point, byte texID)
        {
            return (GetFacesFromNormal(point, facing.NormalVector(), facing.CrossNormalVector(), texID, BlockRenderer.BlockSize));
        }

        /// <summary>
        /// Gets the inverted faces for a block face
        /// </summary>
        /// <param name="facing">The facing to generate for</param>
        /// <param name="point">The centre of the face</param>
        /// <param name="texID">The texture ID to use</param>
        /// <returns>A VPNTC array representing the inverted face</returns>
        public static VertexPositionNormalTextureColor[] GetInvertedFacesFromFacing(BlockFacing facing, Point3 point, byte texID)
        {
            return (GetFacesFromNormal(point, facing.NormalVector() * -1, facing.CrossNormalVector() * -1, texID, BlockRenderer.BlockSize));
        }
    }

    /// <summary>
    /// Represents a vertex buffer to render a block with
    /// </summary>
    public class BlockRenderer
    {
        /// <summary>
        /// Represents the width of a single block
        /// </summary>
        public const float BlockSize = 2.0F;
        /// <summary>
        /// Represents half the size of a single block
        /// </summary>
        public static readonly float HalfSize = BlockSize / 2F;

        /// <summary>
        /// Gets or sets the vertices for this block model
        /// </summary>
        public VertexPositionNormalTextureColor[] verts = new VertexPositionNormalTextureColor[0];
    }
}
