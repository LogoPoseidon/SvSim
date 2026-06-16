using System.Numerics;
using SvDesSim.Simulation.Engine;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public partial class SystemCallStatement(
    string subroutine, 
    string formatString, 
    List<IExpression<SimLogic<BigInteger>>> args, 
    EventScheduler scheduler) : IStatement
{
    private static ISimEvent? _activeMonitor;
    public IEnumerable<YieldInstruction> Execute()
    {
        string formatted;
        switch (subroutine)
        {
            case "$display":
                formatted = FormatMessage(formatString, args);
                Console.WriteLine(formatted);
                break;
            case "$write":
                formatted = FormatMessage(formatString, args);
                Console.Write(formatted);
                break;
            case "$strobe":
                var strobeEv = new StrobeEvent(formatString, args);
                scheduler.Schedule(EventRegion.Postponed, strobeEv);
                break;
            case "$monitor":
                if (_activeMonitor is MonitorEvent oldMonitor)
                {
                    scheduler.OnPostponedStep -= oldMonitor.CheckMonitor;
                }

                var monitorEv = new MonitorEvent(formatString, args, scheduler);
                _activeMonitor = monitorEv;

                scheduler.OnPostponedStep += monitorEv.CheckMonitor;

                scheduler.Schedule(EventRegion.Postponed, monitorEv);
                break;
            case "$info":
                formatted = FormatMessage(formatString, args);
                Console.WriteLine($"[INFO] {formatted}");
                break;
            case "$warning":
                formatted = FormatMessage(formatString, args);
                Console.WriteLine($"\e[33m[WARNING] {formatted}\e[0m"); // Yellow
                break;
            case "$error":
                formatted = FormatMessage(formatString, args);
                Console.WriteLine($"\e[31m[ERROR] {formatted}\e[0m"); // Red
                break;
            case "$fatal":
            {
                var finishNum = args.Count > 0 ? (int)args[0].Evaluate().Value : 1;
                
                var formatArgs = args.Skip(1).ToList();
                formatted = FormatMessage(formatString, formatArgs);
                
                if (finishNum > 0)
                {
                    Console.Write($"\e[31;1m[FATAL] (level {finishNum}) {formatted}\e[0m");
                    if (finishNum == 2)
                    {
                        Console.Write($" | Location: Sim Time {scheduler.CurrentTime}");
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"\e[31;1m[FATAL] {formatted}\e[0m");
                }
                Environment.Exit(1);
                break;
            }
            case "$finish":
            {
                var finishNum = args.Count > 0 ? (int)args[0].Evaluate().Value : 1;
                
                if (finishNum > 0)
                {
                    Console.Write($"[Sim Time {scheduler.CurrentTime}] $finish called");
                    if (finishNum == 2)
                    {
                        var ramUsage = GC.GetTotalMemory(false) / 1024;
                        Console.Write($" | Diagnostics [Memory: {ramUsage} KB]");
                    }
                    Console.WriteLine(".");
                }
                Environment.Exit(0);
                break;
            }
        }

        yield break;
    }

    private static string FormatMessage(string format, List<IExpression<SimLogic<BigInteger>>> formatArgs)
    {
        var formatted = format;
        var matches = FormatSpecifierRegex().Matches(formatted);

        for (var i = 0; i < Math.Min(matches.Count, formatArgs.Count); i++)
        {
            var val = formatArgs[i].Evaluate().Value;
            var specifier = matches[i].Value;
            var lastChar = char.ToLower(specifier[^1]);
            
            var replacement = lastChar switch
            {
                'h' or 'x' => val.ToString("X"),
                'd' => val.ToString(),
                'b' => ToBinaryString(val),
                'o' => ToOctalString(val),
                's' => DecodeBigIntegerToString(val),
                _ => val.ToString()
            };
            
            formatted = ReplaceFirst(formatted, specifier, replacement);
        }

        formatted = formatted.Replace("%%", "%");

        return formatted;
    }

    private static string DecodeBigIntegerToString(BigInteger val)
    {
        var chars = new List<char>();
        var temp = val;
        while (temp > 0)
        {
            chars.Add((char)(temp & 0xFF));
            temp >>= 8;
        }
        chars.Reverse();
        return new string(chars.ToArray());
    }

    private static string ToOctalString(BigInteger bigint)
    {
        if (bigint == 0) return "0";
        var result = new System.Text.StringBuilder();
        var temp = bigint;
        while (temp > 0)
        {
            result.Insert(0, (temp % 8).ToString());
            temp /= 8;
        }
        return result.ToString();
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

    [System.Text.RegularExpressions.GeneratedRegex("%[0-9.]*[a-zA-Z]")]
    private static partial System.Text.RegularExpressions.Regex FormatSpecifierRegex();
    
    private class StrobeEvent(string formatted, List<IExpression<SimLogic<BigInteger>>> args) : ISimEvent
    {
        public void Execute()
        {
            var msg = FormatMessage(formatted, args);
            Console.WriteLine(msg);
        }

        public void Trigger()
        {
            throw new NotImplementedException();
        }
    }

    private class MonitorEvent(string formatted, List<IExpression<SimLogic<BigInteger>>> args, EventScheduler scheduler) : ISimEvent
    {
        private List<BigInteger> _prevValues = EvaluateArgs(args);
        private bool _firstRun = true;

        private static List<BigInteger> EvaluateArgs(List<IExpression<SimLogic<BigInteger>>> expressions)
        {
            return expressions.Select(expr => expr.Evaluate().Value).ToList();
        }

        public void Execute()
        {
            CheckMonitor();
        }

        public void Trigger()
        {
        }

        public void CheckMonitor()
        {
            if (_activeMonitor != this)
            {
                scheduler.OnPostponedStep -= CheckMonitor;
                return;
            }

            var currentValues = EvaluateArgs(args);
            var changed = _firstRun;

            if (!_firstRun)
            {
                if (currentValues.Where((t, i) => !args[i].GetType().Name.StartsWith("TimeExpr") && t != _prevValues[i]).Any())
                {
                    changed = true;
                }
            }

            if (changed)
            {
                var msg = FormatMessage(formatted, args);
                Console.WriteLine(msg);
                _prevValues = currentValues;
            }

            _firstRun = false;
        }
    }
}