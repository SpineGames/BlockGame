using BlockGame;
using BlockGame.Utilities;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a sand block
    /// </summary>
    public class BlockSand : Block
    {
        public override byte ID { get { return 6; } }
        public override byte texRef { get { return 6; } }
    }
}