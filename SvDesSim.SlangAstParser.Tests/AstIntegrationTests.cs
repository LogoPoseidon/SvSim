using System.Collections.Concurrent;
using SvAstParser;
using SvDesSim.Elaboration;
using SvDesSim.Simulation.Engine;

namespace SvDesSim.SlangAstParser.Tests;

public static class SystemVerilogTestCases
{
    public static IEnumerable<TestCaseData> GetSvTestFiles()
    {
        var testDataDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
        if (!Directory.Exists(testDataDir))
        {
            yield break;
        }

        var files = Directory.GetFiles(testDataDir, "*.sv", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(testDataDir, file);
            yield return new TestCaseData(file).SetName(relativePath);
        }
    }
}

[TestFixture]
public class SystemVerilogIntegrationTests
{
    private static readonly ConcurrentDictionary<string, TopLevel> AstCache = new();

    private static TopLevel GetOrParseAst(string svFilePath)
    {
        return AstCache.GetOrAdd(svFilePath, path =>
        {
            var slangPath = Environment.GetEnvironmentVariable("SLANG_PATH") ?? "slang";
            return SvParser.ParseFromSystemVerilogFilePath(path, slangExecutable: slangPath);
        });
    }

    [TestCaseSource(typeof(SystemVerilogTestCases), nameof(SystemVerilogTestCases.GetSvTestFiles))]
    [Order(1)]
    public void TestParserOnly(string svFilePath)
    {
        Assert.DoesNotThrow(() =>
        {
            var topLevel = GetOrParseAst(svFilePath);
            Assert.That(topLevel, Is.Not.Null, "Parser returned a null AST representation.");
        }, $"Parsing failed for: {Path.GetFileName(svFilePath)}");
    }

    [TestCaseSource(typeof(SystemVerilogTestCases), nameof(SystemVerilogTestCases.GetSvTestFiles))]
    [Order(2)]
    public void TestElaboratorOnly(string svFilePath)
    {
        TopLevel? topLevel = null;
        try
        {
            topLevel = GetOrParseAst(svFilePath);
        }
        catch (Exception ex)
        {
            Assert.Ignore($"Skipping elaboration because parsing failed: {ex.Message}");
        }

        Assert.That(topLevel, Is.Not.Null, "Cannot elaborate because AST is null.");

        Assert.DoesNotThrow(() =>
        {
            var scheduler = new EventScheduler();
            var elaborator = new StructuralElaborator(scheduler);
            var rootScope = elaborator.ElaborateDesign(topLevel);

            Assert.That(rootScope, Is.Not.Null, "Elaboration returned a null root scope.");
        }, $"Elaboration failed for: {Path.GetFileName(svFilePath)}");
    }
}