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

    public static Node ParseExpression(string line, int start = 0)
    {
        while (start < line.Length && char.IsWhiteSpace(line[start])) start++;

        throw new Exception("not implemented");
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

    public static bool IsNumber(char c) =>
        c >= '0' && c <= '9';

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

    public static bool IsComparisonOperator(char c) =>
        c == '>' ||
        c == '<';
}
