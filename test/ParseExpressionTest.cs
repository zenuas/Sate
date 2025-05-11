using Sate.Expression;
using System;

namespace Sate.Test;

public class ParseExpressionTest
{
    [Fact]
    public void NullTest()
    {
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.ParseExpression("")).Message, "expression not found");
    }

    [Fact]
    public void VarTest()
    {
        var (len, node) = LineParser.ParseExpression("a");
        Assert.Equal(len, 1);
        Assert.Equivalent(node, new Node(Operands.Variable, "a"));
    }

    [Fact]
    public void VarAddVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a+b");
        Assert.Equal(len, 3);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "+")
            {
                Left = new(Operands.Variable, "a"),
                Right = new(Operands.Variable, "b")
            });
    }

    [Fact]
    public void VarSubVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a-b");
        Assert.Equal(len, 3);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "-")
            {
                Left = new(Operands.Variable, "a"),
                Right = new(Operands.Variable, "b")
            });
    }

    [Fact]
    public void VarMulVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a*b");
        Assert.Equal(len, 3);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "*")
            {
                Left = new(Operands.Variable, "a"),
                Right = new(Operands.Variable, "b")
            });
    }

    [Fact]
    public void VarDivVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a/b");
        Assert.Equal(len, 3);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "/")
            {
                Left = new(Operands.Variable, "a"),
                Right = new(Operands.Variable, "b")
            });
    }

    [Fact]
    public void VarRemVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a%b");
        Assert.Equal(len, 3);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "%")
            {
                Left = new(Operands.Variable, "a"),
                Right = new(Operands.Variable, "b")
            });
    }

    [Fact]
    public void VarAddVarAddVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a+b+c");
        Assert.Equal(len, 5);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "+")
            {
                Left = new(Operands.Operand, "+")
                {
                    Left = new(Operands.Variable, "a"),
                    Right = new(Operands.Variable, "b")
                },
                Right = new(Operands.Variable, "c")
            });
    }

    [Fact]
    public void VarAddVarSubVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a+b-c");
        Assert.Equal(len, 5);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "-")
            {
                Left = new(Operands.Operand, "+")
                {
                    Left = new(Operands.Variable, "a"),
                    Right = new(Operands.Variable, "b")
                },
                Right = new(Operands.Variable, "c")
            });
    }

    [Fact]
    public void VarAddVarMulVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a+b*c");
        Assert.Equal(len, 5);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "+")
            {
                Left = new(Operands.Variable, "a"),
                Right = new(Operands.Operand, "*")
                {
                    Left = new(Operands.Variable, "b"),
                    Right = new(Operands.Variable, "c")
                }
            });
    }

    [Fact]
    public void VarMulVarAddVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a*b+c");
        Assert.Equal(len, 5);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "+")
            {
                Left = new(Operands.Operand, "*")
                {
                    Left = new(Operands.Variable, "a"),
                    Right = new(Operands.Variable, "b")
                },
                Right = new(Operands.Variable, "c")
            });
    }

    [Fact]
    public void UnaryAddVarTest()
    {
        var (len, node) = LineParser.ParseExpression("+a");
        Assert.Equal(len, 2);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "+")
            {
                Left = new(Operands.Variable, "a")
            });
    }

    [Fact]
    public void VarSubUnaryAddVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a+-b");
        Assert.Equal(len, 4);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "+")
            {
                Left = new(Operands.Variable, "a"),
                Right = new(Operands.Operand, "-")
                {
                    Left = new(Operands.Variable, "b")
                }
            });
    }

    [Fact]
    public void UnaryAddSubAddSubVarTest()
    {
        var (len, node) = LineParser.ParseExpression("+-+-a");
        Assert.Equal(len, 5);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "+")
            {
                Left = new(Operands.Operand, "-")
                {
                    Left = new(Operands.Operand, "+")
                    {
                        Left = new(Operands.Operand, "-")
                        {
                            Left = new(Operands.Variable, "a")
                        }
                    }
                }
            });
    }

    [Fact]
    public void VarMulParenthVarAddVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a*(b+c)");
        Assert.Equal(len, 7);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "*")
            {
                Left = new(Operands.Variable, "a"),
                Right = new(Operands.LeftParenthesis, "(")
                {
                    Left = new(Operands.Operand, "+")
                    {
                        Left = new(Operands.Variable, "b"),
                        Right = new(Operands.Variable, "c")
                    }
                }
            });
    }

    [Fact]
    public void VarAddParenthVarMulVarTest()
    {
        var (len, node) = LineParser.ParseExpression("a+(b*c)");
        Assert.Equal(len, 7);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "+")
            {
                Left = new(Operands.Variable, "a"),
                Right = new(Operands.LeftParenthesis, "(")
                {
                    Left = new(Operands.Operand, "*")
                    {
                        Left = new(Operands.Variable, "b"),
                        Right = new(Operands.Variable, "c")
                    }
                }
            });
    }

    [Fact]
    public void VarLtNumTest()
    {
        var (len, node) = LineParser.ParseExpression("a<1");
        Assert.Equal(len, 3);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "<")
            {
                Left = new(Operands.Variable, "a"),
                Right = new(Operands.Number, 1)
            });
    }

    [Fact]
    public void AndOrAndTest()
    {
        var (len, node) = LineParser.ParseExpression("a<1 and b>2 or c>=3 and d<=4");
        Assert.Equal(len, 28);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "OR")
            {
                Left = new(Operands.Operand, "AND")
                {
                    Left = new(Operands.Operand, "<") { Left = new(Operands.Variable, "a"), Right = new(Operands.Number, 1) },
                    Right = new(Operands.Operand, ">") { Left = new(Operands.Variable, "b"), Right = new(Operands.Number, 2) }
                },
                Right = new(Operands.Operand, "AND")
                {
                    Left = new(Operands.Operand, ">=") { Left = new(Operands.Variable, "c"), Right = new(Operands.Number, 3) },
                    Right = new(Operands.Operand, "<=") { Left = new(Operands.Variable, "d"), Right = new(Operands.Number, 4) }
                }
            });
    }

    [Fact]
    public void MulAddDivTest()
    {
        var (len, node) = LineParser.ParseExpression("1*2+3/4");
        Assert.Equal(len, 7);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "+")
            {
                Left = new(Operands.Operand, "*")
                {
                    Left = new(Operands.Number, 1),
                    Right = new(Operands.Number, 2)
                },
                Right = new(Operands.Operand, "/")
                {
                    Left = new(Operands.Number, 3),
                    Right = new(Operands.Number, 4)
                }
            });
    }

    [Fact]
    public void AddMulSubTest()
    {
        var (len, node) = LineParser.ParseExpression("1+2*3-4");
        Assert.Equal(len, 7);
        Assert.Equivalent(node,
            new Node(Operands.Operand, "+")
            {
                Left = new(Operands.Number, 1),
                Right = new(Operands.Operand, "-")
                {
                    Left = new(Operands.Operand, "*")
                    {
                        Left = new(Operands.Number, 2),
                        Right = new(Operands.Number, 3)
                    },
                    Right = new(Operands.Number, 4)
                },
            });
    }

    [Fact]
    public void ErrorTest()
    {
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.ParseExpression("*a")).Message, "unexpected operator *");
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.ParseExpression("a+")).Message, "expression not found");
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.ParseExpression(")")).Message, "unexpected expression )");
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.ParseExpression("()")).Message, "unexpected expression )");
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.ParseExpression("())")).Message, "unexpected expression )");
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.ParseExpression("a+(")).Message, "expression not found");
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.ParseExpression("a+(b")).Message, "unexpected operator EOL");
        Assert.Equal(Assert.Throws<Exception>(() => LineParser.ParseExpression("a+(b+c")).Message, "unexpected operator EOL");
    }
}
