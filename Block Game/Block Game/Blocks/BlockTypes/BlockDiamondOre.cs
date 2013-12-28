using BlockGame;
using BlockGame.Utilities;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a block of Diamond ore
    /// </summary>
    public class BlockDiamondOre : Block
    {
        public override byte ID { get { return 12; } }
        public override byte texRef { get { return 13; } }
    }
}