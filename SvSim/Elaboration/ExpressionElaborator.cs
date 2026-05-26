using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SvSim.Simulation.Engine;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Signal;
using SvSim.SlangAstParser.AstTree;
using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.Expression.AssignmentPatternExpression;
using SvSim.SlangAstParser.AstTree.Expression.ValueExpressionBase;
using SvSim.SlangAstParser.AstTree.SvEnums;

namespace SvSim.Elaboration;

public class ExpressionElaborator(EventScheduler scheduler)
{
    private readonly Dictionary<long, object> _signalSymbolTable = new();

    // Virtual Object Heap representing dynamic class allocations at runtime
    private static readonly Dictionary<long, ClassInstanceVar> ObjectHeap = new();
    private static int _nextObjectHandle = 1;

    public HashSet<ISimEventSource> Dependencies { get; } = [];

    public static ClassInstanceVar? GetObject(long handle) => ObjectHeap.GetValueOrDefault(handle);

    public static long ExtractId(string? slangSymbolId)
    {
        if (string.IsNullOrEmpty(slangSymbolId)) return 0;
        var firstPart = slangSymbolId.Split(' ')[0];
        return long.TryParse(firstPart, out var id) ? id : 0;
    }

    public static string ExtractName(string? slangSymbolName)
    {
        if (string.IsNullOrEmpty(slangSymbolName)) return string.Empty;
        var parts = slangSymbolName.Trim().Split(' ');
        return parts.Length > 1 ? parts[^1] : parts[0];
    }

    public void RegisterSignal(long addr, object simVar) => _signalSymbolTable[addr] = simVar;

    public object? GetSignal(long addr) => _signalSymbolTable.GetValueOrDefault(addr);
    public void ClearDependencies() => Dependencies.Clear();

