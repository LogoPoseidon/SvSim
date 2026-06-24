using SvAstParser;

namespace PDesSimulator.SystemVerilogSimulator;

public static class SvSimulationRunner
{

    public static void RunSimulation(string[] svFilePath, ulong maxTime, string? vcdFilePath = null, string slangExecutable = "slang", IEnumerable<string>? additionalArgs = null)
    {
        SvSimulator simulator = new();
        var topLevel = SvParser.ParseFromSystemVerilogFilePaths(svFilePath,slangExecutable, additionalArgs);
        simulator.LoadAndSimulate(topLevel,maxTime,vcdFilePath);
    }
}