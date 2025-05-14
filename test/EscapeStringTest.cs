namespace Sate.Test;

public class EscapeStringTest
{
    [Fact]
    public void NullTest()
    {
        Assert.Equal(Generator.EscapeString(""), "");
    }

    [Fact]
    public void StringTest()
    {
        Assert.Equal(Generator.EscapeString("a"), "a");
    }

    [Fact]
    public void SigleQuoteTest()
    {
        Assert.Equal(Generator.EscapeString("a'b"), "a'b");
    }

    [Fact]
    public void DoubleQuote1Test()
    {
        Assert.Equal(Generator.EscapeString("a\"b"), "a\\\"b");
    }

    [Fact]
    public void DoubleQuote2Test()
    {
        Assert.Equal(Generator.EscapeString("a\"b\"c"), "a\\\"b\\\"c");
    }
}
