
using BlockGame;
using BlockGame.Utilities;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents an empty air block
    /// </summary>
    public class BlockAir : Block
    {
        public override byte ID { get { return 0; } }
        public override byte texRef { get { return 0; } }
        public override bool IsOpaque { get { return false; } }

        /// <summary>
        /// Gets a blank VPNTC array for the model
        /// </summary>
        /// <param name="facings">The block facings</param>
        /// <param name="pos">The position in the world</param>
        /// <param name="Meta">The air's meta-data</param>
        /// <returns></returns>
        public override VertexPositionNormalTextureColor[] GetModel(BlockRenderStates facings, Point3 pos, byte Meta)
        {
            return new VertexPositionNormalTextureColor[0];
        }
    }
}
