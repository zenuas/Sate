using Sate.Block;
using System;

namespace Sate.Test;

public class LineParserTest
{
    public static IBlock[] Parse(string body) => [.. LineParser.ParseTopLevel(body.Split(["\r\n", "\n", "\r"], StringSplitOptions.None))];

    [Fact]
    public void NullTest()
    {
        var b = Parse("");
        Assert.Equal(b.Length, 1);
        var b0 = Assert.IsType<StaticBlock>(b[0]);
        Assert.Equal(b0.Value, "");
    }

    [Fact]
    public void StaticTest()
    {
        var b = Parse("A");
        Assert.Equal(b.Length, 1);
        var b0 = Assert.IsType<StaticBlock>(b[0]);
        Assert.Equal(b0.Value, "A");
    }

    [Fact]
    public void Static2Test()
    {
        var b = Parse("A\nB");
        Assert.Equal(b.Length, 1);
        var b0 = Assert.IsType<StaticBlock>(b[0]);
        Assert.Equal(b0.Value, "A\nB");
    }

    [Fact]
    public void IfTest()
    {
        var b = Parse("A\n/* IF @Id */\nB\n/* END */");
        Assert.Equal(b.Length, 2);
        var b0 = Assert.IsType<StaticBlock>(b[0]);
        Assert.Equal(b0.Value, "A");
        var b1 = Assert.IsType<IfBlock>(b[1]);
        Assert.Equal(b1.Condition, "@Id");
        Assert.Equal(b1.Then.Length, 1);
        var b1_then0 = Assert.IsType<StaticBlock>(b1.Then[0]);
        Assert.Equal(b1_then0.Value, "B");
        Assert.Equal(b1.Else.Length, 0);
    }

    [Fact]
    public void IfElseTest()
    {
        var b = Parse("A\n/* IF @Id */\nB\n/* ELSE */\nC\n/* END */");
        Assert.Equal(b.Length, 2);
        var b0 = Assert.IsType<StaticBlock>(b[0]);
        Assert.Equal(b0.Value, "A");
        var b1 = Assert.IsType<IfBlock>(b[1]);
        Assert.Equal(b1.Condition, "@Id");
        Assert.Equal(b1.Then.Length, 1);
        var b1_then0 = Assert.IsType<StaticBlock>(b1.Then[0]);
        Assert.Equal(b1_then0.Value, "B");
        Assert.Equal(b1.Else.Length, 1);
        var b1_else0 = Assert.IsType<StaticBlock>(b1.Else[0]);
        Assert.Equal(b1_else0.Value, "C");
    }

    [Fact]
    public void IfElseIfTest()
    {
        var b = Parse("A\n/* IF @Id */\nB\n/* ELSE IF @Name */\nC\n/* END */");
        Assert.Equal(b.Length, 2);
        var b0 = Assert.IsType<StaticBlock>(b[0]);
        Assert.Equal(b0.Value, "A");
        var b1 = Assert.IsType<IfBlock>(b[1]);
        Assert.Equal(b1.Condition, "@Id");
        Assert.Equal(b1.Then.Length, 1);
        var b1_then0 = Assert.IsType<StaticBlock>(b1.Then[0]);
        Assert.Equal(b1_then0.Value, "B");
        Assert.Equal(b1.Else.Length, 1);
        var b1_else0 = Assert.IsType<IfBlock>(b1.Else[0]);
        Assert.Equal(b1_else0.Condition, "@Name");
        Assert.Equal(b1_else0.Then.Length, 1);
        var b1_else0_then0 = Assert.IsType<StaticBlock>(b1_else0.Then[0]);
        Assert.Equal(b1_else0_then0.Value, "C");
        Assert.Equal(b1_else0.Else.Length, 0);
    }

    [Fact]
    public void ErrorTest()
    {
        Assert.Equal(Assert.Throws<Exception>(() => Parse("/* END */")).Message, "unexpected end statement");
        Assert.Equal(Assert.Throws<Exception>(() => Parse("/* ELSE */")).Message, "unexpected else statement");
        Assert.Equal(Assert.Throws<Exception>(() => Parse("/* ELSE IF @Id */")).Message, "unexpected else if statement");
        Assert.Equal(Assert.Throws<Exception>(() => Parse("A\n/* IF @Id */\nB\n/* END */\nC\n/* END */")).Message, "unexpected end statement");
        Assert.Equal(Assert.Throws<Exception>(() => Parse("A\n/* IF @Id */")).Message, "end statement not found");
        Assert.Equal(Assert.Throws<Exception>(() => Parse("A\n/* IF @Id */\nB\n/* ELSE IF */")).Message, "end statement not found");
        Assert.Equal(Assert.Throws<Exception>(() => Parse("A\n/* IF @Id */\nB\n/* ELSE */\nC\n/* ELSE */")).Message, "unexpected else statement");
    }
}
