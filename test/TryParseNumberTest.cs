namespace Sate.Test;

public class TryParseNumberTest
{
    [Fact]
    public void NullTest()
    {
        var len = LineParser.TryParseNumber("", 0, out var num);
        Assert.Equal(len, 0);
        Assert.Equal(num, 0);
    }

    [Fact]
    public void Num1Test()
    {
        var len = LineParser.TryParseNumber("0", 0, out var num);
        Assert.Equal(len, 1);
        Assert.Equal(num, 0);
    }

    [Fact]
    public void Num2Test()
    {
        var len = LineParser.TryParseNumber("1", 0, out var num);
        Assert.Equal(len, 1);
        Assert.Equal(num, 1);
    }

    [Fact]
    public void Num3Test()
    {
        var len = LineParser.TryParseNumber("12", 0, out var num);
        Assert.Equal(len, 2);
        Assert.Equal(num, 12);
    }

    [Fact]
    public void Num4Test()
    {
        var len = LineParser.TryParseNumber("123", 0, out var num);
        Assert.Equal(len, 3);
        Assert.Equal(num, 123);
    }

    [Fact]
    public void Num5Test()
    {
        var len = LineParser.TryParseNumber("1234", 0, out var num);
        Assert.Equal(len, 4);
        Assert.Equal(num, 1234);
    }

    [Fact]
    public void Num6Test()
    {
        var len = LineParser.TryParseNumber("12345", 0, out var num);
        Assert.Equal(len, 5);
        Assert.Equal(num, 12345);
    }

    [Fact]
    public void Num7Test()
    {
        var len = LineParser.TryParseNumber("12_345", 0, out var num);
        Assert.Equal(len, 6);
        Assert.Equal(num, 12345);
    }

    [Fact]
    public void Num8Test()
    {
        var len = LineParser.TryParseNumber("2147483647", 0, out var num);
        Assert.Equal(len, 10);
        Assert.Equal(num, 2147483647);
    }

    [Fact]
    public void Error1Test()
    {
        var len = LineParser.TryParseNumber("-1", 0, out var num);
        Assert.Equal(len, 0);
        Assert.Equal(num, 0);
    }

    [Fact]
    public void Error2Test()
    {
        var len = LineParser.TryParseNumber("123.45", 0, out var num);
        Assert.Equal(len, 3);
        Assert.Equal(num, 123);
    }

    [Fact]
    public void Error3Test()
    {
        var len = LineParser.TryParseNumber("2147483648", 0, out var num);
        Assert.Equal(len, 10);
        Assert.Equal(num, -2147483648);
    }

    [Fact]
    public void Error4Test()
    {
        var len = LineParser.TryParseNumber("2147483649", 0, out var num);
        Assert.Equal(len, 10);
        Assert.Equal(num, -2147483647);
    }

    [Fact]
    public void Error5Test()
    {
        var len = LineParser.TryParseNumber("4294967294", 0, out var num);
        Assert.Equal(len, 10);
        Assert.Equal(num, -2);
    }

    [Fact]
    public void Error6Test()
    {
        var len = LineParser.TryParseNumber("4294967295", 0, out var num);
        Assert.Equal(len, 10);
        Assert.Equal(num, -1);
    }

    [Fact]
    public void Error7Test()
    {
        var len = LineParser.TryParseNumber("4294967296", 0, out var num);
        Assert.Equal(len, 10);
        Assert.Equal(num, 0);
    }
}
