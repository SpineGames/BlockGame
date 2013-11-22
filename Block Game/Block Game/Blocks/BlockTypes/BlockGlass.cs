using BlockGame;
using BlockGame.Utilities;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a glass block
    /// </summary>
    public class BlockGlass : Block
    {
        public override byte ID { get { return 4; } }
        public override byte texRef { get { return 3; } }
        public override bool IsOpaque { get { return false; } }
    }
}