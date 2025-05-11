namespace Sate.Expression;

public class Node(Operands ope, object value)
{
    public Operands Operand { get; } = ope;

    public object Value { get; } = value;

    public Node? Left { get; set; }

    public Node? Right { get; set; }

    public override string ToString() =>
        Right is { } ? $"( {Left}  {Value}  {Right} )" :
        Operand == Operands.LeftParenthesis ? $"( {Left} )" :
        Left is { } ? $"( {Value}  {Left} )" :
        $"{Value}";
}
