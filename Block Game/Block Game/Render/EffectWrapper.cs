///Wrappers for custom HLSL shaders
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Block_Game.Utilities;

namespace Block_Game.Render
{
    /// <summary>
    /// Represents a point light shader
    /// </summary>
    public class PointLightEffect
    {
        /// <summary>
        /// The Effect to render with
        /// </summary>
        public readonly Effect BaseEffect;

        /// <summary>
        /// The maximum number of lights to use
        /// </summary>
        const int MAX_LIGHTS = 50;
        
        #region Transform
        /// <summary>
        /// The world transformation to use
        /// </summary>
        public Matrix World
        {
            get { return BaseEffect.Parameters["World"].GetValueMatrix(); }
            set { BaseEffect.Parameters["World"].SetValue(value); }
        }

        /// <summary>
        /// The view transformation to use
        /// </summary>
        public Matrix View
        {
            get { return BaseEffect.Parameters["View"].GetValueMatrix(); }
            set { BaseEffect.Parameters["View"].SetValue(value); }
        }

        /// <summary>
        /// The projection transformation to use
        /// </summary>
        public Matrix Projection
        {
            get { return BaseEffect.Parameters["Projection"].GetValueMatrix(); }
            set { BaseEffect.Parameters["Projection"].SetValue(value); }
        }
        #endregion

        #region DefaultLighting
        /// <summary>
        /// The color of the ambient lighting
        /// </summary>
        public Vector4 AmbientLightColor
        {
            get { return BaseEffect.Parameters["AmbientColor"].GetValueVector4(); }
            set { BaseEffect.Parameters["AmbientColor"].SetValue(value); }
        }

        /// <summary>
        /// The intensity for the ambient lighting. Default is 0.1
        /// </summary>
        public float AmbientLightIntensity
        {
            get { return BaseEffect.Parameters["AmbientIntensity"].GetValueSingle(); }
            set { BaseEffect.Parameters["AmbientIntensity"].SetValue(value); }
        }

        /// <summary>
        /// Represents the normal for the diffuse light
        /// </summary>
        public Vector3 DiffuseDirection
        {
            get { return BaseEffect.Parameters["DiffuseLightDirection"].GetValueVector3(); }
            set { BaseEffect.Parameters["DiffuseLightDirection"].SetValue(value); }
        }

        /// <summary>
        /// The color for the diffuse light
        /// </summary>
        public Vector4 DiffuseColor
        {
            get { return BaseEffect.Parameters["DiffuseColor"].GetValueVector4(); }
            set { BaseEffect.Parameters["DiffuseColor"].SetValue(value); }
        }

        /// <summary>
        /// The intensity of the diffuse
        /// </summary>
        public float DiffuseIntensity
        {
            get { return BaseEffect.Parameters["DiffuseIntensity"].GetValueSingle(); }
            set { BaseEffect.Parameters["DiffuseIntensity"].SetValue(value); }
        }

        #region Point Lights
        /// <summary>
        /// The array of all positions for point lights
        /// </summary>
        Vector3[] LightPositions = new Vector3[MAX_LIGHTS];

        /// <summary>
        /// The array of all Colors for point lights
        /// </summary>
        public Vector4[] LightDiffuseColors = new Vector4[MAX_LIGHTS];

        /// <summary>
        /// The array of all the light radui for point lights
        /// </summary>
        public float[] LightDistances = new float[MAX_LIGHTS];

        /// <summary>
        /// The number of point lights currently used
        /// </summary>
        int lightCount = 0;
        #endregion
        #endregion

        /// <summary>
        /// The texture to draw with
        /// </summary>
        public Texture2D Texture
        {
            get { return BaseEffect.Parameters["Texture"].GetValueTexture2D(); }
            set { BaseEffect.Parameters["Texture"].SetValue(value); }
        }

        /// <summary>
        /// Creates a new point light shader from a content manager
        /// </summary>
        /// <param name="content">The content manager to load from</param>
        public PointLightEffect(ContentManager content)
        {
            BaseEffect = content.Load<Effect>("Shaders/PointLight");
        }

