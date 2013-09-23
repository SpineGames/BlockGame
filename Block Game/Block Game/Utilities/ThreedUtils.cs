using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Block_Game.Utilities
{
    class ThreedUtils
    {
    }

    /// <summary>
    /// Custom vertex structure for more advanced drawing
    /// </summary>
    public struct VertexPositionNormalTextureColor : IVertexType
    {
        // Stores the positiom
        public Vector3 Position;

        // Stores the normal
        public Vector3 Normal;

        // Stores the Color
        public Color Color;

        // The texco-ords for this vertex
        public Vector2 TexCoords;

        public VertexPositionNormalTextureColor(Vector3 position, Vector3 Normal, Vector2 TexCoords)
        {
            this.Position = position;
            this.Normal = Normal;
            this.TexCoords = TexCoords;
            this.Color = Color.Black;
        }

        public VertexPositionNormalTextureColor(Vector3 position, Vector3 Normal, Vector2 TexCoords, Color color)
        {
            this.Position = position;
            this.Normal = Normal;
            this.TexCoords = TexCoords;
            this.Color = color;
        }

        // Describe the layout of this vertex structure.
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return new VertexDeclaration
                    (
                    new VertexElement(0, VertexElementFormat.Vector3,
                    VertexElementUsage.Position, 0),

                    new VertexElement(12, VertexElementFormat.Vector3,
                    VertexElementUsage.Normal, 0),

                    new VertexElement(24, VertexElementFormat.Color,
                    VertexElementUsage.Color, 0),

                    new VertexElement(28, VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate, 0)
                    );
            }
        }


        // Describe the size of this vertex structure.
        public const int SizeInBytes = 36;
    }
}
