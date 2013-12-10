///A dysfunctional skybox class
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BlockGame;
using BlockGame.Utilities;
using Spine_Library.Tools;
using BlockGame.Blocks;

namespace BlockGame.Render
{
    /// <summary>
    /// Represents the sun
    /// </summary>
    public class Sun
    {
        /// <summary>
        /// The pixel-distance from the sun to the player
        /// </summary>
        const float Radius = 100;
        /// <summary>
        /// The angle that the sun is relative to the player
        /// </summary>
        float sunAngle;
        /// <summary>
        /// The centre point of the sun
        /// </summary>
        Vector3 Pos;
        /// <summary>
        /// The vertices to render
        /// </summary>
        VertexPositionNormalTextureColor[] verts =
        BlockRenderFaces.GetFacesFromState(
            BlockRenderStates.Back |
            BlockRenderStates.Front |
            BlockRenderStates.Left |
            BlockRenderStates.Right |
            BlockRenderStates.Top |
            BlockRenderStates.Bottom, new Point3(0, 0, 0), 23);

        /// <summary>
        /// Creates a new sun
        /// </summary>
        public Sun()
        {
            foreach (VertexPositionNormalTextureColor t in verts)
            {
                Vector3.Transform(t.Position, Matrix.CreateScale(10F));
            }
        }

        /// <summary>
        /// Renders this sun
        /// </summary>
        /// <param name="camera">The camera to render with</param>
        public void Render(Camera camera)
        {
            Pos = camera.CameraPos + new Vector3(
                    0,
                    (float)extraMath.lengthdir_x(MathHelper.ToRadians(sunAngle), 500),
                    (float)extraMath.lengthdir_y(MathHelper.ToRadians(sunAngle), 500));

            Game1.worldEffect.World = Matrix.CreateScale(10F) * Matrix.CreateTranslation(Pos);
            
            foreach (EffectPass p in Game1.worldEffect.CurrentTechnique.Passes)
            {
                p.Apply();

                Game1.worldEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTextureColor>(PrimitiveType.TriangleList,
                       verts, 0, 2);
            }
        }

        /// <summary>
        /// Ticks this sun
        /// </summary>
        public void SunTick()
        {
            sunAngle += 0.1F;
            if (sunAngle >= 360)
                sunAngle -= 360;
        }

    }
}
