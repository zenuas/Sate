namespace Sate.Expression;

public class Node(Operands ope, object value)
{
    public Operands Operand { get; } = ope;

    public object Value { get; } = value;

    public Node? Left { get; set; }

    public Node? Right { get; set; }

    public override string ToString() =>
        Operand == Operands.LeftParenthesis ? $"( {Left} )" :
        Operand == Operands.Operand && Left is { } ? $"( {Left}  {Value}  {Right} )" :
        Operand == Operands.Operand ? $"( {Value}  {Left} )" :
        $"{Value}";
}
