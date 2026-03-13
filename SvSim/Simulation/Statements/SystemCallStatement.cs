using System.Numerics;
using SvSim.Simulation.Engine;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public partial class SystemCallStatement(
    string subroutine, 
    string formatString, 
    List<IExpression<SimLogic<BigInteger>>> args, 
    EventScheduler scheduler) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        switch (subroutine)
        {
            case "$display":
            {
                var formatted = formatString;
                var matches = MyRegex().Matches(formatted);
        
                for (var i = 0; i < Math.Min(matches.Count, args.Count); i++)
                {
                    var val = args[i].Evaluate().Value;
                    var specifier = matches[i].Value;
                    var replacement = specifier switch
                    {
                        not null when specifier.EndsWith('h') => val.ToString("X"),
                        not null when specifier.EndsWith('d') => val.ToString(),
                        not null when specifier.EndsWith('b') => ToBinaryString(val),
                        _ => val.ToString()
                    };
                    var pos = formatted.IndexOf(specifier);
                    formatted = formatted.Remove(pos, specifier.Length).Insert(pos, replacement);
                }
                Console.WriteLine(formatted);
                break;
            }
            case "$finish":
                Console.WriteLine($"[Sim Time {scheduler.CurrentTime}] $finish called.");
                Environment.Exit(0);
                break;
        }

        yield break;
    }

    private static string ToBinaryString(BigInteger bigint)
    {
        var bytes = bigint.ToByteArray();
        var idx = bytes.Length - 1;
        var base2 = new System.Text.StringBuilder(bytes.Length * 8);
        var binary = Convert.ToString(bytes[idx], 2);
        base2.Append(binary);
        for (idx--; idx >= 0; idx--)
        {
            base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
        }
        return base2.ToString();
    }

    private static string ReplaceFirst(string text, string search, string replace)
    {
        var pos = text.IndexOf(search, StringComparison.Ordinal);
        if (pos < 0) return text;
        return text[..pos] + replace + text[(pos + search.Length)..];
    }

    [System.Text.RegularExpressions.GeneratedRegex("%[0-9]*[a-zA-Z]")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}