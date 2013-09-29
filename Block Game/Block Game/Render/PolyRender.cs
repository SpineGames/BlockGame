///A renderer for rending poly batches
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Block_Game.Render;
using Block_Game;
using Microsoft.Xna.Framework;
using Block_Game.Utilities;

namespace BlockGame.Render
{
    class PolyRender
    {
        VertexPositionNormalTextureColor[] OpaqueBuffer = new VertexPositionNormalTextureColor[] { };
        VertexPositionNormalTextureColor[] NonOpaqueBuffer = new VertexPositionNormalTextureColor[] { };
        int OpaquePrimitiveCount = 0;
        int NonOpaquePrimitiveCount = 0;
        List<VertexPositionNormalTextureColor> OpaqueTemp = new List<VertexPositionNormalTextureColor>();
        List<VertexPositionNormalTextureColor> NonOpaqueTemp = new List<VertexPositionNormalTextureColor>();
        public Matrix World;

        public PolyRender(Matrix World)
        {
            this.World = World;
        }

        public void Clear()
        {
            OpaqueTemp.Clear();
            OpaqueBuffer = new VertexPositionNormalTextureColor[] { };

            NonOpaqueTemp.Clear();
            NonOpaqueBuffer = new VertexPositionNormalTextureColor[] { };
        }

        public void AddOpaquePolys(VertexPositionNormalTextureColor[] buffer)
        {
            OpaqueTemp.AddRange(buffer);
        }

        public void AddNonOpaquePolys(VertexPositionNormalTextureColor[] buffer)
        {
            NonOpaqueTemp.AddRange(buffer);
        }

        public void FinalizePolys()
        {
            OpaqueBuffer = OpaqueTemp.ToArray();
            OpaqueTemp.Clear();
            OpaquePrimitiveCount = OpaqueBuffer.Length / 3;

            NonOpaqueBuffer = NonOpaqueTemp.ToArray();
            NonOpaqueTemp.Clear();
            NonOpaquePrimitiveCount = NonOpaqueBuffer.Length / 3;
        }

        public void Render(ViewParameters view)
        {
            RenderOpaque(view);
            RenderNonOpaque(view);
        }

        public void RenderOpaque(ViewParameters view)
        {
            if (OpaquePrimitiveCount > 0)
            {
                Game1.worldEffect.World = World;

                foreach (EffectPass p in Game1.worldEffect.BaseEffect.CurrentTechnique.Passes)
                {
                    p.Apply();

                    Game1.worldEffect.BaseEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTextureColor>(PrimitiveType.TriangleList,
                        OpaqueBuffer, 0, OpaquePrimitiveCount);
                }
            }
        }

        public void RenderNonOpaque(ViewParameters view)
        {
            if (NonOpaquePrimitiveCount > 0)
            {
                Game1.worldEffect.World = World;

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
