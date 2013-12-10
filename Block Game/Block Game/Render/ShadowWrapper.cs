using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace BlockGame.Render
{
    public class ShadowWrapper : Effect
    {
        /// <summary>
        /// The texture to apply to the drawn elements
        /// </summary>
        public Texture2D Texture
        {
            get { return Parameters["xTexture"].GetValueTexture2D();}
            set { Parameters["xTexture"].SetValue(value); }
        }

        /// <summary>
        /// The shadow map generated
        /// </summary>
        public Texture2D ShadowMap
        {
            get { return Parameters["xShadowMap"].GetValueTexture2D(); }
            set { Parameters["xShadowMap"].SetValue(value); }
        }

        /// <summary>
        /// The light mask to apply to the light
        /// </summary>
        public Texture2D LightMask
        {
            get { return Parameters["xCarLightTexture"].GetValueTexture2D(); }
            set { Parameters["xCarLightTexture"].SetValue(value); }
        }

        /// <summary>
        /// The camera's view projection (view * projection)
        /// </summary>
        public Matrix CameraViewProjection
        {
            get { return Parameters["xCamerasViewProjection"].GetValueMatrix(); }
            set { Parameters["xCamerasViewProjection"].SetValue(value); }
        }

        /// <summary>
        /// The light's view projection (view * projection)
        /// </summary>
        public Matrix LightViewProjection
        {
            get { return Parameters["xLightsViewProjection"].GetValueMatrix(); }
            set { Parameters["xLightsViewProjection"].SetValue(value); }
        }

        /// <summary>
        /// The world transformation matrix
        /// </summary>
        public Matrix World
        {
            get { return Parameters["xWorld"].GetValueMatrix(); }
            set { Parameters["xWorld"].SetValue(value); }
        }

        /// <summary>
        /// The power of the light
        /// </summary>
        public float LightPower
        {
            get { return Parameters["xLightPower"].GetValueSingle(); }
            set { Parameters["xLightPower"].SetValue(value); }
        }

        /// <summary>
        /// The ambient light power
        /// </summary>
        public float AmbientPower
        {
            get { return Parameters["xAmbient"].GetValueSingle(); }
            set { Parameters["xAmbient"].SetValue(value); }
        }

        /// <summary>
        /// The render target to draw the shadow map to
        /// </summary>
        private RenderTarget2D tempTarget;

        public ShadowWrapper(ContentManager Content, string name)
            : base(Content.Load<Effect>(name))
        {
            
        }

        public void StartShadowMapRender()
        {
            //GraphicsDevice.SetRenderTarget(tempTarget);
            CurrentTechnique = Techniques["ShadowMap"];
        }

        public void EndShadowMapRender()
        {
            GraphicsDevice.SetRenderTarget(null);
            ShadowMap = tempTarget;
        }

        public void StartShadowedRender()
        {
            CurrentTechnique = Techniques["ShadowedScene"];
        }
    }
}
