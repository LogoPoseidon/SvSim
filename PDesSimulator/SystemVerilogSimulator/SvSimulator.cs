using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using PDesSimulator.Simulator;
using SvAstParser;
using SvAstParser.AstTree;
using SvAstParser.AstTree.Expression;
using SvAstParser.AstTree.Expression.ValueExpressionBase;
using SvAstParser.AstTree.Statement;
using SvAstParser.AstTree.SvEnums;
using SvAstParser.AstTree.Symbol;
using SvAstParser.AstTree.Symbol.InstanceSymbolBase;
using SvAstParser.AstTree.Symbol.Type.IntegralType;
using SvAstParser.AstTree.Symbol.ValueSymbol;
using SvAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol;
using SvAstParser.AstTree.TimingControl;
using SvConditional = SvAstParser.AstTree.Statement.SvConditional;

namespace PDesSimulator.SystemVerilogSimulator;

public partial class SvSimulator
{
    private readonly Dictionary<string, SvSignal> _signals = new(StringComparer.Ordinal);
    private readonly Dictionary<string, BigInteger> _params = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _portAliases = new(StringComparer.Ordinal);
    private readonly HashSet<object> _visitedNodes = new(ReferenceEqualityComparer.Instance);

    private readonly List<(string format, IReadOnlyList<ISvExpression> exprs, string scope, List<BigInteger> lastValues)> _monitors = [];

    private string? _vcdDirectory;
    private string? _pendingDumpfile;
    private Predicate<ISignal>? _vcdFilter;
    private bool _vcdInitialized;

    public void LoadAndSimulate(TopLevel topLevel, ulong maxTime, string? vcdFile = null)
    {
        Kernel.Instance.Reset();
        _signals.Clear();
        _params.Clear();
        _portAliases.Clear();
        _monitors.Clear();
        _visitedNodes.Clear();
        
        _vcdDirectory = vcdFile;
        if (!string.IsNullOrEmpty(_vcdDirectory))
        {
            if (Path.HasExtension(_vcdDirectory))
            {
                _vcdDirectory = Path.GetDirectoryName(_vcdDirectory);
            }
        }

        _pendingDumpfile = null;
        _vcdFilter = null;
        _vcdInitialized = false;

        TraverseScopes(topLevel, "top");

        Kernel.Instance.RegisterWait(0, TryInitVcd);
        Kernel.Instance.OnTimeStepComplete += ProcessMonitors;

        // DumpDebugState();
        Kernel.Instance.StartSimulation(maxTime);
    }
    
    private void DumpDebugState()
    {
        Console.WriteLine("\n[DEBUG] --- Registered Signals ---");
        foreach (var sig in _signals.Keys) Console.WriteLine("  " + sig);
        
        Console.WriteLine("\n[DEBUG] --- Port Aliases ---");
        foreach (var alias in _portAliases) Console.WriteLine($"  {alias.Key} -> {alias.Value}");
        Console.WriteLine("----------------------------------\n");
    }

    private void TryInitVcd()
    {
        if (_vcdInitialized || string.IsNullOrEmpty(_pendingDumpfile)) return;

        var finalPath = _pendingDumpfile;
        if (!string.IsNullOrEmpty(_vcdDirectory))
        {
            finalPath = Path.Combine(_vcdDirectory, _pendingDumpfile);
        }

        if (finalPath.EndsWith(".vcd", StringComparison.OrdinalIgnoreCase))
        {
            finalPath = finalPath[..^4];
        }

        Kernel.Instance.VcdInit(finalPath, _signals.Values, _vcdFilter);
        _vcdInitialized = true;
    }

    private void TraverseScopes(object? node, string scope)
    {
        if (node == null) return;
        if (!_visitedNodes.Add(node)) return;

        RegisterDeclaration(node, scope);
        RegisterBehaviors(node, scope);

        if (node is SvInstance inst)
        {
            var childScope = $"{scope}.{inst.Name}";
            if (inst.Body?.Members != null)
            {
                foreach (var member in inst.Body.Members)
                {
                    TraverseScopes(member, childScope);
                }
            }
            return;
        }

        var type = node.GetType();
        if (type.IsPrimitive || node is string) return;

        if (node is System.Collections.IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                TraverseScopes(item, scope);
            }
            return;
        }