    public IExpression<SimLogic<T>> ElaborateExpression<T>(ISvExpression astNode) where T : IBinaryInteger<T>
    {
        return astNode switch
        {
            SvIntegerLiteral literal => new LiteralExpr<SimLogic<T>>(ParseSlangInt<T>(literal.Value)),
            SvUnbasedUnsizedIntegerLiteral unsized => new LiteralExpr<SimLogic<T>>(ParseSlangInt<T>(unsized.Value)),
            SvStringLiteral strLit => new LiteralExpr<SimLogic<T>>(ParseStringLiteral<T>(strLit.Literal)),
            SvConversion conv => ElaborateExpression<T>(conv.Operand!),
            SvNamedValue namedVal => ElaborateSymbolLookup<T>(namedVal),
            SvHierarchicalValue hv => ElaborateSymbolLookup<T>(hv),
            SvMemberAccess memAcc => ElaborateMemberAccess<T>(memAcc),
            SvArbitrarySymbol arbSym => ElaborateSymbolLookupByName<T>(arbSym.Symbol),
            SvBinary binOp => ElaborateBinaryOp<T>(binOp),
            SvUnaryOp unaryOp => ElaborateUnaryOp<T>(unaryOp),
            SvCall callAst => ElaborateSystemCall<T>(callAst),
            SvRangeSelect rs => new SliceReadExpr<T>(ResolveSignal(rs.Value), EvaluateConstantInt(rs.Left),
                EvaluateConstantInt(rs.Right)),
            SvElementSelect es => ResolveElementSelect<T>(es),
            SvConditionalOp condOp => ElaborateConditionalOp<T>(condOp),
            SvNewClass newClass => ElaborateNewClass<T>(newClass),
            SvNewCovergroup => ElaborateNewCovergroup<T>(),
            SvNullLiteral => new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.Zero, T.Zero)),
            SvDataType => new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.Zero, T.Zero)),
            SvStructuredAssignmentPattern structPattern => ElaborateStructuredAssignmentPattern<T>(structPattern),
            _ => throw new NotImplementedException(
                $"AST Expression Node {astNode.GetType().Name} is not supported yet.")
        };
    }

    private IExpression<SimLogic<T>> ElaborateNewClass<T>(SvNewClass newClass) where T : IBinaryInteger<T>
    {
        var handle = System.Threading.Interlocked.Increment(ref _nextObjectHandle);
        var typeName = string.IsNullOrEmpty(newClass.Type) ? "anonymous_class" : newClass.Type;

        var instance = new ClassInstanceVar(typeName);
        ObjectHeap[handle] = instance;

        return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.CreateTruncating(handle), T.Zero));
    }

    private IExpression<SimLogic<T>> ElaborateNewCovergroup<T>() where T : IBinaryInteger<T>
    {
        var handle = System.Threading.Interlocked.Increment(ref _nextObjectHandle);
        var instance = new ClassInstanceVar("covergroup");
        ObjectHeap[handle] = instance;

        return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.CreateTruncating(handle), T.Zero));
    }

    private IExpression<SimLogic<T>> ElaborateStructuredAssignmentPattern<T>(SvStructuredAssignmentPattern pattern)
        where T : IBinaryInteger<T>
    {
        var cleanTypeName = ExtractCleanTypeName(pattern.Type);
        BigInteger finalVal = 0;
        BigInteger finalUnk = 0;

        if (TypeRegistry.TryGetType(cleanTypeName, out var def))
        {
            foreach (var field in def.Fields)
            {
                var fieldName = field.Key;
                var slice = field.Value;

                var setter = pattern.MemberSetters?.FirstOrDefault(s => s.Member == fieldName);
                if (setter?.Expr != null)
                {
                    var evaluated = ElaborateExpression<BigInteger>(setter.Expr).Evaluate();
                    var mask = (BigInteger.One << (slice.Msb - slice.Lsb + 1)) - 1;

                    finalVal |= (evaluated.Value & mask) << slice.Lsb;
                    finalUnk |= (evaluated.Unknown & mask) << slice.Lsb;
                }
            }
        }

        return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.CreateTruncating(finalVal),
            T.CreateTruncating(finalUnk)));
    }

    private IExpression<SimLogic<T>> ElaborateConditionalOp<T>(SvConditionalOp condOp) where T : IBinaryInteger<T>
    {
        var cond = ElaborateExpression<T>(condOp.Conditions![0].Expr);
        var left = ElaborateExpression<T>(condOp.Left!);
        var right = ElaborateExpression<T>(condOp.Right!);
        return new ConditionalOpExpr<T>(cond, left, right);
    }

    private UnaryOpExpr<SimLogic<T>> ElaborateUnaryOp<T>(SvUnaryOp unaryOp) where T : IBinaryInteger<T>
    {
        var operand = ElaborateExpression<T>(unaryOp.Operand!);

        return unaryOp.Op switch
        {
            SvUnaryOperator.Plus => new UnaryOpExpr<SimLogic<T>>(operand, val => val),
            SvUnaryOperator.Minus => new UnaryOpExpr<SimLogic<T>>(operand,
                val => new SimLogic<T>(T.Zero - val.Value, val.Unknown)),

            SvUnaryOperator.BitwiseNot => new UnaryOpExpr<SimLogic<T>>(operand, val => ~val),

            SvUnaryOperator.LogicalNot => new UnaryOpExpr<SimLogic<T>>(operand, val =>
                new SimLogic<T>(val.Value == T.Zero && val.Unknown == T.Zero ? T.One : T.Zero, T.Zero)),

            SvUnaryOperator.BitwiseOr => new UnaryOpExpr<SimLogic<T>>(operand, val =>
                new SimLogic<T>(val.Value != T.Zero ? T.One : T.Zero, val.Unknown != T.Zero ? T.One : T.Zero)),

            SvUnaryOperator.BitwiseAnd => new UnaryOpExpr<SimLogic<T>>(operand, val =>
            {
                var allOn = (val.Value == T.AllBitsSet);
                return new SimLogic<T>(allOn ? T.One : T.Zero, val.Unknown != T.Zero ? T.One : T.Zero);
            }),

            SvUnaryOperator.BitwiseXor => new UnaryOpExpr<SimLogic<T>>(operand, val =>
            {
                var count = int.CreateTruncating(T.PopCount(val.Value));
                return new SimLogic<T>((count % 2 != 0) ? T.One : T.Zero, val.Unknown != T.Zero ? T.One : T.Zero);
            }),

            SvUnaryOperator.BitwiseNand => new UnaryOpExpr<SimLogic<T>>(operand, val =>
            {
                var res = val.Value == T.AllBitsSet ? T.One : T.Zero;
                return new SimLogic<T>(res == T.Zero ? T.One : T.Zero, val.Unknown);
            }),
            SvUnaryOperator.BitwiseNor => new UnaryOpExpr<SimLogic<T>>(operand, val =>
                new SimLogic<T>(val.Value == T.Zero ? T.One : T.Zero, val.Unknown)),
            SvUnaryOperator.BitwiseXnor => new UnaryOpExpr<SimLogic<T>>(operand, val =>
            {
                var count = int.CreateTruncating(T.PopCount(val.Value));
                return new SimLogic<T>((count % 2 == 0) ? T.One : T.Zero, val.Unknown);
            }),

            SvUnaryOperator.Preincrement or SvUnaryOperator.Postincrement =>
                new UnaryOpExpr<SimLogic<T>>(operand, val => new SimLogic<T>(val.Value + T.One, val.Unknown)),

            SvUnaryOperator.Predecrement or SvUnaryOperator.Postdecrement =>
                new UnaryOpExpr<SimLogic<T>>(operand, val => new SimLogic<T>(val.Value - T.One, val.Unknown)),

            _ => throw new NotImplementedException($"Unary Operator {unaryOp.Op} not supported.")
        };
    }

    private ArrayReadExpr<T> ResolveElementSelect<T>(SvElementSelect es) where T : IBinaryInteger<T>
    {
        var container = ResolveRawObject(es.Value);
        var indexExpr = ElaborateExpression<BigInteger>(es.Selector);

        return new ArrayReadExpr<T>(container, indexExpr);
    }

    private IExpression<SimLogic<T>> ElaborateSymbolLookup<T>(ISvValueExpressionBase symbolNode)
        where T : IBinaryInteger<T>
    {
        var addr = symbolNode.ResolvedSymbol?.Addr ?? ExtractId(symbolNode.Symbol);
        return ElaborateSymbolLookupById<T>(addr, symbolNode.Symbol);
    }

    private IExpression<SimLogic<T>> ElaborateSymbolLookupByName<T>(string symbolStr) where T : IBinaryInteger<T>
    {
        var addr = ExtractId(symbolStr);
        return ElaborateSymbolLookupById<T>(addr, symbolStr);
    }

    private IExpression<SimLogic<T>> ElaborateSymbolLookupById<T>(long addr, string rawSymbolName)
        where T : IBinaryInteger<T>
    {
        var obj = GetSignal(addr);

        if (obj is null)
        {
            Console.WriteLine($"[Warning] Symbol ID {addr} ({rawSymbolName}) not found. Defaulting to 0.");
            return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.Zero, T.Zero));
        }

        switch (obj)
        {
            case ISimLogicSignal sig:
                Dependencies.Add(sig);
                return new SignalCastReadExpr<T>(sig);
            case SimLogic<BigInteger> bigConst:
            {
                var converted = new SimLogic<T>(
                    T.CreateTruncating(bigConst.Value),
                    T.CreateTruncating(bigConst.Unknown)
                );
                return new LiteralExpr<SimLogic<T>>(converted);
            }
            case BigInteger rawInt:
                return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(
                    T.CreateTruncating(rawInt),
                    T.Zero
                ));
            default:
                throw new Exception(
                    $"Symbol {rawSymbolName} type {obj.GetType().Name} is not supported in expressions.");
        }
    }

    public static int EvaluateConstantInt(ISvExpression node)
    {
        if (node is SvIntegerLiteral lit) return int.Parse(lit.Value!);
        throw new NotImplementedException("Dynamic indices not supported yet.");
    }

    public static string ExtractCleanTypeName(string? typeStr)
    {
        if (string.IsNullOrEmpty(typeStr)) return string.Empty;
        var name = typeStr.Trim();

        if (name.Contains("::"))
        {
            name = name[(name.LastIndexOf("::", StringComparison.Ordinal) + 2)..];
        }

        var lastCloseBrace = name.LastIndexOf('}');
        if (lastCloseBrace != -1 && lastCloseBrace < name.Length - 1)
        {
            name = name[(lastCloseBrace + 1)..].Trim();
        }

        var dollarIndex = name.IndexOf('$');
        if (dollarIndex != -1)
        {
            name = name[..dollarIndex];
        }

        return name;
    }

    public object ResolveRawObject(ISvExpression node)
    {
        var addr = node switch
        {
            ISvValueExpressionBase { ResolvedSymbol: not null } valExpr => valExpr.ResolvedSymbol.Addr,
            SvNamedValue nv => ExtractId(nv.Symbol),
            SvHierarchicalValue hv => ExtractId(hv.Symbol),
            _ => 0
        };

        var obj = GetSignal(addr);
        if (obj is not null) return obj;

        if (node is SvElementSelect es)
        {
            var container = ResolveRawObject(es.Value);
            var indexExpr = ElaborateExpression<BigInteger>(es.Selector);
            return new DynamicElementSignal(container, indexExpr);
        }

        if (node is not SvMemberAccess ma)
            return obj ?? throw new Exception($"Could not resolve raw object for {node.GetType().Name}.");

        var parent = ResolveRawObject(ma.Value!);
        var memberName = ExtractName(ma.Member);

        if (parent is ISimLogicSignal handleSig and not DynamicElementSignal and not SliceSignal and not PackedStructVar
            and not PackedUnionVar)
        {
            try
            {
                var handle = (long)handleSig.GetValueAsBigInt();
                var classObj = GetObject(handle);
                if (classObj != null)
                {
                    return classObj.GetProperty(memberName);
                }
            }
            catch
            {
                // Elaboration-time safe fallback
            }
        }

        switch (parent)
        {
            case HierarchicalScope scope when scope.Signals.TryGetValue(memberName, out var sig):
                return sig;
            case HierarchicalScope scope when scope.Children.TryGetValue(memberName, out var child):
                return child;
            case UnpackedStructVar structVar when structVar.Members.TryGetValue(memberName, out var memberSig):
                return memberSig;
        }

        if (parent is not IStructSignal structSig || !TypeRegistry.TryGetType(structSig.StructTypeName, out var def))
            throw new Exception($"Could not resolve member '{memberName}' in container.");

        var (msb, lsb, subType) = def.Fields[memberName];
        return new SliceSignal(structSig, msb, lsb) { StructTypeName = subType };
    }

    public ISimLogicSignal ResolveSignal(ISvExpression node)
    {
        var addr = node switch
        {
            ISvValueExpressionBase { ResolvedSymbol: not null } valExpr => valExpr.ResolvedSymbol.Addr,
            SvNamedValue nv => ExtractId(nv.Symbol),
            SvHierarchicalValue hv => ExtractId(hv.Symbol),
            _ => 0
        };
        if (addr != 0 && GetSignal(addr) is ISimLogicSignal sig) return sig;

        if (node is SvElementSelect es)
        {
            var container = ResolveRawObject(es.Value);
            var indexExpr = ElaborateExpression<BigInteger>(es.Selector);
            return new DynamicElementSignal(container, indexExpr);
        }

        if (node is not SvMemberAccess ma)
            throw new Exception($"Could not resolve signal source for {node.GetType().Name}.");

        var containerObj = ResolveRawObject(ma.Value!);
        var memberName = ExtractName(ma.Member);

        if (containerObj is ISimLogicSignal handleSig and not DynamicElementSignal and not SliceSignal
            and not PackedStructVar and not PackedUnionVar)
        {
            try
            {
                var handle = (long)handleSig.GetValueAsBigInt();
                var classObj = GetObject(handle);
                if (classObj != null)
                {
                    return classObj.GetProperty(memberName);
                }
            }
            catch
            {
                // Elaboration-time safe fallback
            }
        }

        switch (containerObj)
        {
            case HierarchicalScope scope when scope.Signals.TryGetValue(memberName, out var s):
                return (ISimLogicSignal)s;
            case UnpackedStructVar structVar when structVar.Members.TryGetValue(memberName, out var memberSig):
                return memberSig;
            case IStructSignal structSig when TypeRegistry.TryGetType(structSig.StructTypeName, out var def):
            {
                var (msb, lsb, subType) = def.Fields[memberName];
                return new SliceSignal(structSig, msb, lsb) { StructTypeName = subType };
            }
            default:
                throw new Exception($"Could not resolve signal source for member access: {memberName}.");
        }
    }

    private SignalCastReadExpr<T> ElaborateMemberAccess<T>(SvMemberAccess memAcc) where T : IBinaryInteger<T>
    {
        var sig = ResolveSignal(memAcc);
        Dependencies.Add(sig);
        return new SignalCastReadExpr<T>(sig);
    }

    private IExpression<SimLogic<T>> ElaborateSystemCall<T>(SvCall callAst)
        where T : IBinaryInteger<T>
    {
        var cleanSubroutine = ExtractName(callAst.Subroutine);

        switch (cleanSubroutine)
        {
            case "size":
            {
                var targetObj = ResolveRawObject(callAst.Arguments![0]);
                return new ArraySizeExpr<T>(targetObj);
            }
            case "exists":
            {
                var targetObj = ResolveRawObject(callAst.Arguments![0]);
                var keyExpr = ElaborateExpression<BigInteger>(callAst.Arguments![1]);
                return new ArrayExistsExpr<T>(targetObj, keyExpr);
            }
            case "$time":
                return new TimeExpr<T>(scheduler);
            case "$urandom":
                return new URandomExpr<T>();
            case "$urandom_range":
            {
                var min = ElaborateExpression<T>(callAst.Arguments![0]);
                var max = ElaborateExpression<T>(callAst.Arguments![1]);
                return new URandomRangeExpr<T>(min, max);
            }
            case "name":
            {
                var instance = callAst.ThisClass;

                if (instance == null && callAst.Arguments is { Length: > 0 })
                {
                    instance = callAst.Arguments[0];
                }

                if (instance == null)
                    throw new Exception($".name() called on null instance. Subroutine: {callAst.Subroutine}");

                var sig = ResolveSignal(instance);
                return new EnumNameExpr<T>(sig);
            }
            case "randomize":
            {
                if (callAst.ThisClass == null) return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.One, T.Zero));
                var targetObj = ResolveRawObject(callAst.ThisClass);
                if (targetObj is not ISimLogicSignal handleSig)
                    return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.One, T.Zero));
                var handle = (long)handleSig.GetValueAsBigInt();
                var classObj = GetObject(handle);
                if (classObj is null) return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.One, T.Zero));
                foreach (var prop in classObj.Properties.Values)
                {
                    var randVal = SvSim.Simulation.RandomGenerator.RandomGen.Random.Next();
                    prop.AssignFromBigInteger(randVal, 0);
                }

                return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.One, T.Zero));
            }
            case "get_inst_coverage":
                return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.CreateTruncating(85), T.Zero));
            case "$isunknown":
                if (callAst.Arguments is not { Length: > 0 })
                    return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.Zero, T.Zero));
                var arg = ElaborateExpression<BigInteger>(callAst.Arguments[0]).Evaluate();
                var isUnk = arg.Unknown != 0;
                return new LiteralExpr<SimLogic<T>>(new SimLogic<T>(isUnk ? T.One : T.Zero, T.Zero));
            default:
                throw new NotImplementedException($"System function {callAst.Subroutine} not implemented.");
        }
    }

    private BinaryOpExpr<SimLogic<T>> ElaborateBinaryOp<T>(SvBinary binOp)
        where T : IBinaryInteger<T>
    {
        var left = ElaborateExpression<T>(binOp.Left);
        var right = ElaborateExpression<T>(binOp.Right);

        Func<SimLogic<T>, SimLogic<T>, SimLogic<T>> operation = binOp.Op switch
        {
            SvBinaryOperator.Add => (l, r) => l + r,
            SvBinaryOperator.Subtract => (l, r) => l - r,
            SvBinaryOperator.Multiply => (l, r) => l * r,
            SvBinaryOperator.Divide => (l, r) => r.Value == T.Zero
                ? new SimLogic<T>(T.Zero, T.AllBitsSet)
                : new SimLogic<T>(l.Value / r.Value, l.Unknown | r.Unknown),
            SvBinaryOperator.Mod => (l, r) => r.Value == T.Zero
                ? new SimLogic<T>(T.Zero, T.AllBitsSet)
                : new SimLogic<T>(l.Value % r.Value, l.Unknown | r.Unknown),
            SvBinaryOperator.Power => (l, r) =>
            {
                var res = T.One;
                for (var i = T.Zero; i < r.Value; i++) res *= l.Value;
                return new SimLogic<T>(res, l.Unknown | r.Unknown);
            },

            SvBinaryOperator.BinaryAnd => (l, r) => l & r,
            SvBinaryOperator.BinaryOr => (l, r) => l | r,
            SvBinaryOperator.BinaryXor => (l, r) => l ^ r,
            SvBinaryOperator.BinaryXnor => (l, r) => ~(l ^ r),

            SvBinaryOperator.LogicalShiftLeft => (l, r) => l << int.CreateTruncating(r.Value),
            SvBinaryOperator.LogicalShiftRight => (l, r) => l >> int.CreateTruncating(r.Value),
            SvBinaryOperator.ArithmeticShiftLeft => (l, r) => l << int.CreateTruncating(r.Value),
            SvBinaryOperator.ArithmeticShiftRight => (l, r) =>
                l.ArithmeticRightShift(int.CreateTruncating(r.Value), 32),
            SvBinaryOperator.Equality => (l, r) => new SimLogic<T>(l == r ? T.One : T.Zero, T.Zero),
            SvBinaryOperator.Inequality => (l, r) => new SimLogic<T>(l != r ? T.One : T.Zero, T.Zero),
            SvBinaryOperator.CaseEquality => (l, r) => new SimLogic<T>(l == r ? T.One : T.Zero, T.Zero),
            SvBinaryOperator.CaseInequality => (l, r) => new SimLogic<T>(l != r ? T.One : T.Zero, T.Zero),
            SvBinaryOperator.GreaterThan => (l, r) => new SimLogic<T>(l.Value > r.Value ? T.One : T.Zero, T.Zero),
            SvBinaryOperator.GreaterThanEqual => (l, r) => new SimLogic<T>(l.Value >= r.Value ? T.One : T.Zero, T.Zero),
            SvBinaryOperator.LessThan => (l, r) => new SimLogic<T>(l.Value < r.Value ? T.One : T.Zero, T.Zero),
            SvBinaryOperator.LessThanEqual => (l, r) => new SimLogic<T>(l.Value <= r.Value ? T.One : T.Zero, T.Zero),

            SvBinaryOperator.LogicalAnd => (l, r) =>
                new SimLogic<T>((l.Value != T.Zero && r.Value != T.Zero) ? T.One : T.Zero, T.Zero),
            SvBinaryOperator.LogicalOr => (l, r) =>
                new SimLogic<T>((l.Value != T.Zero || r.Value != T.Zero) ? T.One : T.Zero, T.Zero),
            SvBinaryOperator.LogicalImplication => (l, r) =>
                new SimLogic<T>((l.Value == T.Zero || r.Value != T.Zero) ? T.One : T.Zero, T.Zero),
            SvBinaryOperator.LogicalEquivalence => (l, r) =>
                new SimLogic<T>((l.Value == r.Value) ? T.One : T.Zero, T.Zero),

            SvBinaryOperator.WildcardEquality => (l, r) => new SimLogic<T>(l == r ? T.One : T.Zero, T.Zero),
            SvBinaryOperator.WildcardInequality => (l, r) => new SimLogic<T>(l != r ? T.One : T.Zero, T.Zero),

            _ => throw new NotImplementedException($"Operator {binOp.Op} not supported.")
        };

        return new BinaryOpExpr<SimLogic<T>>(left, right, operation);
    }

    private static SimLogic<T> ParseStringLiteral<T>(string? text) where T : IBinaryInteger<T>
    {
        if (string.IsNullOrEmpty(text)) return new SimLogic<T>(T.Zero, T.Zero);
        var cleanText = text.Trim('"');
        var bytes = System.Text.Encoding.ASCII.GetBytes(cleanText);
        var val = bytes.Aggregate<byte, BigInteger>(0, (current, b) => (current << 8) | b);

        return new SimLogic<T>(T.CreateTruncating(val), T.Zero);
    }

    private static SimLogic<T> ParseSlangInt<T>(string? slangValue) where T : IBinaryInteger<T>
    {
        if (string.IsNullOrEmpty(slangValue)) return new SimLogic<T>(T.Zero, T.Zero);

        var parts = slangValue.Split('\'');
        if (parts.Length != 2)
            return new SimLogic<T>(T.CreateTruncating(BigInteger.TryParse(slangValue, out var d) ? d : 0), T.Zero);

        var baseChar = parts[1][0];
        var numStr = parts[1][1..].Replace("_", "");

        BigInteger value = 0;
        BigInteger unknown = 0;

        switch (baseChar)
        {
            case 'h':
                value = BigInteger.Parse(numStr, System.Globalization.NumberStyles.HexNumber);
                break;
            case 'd':
                value = BigInteger.Parse(numStr);
                break;
            case 'b':
                foreach (var c in numStr)
                {
                    value <<= 1;
                    unknown <<= 1;
                    switch (c)
                    {
                        case '1':
                            value |= 1;
                            break;
                        case 'x' or 'X' or 'z' or 'Z':
                            unknown |= 1;
                            break;
                    }
                }

                break;
        }

        return new SimLogic<T>(T.CreateTruncating(value), T.CreateTruncating(unknown));
    }

    public static SimLogic<BigInteger> ParseSlangIntToBigInt(string? slangValue)
    {
        return ParseSlangInt<BigInteger>(slangValue);
    }
}