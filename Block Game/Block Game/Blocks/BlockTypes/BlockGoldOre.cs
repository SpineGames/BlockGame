using BlockGame;
using BlockGame.Utilities;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a block of Gold ore
    /// </summary>
    public class BlockGoldOre : Block
    {
        public override byte ID { get { return 11; } }
        public override byte texRef { get { return 12; } }
    }
}