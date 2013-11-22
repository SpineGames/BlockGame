using BlockGame;
using BlockGame.Utilities;

namespace BlockGame.Blocks.BlockTypes
{
    /// <summary>
    /// Represents a gravel block
    /// </summary>
    public class BlockGravel : Block
    {
        public override byte ID { get { return 3; } }
        public override byte texRef { get { return 2; } }
    }
}