        var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var prop in props)
        {
            if (prop.GetIndexParameters().Length != 0 || prop.IsDefined(typeof(JsonIgnoreAttribute), true)) continue;
            if (!typeof(ISvAstNode).IsAssignableFrom(prop.PropertyType) &&
                (!typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) ||
                 prop.PropertyType == typeof(string))) continue;

            var val = prop.GetValue(node);
            if (val != null) TraverseScopes(val, scope);
        }
    }

    private void RegisterDeclaration(object node, string scope)
    {
        switch (node)
        {
            case SvParameter param:
            {
                var val = param.Initializer != null ? Eval(param.Initializer, scope) : BigInteger.Zero;
                _params[$"{scope}.{param.Name}"] = val;
                break;
            }
            case SvVariable v:
            {
                var width = ParseWidthFromTypeString(v.Type ?? "");
                var isSigned = v.ResolvedType is ISvIntegralType { IsSigned: true };
                var initVal = v.Initializer != null ? Eval(v.Initializer, scope) : BigInteger.Zero;
                var key = $"{scope}.{v.Name}";
                // if (_signals.ContainsKey(key))
                // {
                //     Console.WriteLine($"[DEBUG-WARNING] Signal '{key}' is being registered multiple times!");
                //     break;
                // }
                _signals[key] = new SvSignal($"{scope}.{v.Name}", width, isSigned, initVal);
                break;
            }
            case SvNet net:
            {
                var width = ParseWidthFromTypeString(net.Type ?? "");
                var isSigned = net.ResolvedType is ISvIntegralType { IsSigned: true };
                var initVal = net.Initializer != null ? Eval(net.Initializer, scope) : BigInteger.Zero;
                _signals[$"{scope}.{net.Name}"] = new SvSignal($"{scope}.{net.Name}", width, isSigned, initVal);
                break;
            }
            case SvInstance inst:
            {
                var childScope = $"{scope}.{inst.Name}";
                if (inst.Connections != null)
                {
                    foreach (var conn in inst.Connections)
                    {
                        if (conn.Port != null)
                        {
                            var parentSymbol = GetParentSymbolFromExpr(conn.Expr);
                            if (parentSymbol != null)
                            {
                                var cleanParentSymbol = CleanSymbolName(parentSymbol);
                                _portAliases[$"{childScope}.{conn.Port.Name}"] = $"{scope}.{cleanParentSymbol}";
                            }
                        }
                    }
                }
                break;
            }
        }
    }

    private void RegisterBehaviors(object node, string scope)
    {
        switch (node)
        {
            case SvContinuousAssign { Assignment: SvAssignment assign }:
            {
                var sensitivity = new List<ISignal>();
                ExtractSensitivity(assign.Right, sensitivity, scope);

                var desEvent = new DesEvent
                {
                    Action = () => Eval(assign, scope)
                };
                Kernel.Instance.RegisterProcess(desEvent, sensitivity);
                break;
            }
            case SvProceduralBlock proc:
            {
                if (proc.Body is SvTimed { Timing: SvSignalEvent ev } timed)
                {
                    var sensitivity = new List<ISignal>();
                    ExtractSensitivity(ev.Expr, sensitivity, scope);

                    var desEvent = new DesEvent
                    {
                        Action = () =>
                        {
                            if (CheckSignalEvent(ev, scope))
                            {
                                ExecuteStatement(timed.Stmt, scope, () => { });
                            }
                        }
                    };
                    Kernel.Instance.RegisterProcess(desEvent, sensitivity);
                }
                else
                    switch (proc)
                    {
                        case { ProcedureKind: SvProceduralBlockKind.Initial, Body: not null }:
                        {
                            var desEvent = new DesEvent
                            {
                                Action = () => ExecuteStatement(proc.Body, scope, () => { })
                            };
                            Kernel.Instance.RegisterProcess(desEvent);
                            break;
                        }
                        case { ProcedureKind: SvProceduralBlockKind.Always, Body: not null }:
                        {
                            var desEvent = new DesEvent
                            {
                                Action = Loop
                            };
                            Kernel.Instance.RegisterProcess(desEvent);

                            void Loop() => ExecuteStatement(proc.Body, scope, () => ((Action?)Loop)!());
                            break;
                        }
                        case { ProcedureKind: SvProceduralBlockKind.AlwaysComb, Body: not null }:
                        {
                            var sensitivity = new List<ISignal>();
                            ExtractAllReferencedSignals(proc.Body, sensitivity, scope);
                            
                            // Console.WriteLine($"[DEBUG] always_comb in '{scope}' registered.");
                            // Console.WriteLine($"[DEBUG]   Sensitive to: {string.Join(", ", sensitivity.Select(s => s.Name))}");

                            var desEvent = new DesEvent
                            {
                                Action = () => ExecuteStatement(proc.Body, scope, () => { })
                            };
                            Kernel.Instance.RegisterProcess(desEvent, sensitivity);
                            break;
                        }
                    }

                break;
            }
        }
    }

    private BigInteger Eval(ISvExpression expr, string scope)
    {
        switch (expr)
        {
            case SvConversion conv:
                return Eval(conv.Operand, scope);

            case SvIntegerLiteral lit:
                var literalVal = lit.Value ?? lit.Constant ?? "";
                return ParseVerilogLiteral(literalVal);

            case SvNamedValue nv:
                var sig = ResolveSignal(nv.Symbol, scope);
                return sig?.Read() ?? ResolveParameter(nv.Symbol, scope);

            case SvUnaryOp unary:
                return EvaluateUnary(unary.Op ?? throw new NotSupportedException($"{nameof(unary.Op)} not supported yet"), Eval(unary.Operand, scope));

            case SvBinary binary:
                return EvaluateBinary(binary.Op, Eval(binary.Left, scope), Eval(binary.Right, scope));

            case SvConditionalOp cond:
                var pred = Eval(cond.Conditions?[0].Expr!, scope);
                return pred != 0 ? Eval(cond.Left!, scope) : Eval(cond.Right!, scope);

            case SvCall { Subroutine: not null } call when call.Subroutine.StartsWith('$'):
                switch (call.Subroutine)
                {
                    case "$time" or "$realtime":
                        return new BigInteger(Kernel.Instance.Time);
                    case "$display" or "$write" when call.Arguments is { Length: > 0 }:
                    {
                        var firstArg = call.Arguments[0];
                        var formatStr = ExtractStringLiteral(firstArg);

                        if (formatStr != null)
                        {
                            var printed = FormatSystemOutput(formatStr, call.Arguments.Skip(1).ToArray(), scope);
                            Console.WriteLine(printed);
                        }
                        else
                        {
                            var args = call.Arguments.Select(a => Eval(a, scope).ToString()).ToArray();
                            Console.WriteLine(string.Join(" ", args));
                        }

                        break;
                    }
                    case "$display" or "$write":
                        Console.WriteLine();
                        break;
                    case "$monitor" when call.Arguments is not { Length: > 0 }:
                        return BigInteger.Zero;
                    case "$monitor":
                    {
                        var formatStr = ExtractStringLiteral(call.Arguments[0]);

                        if (formatStr == null) return BigInteger.Zero;
                        var monArgs = call.Arguments.Skip(1).ToList();
                        var lastVals = monArgs.Select(_ => BigInteger.MinusOne).ToList();
                        _monitors.Add((formatStr, monArgs, scope, lastVals));
                        break;
                    }
                    case "$dumpfile":
                    {
                        if (call.Arguments is { Length: > 0 })
                        {
                            _pendingDumpfile = ExtractStringLiteral(call.Arguments[0]);
                        }

                        break;
                    }
                    case "$dumpvars" when call.Arguments is { Length: > 1 }:
                    {
                        string? targetScope = null;
                        var arg = call.Arguments[1];
                        switch (arg)
                        {
                            case SvNamedValue nv:
                                targetScope = CleanSymbolName(nv.Symbol);
                                break;
                            case SvArbitrarySymbol asym:
                                targetScope = CleanSymbolName(asym.Symbol);
                                break;
                            default:
                            {
                                var argStr = arg.ToString() ?? "";
                                var matches = IdentifierWordRegex().Matches(argStr);
                                foreach (var m in matches.Cast<Match>())
                                {
                                    var word = m.Value;
                                    if (word is "SvDataType" or "SvNamedValue" or "Type" or "Kind" or "SvArbitrarySymbol") continue;
                                    targetScope = word;
                                    break;
                                }
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(targetScope))
                        {
                            _vcdFilter = s => s.Name.Contains(targetScope, StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            _vcdFilter = _ => true;
                        }

                        break;
                    }
                    case "$dumpvars":
                        _vcdFilter = _ => true;
                        break;
                    case "$finish":
                        Kernel.Instance.Reset();
                        break;
                }

                return BigInteger.Zero;

            case SvAssignment assign:
                var rVal = Eval(assign.Right, scope);
                if (assign.Left is not SvNamedValue lValue) return rVal;
                var targetSig = ResolveSignal(lValue.Symbol, scope);
                if (targetSig == null) 
                {
                    throw new Exception($"[DEBUG-ERROR] Failed to resolve target signal: {lValue.Symbol} in scope '{scope}'");
                }

                // Console.WriteLine($"[DEBUG] Write: '{targetSig.Name}' <= {rVal} (Time: {Kernel.Instance.Time})");

                if (assign.IsNonBlocking)
                {
                    targetSig.Write(rVal);
                }
                else
                {
                    targetSig.WriteImmediate(rVal);
                }

                return rVal;

            case SvConcatenation { Operands: not null } concat:
                BigInteger result = 0;
                var shift = 0;
                foreach (var op in concat.Operands.Reverse())
                {
                    var v = Eval(op, scope);
                    var width = GetExpressionWidth(op, scope);
                    result |= (v << shift);
                    shift += width;
                }

                return result;

            default:
                throw new NotSupportedException($"{nameof(expr)} not supported yet");
        }
    }

    private void ExecuteStatement(ISvStatement stmt, string scope, Action onComplete)
    {
        switch (stmt)
        {
            case SvList list:
                ExecuteList(list.List, 0, scope, onComplete);
                break;

            case SvBlock block:
                ExecuteStatement(block.Body, scope, onComplete);
                break;

            case SvExpressionStatement { Expr: not null } exprStmt:
                Eval(exprStmt.Expr, scope);
                onComplete();
                break;

            case SvConditional cond:
                var matched = cond.Conditions.All(c => Eval(c.Expr, scope) != 0);
                if (matched)
                {
                    ExecuteStatement(cond.IfTrue, scope, onComplete);
                }
                else if (cond.IfFalse != null)
                {
                    ExecuteStatement(cond.IfFalse, scope, onComplete);
                }
                else
                {
                    onComplete();
                }
                break;

            case SvCase caseStmt:
            {
                var selectVal = Eval(caseStmt.Expr, scope);
                var matchedStmt = (from item in caseStmt.Items where item.Expressions.Any(expr => Eval(expr, scope) == selectVal) select item.Stmt).FirstOrDefault();

                matchedStmt ??= caseStmt.DefaultCase;

                if (matchedStmt != null)
                {
                    ExecuteStatement(matchedStmt, scope, onComplete);
                }
                else
                {
                    onComplete();
                }
                break;
            }

            case SvTimed { Timing: SvDelay delay } timed:
                var delayVal = (ulong)Eval(delay.Expr, scope);
                Kernel.Instance.RegisterWait(delayVal, () => { ExecuteStatement(timed.Stmt, scope, onComplete); });
                break;

            default:
                onComplete();
                break;
        }
    }

    private void ExecuteList(IReadOnlyList<ISvStatement> list, int index, string scope, Action onComplete)
    {
        if (index >= list.Count)
        {
            onComplete();
            return;
        }

        ExecuteStatement(list[index], scope, () => ExecuteList(list, index + 1, scope, onComplete));
    }

    private SvSignal? ResolveSignal(string name, string scope)
    {
        var cleanName = CleanSymbolName(name);
        var key = $"{scope}.{cleanName}";
        while (true)
        {
            if (_portAliases.TryGetValue(key, out var alias))
            {
                key = alias;
                continue;
            }

            if (_signals.TryGetValue(key, out var sig))
            {
                return sig;
            }

            var idx = scope.LastIndexOf('.');
            if (idx < 0) break;
            scope = scope[..idx];
            key = $"{scope}.{cleanName}";
        }

        return null;
    }

    private BigInteger ResolveParameter(string name, string scope)
    {
        var cleanName = CleanSymbolName(name);
        var key = $"{scope}.{cleanName}";
        while (true)
        {
            if (_params.TryGetValue(key, out var val))
            {
                return val;
            }

            var idx = scope.LastIndexOf('.');
            if (idx < 0) break;
            scope = scope[..idx];
            key = $"{scope}.{cleanName}";
        }

        return BigInteger.Zero;
    }

    private static BigInteger EvaluateUnary(SvUnaryOperator op, BigInteger val)
    {
        return op switch
        {
            SvUnaryOperator.Minus => -val,
            SvUnaryOperator.BitwiseNot => ~val,
            SvUnaryOperator.LogicalNot => val == 0 ? 1 : 0,
            _ => throw new NotSupportedException($"{nameof(op)} not supported yet")
        };
    }

    private static BigInteger EvaluateBinary(SvBinaryOperator? op, BigInteger left, BigInteger right)
    {
        if (op == null) return left;
        return op.Value switch
        {
            SvBinaryOperator.Add => left + right,
            SvBinaryOperator.Subtract => left - right,
            SvBinaryOperator.Multiply => left * right,
            SvBinaryOperator.Divide => right == 0 ? 0 : left / right,
            SvBinaryOperator.Mod => right == 0 ? 0 : left % right,
            SvBinaryOperator.BinaryAnd => left & right,
            SvBinaryOperator.BinaryOr => left | right,
            SvBinaryOperator.BinaryXor => left ^ right,
            SvBinaryOperator.Equality => left == right ? 1 : 0,
            SvBinaryOperator.Inequality => left != right ? 1 : 0,
            SvBinaryOperator.GreaterThan => left > right ? 1 : 0,
            SvBinaryOperator.GreaterThanEqual => left >= right ? 1 : 0,
            SvBinaryOperator.LessThan => left < right ? 1 : 0,
            SvBinaryOperator.LessThanEqual => left <= right ? 1 : 0,
            SvBinaryOperator.LogicalAnd => (left != 0 && right != 0) ? 1 : 0,
            SvBinaryOperator.LogicalOr => (left != 0 || right != 0) ? 1 : 0,
            SvBinaryOperator.LogicalShiftLeft => left << (int)right,
            SvBinaryOperator.LogicalShiftRight => left >> (int)right,
            _ => left
        };
    }

    private bool CheckSignalEvent(SvSignalEvent ev, string scope)
    {
        if (ev.Expr is not SvNamedValue nv) return false;
        var sig = ResolveSignal(nv.Symbol, scope);
        if (sig == null) return false;

        return ev.Edge switch
        {
            SvEdgeKind.PosEdge => sig.PosEdge,
            SvEdgeKind.NegEdge => sig.NegEdge,
            _ => sig.Changed
        };
    }

    private void ExtractSensitivity(ISvExpression? expr, List<ISignal> sens, string scope)
    {
        while (true)
        {
            switch (expr)
            {
                case null:
                case SvIntegerLiteral:
                case SvStringLiteral:
                case SvEmptyArgument:
                    return; // Ignore constants

                case SvConversion conv:
                    expr = conv.Operand;
                    continue;

                case SvNamedValue nv:
                    var sig = ResolveSignal(nv.Symbol, scope);
                    if (sig != null) sens.Add(sig);
                    break;

                case SvBinary bin:
                    ExtractSensitivity(bin.Left, sens, scope);
                    expr = bin.Right;
                    continue;

                case SvUnary unary:
                    expr = unary.Operand;
                    continue;

                case SvConcatenation { Operands: not null } concat:
                    foreach (var op in concat.Operands) ExtractSensitivity(op, sens, scope);
                    break;

                case SvAssignment assign:
                    ExtractSensitivity(assign.Left, sens, scope);
                    expr = assign.Right;
                    continue;

                case SvCall call:
                    if (call.Arguments != null)
                        foreach (var arg in call.Arguments) ExtractSensitivity(arg, sens, scope);
                    break;

                case SvConditionalOp condOp:
                    if (condOp.Conditions != null)
                        foreach (var c in condOp.Conditions) ExtractSensitivity(c.Expr, sens, scope);
                    ExtractSensitivity(condOp.Left, sens, scope);
                    expr = condOp.Right;
                    continue;
            }

            break;
        }
    }

    private void ExtractAllReferencedSignals(ISvStatement stmt, List<ISignal> sens, string scope)
    {
        while (true)
        {
            switch (stmt)
            {
                case SvList list:
                    foreach (var s in list.List) ExtractAllReferencedSignals(s, sens, scope);
                    break;
                case SvBlock block:
                    stmt = block.Body;
                    continue;
                case SvExpressionStatement exprStmt:
                    ExtractSensitivity(exprStmt.Expr, sens, scope);
                    break;
                case SvCase caseStmt:
                    ExtractSensitivity(caseStmt.Expr, sens, scope);
                    foreach (var item in caseStmt.Items)
                    {
                        foreach (var e in item.Expressions) ExtractSensitivity(e, sens, scope);
                        ExtractAllReferencedSignals(item.Stmt, sens, scope);
                    }
                    if (caseStmt.DefaultCase != null) ExtractAllReferencedSignals(caseStmt.DefaultCase, sens, scope);
                    break;
                case SvConditional cond:
                    foreach (var c in cond.Conditions) ExtractSensitivity(c.Expr, sens, scope);
                    ExtractAllReferencedSignals(cond.IfTrue, sens, scope);
                    if (cond.IfFalse != null)
                    {
                        stmt = cond.IfFalse;
                        continue;
                    }

                    break;
            }

            break;
        }
    }

    private void ProcessMonitors()
    {
        foreach (var monitor in _monitors)
        {
            var hasChanged = false;
            var currentValues = new List<BigInteger>();

            for (var i = 0; i < monitor.exprs.Count; i++)
            {
                var val = Eval(monitor.exprs[i], monitor.scope);
                currentValues.Add(val);
                if (val != monitor.lastValues[i])
                {
                    hasChanged = true;
                }
            }

            if (!hasChanged) continue;
            {
                for (var i = 0; i < currentValues.Count; i++)
                {
                    monitor.lastValues[i] = currentValues[i];
                }

                var output = FormatSystemOutput(monitor.format, monitor.exprs, monitor.scope);
                Console.WriteLine(output);
            }
        }
    }

    private string FormatSystemOutput(string format, IReadOnlyList<ISvExpression> exprs, string scope)
    {
        var result = format;
        if (exprs.Count == 0) return result;

        var exprIndex = 0;
        var matches = FormatSpecifierRegex().Matches(format);

        foreach (var match in matches.Cast<Match>())
        {
            if (exprIndex >= exprs.Count) break;
            var val = Eval(exprs[exprIndex], scope);
            var specifier = match.Value;

            string replacedVal;
            if (specifier.EndsWith('t'))
            {
                replacedVal = Kernel.Instance.Time.ToString();
            }
            else if (specifier.EndsWith('b'))
            {
                replacedVal = GetBinaryString(val, GetExpressionWidth(exprs[exprIndex], scope));
            }
            else if (specifier.EndsWith('d'))
            {
                replacedVal = val.ToString();
            }
            else if (specifier.EndsWith('h') || specifier.EndsWith('x'))
            {
                replacedVal = val.ToString("X");
            }
            else
            {
                replacedVal = val.ToString();
            }

            var index = result.IndexOf(specifier, StringComparison.Ordinal);
            if (index >= 0)
            {
                result = result.Remove(index, specifier.Length).Insert(index, replacedVal);
            }

            exprIndex++;
        }

        return result;
    }

    private static string GetBinaryString(BigInteger val, int width)
    {
        var sb = new StringBuilder();
        for (var i = width - 1; i >= 0; i--)
        {
            sb.Append(((val >> i) & 1) == 1 ? "1" : "0");
        }

        return sb.ToString();
    }

    private int GetExpressionWidth(ISvExpression expr, string scope)
    {
        if (expr is not SvNamedValue nv) return ParseWidthFromTypeString(expr.Type);
        var sig = ResolveSignal(nv.Symbol, scope);
        return sig?.Width ?? ParseWidthFromTypeString(expr.Type);
    }

    private static int ParseWidthFromTypeString(string typeStr)
    {
        if (string.IsNullOrEmpty(typeStr)) return 1;
        if (typeStr.Contains("int")) return 32;
        if (typeStr.Contains("byte")) return 8;

        var match = PackedRangeRegex().Match(typeStr);
        if (!match.Success) return 1;
        var msb = int.Parse(match.Groups[1].Value);
        var lsb = int.Parse(match.Groups[2].Value);
        return Math.Abs(msb - lsb) + 1;
    }

    private static BigInteger ParseVerilogLiteral(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return BigInteger.Zero;
        if (BigInteger.TryParse(raw, out var simple)) return simple;

        var tickIdx = raw.IndexOf('\'');
        if (tickIdx < 0) return BigInteger.Zero;

        var baseSpec = raw[(tickIdx + 1)..].Trim();
        if (baseSpec.StartsWith("s", StringComparison.OrdinalIgnoreCase)) baseSpec = baseSpec[1..];
        if (baseSpec.Length < 2) return BigInteger.Zero;

        var baseChar = char.ToLowerInvariant(baseSpec[0]);
        var digits = baseSpec[1..].Replace("_", "");

        try
        {
            return baseChar switch
            {
                'b' => ParseBasedBigInt(digits, 2),
                'o' => ParseBasedBigInt(digits, 8),
                'd' => BigInteger.Parse(digits),
                'h' => ParseBasedBigInt(digits, 16),
                _ => BigInteger.Zero
            };
        }
        catch
        {
            return BigInteger.Zero;
        }
    }

    private static BigInteger ParseBasedBigInt(string digits, int radix)
    {
        BigInteger res = 0;
        foreach (var c in digits)
        {
            var val = c switch
            {
                >= '0' and <= '9' => c - '0',
                >= 'a' and <= 'f' => 10 + (c - 'a'),
                >= 'A' and <= 'F' => 10 + (c - 'A'),
                _ => 0
            };
            res = res * radix + val;
        }
        return res;
    }

    private static string? ExtractStringLiteral(ISvExpression? expr)
    {
        while (expr != null)
        {
            switch (expr)
            {
                case SvStringLiteral strLit:
                    return strLit.Literal ?? strLit.Constant;
                case SvConversion conv:
                    expr = conv.Operand;
                    continue;
            }

            break;
        }

        return null;
    }

    private static string CleanSymbolName(string rawSymbol)
    {
        if (string.IsNullOrEmpty(rawSymbol)) return rawSymbol;
        var spaceIdx = rawSymbol.IndexOf(' ');
        return spaceIdx >= 0 ? rawSymbol[(spaceIdx + 1)..] : rawSymbol;
    }

    private static string? GetParentSymbolFromExpr(ISvExpression? expr)
    {
        while (expr is SvConversion conv)
        {
            expr = conv.Operand;
        }

        return expr switch
        {
            null => null,
            SvNamedValue nv => nv.Symbol,
            SvAssignment { Left: SvNamedValue leftNv } => leftNv.Symbol,
            _ => null
        };
    }

    [GeneratedRegex(@"\[\s*(\d+)\s*:\s*(\d+)\s*\]")]
    private static partial Regex PackedRangeRegex();

    [GeneratedRegex(@"%[0-9]*[tbdehx]")]
    private static partial Regex FormatSpecifierRegex();

    [GeneratedRegex(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b")]
    private static partial Regex IdentifierWordRegex();
}