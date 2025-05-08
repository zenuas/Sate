namespace Sate.Test;

public class TryParseVariableTest
{
    [Fact]
    public void NullTest()
    {
        var len = LineParser.TryParseVariable("", 0, out var name);
        Assert.Equal(len, 0);
        Assert.Equal(name, "");
    }

    [Fact]
    public void Var1Test()
    {
        var len = LineParser.TryParseVariable("abc123", 0, out var name);
        Assert.Equal(len, 6);
        Assert.Equal(name, "abc123");
    }

    [Fact]
    public void Var2Test()
    {
        var len = LineParser.TryParseVariable("abc123+", 0, out var name);
        Assert.Equal(len, 6);
        Assert.Equal(name, "abc123");
    }

    [Fact]
    public void Var3Test()
    {
        var len = LineParser.TryParseVariable("@abc123", 0, out var name);
        Assert.Equal(len, 7);
        Assert.Equal(name, "@abc123");
    }

    [Fact]
    public void Var4Test()
    {
        var len = LineParser.TryParseVariable(":abc123", 0, out var name);
        Assert.Equal(len, 7);
        Assert.Equal(name, ":abc123");
    }

    [Fact]
    public void Var5Test()
    {
        var len = LineParser.TryParseVariable("__abc123+", 0, out var name);
        Assert.Equal(len, 8);
        Assert.Equal(name, "__abc123");
    }

    [Fact]
    public void Var6Test()
    {
        var len = LineParser.TryParseVariable("@__abc123", 0, out var name);
        Assert.Equal(len, 9);
        Assert.Equal(name, "@__abc123");
    }

    [Fact]
    public void Var7Test()
    {
        var len = LineParser.TryParseVariable(":__abc123", 0, out var name);
        Assert.Equal(len, 9);
        Assert.Equal(name, ":__abc123");
    }

    [Fact]
    public void Error1Test()
    {
        var len = LineParser.TryParseVariable("_", 0, out var name);
        Assert.Equal(len, 0);
        Assert.Equal(name, "");
    }

    [Fact]
    public void Error2Test()
    {
        var len = LineParser.TryParseVariable("123abc", 0, out var name);
        Assert.Equal(len, 0);
        Assert.Equal(name, "");
    }

    [Fact]
    public void Error3Test()
    {
        var len = LineParser.TryParseVariable("__123abc", 0, out var name);
        Assert.Equal(len, 0);
        Assert.Equal(name, "");
    }

    [Fact]
    public void Error4Test()
    {
        var len = LineParser.TryParseVariable("@_", 0, out var name);
        Assert.Equal(len, 0);
        Assert.Equal(name, "");
    }

    [Fact]
    public void Error5Test()
    {
        var len = LineParser.TryParseVariable("@123abc", 0, out var name);
        Assert.Equal(len, 0);
        Assert.Equal(name, "");
    }

    [Fact]
    public void Error6Test()
    {
        var len = LineParser.TryParseVariable("@__123abc", 0, out var name);
        Assert.Equal(len, 0);
        Assert.Equal(name, "");
    }

    [Fact]
    public void Error7Test()
    {
        var len = LineParser.TryParseVariable(":_", 0, out var name);
        Assert.Equal(len, 0);
        Assert.Equal(name, "");
    }

    [Fact]
    public void Error8Test()
    {
        var len = LineParser.TryParseVariable(":123abc", 0, out var name);
        Assert.Equal(len, 0);
        Assert.Equal(name, "");
    }

    [Fact]
    public void Error9Test()
    {
        var len = LineParser.TryParseVariable(":__123abc", 0, out var name);
        Assert.Equal(len, 0);
        Assert.Equal(name, "");
    }
}