        /// <summary>
        /// Creates a new point light shader from an effect
        /// </summary>
        /// <param name="BaseEffect">The effect to use</param>
        public PointLightEffect(Effect BaseEffect)
        {
            this.BaseEffect = BaseEffect;
        }

        /// <summary>
        /// Creates an exact duplicate of this shader
        /// </summary>
        /// <returns>An exact clone of this shader</returns>
        public PointLightEffect Clone()
        {
            PointLightEffect temp = new PointLightEffect(BaseEffect.Clone());

            temp.World = World;
            temp.View = View;
            temp.Projection = Projection;

            temp.AmbientLightColor = AmbientLightColor;
            temp.AmbientLightIntensity = AmbientLightIntensity;

            temp.DiffuseColor = DiffuseColor;
            temp.DiffuseDirection = DiffuseDirection;
            temp.DiffuseIntensity = DiffuseIntensity;

            temp.LightDiffuseColors = LightDiffuseColors;
            temp.LightDistances = LightDistances;
            temp.LightPositions = LightPositions;
            temp.lightCount = lightCount;

            return temp;
        }

        /// <summary>
        /// Adds a new point light to this shader
        /// </summary>
        /// <param name="light">The pointLight to add</param>
        /// <returns>The ID of the light, or -1 if it ould not be added</returns>
        public int AddPointLight(PointLight light)
        {
            if (lightCount + 1 < MAX_LIGHTS)
            {
                LightPositions[lightCount] = light.LightPos;

                LightDiffuseColors[lightCount] = light.DiffuseColor;
                LightDistances[lightCount] = light.Radius;

                lightCount++;

                UpdatePointLightLists();

                return lightCount - 1;
            }
            else
                return -1;
        }

        /// <summary>
        /// Sets the lights position
        /// </summary>
        /// <param name="lightID">The light ID of the light</param>
        /// <param name="pos">The new position for the light</param>
        public void SetLightPos(int lightID, Vector3 pos)
        {
            if (lightID >= 0 & lightID < lightCount)
            {
                LightPositions[lightID] = pos;
                
                UpdatePointLightLists();
            }
        }

        /// <summary>
        /// Sets the color of a light
        /// </summary>
        /// <param name="lightID">The light ID for the light</param>
        /// <param name="color">The new color for the light</param>
        public void SetLightColor(int lightID, Vector4 color)
        {
            if (lightID >= 0 & lightID < lightCount)
            {
                LightDiffuseColors[lightID] = color;

                UpdatePointLightLists();
            }
        }

        /// <summary>
        /// Sets a new radius for a light
        /// </summary>
        /// <param name="lightID">The light ID of the light</param>
        /// <param name="radius">The new radius of the light</param>
        public void SetLightRadius(int lightID, float radius)
        {
            if (lightID >= 0 & lightID < lightCount)
            {
                LightDistances[lightID] = radius;

                UpdatePointLightLists();
            }
        }

        /// <summary>
        /// Updates all the lighting lists on the GPU
        /// </summary>
        private void UpdatePointLightLists()
        {
            BaseEffect.Parameters["LightPositions"].SetValue(LightPositions);
            BaseEffect.Parameters["LightDiffuseColors"].SetValue(LightDiffuseColors);
            BaseEffect.Parameters["LightDistances"].SetValue(LightDistances);

            BaseEffect.Parameters["lightCount"].SetValue(lightCount);
        }
    }

    /// <summary>
    /// A wrapper for the normal mapping shader
    /// </summary>
    public class NormalMapEffect
    {
        /// <summary>
        /// The basic efefect to render with
        /// </summary>
        public readonly Effect BaseEffect;
        
        #region Transform
        /// <summary>
        /// The world transformation to use
        /// </summary>
        public Matrix World
        {
            get { return BaseEffect.Parameters["World"].GetValueMatrix(); }
            set { BaseEffect.Parameters["World"].SetValue(value); }
        }
        /// <summary>
        /// The view transformation to use
        /// </summary>
        public Matrix View
        {
            get { return BaseEffect.Parameters["View"].GetValueMatrix(); }
            set { BaseEffect.Parameters["View"].SetValue(value); }
        }
        /// <summary>
        /// The projection transformation to use
        /// </summary>
        public Matrix Projection
        {
            get { return BaseEffect.Parameters["Projection"].GetValueMatrix(); }
            set { BaseEffect.Parameters["Projection"].SetValue(value); }
        }
        #endregion

