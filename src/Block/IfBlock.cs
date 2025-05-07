namespace Sate.Block;

public class IfBlock(string cond, IBlock[] then_, IBlock[] else_) : IBlock
{
    public string Condition => cond;

    public IBlock[] Then => then_;

    public IBlock[] Else => else_;
}
