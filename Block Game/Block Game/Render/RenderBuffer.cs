using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Block_Game.Utilities;

namespace Block_Game.Render
{
    public struct RenderBuffer
    {
        public int[] IndexBuffer;
        public VertexPositionNormalTextureColor[] Indices;

        public static RenderBuffer Merge(RenderBuffer destination, RenderBuffer source)
        {
            List<VertexPositionNormalTextureColor> vertTemp = new List<VertexPositionNormalTextureColor>();
            vertTemp.AddRange(destination.Indices);
            vertTemp.AddRange(source.Indices);

            return Simplify(vertTemp);
        }
        
        public static RenderBuffer Simplify(List<VertexPositionNormalTextureColor> list)
        {
            List<int> iBuffer = new List<int>();
            List<VertexPositionNormalTextureColor> iVertex = new List<VertexPositionNormalTextureColor>();

            foreach (VertexPositionNormalTextureColor v in list)
            {
                if (!iVertex.Contains(v))
                {
                    iBuffer.Add(iBuffer.Count);
                    iVertex.Add(v);
                }
                else
                {
                    iBuffer.Add(list.IndexOf(v));
                }
            }

            return new RenderBuffer { IndexBuffer = iBuffer.ToArray(), Indices = iVertex.ToArray() };
        }
    }
}
