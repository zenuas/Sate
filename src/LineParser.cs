﻿using Sate.Block;
using Sate.Expression;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sate;

public static class LineParser
{
    public static Regex IfStatement = new(@"^\s*/\*\s*IF\s+(.+)\*/\s*$", RegexOptions.IgnoreCase);
    public static Regex ElseStatement = new(@"^\s*/\*\s*ELSE\s*\*/\s*$", RegexOptions.IgnoreCase);
    public static Regex ElseIfStatement = new(@"^\s*/\*\s*ELSE\s+IF\s+(.+)\*/\s*$", RegexOptions.IgnoreCase);
    public static Regex EndStatement = new(@"^\s*/\*\s*END\s*\*/\s*$", RegexOptions.IgnoreCase);

    public static Dictionary<string, string> ReservedString { get; } = new()
        {
            { "OR", "||" },
            { "AND", "&&" },
        };

    public static IEnumerable<IBlock> ParseTopLevel(string[] lines)
    {
        var body = new List<string>();
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (EndStatement.IsMatch(line)) throw new Exception("unexpected end statement");
            if (ElseStatement.IsMatch(line)) throw new Exception("unexpected else statement");
            if (ElseIfStatement.IsMatch(line)) throw new Exception("unexpected else if statement");

            if (IfStatement.Match(line) is { } ifs && ifs.Success)
            {
                if (body.Count > 0)
                {
                    yield return new StaticBlock(string.Join("\n", body));
                    body.Clear();
                }
                var (block, readed) = ParseIf(ifs.Groups[1].Value.Trim(), lines, i);
                yield return block;
                i += readed - 1;
            }
            else
            {
                body.Add(line);
            }
        }
        if (body.Count > 0) yield return new StaticBlock(string.Join("\n", body));
    }

    public static (IfBlock IfBlock, int ReadedLine) ParseIf(string cond, string[] lines, int start)
    {
        var body = new List<string>();
        var then_ = new List<IBlock>();
        var else_ = new List<IBlock>();
        var stmt_type = StatementTypes.Then;

        for (var i = start + 1; i < lines.Length; i++)
        {
            var line = lines[i];

            if (ElseStatement.IsMatch(line))
            {
                if (stmt_type == StatementTypes.Else) throw new Exception("unexpected else statement");
                if (body.Count > 0)
                {
                    then_.Add(new StaticBlock(string.Join("\n", body)));
                    body.Clear();
                }
                stmt_type = StatementTypes.Else;
            }
            else if (ElseIfStatement.Match(line) is { } elseifs && elseifs.Success)
            {
                if (stmt_type == StatementTypes.Else) throw new Exception("unexpected else statement");
                if (body.Count > 0)
                {
                    then_.Add(new StaticBlock(string.Join("\n", body)));
                    body.Clear();
                }
                var (block, readed) = ParseIf(elseifs.Groups[1].Value.Trim(), lines, i);
                else_.Add(block);
                return (new(cond, [.. then_], [.. else_]), i - start + readed + 1);
            }
            else if (EndStatement.IsMatch(line))
            {
                (stmt_type == StatementTypes.Then ? then_ : else_).Add(new StaticBlock(string.Join("\n", body)));
                return (new(cond, [.. then_], [.. else_]), i - start + 1);
            }
            else
            {
                body.Add(line);
            }
        }
        throw new Exception("end statement not found");
    }

    public static (IBlock[] Blocks, int ReadedLine) ParseStatements(string[] lines, int start)
    {
        var body = new List<string>();
        var stmt = new List<IBlock>();

        for (var i = start + 1; i < lines.Length; i++)
        {
            var line = lines[i];

            if (ElseStatement.IsMatch(line)) throw new Exception("unexpected else statement");
            if (ElseIfStatement.IsMatch(line)) throw new Exception("unexpected else if statement");

            if (EndStatement.IsMatch(line))
            {
                if (body.Count > 0) stmt.Add(new StaticBlock(string.Join("\n", body)));
                return ([.. stmt], i - start + 1);
            }
            else if (IfStatement.Match(line) is { } ifs && ifs.Success)
            {
                if (body.Count > 0)
                {
                    stmt.Add(new StaticBlock(string.Join("\n", body)));
                    body.Clear();
                }
                var (block, readed) = ParseIf(ifs.Groups[1].Value.Trim(), lines, i);
                stmt.Add(block);
                i += readed - 1;
            }
            else
            {
                body.Add(line);
            }
        }
        throw new Exception("end statement not found");
    }

    public static (int Length, Node Node) ParseExpression(string line, int start = 0, bool is_parentheses = false)
    {
        var (len, left) = ParseOneExpression(line, start);
        var next = ParseOperandExpression(line, start + len, left, is_parentheses);
        return (len + next.Length, next.Node);
    }

    public static (int Length, Node Node) ParseOperandExpression(string line, int start, Node left, bool is_parentheses)
    {
        var (len, ope) = ReadNextToken(line, start);
        if ((!is_parentheses && ope.Operand == Operands.None) ||
            (is_parentheses && ope.Operand == Operands.RightParenthesis)) return (len, left);
        if (ope.Operand != Operands.Operand) throw new Exception($"unexpected operator {ope.Value}");

        ope.Left = left;
        var right = ParseOneExpression(line, start + len);
        len += right.Length;
        ope.Right = right.Node;

        if (left.Operand == Operands.Operand &&
            !IsUnaryOperator(left) &&
            GetOperatorPriority(left.Value.ToString()) < GetOperatorPriority(ope.Value.ToString()))
        {
            ope.Left = left.Right;
            var next = ParseOperandExpression(line, start + len, ope, is_parentheses);
            len += next.Length;

            if (next.Node.Operand == Operands.Operand &&
                GetOperatorPriority(left.Value.ToString()) > GetOperatorPriority(next.Node.Value.ToString()))
            {
                // "A and B or C and D" => A and (B or (C and D)) => (A and B) or (C and D)
                left.Right = next.Node.Left;
                next.Node.Left = left;
                return (len, next.Node);
            }
            left.Right = next.Node;
            return (len, left);
        }
        else
        {
            var next = ParseOperandExpression(line, start + len, ope, is_parentheses);
            return (len + next.Length, next.Node);
        }

    }

    public static (int Length, Node Node) ParseOneExpression(string line, int start)
    {
        var token = ReadNextToken(line, start);

        if (token.Node.Operand == Operands.None) throw new Exception("expression not found");
        if (token.Node.Operand == Operands.RightParenthesis) throw new Exception("unexpected expression )");

        if (token.Node.Operand == Operands.LeftParenthesis)
        {
            var expr = ParseExpression(line, start + token.Length, true);
            token.Node.Left = expr.Node;
            return (token.Length + expr.Length, token.Node);
        }
        else if (token.Node.Operand == Operands.Operand)
        {
            if (!IsUnaryOperator(token.Node.Value.ToString())) throw new Exception($"unexpected operator {token.Node.Value}");
            var expr = ParseOneExpression(line, start + token.Length);
            token.Node.Right = expr.Node;
            return (token.Length + expr.Length, token.Node);
        }
        return (token.Length, token.Node);
    }

    public static (int Length, Node Node) ReadNextToken(string line, int start)
    {
        var len = 0;
        while (start + len < line.Length && char.IsWhiteSpace(line[start + len])) len++;

        if (start + len < line.Length && line[start + len] == '(') return (len + 1, new(Operands.LeftParenthesis, "("));

        if (start + len < line.Length && line[start + len] == ')') return (len + 1, new(Operands.RightParenthesis, ")"));

        var ope_len = TryParseOperator(line, start + len, out var ope);
        if (ope_len > 0) return (len + ope_len, new(Operands.Operand, ope));

        var str_len = TryParseString(line, start + len, out var str);
        if (str_len > 0) return (len + str_len, new(Operands.String, str));

        var var_len = TryParseVariable(line, start + len, out var name);
        if (var_len > 0)
        {
            return (
                len + var_len,
                ReservedString.TryGetValue(name.ToUpper(), out var value) ?
                    new(Operands.Operand, value) :
                    new(Operands.Variable, name)
            );
        }

        var num_len = TryParseNumber(line, start + len, out var num);
        if (num_len > 0) return (len + num_len, new(Operands.Number, num));

        return (len, new(Operands.None, "EOL"));
    }

    public static int TryParseOperator(string line, int start, out string ope)
    {
        ope = "";
        if (start >= line.Length) return 0;

        var c = line[start];
        if (IsEqualOperator(c) || IsNotOperator(c))
        {
            // == or !=
            if (start + 1 >= line.Length || !IsEqualOperator(line[start + 1])) throw new Exception($"unexpected operator {c}");
            ope = line.Substring(start, 2);
            return 2;
        }
        else if (IsAndOperator(c) || IsOrOperator(c))
        {
            // && or ||
            if (start + 1 >= line.Length || line[start + 1] != c) throw new Exception($"unexpected operator {c}");
            ope = line.Substring(start, 2);
            return 2;
        }
        else if (IsComparisonOperator(c))
        {
            // <= or >=
            if (start + 1 < line.Length && IsEqualOperator(line[start + 1]))
            {
                ope = line.Substring(start, 2);
                return 2;
            }
            ope = c.ToString();
            return 1;
        }
        else if (IsOperator(c))
        {
            ope = c.ToString();
            return 1;
        }
        return 0;
    }

    public static int TryParseString(string line, int start, out string str)
    {
        str = "";
        if (start >= line.Length) return 0;

        var q = line[start];
        var s = new StringBuilder();
        if (q == '"' || q == '\'')
        {
            var len = 1;
            while (true)
            {
                if (start + len >= line.Length) return 0;
                var c = line[start + len++];
                if (c == q) break;
                else if (c == '\\')
                {
                    if (start + len >= line.Length) return 0;
                    c = line[start + len++] switch
                    {
                        '\'' => '\'',
                        '"' => '"',
                        'r' => '\r',
                        'n' => '\n',
                        't' => '\t',
                        _ => throw new Exception($"invalid string escape"),
                    };
                }
                s.Append(c);
            }
            str = s.ToString();
            return len;
        }
        return 0;
    }

    public static int TryParseVariable(string line, int start, out string name)
    {
        name = "";
        if (start >= line.Length) return 0;

        var len = 0;
        if (IsVariablePrefix(line[start])) len++;
        while (start + len < line.Length && line[start + len] == '_') len++;

        if (start + len >= line.Length) return 0;
        if (!IsVariableFirst(line[start + len])) return 0;

        while (start + len < line.Length && IsVariable(line[start + len])) len++;

        name = line.Substring(start, len);
        return len;
    }

    public static int TryParseNumber(string line, int start, out int num)
    {
        num = 0;
        if (start >= line.Length || !IsNumber(line[start])) return 0;

        var len = 0;
        while (start + len < line.Length)
        {
            var c = line[start + len];

            if (c == '_') { len++; continue; }
            if (!IsNumber(c)) break;

            len++;
            num = (num * 10) + (c - '0');
        }
        return len;
    }

    public static bool IsNumber(char c) =>
        c >= '0' && c <= '9';

    public static bool IsVariableFirst(char c) =>
        (c >= 'a' && c <= 'z') ||
        (c >= 'A' && c <= 'Z');

    public static bool IsVariable(char c) =>
        IsVariableFirst(c) ||
        IsNumber(c) ||
        c == '_';

    public static bool IsVariablePrefix(char c) =>
        c == '@' ||
        c == ':';

    public static bool IsUnaryOperator(Node node) => node.Operand == Operands.Operand && IsUnaryOperator(node.Value.ToString()) && node.Left is null;

    public static bool IsUnaryOperator(string s) => s.Length == 1 && IsUnaryOperator(s[0]);

    public static bool IsUnaryOperator(char c) =>
        c == '+' ||
        c == '-';

    public static bool IsEqualOperator(char c) =>
        c == '=';

    public static bool IsNotOperator(char c) =>
        c == '!';

    public static bool IsOrOperator(char c) =>
        c == '|';

    public static bool IsAndOperator(char c) =>
        c == '&';

    public static bool IsOperator(char c) =>
        c == '+' ||
        c == '-' ||
        c == '*' ||
        c == '/' ||
        c == '%';

    public static int GetOperatorPriority(string ope) =>
        ope == "||" ? 1 :
        ope == "&&" ? 2 :
        ope == "==" ? 3 :
        ope == "!=" ? 3 :
        ope == "<" ? 4 :
        ope == ">" ? 4 :
        ope == "<=" ? 4 :
        ope == ">=" ? 4 :
        ope == "+" ? 5 :
        ope == "-" ? 5 :
        ope == "*" ? 6 :
        ope == "/" ? 6 :
        ope == "%" ? 6 :
        throw new Exception($"unexpected operator {ope}");

    public static bool IsComparisonOperator(char c) =>
        c == '>' ||
        c == '<';
}
