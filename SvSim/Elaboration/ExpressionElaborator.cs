using System.Numerics;
using SvSim.Simulation.Engine;
using SvSim.SlangAstParser.AstTree;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Signal;
using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.Expressions;
using SvSim.SlangAstParser.AstTree.SvEnums;

namespace SvSim.Elaboration;

public class ExpressionElaborator(EventScheduler scheduler)
{
    private readonly Dictionary<long, object> _signalSymbolTable = new();

    public HashSet<ISimEventSource> Dependencies { get; } = [];

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

    public IExpression<SimLogic<T>> ElaborateExpression<T>(IKind astNode) where T : IBinaryInteger<T>
    {
        return astNode switch
        {
            SvIntegerLiteral literal => new LiteralExpr<SimLogic<T>>(ParseSlangInt<T>(literal.Value)),
            SvUnbasedUnsizedIntegerLiteral unsized => new LiteralExpr<SimLogic<T>>(ParseSlangInt<T>(unsized.Value)),
            SvStringLiteral strLit => new LiteralExpr<SimLogic<T>>(ParseStringLiteral<T>(strLit.Literal)),
            SvConversion conv => ElaborateExpression<T>(conv.Operand!),
            SvNamedValue namedVal => ElaborateSymbolLookup<T>(namedVal.Symbol!),
            SvHierarchicalValue hv => ElaborateSymbolLookup<T>(hv.Symbol!),
            SvMemberAccess memAcc => ElaborateMemberAccess<T>(memAcc),
            SvArbitrarySymbol arbSym => ElaborateSymbolLookup<T>(arbSym.Symbol!),
            SvBinaryOp binOp => ElaborateBinaryOp<T>(binOp),
            SvUnaryOp unaryOp => ElaborateUnaryOp<T>(unaryOp),
            SvCall callAst => ElaborateSystemCall<T>(callAst),
            SvRangeSelect rs => new SliceReadExpr<T>(ResolveSignal(rs.Value!), EvaluateConstantInt(rs.Left!),
                EvaluateConstantInt(rs.Right!)),
            SvElementSelect es => ResolveElementSelect<T>(es),
            _ => throw new NotImplementedException($"AST Node {astNode.GetType().Name} not supported yet.")
        };
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

    private IExpression<SimLogic<T>> ResolveElementSelect<T>(SvElementSelect es) where T : IBinaryInteger<T>
    {
        var container = ResolveRawObject(es.Value!);
        var indexExpr = ElaborateExpression<BigInteger>(es.Selector!);

        return new ArrayReadExpr<T>(container, indexExpr);
    }

    private IExpression<SimLogic<T>> ElaborateSymbolLookup<T>(string symbolId) where T : IBinaryInteger<T>
    {
        var addr = ExtractId(symbolId);
        var obj = GetSignal(addr);

        if (obj is null)
        {
            Console.WriteLine($"[Warning] Symbol ID {addr} ({symbolId}) not found. Defaulting to 0.");
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
                throw new Exception($"Symbol {symbolId} type {obj.GetType().Name} is not supported in expressions.");
        }
    }

    public static int EvaluateConstantInt(IKind node)
    {
        if (node is SvIntegerLiteral lit) return int.Parse(lit.Value!);
        throw new NotImplementedException("Dynamic indices not supported yet.");
    }

    public static string ExtractCleanTypeName(string? typeStr)
    {
        if (string.IsNullOrEmpty(typeStr)) return string.Empty;
        var parts = typeStr.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var name = parts[0];

        if (long.TryParse(name, out _) && parts.Length > 1)
        {
            name = parts[1];
        }

        if (name.Contains("::"))
        {
            name = name[(name.LastIndexOf("::", StringComparison.Ordinal) + 2)..];
        }

        var bracketIndex = name.IndexOf("$[", StringComparison.Ordinal);
        if (bracketIndex != -1)
        {
            name = name[..bracketIndex];
        }

        return name;
    }

    public object ResolveRawObject(IKind node)
    {
        var addr = node switch
        {
            SvNamedValue nv => ExtractId(nv.Symbol),
            SvHierarchicalValue hv => ExtractId(hv.Symbol),
            _ => 0
        };

        var obj = GetSignal(addr);
        if (obj is not null) return obj;

        if (node is SvElementSelect es)
        {
            var container = ResolveRawObject(es.Value!);
            var indexExpr = ElaborateExpression<BigInteger>(es.Selector!);
            return new DynamicElementSignal(container, indexExpr);
        }

        if (node is not SvMemberAccess ma)
            return obj ?? throw new Exception($"Could not resolve raw object for {node.GetType().Name}.");

        var parent = ResolveRawObject(ma.Value!);
        var memberName = ExtractName(ma.Member);

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

    public ISimLogicSignal ResolveSignal(IKind node)
    {
        var addr = node switch
        {
            SvNamedValue nv => ExtractId(nv.Symbol),
            SvHierarchicalValue hv => ExtractId(hv.Symbol),
            _ => 0
        };
        if (addr != 0 && GetSignal(addr) is ISimLogicSignal sig) return sig;

        if (node is SvElementSelect es)
        {
            var container = ResolveRawObject(es.Value!);
            var indexExpr = ElaborateExpression<BigInteger>(es.Selector!);
            return new DynamicElementSignal(container, indexExpr);
        }

        if (node is not SvMemberAccess ma)
            throw new Exception($"Could not resolve signal source for {node.GetType().Name}.");

        var containerObj = ResolveRawObject(ma.Value!);
        var memberName = ExtractName(ma.Member);

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
        switch (callAst.Subroutine)
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
            default:
                throw new NotImplementedException($"System function {callAst.Subroutine} not implemented.");
        }
    }

    private BinaryOpExpr<SimLogic<T>> ElaborateBinaryOp<T>(SvBinaryOp binOp)
        where T : IBinaryInteger<T>
    {
        var left = ElaborateExpression<T>(binOp.Left!);
        var right = ElaborateExpression<T>(binOp.Right!);

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
        BigInteger val = 0;
        foreach (var b in bytes)
        {
            val = (val << 8) | b;
        }

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