using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BlockGame.Render
{
    /// <summary>
    /// Represents a peice of 3D geometry
    /// </summary>
    /// <typeparam name="T">The vertex type to use</typeparam>
    public class Geometry<T> where T : struct, IVertexType
    {
        /// <summary>
        /// The vertex buffer to render with
        /// </summary>
        protected VertexBuffer _vertBuffer;
        /// <summary>
        /// The index buffer to render with
        /// </summary>
        protected IndexBuffer _indexBuffer;

        /// <summary>
        /// A temporary list of vertices
        /// </summary>
        protected List<T> _vertices;
        /// <summary>
        /// A temporary list of indices
        /// </summary>
        protected List<int> _indices;

        /// <summary>
        /// The graphics device to use for rendering
        /// </summary>
        protected GraphicsDevice _graphics;

        int _primitiveCount;
        /// <summary>
        /// The primitive type to use for rendering
        /// </summary>
        protected PrimitiveType _primitiveType;

        /// <summary>
        /// Creates a new 3D geometry
        /// </summary>
        /// <param name="graphics">The graphics device to use</param>
        /// <param name="primitiveType">The primitive type to use</param>
        public Geometry(GraphicsDevice graphics, PrimitiveType primitiveType)
        {
            _graphics = graphics;

            _vertices = new List<T>();
            _indices = new List<int>();

            _primitiveType = primitiveType;
        }

        /// <summary>
        /// Adds a vertex to this geometry
        /// </summary>
        /// <param name="vertex">The vertex to add</param>
        public int AddVertex(T vertex)
        {
            _vertices.Add(vertex);
            return _vertices.Count - 1;
        }

        /// <summary>
        /// Adds an index to this geometry
        /// </summary>
        /// <param name="index">The index to add</param>
        public void AddIndex(int index)
        {
            _indices.Add(index);
        }

        /// <summary>
        /// Adds a triangle to this geometry. Must be defined in the correct culling order
        /// </summary>
        /// <param name="id1">The ID of the first index</param>
        /// <param name="id2">The ID of the second index</param>
        /// <param name="id3">The ID of the third index</param>
        public void AddTri(int id1, int id2, int id3)
        {
            if (_primitiveType == PrimitiveType.TriangleList)
            {
                _indices.Add(id1);
                _indices.Add(id2);
                _indices.Add(id3);
            }
            else
            {
                throw new InvalidOperationException("Primitive type must be PrimitiveType.TriangleList");
            }
        }

        /// <summary>
        /// Adds a quad between 4 vertices. Must be defined in clockwise order
        /// </summary>
        /// <param name="id1">The first index</param>
        /// <param name="id2">The second index</param>
        /// <param name="id3">The third index</param>
        /// <param name="id4">The fourth index</param>
        public void AddQuad(int id1, int id2, int id3, int id4)
        {
            AddTri(id1, id2, id3);
            AddTri(id3, id4, id1);
        }

        /// <summary>
        /// Finishes this geometry and prepares it for rendering
        /// </summary>
        public void Finish()
        {
            _vertBuffer = new VertexBuffer(_graphics, typeof(T), _vertices.Count, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(_graphics, IndexElementSize.ThirtyTwoBits, _indices.Count, BufferUsage.WriteOnly);

            _vertBuffer.SetData<T>(_vertices.ToArray());
            _indexBuffer.SetData<int>(_indices.ToArray());

            _vertices.Clear();
            _indices.Clear();

            _vertices = null;
            _indices = null;

            switch (_primitiveType)
            {
                case PrimitiveType.LineList:
                    _primitiveCount = _indexBuffer.IndexCount / 2;
                    break;
                case PrimitiveType.LineStrip:
                    _primitiveCount = _indexBuffer.IndexCount - 1;
                    break;
                case PrimitiveType.TriangleList:
                    _primitiveCount = _indexBuffer.IndexCount / 3;
                    break;
                case PrimitiveType.TriangleStrip:
                    _primitiveCount = _indexBuffer.IndexCount - 2;
                    break;
            }
        }

        /// <summary>
        /// Renders this geometry
        /// </summary>
        public void Render()
        {
            _graphics.SetVertexBuffer(_vertBuffer);
            _graphics.Indices = _indexBuffer;

            _graphics.DrawIndexedPrimitives(_primitiveType, 0, 0, _vertBuffer.VertexCount, 0, _primitiveCount);
        }
    }
}
