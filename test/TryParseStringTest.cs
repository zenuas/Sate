using System;

namespace Sate.Test;

public class TryParseStringTest
{
    [Fact]
    public void NullTest()
    {
        var len = LineParser.TryParseString("", 0, out var str);
        Assert.Equal(len, 0);
        Assert.Equal(str, "");
    }

    [Fact]
    public void SingleQuote1Test()
    {
        var len = LineParser.TryParseString("'a'", 0, out var str);
        Assert.Equal(len, 3);
        Assert.Equal(str, "a");
    }

    [Fact]
    public void SingleQuote2Test()
    {
        var len = LineParser.TryParseString("'a '", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "a ");
    }

    [Fact]
    public void SingleQuote3Test()
    {
        var len = LineParser.TryParseString("'a b'", 0, out var str);
        Assert.Equal(len, 5);
        Assert.Equal(str, "a b");
    }

    [Fact]
    public void SingleQuote4Test()
    {
        var len = LineParser.TryParseString("'\\''", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "'");
    }

    [Fact]
    public void SingleQuote5Test()
    {
        var len = LineParser.TryParseString("'\\\"'", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "\"");
    }

    [Fact]
    public void SingleQuote6Test()
    {
        var len = LineParser.TryParseString("'\\t'", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "\t");
    }

    [Fact]
    public void SingleQuote7Test()
    {
        var len = LineParser.TryParseString("'\\r'", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "\r");
    }

    [Fact]
    public void SingleQuote8Test()
    {
        var len = LineParser.TryParseString("'\\n'", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "\n");
    }

    [Fact]
    public void DoubleQuote1Test()
    {
        var len = LineParser.TryParseString("\"a\"", 0, out var str);
        Assert.Equal(len, 3);
        Assert.Equal(str, "a");
    }

    [Fact]
    public void DoubleQuote2Test()
    {
        var len = LineParser.TryParseString("\"a \"", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "a ");
    }

    [Fact]
    public void DoubleQuote3Test()
    {
        var len = LineParser.TryParseString("\"a b\"", 0, out var str);
        Assert.Equal(len, 5);
        Assert.Equal(str, "a b");
    }

    [Fact]
    public void DoubleQuote4Test()
    {
        var len = LineParser.TryParseString("\"\\'\"", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "'");
    }

    [Fact]
    public void DoubleQuote5Test()
    {
        var len = LineParser.TryParseString("\"\\\"\"", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "\"");
    }

    [Fact]
    public void DoubleQuote6Test()
    {
        var len = LineParser.TryParseString("\"\\t\"", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "\t");
    }

    [Fact]
    public void DoubleQuote7Test()
    {
        var len = LineParser.TryParseString("\"\\r\"", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "\r");
    }

    [Fact]
    public void DoubleQuote8Test()
    {
        var len = LineParser.TryParseString("\"\\n\"", 0, out var str);
        Assert.Equal(len, 4);
        Assert.Equal(str, "\n");
    }

    [Fact]
    public void ErrorTest()
    {
        Assert.Equal(LineParser.TryParseString("'", 0, out var _), 0);
        Assert.Equal(LineParser.TryParseString("\"", 0, out var _), 0);
        Assert.Equal(LineParser.TryParseString("'\"", 0, out var _), 0);
        Assert.Equal(LineParser.TryParseString("\"'", 0, out var _), 0);
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.TryParseString("'\\a'", 0, out var _)).Message, "invalid string escape");
    }
}
