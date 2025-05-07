namespace Sate.Block;

public class ForJoinBlock(string iterator, string join, IBlock[] loop) : IBlock
{
    public string Iterator => iterator;

    public string Join => join;

    public IBlock[] Loop => loop;
}
