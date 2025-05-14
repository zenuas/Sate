using System;

namespace Sate.Test;

public class TryParseOperatorTest
{
    [Fact]
    public void NullTest()
    {
        var len = LineParser.TryParseOperator("", 0, out var ope);
        Assert.Equal(len, 0);
        Assert.Equal(ope, "");
    }

    [Fact]
    public void Addition1Test()
    {
        var len = LineParser.TryParseOperator("+", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "+");
    }

    [Fact]
    public void Addition2Test()
    {
        var len = LineParser.TryParseOperator("+a", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "+");
    }

    [Fact]
    public void Addition3Test()
    {
        var len = LineParser.TryParseOperator("++", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "+");
    }

    [Fact]
    public void Addition4Test()
    {
        var len = LineParser.TryParseOperator("+=", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "+");
    }

    [Fact]
    public void Subtraction1Test()
    {
        var len = LineParser.TryParseOperator("-", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "-");
    }

    [Fact]
    public void Subtraction2Test()
    {
        var len = LineParser.TryParseOperator("-a", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "-");
    }

    [Fact]
    public void Subtraction3Test()
    {
        var len = LineParser.TryParseOperator("--", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "-");
    }

    [Fact]
    public void Subtraction4Test()
    {
        var len = LineParser.TryParseOperator("-=", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "-");
    }

    [Fact]
    public void Multiplication1Test()
    {
        var len = LineParser.TryParseOperator("*", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "*");
    }

    [Fact]
    public void Multiplication2Test()
    {
        var len = LineParser.TryParseOperator("*a", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "*");
    }

    [Fact]
    public void Multiplication3Test()
    {
        var len = LineParser.TryParseOperator("**", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "*");
    }

    [Fact]
    public void Multiplication4Test()
    {
        var len = LineParser.TryParseOperator("*=", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "*");
    }

    [Fact]
    public void Division1Test()
    {
        var len = LineParser.TryParseOperator("/", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "/");
    }

    [Fact]
    public void Division2Test()
    {
        var len = LineParser.TryParseOperator("/a", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "/");
    }

    [Fact]
    public void Division3Test()
    {
        var len = LineParser.TryParseOperator("//", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "/");
    }

    [Fact]
    public void Division4Test()
    {
        var len = LineParser.TryParseOperator("/=", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "/");
    }

    [Fact]
    public void Remainder1Test()
    {
        var len = LineParser.TryParseOperator("%", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "%");
    }

    [Fact]
    public void Remainder2Test()
    {
        var len = LineParser.TryParseOperator("%a", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "%");
    }

    [Fact]
    public void Remainder3Test()
    {
        var len = LineParser.TryParseOperator("%%", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "%");
    }

    [Fact]
    public void Remainder4Test()
    {
        var len = LineParser.TryParseOperator("%=", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "%");
    }

    [Fact]
    public void Less1Test()
    {
        var len = LineParser.TryParseOperator("<", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "<");
    }

    [Fact]
    public void Less2Test()
    {
        var len = LineParser.TryParseOperator("<a", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "<");
    }

    [Fact]
    public void Less3Test()
    {
        var len = LineParser.TryParseOperator("<<", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, "<");
    }

    [Fact]
    public void Less4Test()
    {
        var len = LineParser.TryParseOperator("<=", 0, out var ope);
        Assert.Equal(len, 2);
        Assert.Equal(ope, "<=");
    }

    [Fact]
    public void Greater1Test()
    {
        var len = LineParser.TryParseOperator(">", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, ">");
    }

    [Fact]
    public void Greater2Test()
    {
        var len = LineParser.TryParseOperator(">a", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, ">");
    }

    [Fact]
    public void Greater3Test()
    {
        var len = LineParser.TryParseOperator(">>", 0, out var ope);
        Assert.Equal(len, 1);
        Assert.Equal(ope, ">");
    }

    [Fact]
    public void Greater4Test()
    {
        var len = LineParser.TryParseOperator(">=", 0, out var ope);
        Assert.Equal(len, 2);
        Assert.Equal(ope, ">=");
    }

    [Fact]
    public void AndTest()
    {
        var len = LineParser.TryParseOperator("&&", 0, out var ope);
        Assert.Equal(len, 2);
        Assert.Equal(ope, "&&");
    }

    [Fact]
    public void OrTest()
    {
        var len = LineParser.TryParseOperator("||", 0, out var ope);
        Assert.Equal(len, 2);
        Assert.Equal(ope, "||");
    }

    [Fact]
    public void Equality1Test()
    {
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.TryParseOperator("=", 0, out var ope)).Message, "unexpected operator =");
    }

    [Fact]
    public void Equality2Test()
    {
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.TryParseOperator("=a", 0, out var ope)).Message, "unexpected operator =");
    }

    [Fact]
    public void Equality3Test()
    {
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.TryParseOperator("=+", 0, out var ope)).Message, "unexpected operator =");
    }

    [Fact]
    public void Equality4Test()
    {
        var len = LineParser.TryParseOperator("==", 0, out var ope);
        Assert.Equal(len, 2);
        Assert.Equal(ope, "==");
    }

    [Fact]
    public void Inequality1Test()
    {
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.TryParseOperator("!", 0, out var ope)).Message, "unexpected operator !");
    }

    [Fact]
    public void Inequality2Test()
    {
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.TryParseOperator("!a", 0, out var ope)).Message, "unexpected operator !");
    }

    [Fact]
    public void Inequality3Test()
    {
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.TryParseOperator("!!", 0, out var ope)).Message, "unexpected operator !");
    }

    [Fact]
    public void Inequality4Test()
    {
        var len = LineParser.TryParseOperator("!=", 0, out var ope);
        Assert.Equal(len, 2);
        Assert.Equal(ope, "!=");
    }
}
