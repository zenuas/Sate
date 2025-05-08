using Sate.Block;
using Sate.Expression;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sate;

public static class LineParser
{
    public static Regex IfStatement = new(@"^\s*/\*\s*IF\s+(.+)\*/\s*$", RegexOptions.IgnoreCase);
    public static Regex ElseStatement = new(@"^\s*/\*\s*ELSE\s*\*/\s*$", RegexOptions.IgnoreCase);
    public static Regex ElseIfStatement = new(@"^\s*/\*\s*ELSE\s+IF\s+(.+)\*/\s*$", RegexOptions.IgnoreCase);
    public static Regex ForStatement = new(@"^\s*/\*\s*FOR\s+(.+)\*/\s*$", RegexOptions.IgnoreCase);
    public static Regex ForJoinStatement = new(@"^\s*/\*\s*FOR\s+(.+)\s+JOIN\s+(.+)\*/\s*$", RegexOptions.IgnoreCase);
    public static Regex EndStatement = new(@"^\s*/\*\s*END\s*\*/\s*$", RegexOptions.IgnoreCase);

    public static HashSet<string> ReservedString { get; } = new()
        {
            { "OR" },
            { "AND" },
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
            else if (ForJoinStatement.Match(line) is { } forjoins && forjoins.Success)
            {
                if (body.Count > 0)
                {
                    yield return new StaticBlock(string.Join("\n", body));
                    body.Clear();
                }
                var (block, readed) = ParseForJoin(forjoins.Groups[1].Value.Trim(), forjoins.Groups[2].Value.Trim(), lines, i);
                yield return block;
                i += readed - 1;
            }
            else if (ForStatement.Match(line) is { } fors && fors.Success)
            {
                if (body.Count > 0)
                {
                    yield return new StaticBlock(string.Join("\n", body));
                    body.Clear();
                }
                var (block, readed) = ParseFor(fors.Groups[1].Value.Trim(), lines, i);
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
            else if (ForStatement.Match(line) is { } fors && fors.Success)
            {
                if (body.Count > 0)
                {
                    (stmt_type == StatementTypes.Then ? then_ : else_).Add(new StaticBlock(string.Join("\n", body)));
                    body.Clear();
                }
                var (block, readed) = ParseFor(fors.Groups[1].Value.Trim(), lines, i);
                (stmt_type == StatementTypes.Then ? then_ : else_).Add(block);
                i += readed - 1;
            }
            else if (ForJoinStatement.Match(line) is { } forjoins && forjoins.Success)
            {
                if (body.Count > 0)
                {
                    (stmt_type == StatementTypes.Then ? then_ : else_).Add(new StaticBlock(string.Join("\n", body)));
                    body.Clear();
                }
                var (block, readed) = ParseForJoin(forjoins.Groups[1].Value.Trim(), forjoins.Groups[2].Value.Trim(), lines, i);
                (stmt_type == StatementTypes.Then ? then_ : else_).Add(block);
                i += readed - 1;
            }
            else
            {
                body.Add(line);
            }
        }
        throw new Exception("end statement not found");
    }

    public static (ForBlock ForBlock, int ReadedLine) ParseFor(string iterator, string[] lines, int start)
    {
        var (blocks, readed) = ParseStatements(lines, start);
        return (new(iterator, blocks), readed);
    }

