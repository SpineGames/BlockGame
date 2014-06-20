using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BlockGame.Render;

namespace BlockGame.Utilities
{
    /// <summary>
    /// Some simple utilities for rendering things
    /// </summary>
    public static class Utils
    {
        public static BasicEffect Effect;
        private static bool Initialized;

        #region Line
        private static VertexPositionColor[] lineVerts = new VertexPositionColor[2];
        private static int[] lineIndices = new int[2] { 0, 1 };
        #endregion

        #region quad
        private static VertexPositionColor[] quadCVerts = new VertexPositionColor[4];
        private static VertexPositionColorTexture[] quadTVerts = new VertexPositionColorTexture[4];
        private static int[] quadIndices = new int[4] { 0, 1, 2, 3 };
        #endregion

        #region Bounding box
        static VertexPositionColor[] verts = new VertexPositionColor[8];
        static Int16[] indices = new Int16[]  
        {  
            0, 1,  
            1, 2,  
            2, 3,  
            3, 0,  
            0, 4,  
            1, 5,  
            2, 6,  
            3, 7,  
            4, 5,  
            5, 6,  
            6, 7,  
            7, 4,  
        };
        #endregion

        #region BoundingFrustum
        static VertexPositionColor[] frustumVerts = new VertexPositionColor[8];
        static Int16[] frustumIndices = new Int16[]  
        {  
            0, 1,  
            1, 2,  
            2, 3,  
            3, 0,  
            0, 4,  
            1, 5,  
            2, 6,  
            3, 7,  
            4, 5,  
            5, 6,  
            6, 7,  
            7, 4,  
        };
        #endregion

        public static void Initialize(GraphicsDevice graphics)
        {
            Effect = new BasicEffect(graphics);
            Effect.VertexColorEnabled = true;

            Initialized = true;
        }

        /// <summary>
        /// Draws a single line segment between two points
        /// </summary>
        /// <param name="Point1">The first point</param>
        /// <param name="Point2">The second point</param>
        /// <param name="color">The color of the line to draw</param>
        /// <param name="Graphics">The GraphicsDevice to draw with</param>
        public static void DrawLine
            (Vector3 Point1, Vector3 Point2, Color color, GraphicsDevice Graphics)
        {
            if (!Initialized)
                Initialize(Graphics);

            lineVerts[0].Position = Point1;
            lineVerts[0].Color = color;

            lineVerts[1].Position = Point2;
            lineVerts[1].Color = color;
            
            Effect.CurrentTechnique.Passes[0].Apply();

            Graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                lineVerts, 0, 2, lineIndices, 0, 1);
        }

        /// <summary>
        /// Draws a bounding box with lines
        /// </summary>
        /// <param name="Box">The Bounding Box to render</param>
        /// <param name="color">The color of the line to draw</param>
        /// <param name="Graphics">The GraphicsDevice to draw with</param>
        public static void DrawBoundingBox
            (BoundingBox Box, Color color, GraphicsDevice Graphics)
        {
            if (!Initialized)
                Initialize(Graphics);

            Vector3[] corners = Box.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                verts[i].Position = corners[i];
                verts[i].Color = color;
            }

            Effect.CurrentTechnique.Passes[0].Apply();

            Graphics.DrawUserIndexedPrimitives(
                // TEST PrimitiveType.LineList,  
                PrimitiveType.LineList,
               verts,
                0,
                8,
                indices,
                0,
                indices.Length / 2);
        }
        
        /// <summary>
        /// Draws a bounding box with lines
        /// </summary>
        /// <param name="Frustum">The bounding frustum to draw</param>
        /// <param name="color">The color of the line to draw</param>
        /// <param name="Graphics">The GraphicsDevice to draw with</param>
        public static void DrawBoundingFrustum
            (BoundingFrustum Frustum, Color color, GraphicsDevice Graphics)
        {
            if (!Initialized)
                Initialize(Graphics);

            Vector3[] corners = Frustum.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                frustumVerts[i].Position = corners[i];
                frustumVerts[i].Color = color;
            }

            Effect.CurrentTechnique.Passes[0].Apply();

            Graphics.DrawUserIndexedPrimitives(
                // TEST PrimitiveType.LineList,  
                PrimitiveType.LineList,
                frustumVerts,
                0,
                8,
                frustumIndices,
                0,
                frustumIndices.Length / 2);
        }
        
        /// <summary>
        /// Draws a quad
        /// </summary>
        /// <param name="Point1">The first point</param>
        /// <param name="Point2">The second point</param>
        /// <param name="color">The color of the line to draw</param>
        /// <param name="Graphics">The GraphicsDevice to draw with</param>
        public static void DrawQuad2D
            (Rectangle rectangle, Color color,
            GraphicsDevice Graphics)
        {
            if (!Initialized)
                Initialize(Graphics);

            quadCVerts[0].Position =
                new Vector3(rectangle.X, rectangle.Y, 0);
            quadCVerts[0].Color = color;

            quadCVerts[1].Position =
                new Vector3(rectangle.X + rectangle.Width, rectangle.Y, 0);
            quadCVerts[1].Color = color;

            quadCVerts[2].Position =
                new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0);
            quadCVerts[2].Color = color;

            quadCVerts[3].Position =
                new Vector3(rectangle.X, rectangle.Y + rectangle.Height, 0);
            quadCVerts[3].Color = color;
            
            Effect.CurrentTechnique.Passes[0].Apply();

            Graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip,
                quadCVerts, 0, 2);
        }

        public static void ApplyCamera(Camera camera)
        {
            if (Effect != null)
            {
                Effect.View = camera.View.View;
                Effect.Projection = camera.View.Projection;
                Effect.World = camera.View.World;
            }
        }

        public static int BitCount(this long lValue)
        {
            int iCount = 0;

            //Loop the value while there are still bits
            while (lValue != 0)
            {
                //Remove the end bit
                lValue = lValue & (lValue - 1);

                //Increment the count
                iCount++;
            }

            //Return the count
            return iCount;
        }

        public static int BitCount(this int lValue)
        {
            int iCount = 0;

            //Loop the value while there are still bits
            while (lValue != 0)
            {
                //Remove the end bit
                lValue = lValue & (lValue - 1);

                //Increment the count
                iCount++;
            }

            //Return the count
            return iCount;
        }

        public static int BitCount(this byte lValue)
        {
            int iCount = 0;

            //Loop the value while there are still bits
            while (lValue != 0)
            {
                //Remove the end bit
                lValue = (byte)(lValue & (lValue - 1));

                //Increment the count
                iCount++;
            }

            //Return the count
            return iCount;
        }
    }
}
