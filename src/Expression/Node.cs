namespace Sate.Expression;

public class Node(string value)
{
    public string Value { get; } = value;

    public Node? Left { get; set; }

    public Node? Right { get; set; }
}