        #region DefaultLighting
        /// <summary>
        /// The color of the ambient lighting
        /// </summary>
        public Vector4 AmbientLightColor
        {
            get { return BaseEffect.Parameters["AmbientColor"].GetValueVector4(); }
            set { BaseEffect.Parameters["AmbientColor"].SetValue(value); }
        }

        /// <summary>
        /// The intensity for the ambient lighting. Default is 0.1
        /// </summary>
        public float AmbientLightIntensity
        {
            get { return BaseEffect.Parameters["AmbientIntensity"].GetValueSingle(); }
            set { BaseEffect.Parameters["AmbientIntensity"].SetValue(value); }
        }

        /// <summary>
        /// Represents the normal for the diffuse light
        /// </summary>
        public Vector3 DiffuseDirection
        {
            get { return BaseEffect.Parameters["lightDirection"].GetValueVector3(); }
            set { BaseEffect.Parameters["lightDirection"].SetValue(value); }
        }

        /// <summary>
        /// The color for the diffuse light
        /// </summary>
        public Vector4 DiffuseColor
        {
            get { return BaseEffect.Parameters["diffuseColor"].GetValueVector4(); }
            set { BaseEffect.Parameters["diffuseColor"].SetValue(value); }
        }
        #endregion

        /// <summary>
        /// The texture to render with
        /// </summary>
        public Texture2D Texture
        {
            get { return BaseEffect.Parameters["Texture"].GetValueTexture2D(); }
            set { BaseEffect.Parameters["Texture"].SetValue(value); }
        }
        
        /// <summary>
        /// The normal map to use
        /// </summary>
        public Texture2D NormalMap
        {
            get { return BaseEffect.Parameters["NormalMap"].GetValueTexture2D(); }
            set { BaseEffect.Parameters["NormalMap"].SetValue(value); }
        }

        /// <summary>
        /// Creates a new normal map shader using a content manager
        /// </summary>
        /// <param name="content">The content manager to load from</param>
        public NormalMapEffect(ContentManager content)
        {
            BaseEffect = content.Load<Effect>("Shaders/bumpmap");
        }

        /// <summary>
        /// Creates a new normal map using an effect
        /// </summary>
        /// <param name="BaseEffect">The effect to use</param>
        public NormalMapEffect(Effect BaseEffect)
        {
            this.BaseEffect = BaseEffect;
        }

        /// <summary>
        /// Clones this normal shader
        /// </summary>
        /// <returns>An exact copy of this shader</returns>
        public NormalMapEffect Clone()
        {
            NormalMapEffect temp = new NormalMapEffect(BaseEffect.Clone());

            temp.World = World;
            temp.View = View;
            temp.Projection = Projection;

            temp.AmbientLightColor = AmbientLightColor;
            temp.AmbientLightIntensity = AmbientLightIntensity;

            temp.DiffuseColor = DiffuseColor;
            temp.DiffuseDirection = DiffuseDirection;

            temp.Texture = Texture;
            temp.NormalMap = NormalMap;

            return temp;
        }
    }

    /// <summary>
    /// A wrapper around the standard shader
    /// </summary>
    public class StandardEffect
    {
        /// <summary>
        /// The Effect to render with
        /// </summary>
        public readonly Effect BaseEffect;

        #region Transform
        Matrix world;
        public Matrix World
        {
            get { return world; }
            set
            {
                world = value;
                BaseEffect.Parameters["World"].SetValue(world);
            }
        }
        Matrix view;
        public Matrix View
        {
            get { return view; }
            set
            {
                view = value;
                BaseEffect.Parameters["View"].SetValue(view);
            }
        }
        Matrix projection;
        public Matrix Projection
        {
            get { return projection; }
            set
            {
                projection = value;
                BaseEffect.Parameters["Projection"].SetValue(projection);
            }
        }
        #endregion

