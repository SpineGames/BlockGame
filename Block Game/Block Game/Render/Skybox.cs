using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BlockGame;
using BlockGame.Utilities;
using Spine_Library.Tools;
using Block_Game.Utilities;

namespace Block_Game.Render
{
    public class Sun
    {
        const float Radius = 100;
        float sunAngle;
        Vector3 Pos;
        VertexPositionNormalTextureColor[] verts =
        BlockRenderFaces.GetFacesFromState(
            BlockRenderStates.Back |
            BlockRenderStates.Front |
            BlockRenderStates.Left |
            BlockRenderStates.Right |
            BlockRenderStates.Top |
            BlockRenderStates.Bottom, new Point3(0, 0, 0), 23);

        public Sun()
        {
            foreach (VertexPositionNormalTextureColor t in verts)
            {
                Vector3.Transform(t.Position, Matrix.CreateScale(10F));
            }
        }

        public void Render(Camera camera)
        {
            Pos = camera.CameraPos + new Vector3(
                    0,
                    (float)extraMath.lengthdir_x(MathHelper.ToRadians(sunAngle), 500),
                    (float)extraMath.lengthdir_y(MathHelper.ToRadians(sunAngle), 500));

            Game1.worldEffect.World = Matrix.CreateScale(10F) * Matrix.CreateTranslation(Pos);

            Game1.worldEffect.View = camera.View.View;
            Game1.worldEffect.Projection = camera.View.Projection;

            foreach (EffectPass p in Game1.worldEffect.BaseEffect.CurrentTechnique.Passes)
            {
                p.Apply();

                Game1.worldEffect.BaseEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTextureColor>(PrimitiveType.TriangleList,
                       verts, 0, 2);
            }
        }

        public void SunTick()
        {
            sunAngle += 0.1F;
            if (sunAngle >= 360)
                sunAngle -= 360;
        }

    }
}
