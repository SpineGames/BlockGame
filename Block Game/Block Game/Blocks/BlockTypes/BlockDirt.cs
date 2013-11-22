using BlockGame;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a dirt block
    /// </summary>
    public class BlockDirt : Block
    {
        public override byte ID { get { return 2; } }
        public override byte texRef { get { return 1; } }

        /// <summary>
        /// Overrides the GetTexID to send the right texture data
        /// </summary>
        /// <param name="facing">The facing to get</param>
        /// <param name="meta">The metadata of the block</param>
        /// <returns>The texID for the given facing</returns>
        public override byte GetTexIDForFacing(BlockFacing facing, byte meta = 0)
        {
            if (meta != 0)
            {
                switch (facing)
                {
                    case BlockFacing.Top:
                        return 7;
                    case BlockFacing.Bottom:
                        return 1;
                    default:
                        return 8;
                }
            }
            else
                return 1;
        }
    }
}
