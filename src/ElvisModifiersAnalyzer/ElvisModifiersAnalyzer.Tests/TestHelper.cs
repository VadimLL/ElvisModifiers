// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System.Text;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace ElvisModifiersAnalyzer.Tests;

internal static class TestHelper
{
    record TestInfo(string TestCode, IEnumerable<DiagnosticResult> DiagnosticResults);

    static readonly string ElvisModifiersLibPath = MetadataReference.CreateFromFile(typeof(OnlyYouAttribute).Assembly.Location).FilePath!;

    static readonly Regex rulePrefixRegex = new Regex(@$"/\*{ElvisModifiersAnalyzer.EA}_", RegexOptions.Compiled);
    static readonly Regex endExpRegex = new Regex(@"\s*[;=/\+\-\*\|\[]", RegexOptions.Compiled);
    const string identifier = @"([^\W\d_]|_)+[\w]*";
    //const string complexIdr = @$"{identifier}(\.{identifier})*";
    static readonly Regex indexerRegex = new Regex(@$"{identifier}\[", RegexOptions.Compiled);

    static TestInfo getTestInfo(in string testCode)
    {
        var diagnosticResults = new List<DiagnosticResult>();
        var sb = new StringBuilder();
        int count = 0;
        int end_exp = 0;
        foreach (Match match in rulePrefixRegex.Matches(testCode))
        {
            // skip line-comments
            int lineIndex = testCode.LastIndexOf(Environment.NewLine, match.Index);
            lineIndex = lineIndex < 0 ? 0 : testCode.SkipWhiteSpaces(lineIndex);
            if (testCode.StartsWith(lineIndex, "//"))
            {
                continue;
            }

            sb.Append(testCode.Substring(end_exp, match.Index - end_exp));
            int end_rule = testCode.IndexOf("*/", match.Index + 2);
            string ruleId = testCode.Substring(match.Index + 2, end_rule - match.Index - 2);
            end_rule += 2;
            int parentheses = 0;
            if (ruleId.StartsWith(ElvisModifiersAnalyzer.EA_METH))
            {
                int end_line = testCode.IndexOf(Environment.NewLine, end_rule);
                if (indexerRegex.Match(testCode, end_rule + 1, end_line - end_rule).Success)
                {
                    end_exp = testCode.IndexOf("]", end_rule) + 1;
                }
                else
                {
                    end_exp = testCode.IndexOf(";", end_rule);
                }
            }
            else if (ruleId.StartsWith(ElvisModifiersAnalyzer.EA_PROP)
                  || ruleId.StartsWith(ElvisModifiersAnalyzer.EA_TYPE))
            {
                end_rule = testCode.SkipWhiteSpaces(++end_rule);
                parentheses = testCode.SkipChars(end_rule, '(') - end_rule;
                end_rule += parentheses;
                if (testCode.StartsWith(end_rule, "++"))
                {
                    sb.Append("++");
                    end_rule += 2;
                    parentheses = testCode.SkipChars(end_rule, '(') - end_rule;
                    end_rule += parentheses;
                }
                else if (testCode.StartsWith(end_rule, "--"))
                {
                    sb.Append("--");
                    end_rule += 2;
                    parentheses = testCode.SkipChars(end_rule, '(') - end_rule;
                    end_rule += parentheses;
                }

                Match end_match = endExpRegex.Match(testCode, end_rule + 1);
                string raw_exp = end_match.Value.CutLastChar();
                int d = raw_exp.Length - raw_exp.TrimEnd().Length;
                end_exp = end_match.Index + end_match.Length - 1 - d - parentheses;
            }
            //else if (ruleId.StartsWith(ElvisModifiersAnalyzer.EA_TYPE))
            //{
            //    var end_match = endExpRegex.Match(testCode, end_rule + 1);
            //    string raw_exp = end_match.Value.CutLastChar();
            //    int d = raw_exp.Length - raw_exp.TrimEnd().Length;
            //    end_exp = end_match.Index + end_match.Length - 1 - d;
            //}
            else
            {
                throw new Exception($"Unknown ruleId: {ruleId}");
            }

            string exp = testCode.Substring(end_rule, end_exp - end_rule).TrimStart();
            string markup = $"{"(".Replicate(parentheses)}{{|#{count}:{exp}|}}";
            sb.Append(markup);
            diagnosticResults.Add(
                    DiagnosticResult.CompilerError(ruleId)
                    //.WithArguments("TakeMyHalfMoney", "CantAcceptMoney")
                    //.WithLocation(22, 56)
                    .WithLocation(count)
            );
            ++count;
        }

        sb.Append(testCode.Substring(end_exp));
        return new TestInfo(sb.ToString(), diagnosticResults);
    }


    static CSharpAnalyzerTest<ElvisModifiersAnalyzer, DefaultVerifier> сreateTest(in string testCode)
    {
        var testInfo = getTestInfo(testCode);
        var test = new CSharpAnalyzerTest<ElvisModifiersAnalyzer, DefaultVerifier> {
            TestCode = testInfo.TestCode,
        };
        test.TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(ElvisModifiersLibPath));
        test.ExpectedDiagnostics.AddRange(testInfo.DiagnosticResults);
        return test;
    }

    public static async Task RunTestAsync(string testCode)
    {
        var test = сreateTest(testCode);
        await test.RunAsync();
        //await (test?.RunAsync() ?? Task.CompletedTask);
    }


    static readonly Regex attrRegex = new Regex(@"\[(OnlyYou|OnlyAlias)<", RegexOptions.Compiled);
    public static string ToCSharp10(in string code11)
    {
        var sb = new StringBuilder();
        Match match = attrRegex.Match(code11);
        int tail_i = 0;
        while(match.Success)
        {
            int attr_i = match.Index;
            int type_start = attr_i + match.Value.Length;
            sb.Append(code11.Substring(tail_i, type_start - 1 - tail_i));
            sb.Append("(typeof(");
            int type_end = code11.IndexOf('>', type_start + 1);
            string type = code11.Substring(type_start, type_end - type_start);
            sb.Append(type).Append(')');
            type_end = code11.SkipWhiteSpaces(++type_end);
            tail_i = type_end;
            if (code11[type_end] == '(')
            {
                sb.Append(", ");
                ++tail_i;
            }
            else
            {
                sb.Append(')');
            }

            match = attrRegex.Match(code11, tail_i);
        }

        sb.Append(code11.Substring(tail_i));
        return sb
            .Replace("file interface", "interface")
            .Replace("file abstract class", "abstract class")
            .Replace("file class", "class").ToString();
    }
}

internal static class StringEx
{
    public static bool StartsWith(this string str, in int startIndex, in string subString)
    {
        if (startIndex + subString.Length > str.Length)
        {
            return false;
        }

        for (int i = 0; i < subString.Length; i++)
        {
            if (str[startIndex + i] != subString[i])
            {
                return false;
            }
        }

        return true;
    }

    public static string CutLastChar(this string str) 
        => str.Length == 0 ? str : str.Substring(0, str.Length - 1);

    public static int SkipWhiteSpaces(this string str, int index)
    {
        while (index < str.Length - 1 && char.IsWhiteSpace(str[index])) ++index;
        return index;
    }

    public static int SkipChars(this string str, int index, in char ch)
    {
        while (index < str.Length - 1 && str[index] == ch) ++index;
        return index;
    }

    public static string Replicate(this string stringToRepeat, in int count)
        => new StringBuilder(stringToRepeat.Length * count)
                .Insert(0, stringToRepeat, count)
                .ToString();
}