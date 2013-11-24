using BlockGame;
using BlockGame.Utilities;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a Log block
    /// </summary>
    /// <remarks>
    /// The logs meta mask is as follows:
    /// b0 - is sideways (false if the the open face is on the top/bottom)
    /// b0 - is rotated (false if open face faces front/back)
    /// </remarks>
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
                    if (meta.IsBitSet(0))
                        return 5;
                    else
                        return 4;

                case BlockFacing.Bottom:
                    if (meta.IsBitSet(0))
                        return 5;
                    else
                        return 4;

                case BlockFacing.Left:
                    if (meta.IsBitSet(1) & meta.IsBitSet(0))
                        return 5;
                    else
                        return 4;

                case BlockFacing.Right:
                    if (meta.IsBitSet(1) & meta.IsBitSet(0))
                        return 5;
                    else
                        return 4;

                case BlockFacing.Front:
                    if (meta.IsBitSet(0) & !meta.IsBitSet(1))
                        return 4;
                    else
                        return 5;

                case BlockFacing.Back:
                    if (meta.IsBitSet(0) & !meta.IsBitSet(1))
                        return 4;
                    else
                        return 5;
                default:
                    if (meta.IsBitSet(0))
                        return 5;
                    else
                        return 4;
            }
        }
    }
}