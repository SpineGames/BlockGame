///A texture manager for acessing multiple sub textures from one main texture
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BlockGame.Render
{
    /// <summary>
    /// Manages a texture for 3D shiat
    /// </summary>
    public static class TextureManager
    {
        /// <summary>
        /// The terrain texture
        /// </summary>
        static Texture2D terrain;
        /// <summary>
        /// The normal map for terrain
        /// </summary>
        static Texture2D normal;
        /// <summary>
        /// The width/height of each texture in the sheet
        /// </summary>
        const float TexSize = 16;
        /// <summary>
        /// What percentage each pixel represents
        /// </summary>
        const float PercentPerBlock = (float)(0.0625F);

        /// <summary>
        /// Initializes the texture manager
        /// </summary>
        /// <param name="terrain">The terrain texture to use</param>
        /// <param name="normal">The normal map for terrain</param>
        public static void Initialize(Texture2D terrain, Texture2D normal)
        {
            TextureManager.terrain = terrain;
            TextureManager.normal = normal;
        }

        /// <summary>
        /// Gets the bottom-left texture co-ord for the texture ID
        /// </summary>
        /// <param name="ID">The texture ID to search for</param>
        /// <returns>The bottom-left corner for the texture refrence</returns>
        public static Vector2 BL(byte ID)
        {
            int y = (int)Math.Round(ID / 16F);
            int x = ID - (int)(y * 16);

            return new Vector2((float)x * PercentPerBlock, (float)y * PercentPerBlock);
        }

        /// <summary>
        /// Gets the bottom-right texture co-ord for the texture ID
        /// </summary>
        /// <param name="ID">The texture ID to search for</param>
        /// <returns>The bottom-right corner for the texture refrence</returns>
        public static Vector2 BR(byte ID)
        {
            int y = (int)Math.Round(ID / 16F);
            int x = ID - (int)(y * 16);

            return new Vector2(((float)x + 1F) * PercentPerBlock, ((float)y) * PercentPerBlock);
        }

        /// <summary>
        /// Gets the top-left texture co-ord for the texture ID
        /// </summary>
        /// <param name="ID">The texture ID to search for</param>
        /// <returns>The top-left corner for the texture refrence</returns>
        public static Vector2 TL(byte ID)
        {
            int y = (int)Math.Round(ID / 16F);
            int x = ID - (int)((float)y * 16);

            return new Vector2(((float)x) * PercentPerBlock, ((float)y + 1F) * PercentPerBlock);
        }

        /// <summary>
        /// Gets the top-right texture co-ord for the texture ID
        /// </summary>
        /// <param name="ID">The texture ID to search for</param>
        /// <returns>The top-right corner for the texture refrence</returns>
        public static Vector2 TR(byte ID)
        {
            int y = (int)Math.Round(ID / 16F);
            int x = ID - (int)(y * 16);

            return new Vector2(((float)x + 1F) * PercentPerBlock, ((float)y + 1F) * PercentPerBlock);
        }

        /// <summary>
        /// Gets the terrain texture for this texture manager
        /// </summary>
        public static Texture2D Terrain
        {
            get { return terrain; }
        }

        /// <summary>
        /// Gets the normal map texture for this texture manager
        /// </summary>
        public static Texture2D NormalMap
        {
            get { return normal; }
        }
    }
}
