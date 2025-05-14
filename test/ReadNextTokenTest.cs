using Sate.Expression;

namespace Sate.Test;

public class ReadNextTokenTest
{
    [Fact]
    public void Null1Test()
    {
        var (len, node) = LineParser.ReadNextToken("", 0);
        Assert.Equal(len, 0);
        Assert.Equal(node.Operand, Operands.None);
        Assert.Equal(node.Value, "EOL");
        Assert.Equal(node.Left, null);
        Assert.Equal(node.Right, null);
    }

    [Fact]
    public void Null2Test()
    {
        var (len, node) = LineParser.ReadNextToken(" ", 0);
        Assert.Equal(len, 1);
        Assert.Equal(node.Operand, Operands.None);
        Assert.Equal(node.Value, "EOL");
        Assert.Equal(node.Left, null);
        Assert.Equal(node.Right, null);
    }

    [Fact]
    public void AndTest()
    {
        var (len, node) = LineParser.ReadNextToken("and", 0);
        Assert.Equal(len, 3);
        Assert.Equal(node.Operand, Operands.Operand);
        Assert.Equal(node.Value, "&&");
        Assert.Equal(node.Left, null);
        Assert.Equal(node.Right, null);
    }

    [Fact]
    public void OrTest()
    {
        var (len, node) = LineParser.ReadNextToken("||", 0);
        Assert.Equal(len, 2);
        Assert.Equal(node.Operand, Operands.Operand);
        Assert.Equal(node.Value, "||");
        Assert.Equal(node.Left, null);
        Assert.Equal(node.Right, null);
    }
}
