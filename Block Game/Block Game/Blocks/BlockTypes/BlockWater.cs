using BlockGame;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a water block
    /// </summary>
    public class BlockWater : Block
    {
        public override byte ID { get { return 9; } }
        public override byte texRef { get { return 10; } }
        public override bool IsOpaque { get { return false; } }
    }
}