    public static (ForJoinBlock ForJoinBlock, int ReadedLine) ParseForJoin(string iterator, string join, string[] lines, int start)
    {
        var (blocks, readed) = ParseStatements(lines, start);
        return (new(iterator, join, blocks), readed);
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
            else if (ForJoinStatement.Match(line) is { } forjoins && forjoins.Success)
            {
                if (body.Count > 0)
                {
                    stmt.Add(new StaticBlock(string.Join("\n", body)));
                    body.Clear();
                }
                var (block, readed) = ParseForJoin(forjoins.Groups[1].Value.Trim(), forjoins.Groups[2].Value.Trim(), lines, i);
                stmt.Add(block);
                i += readed - 1;
            }
            else if (ForStatement.Match(line) is { } fors && fors.Success)
            {
                if (body.Count > 0)
                {
                    stmt.Add(new StaticBlock(string.Join("\n", body)));
                    body.Clear();
                }
                var (block, readed) = ParseFor(fors.Groups[1].Value.Trim(), lines, i);
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
        var token = ReadNextToken(line, start);
        var len = token.Length;

        if (token.Node.Operand == Operands.None) throw new Exception("expression not found");
        if (token.Node.Operand == Operands.RightParenthesis) throw new Exception("unexpected expression )");

        var left = token.Node;
        if (token.Node.Operand == Operands.LeftParenthesis)
        {
            var expr = ParseExpression(line, start + len, true);
            len += expr.Length;
            token.Node.Left = expr.Node;
        }
        else if (token.Node.Operand == Operands.Operand)
        {
            if (!IsUnaryOperator(token.Node.Value.ToString())) throw new Exception($"unexpected operator {token.Node.Value}");
            var expr = ParseExpression(line, start + len, is_parentheses);
            if (expr.Node.Operand == Operands.Operand && expr.Node.Right is { })
            {
                token.Node.Left = expr.Node.Left;
                expr.Node.Left = token.Node;
                return (len + expr.Length, expr.Node);
            }
            else
            {
                token.Node.Left = expr.Node;
                return (len + expr.Length, token.Node);
            }
        }

        var ope = ReadNextToken(line, start + len);
        len += ope.Length;
        if ((!is_parentheses && ope.Node.Operand == Operands.None) ||
            (is_parentheses && ope.Node.Operand == Operands.RightParenthesis)) return (len, left);
        if (ope.Node.Operand != Operands.Operand) throw new Exception($"unexpected operator {ope.Node.Value}");

        ope.Node.Left = left;
        var right = ParseExpression(line, start + len, is_parentheses);
        len += right.Length;
        if (right.Node.Operand == Operands.Operand &&
            right.Node.Right is { } &&
            GetOperatorPriority(ope.Node.Value.ToString()) >= GetOperatorPriority(right.Node.Value.ToString()))
        {
            ope.Node.Right = right.Node.Left;
            right.Node.Left = ope.Node;
            ope.Node = right.Node;
        }
        else
        {
            ope.Node.Right = right.Node;
        }

        if (!is_parentheses)
        {
            var end = ReadNextToken(line, start + len + right.Length);
            if (end.Node.Operand != Operands.None) throw new Exception($"unexpected expression {end.Node.Value}");
            return (len + end.Length, ope.Node);
        }
        return (len, ope.Node);
    }

    public static (int Length, Node Node) ReadNextToken(string line, int start)
    {
        var len = 0;
        while (start + len < line.Length && char.IsWhiteSpace(line[start + len])) len++;

        if (start + len < line.Length && line[start + len] == '(') return (len + 1, new(Operands.LeftParenthesis, "("));

        if (start + len < line.Length && line[start + len] == ')') return (len + 1, new(Operands.RightParenthesis, ")"));

        var ope_len = TryParseOperator(line, start + len, out var ope);
        if (ope_len > 0) return (len + ope_len, new(Operands.Operand, ope));

        var var_len = TryParseVariable(line, start + len, out var name);
        if (var_len > 0)
        {
            var upper = name.ToUpper();
            return (
                len + var_len,
                ReservedString.Contains(upper) ?
                    new(Operands.Operand, upper) :
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

    public static bool IsUnaryOperator(string s) => s.Length == 1 && IsUnaryOperator(s[0]);

    public static bool IsUnaryOperator(char c) =>
        c == '+' ||
        c == '-';

    public static bool IsEqualOperator(char c) =>
        c == '=';

    public static bool IsNotOperator(char c) =>
        c == '!';

    public static bool IsOperator(char c) =>
        c == '+' ||
        c == '-' ||
        c == '*' ||
        c == '/' ||
        c == '%';

    public static int GetOperatorPriority(string ope) =>
        ope == "OR" ? 1 :
        ope == "AND" ? 2 :
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
