///A renderer for rending poly batches
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using BlockGame.Render;
using BlockGame;
using Microsoft.Xna.Framework;
using BlockGame.Utilities;

namespace BlockGame.Render
{
    /// <summary>
    /// Represents a polynomial renderer
    /// </summary>
    class PolyRender
    {
        /// <summary>
        /// The buffer for all opaque faces
        /// </summary>
        VertexPositionNormalTextureColor[] OpaqueBuffer = new VertexPositionNormalTextureColor[] { };
        /// <summary>
        /// The buffer for all non-opaque faces
        /// </summary>
        VertexPositionNormalTextureColor[] NonOpaqueBuffer = new VertexPositionNormalTextureColor[] { };
        /// <summary>
        /// The number of opaque primitives (triangles)
        /// </summary>
        int OpaquePrimitiveCount = 0;
        /// <summary>
        /// The number of non-opaque primitives (triangles)
        /// </summary>
        int NonOpaquePrimitiveCount = 0;
        /// <summary>
        /// The temporary buffer for the opaque vertices
        /// </summary>
        List<VertexPositionNormalTextureColor> OpaqueTemp = new List<VertexPositionNormalTextureColor>();
        /// <summary>
        /// The temporary buffer for the non-opaque vertices
        /// </summary>
        List<VertexPositionNormalTextureColor> NonOpaqueTemp = new List<VertexPositionNormalTextureColor>();
        /// <summary>
        /// The world transformation for this renderer
        /// </summary>
        public Matrix World;

        /// <summary>
        /// Creates a new poly renderer
        /// </summary>
        /// <param name="World">The world transformation to use</param>
        public PolyRender(Matrix World)
        {
            this.World = World;
        }

        /// <summary>
        /// clears the temporary and final buffers
        /// </summary>
        public void Clear()
        {
            OpaqueTemp.Clear();
            OpaqueBuffer = new VertexPositionNormalTextureColor[] { };

            NonOpaqueTemp.Clear();
            NonOpaqueBuffer = new VertexPositionNormalTextureColor[] { };
        }

        /// <summary>
        /// Adds a range of opaque vertices to the temp buffer
        /// </summary>
        /// <param name="buffer">The buffer to append</param>
        public void AddOpaquePolys(VertexPositionNormalTextureColor[] buffer)
        {
            OpaqueTemp.AddRange(buffer);
        }

        /// <summary>
        /// Adds a range of non-opaque vertices to the temp buffer
        /// </summary>
        /// <param name="buffer">The buffer to append</param>
        public void AddNonOpaquePolys(VertexPositionNormalTextureColor[] buffer)
        {
            NonOpaqueTemp.AddRange(buffer);
        }

        /// <summary>
        /// Copies over and clears the temp buffers
        /// </summary>
        public void FinalizePolys()
        {
            OpaqueBuffer = OpaqueTemp.ToArray();
            OpaqueTemp.Clear();
            OpaquePrimitiveCount = OpaqueBuffer.Length / 3;

            NonOpaqueBuffer = NonOpaqueTemp.ToArray();
            NonOpaqueTemp.Clear();
            NonOpaquePrimitiveCount = NonOpaqueBuffer.Length / 3;
        }

        /// <summary>
        /// Renders Opaque and non-opaque polys
        /// </summary>
        /// <param name="view">The view paramaters to render with</param>
        public void Render(ViewParameters view)
        {
            RenderOpaque(view);
            RenderNonOpaque(view);
        }

        /// <summary>
        /// Renders all opaque polys
        /// </summary>
        /// <param name="view">The view parameters to render with</param>
        public void RenderOpaque(ViewParameters view)
        {
            if (OpaquePrimitiveCount > 0)
            {
                Game1.worldEffect.World = World * view.World;

                foreach (EffectPass p in Game1.worldEffect.BaseEffect.CurrentTechnique.Passes)
                {
                    p.Apply();

                    Game1.worldEffect.BaseEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTextureColor>(
                        PrimitiveType.TriangleList,
                        OpaqueBuffer, 0, OpaquePrimitiveCount);
                }
            }
        }

        /// <summary>
        /// Draws all the non-opaque polys
        /// </summary>
        /// <param name="view">The view parameters to render with</param>
        public void RenderNonOpaque(ViewParameters view)
        {
            if (NonOpaquePrimitiveCount > 0)
            {
                Game1.worldEffect.World = World * view.World;

                foreach (EffectPass p in Game1.worldEffect.BaseEffect.CurrentTechnique.Passes)
                {
                    p.Apply();

                    Game1.worldEffect.BaseEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTextureColor>(PrimitiveType.TriangleList,
                        NonOpaqueBuffer, 0, NonOpaquePrimitiveCount);
                }
            }
        }
    }
}
