using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BlockGame.Render
{
    public static class TextureManager
    {
        static Texture2D terrain;
        static Texture2D normal;
        /// <summary>
        /// The width/height of each texture in the sheet
        /// </summary>
        const float TexSize = 16;
        const float PercentPerBlock = (float)(0.0625F);

        public static void Initialize(Texture2D terrain, Texture2D normal)
        {
            TextureManager.terrain = terrain;
            TextureManager.normal = normal;
        }

        public static Vector2 BL(byte ID)
        {
            int y = (int)Math.Round(ID / 16F);
            int x = ID - (int)(y * 16);

            return new Vector2((float)x * PercentPerBlock, (float)y * PercentPerBlock);
        }

        public static Vector2 BR(byte ID)
        {
            int y = (int)Math.Round(ID / 16F);
            int x = ID - (int)(y * 16);

            return new Vector2(((float)x + 1F) * PercentPerBlock, ((float)y) * PercentPerBlock);
        }

        public static Vector2 TL(byte ID)
        {
            int y = (int)Math.Round(ID / 16F);
            int x = ID - (int)((float)y * 16);

            return new Vector2(((float)x) * PercentPerBlock, ((float)y + 1F) * PercentPerBlock);
        }

        public static Vector2 TR(byte ID)
        {
            int y = (int)Math.Round(ID / 16F);
            int x = ID - (int)(y * 16);

            return new Vector2(((float)x + 1F) * PercentPerBlock, ((float)y + 1F) * PercentPerBlock);
        }

        public static Texture2D Terrain
        {
            get { return terrain; }
        }

        public static Texture2D NormalMap
        {
            get { return normal; }
        }
    }
}
