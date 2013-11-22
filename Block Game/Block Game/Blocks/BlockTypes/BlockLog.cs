using BlockGame;
using BlockGame.Utilities;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a Log block
    /// </summary>
    public class BlockLog : Block
    {
        public override byte ID { get { return 5; } }
        public override byte texRef { get { return 5; } }
        
        /// <summary>
        /// Gets the texture ID for the given facing
        /// </summary>
        /// <param name="facing">The block facing to get</param>
        /// <param name="meta">The meta-data for the given block</param>
        /// <returns>The texture ID for the facing</returns>
        public override byte GetTexIDForFacing(BlockFacing facing, byte meta = 0)
        {
            switch (facing)
            {
                case BlockFacing.Top :
                    return 5;
                case BlockFacing.Bottom:
                    return 5;
                default:
                    return 4;
            }
        }
    }
}