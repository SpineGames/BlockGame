using BlockGame;
using BlockGame.Utilities;
using System.Collections.Generic;
using BlockGame.Render;
using Microsoft.Xna.Framework;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a slope block
    /// </summary>
    public class BlockSlope : Block
    {
        public override byte ID { get { return 7; } }
        public override byte texRef { get { return 6; } }
        public override bool IsOpaque { get { return false; } }

        /// <summary>
        /// Holds the block texrefs
        /// </summary>
        private static byte[] TexRefs = new byte[] { 0, 1, 4, 6, 7 };

        /// <summary>
        /// Overrides the getModel to get a unique model
        /// </summary>
        /// <param name="facings">The facings to use</param>
        /// <param name="pos">The world position of this object</param>
        /// <param name="Meta">The metadata for this block</param>
        /// <returns></returns>
        public override VertexPositionNormalTextureColor[] GetModel(BlockRenderStates facings, Point3 pos, byte Meta)
        {
            List<VertexPositionNormalTextureColor> temp = new List<VertexPositionNormalTextureColor>();

            BlockRenderStates state;
            byte texR = TexRefs[Meta << 4];

            if (Meta >> 7 == 0)
            {
                temp.AddRange(BlockRenderFaces.GetFacesFromFacing(BlockFacing.Left, pos, texR));
                temp.AddRange(BlockRenderFaces.GetFacesFromFacing(BlockFacing.Bottom, pos, texR));
                temp.AddRange(BlockRenderFaces.GetFacesFromNormal(pos, BlockFacing.Bottom.NormalVector(),
                    BlockFacing.Bottom.CrossNormalVector(), texR, BlockRenderer.BlockSize));

                temp.Add(new VertexPositionNormalTextureColor(pos + new Vector3(-BlockRenderer.HalfSize, -BlockRenderer.HalfSize, BlockRenderer.HalfSize),
                    new Vector3(0, -1, 0), TextureManager.TL(texR)));
                temp.Add(new VertexPositionNormalTextureColor(pos + new Vector3(-BlockRenderer.HalfSize, -BlockRenderer.HalfSize, -BlockRenderer.HalfSize),
                    new Vector3(0, -1, 0), TextureManager.BL(texR)));
            }
            else
            {
                state = BlockRenderStates.Left | BlockRenderStates.Top;
            }

            return temp.ToArray();
        }
    }
}