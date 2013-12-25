using BlockGame;
using BlockGame.Utilities;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a block of iron ore
    /// </summary>
    public class BlockIronOre : Block
    {
        public override byte ID { get { return 10; } }
        public override byte texRef { get { return 11; } }
    }
}