        #region DefaultLighting
        /// <summary>
        /// The color of the ambient lighting
        /// </summary>
        public Vector4 AmbientLightColor
        {
            get { return BaseEffect.Parameters["AmbientColor"].GetValueVector4(); }
            set { BaseEffect.Parameters["AmbientColor"].SetValue(value); }
        }

        /// <summary>
        /// The intensity for the ambient lighting. Default is 0.1
        /// </summary>
        public float AmbientLightIntensity
        {
            get { return BaseEffect.Parameters["AmbientIntensity"].GetValueSingle(); }
            set { BaseEffect.Parameters["AmbientIntensity"].SetValue(value); }
        }

        /// <summary>
        /// Represents the normal for the diffuse light
        /// </summary>
        public Vector3 DiffuseDirection
        {
            get { return BaseEffect.Parameters["DiffuseLightDirection"].GetValueVector3(); }
            set { BaseEffect.Parameters["DiffuseLightDirection"].SetValue(value); }
        }

        /// <summary>
        /// The color for the diffuse light
        /// </summary>
        public Vector4 DiffuseColor
        {
            get { return BaseEffect.Parameters["DiffuseColor"].GetValueVector4(); }
            set { BaseEffect.Parameters["DiffuseColor"].SetValue(value); }
        }

        /// <summary>
        /// The color for the diffuse light
        /// </summary>
        public float DiffuseIntensity
        {
            get { return BaseEffect.Parameters["DiffuseIntensity"].GetValueSingle(); }
            set { BaseEffect.Parameters["DiffuseIntensity"].SetValue(value); }
        }
        #endregion

        /// <summary>
        /// The texture to render with
        /// </summary>
        public Texture2D Texture
        {
            get { return BaseEffect.Parameters["Texture"].GetValueTexture2D(); }
            set { BaseEffect.Parameters["Texture"].SetValue(value); }
        }

        /// <summary>
        /// Creates a new StandardEffect
        /// </summary>
        /// <param name="content">The content manager to load from</param>
        public StandardEffect(ContentManager content)
        {
            BaseEffect = content.Load<Effect>("Shaders/StandardShader");
        }

        /// <summary>
        /// Creates a new standardeffect
        /// </summary>
        /// <param name="BaseEffect">The Effect to use</param>
        public StandardEffect(Effect BaseEffect)
        {
            this.BaseEffect = BaseEffect;
        }

        /// <summary>
        /// Creates a clone of this shader
        /// </summary>
        /// <returns>An exact clone of this shader</returns>
        public StandardEffect Clone()
        {
            StandardEffect temp = new StandardEffect(BaseEffect.Clone());

            temp.World = World;
            temp.View = View;
            temp.Projection = Projection;

            temp.AmbientLightColor = AmbientLightColor;
            temp.AmbientLightIntensity = AmbientLightIntensity;

            temp.DiffuseColor = DiffuseColor;
            temp.DiffuseDirection = DiffuseDirection;
            temp.DiffuseIntensity = DiffuseIntensity;

            temp.Texture = Texture;
            
            return temp;
        }
    }

    /// <summary>
    /// A Wrapper for the vegetation shader
    /// </summary>
    public class VegetationEffect
    {
        /// <summary>
        /// The base Effect class
        /// </summary>
        public readonly Effect BaseEffect;

        #region Transform
        /// <summary>
        /// The world transformation to apply to the effect
        /// </summary>
        public Matrix World
        {
            get { return BaseEffect.Parameters["World"].GetValueMatrix(); }
            set { BaseEffect.Parameters["World"].SetValue(value); }
        }

        /// <summary>
        /// The view transformation to apply to the effect
        /// </summary>
        public Matrix View
        {
            get { return BaseEffect.Parameters["View"].GetValueMatrix(); }
            set { BaseEffect.Parameters["View"].SetValue(value); }
        }

