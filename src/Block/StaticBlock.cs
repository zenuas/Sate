namespace Sate.Block;

public class StaticBlock(string value) : IBlock
{
    public string Value => value;
}
