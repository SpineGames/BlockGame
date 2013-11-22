using BlockGame;
using BlockGame.Utilities;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a stone block
    /// </summary>
    public class BlockStone : Block
    {
        public override byte ID { get { return 1; } }
        public override byte texRef { get { return 0; } }
    }
}