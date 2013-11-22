using BlockGame;
using BlockGame.Utilities;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a leaf block
    /// </summary>
    public class BlockLeaves : Block
    {
        public override byte ID { get { return 8; } }
        public override byte texRef { get { return 9; } }
        public override bool IsOpaque { get { return false; } }
    }
}