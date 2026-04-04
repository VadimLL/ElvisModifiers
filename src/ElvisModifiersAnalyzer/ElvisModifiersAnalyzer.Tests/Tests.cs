// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

//#define CORE_TESTS_ONLY

using Microsoft.CodeAnalysis;

namespace ElvisModifiersAnalyzer.Tests;

[TestClass]
public sealed class Tests
{
    static IEnumerable<string[]> getTestFiles()
    {
        static string getSolutionPath()
        {
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? throw new Exception("Solution directory not found");
        }

        string testFilesDirectory = Path.Combine(getSolutionPath(), "src", "ElvisModifiersAnalyzer", "TestAnalyzerLib11");
        if (!Directory.Exists(testFilesDirectory))
        {
            //yield break;
            throw new Exception($"Test project directory ('{testFilesDirectory}') not found");
        }

        foreach (string file in Directory.GetFiles(testFilesDirectory, "*.cs",
#if CORE_TESTS_ONLY
            SearchOption.TopDirectoryOnly
#else
            SearchOption.AllDirectories
#endif
            ))
        {
            //if (new[] { "CertainTest" }.Any(x => file.Contains(x))) // for debug purpose: temporary include only specific files
            if (new[] { "TmpTest", }.All(x => !file.Contains(x))) // for debug purpose: temporary exclude specific files
            {
                yield return [file];
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(getTestFiles), DynamicDataSourceType.Method)]
    public async Task TestEachFile_11(string testFilePath)
    {
        string testCode = await File.ReadAllTextAsync(testFilePath);
        await TestHelper.RunTestAsync(testCode);
    }

    [TestMethod]
    [DynamicData(nameof(getTestFiles), DynamicDataSourceType.Method)]
    public async Task TestEachFile_10(string testFilePath)
    {
        string testCode = await File.ReadAllTextAsync(testFilePath);
        testCode = TestHelper.ToCSharp10(testCode);
        await TestHelper.RunTestAsync(testCode);
    }
}