        /// <summary>
        /// The projection to apply to the effect
        /// </summary>
        public Matrix Projection
        {
            get { return BaseEffect.Parameters["Projection"].GetValueMatrix(); }
            set { BaseEffect.Parameters["Projection"].SetValue(value); }
        }

        /// <summary>
        /// A value describing the current flow of the wind in this shader
        /// </summary>
        public float Wind
        {
            get { return BaseEffect.Parameters["WindVar"].GetValueSingle(); }
            set { BaseEffect.Parameters["WindVar"].SetValue(value.Wrap(0,360)); }
        }
        #endregion

        #region DefaultLighting
        /// <summary>
        /// The color of the ambient lighting
        /// </summary>
        public Vector4 AmbientLightColor
        {
            get { return BaseEffect.Parameters["AmbientColor"].GetValueVector4(); }
            set { BaseEffect.Parameters["AmbientColor"].SetValue(value); }
        }

        /// <summary>
        /// The intensity for the ambient lighting. Default is 0.1
        /// </summary>
        public float AmbientLightIntensity
        {
            get { return BaseEffect.Parameters["AmbientIntensity"].GetValueSingle(); }
            set { BaseEffect.Parameters["AmbientIntensity"].SetValue(value); }
        }

        /// <summary>
        /// Represents the normal for the diffuse light
        /// </summary>
        public Vector3 DiffuseDirection
        {
            get { return BaseEffect.Parameters["DiffuseLightDirection"].GetValueVector3(); }
            set { BaseEffect.Parameters["DiffuseLightDirection"].SetValue(value); }
        }

        /// <summary>
        /// The color for the diffuse light
        /// </summary>
        public Vector4 DiffuseColor
        {
            get { return BaseEffect.Parameters["DiffuseColor"].GetValueVector4(); }
            set { BaseEffect.Parameters["DiffuseColor"].SetValue(value); }
        }

        /// <summary>
        /// The color for the diffuse light
        /// </summary>
        public float DiffuseIntensity
        {
            get { return BaseEffect.Parameters["DiffuseIntensity"].GetValueSingle(); }
            set { BaseEffect.Parameters["DiffuseIntensity"].SetValue(value); }
        }
        #endregion

        /// <summary>
        /// The texture used to render the effect with
        /// </summary>
        public Texture2D Texture
        {
            get { return BaseEffect.Parameters["Texture"].GetValueTexture2D(); }
            set { BaseEffect.Parameters["Texture"].SetValue(value); }
        }

        /// <summary>
        /// Creates a new vegetation shader
        /// </summary>
        /// <param name="content">The content to load from</param>
        public VegetationEffect(ContentManager content)
        {
            BaseEffect = content.Load<Effect>("Shaders/VegShader");
        }

        /// <summary>
        /// Creates a new vegetation shader
        /// </summary>
        /// <param name="BaseEffect">The BaseEffect to use</param>
        public VegetationEffect(Effect BaseEffect)
        {
            this.BaseEffect = BaseEffect;
        }

        /// <summary>
        /// Clones this shader
        /// </summary>
        /// <returns>A shader that has all values set the same</returns>
        public VegetationEffect Clone()
        {
            VegetationEffect temp = new VegetationEffect(BaseEffect.Clone());

            temp.World = World;
            temp.View = View;
            temp.Projection = Projection;
            temp.Wind = Wind;

            temp.AmbientLightColor = AmbientLightColor;
            temp.AmbientLightIntensity = AmbientLightIntensity;

            temp.DiffuseColor = DiffuseColor;
            temp.DiffuseDirection = DiffuseDirection;
            temp.DiffuseIntensity = DiffuseIntensity;

            temp.Texture = Texture;

            return temp;
        }
    }

    /// <summary>
    /// Represents a PointLight for shaders
    /// </summary>
    public struct PointLight
    {
        /// <summary>
        /// The color of this light
        /// </summary>
        public Vector4 DiffuseColor;
        /// <summary>
        /// How far the light travels
        /// </summary>
        public float Radius;
        /// <summary>
        /// The position of the light
        /// </summary>
        public Vector3 LightPos;
    };
}
