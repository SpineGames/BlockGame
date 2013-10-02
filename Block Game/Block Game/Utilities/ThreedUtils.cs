///3D utilities for Block Game
///© 2013 Spine Games

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
        /// <summary>
        /// Stores the position of this vertex
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The normal for this vertex
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// The color of this vertex
        /// </summary>
        public Color Color;

        /// <summary>
        /// The UV co-ords for this vertex (the co-ords in the texture)
        /// </summary>
        public Vector2 TexCoords;

        /// <summary>
        /// Creates a new VertexPositionNormalTextureColor
        /// </summary>
        /// <param name="position">The position in space for this vertex</param>
        /// <param name="Normal">The nomal for this vector</param>
        /// <param name="TexCoords">The UV co-ords for this vertex</param>
        public VertexPositionNormalTextureColor(Vector3 position, Vector3 Normal, Vector2 TexCoords)
        {
            this.Position = position;
            this.Normal = Normal;
            this.TexCoords = TexCoords;
            this.Color = Color.Black;
        }

        /// <summary>
        /// Creates a new VertexPositionNormalTextureColor
        /// </summary>
        /// <param name="position">The position in space for this vertex</param>
        /// <param name="Normal">The nomal for this vector</param>
        /// <param name="TexCoords">The UV co-ords for this vertex</param>
        /// <param name="color">The color of this vertex</param>
        public VertexPositionNormalTextureColor(Vector3 position, Vector3 Normal, Vector2 TexCoords, Color color)
        {
            this.Position = position;
            this.Normal = Normal;
            this.TexCoords = TexCoords;
            this.Color = color;
        }

        /// <summary>
        /// The vertex declaration for this vertex type
        /// </summary>
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
        
        /// <summary>
        /// The byte size of this vertex declaration
        /// </summary>
        public const int SizeInBytes = 36;
    }
}
