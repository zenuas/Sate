namespace Sate.Block;

public class ForBlock(string iterator, IBlock[] loop) : IBlock
{
    public string Iterator => iterator;

    public IBlock[] Loop => loop;
}
