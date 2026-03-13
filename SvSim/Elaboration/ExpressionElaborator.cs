using System.Numerics;
using SvSim.Simulation.Engine;
using SvSim.SlangAstParser.AstTree;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Signal;

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

    public void RegisterSignal(long addr, object simVar) => _signalSymbolTable[addr] = simVar;

    public object? GetSignal(long addr) => _signalSymbolTable.GetValueOrDefault(addr);
    public void ClearDependencies() => Dependencies.Clear();

    public IExpression<SimLogic<T>> ElaborateExpression<T>(IKind astNode) where T : IBinaryInteger<T>
    {
        return astNode switch
        {
            SvIntegerLiteral literal => new LiteralExpr<SimLogic<T>>(ParseSlangInt<T>(literal.Value)),
            SvUnbasedUnsizedIntegerLiteral unsized => new LiteralExpr<SimLogic<T>>(ParseSlangInt<T>(unsized.Value)),
            SvConversion conv => ElaborateExpression<T>(conv.Operand!),
            SvNamedValue namedVal => ElaborateSymbolLookup<T>(namedVal.Symbol!),
            SvHierarchicalValue hv => ElaborateSymbolLookup<T>(hv.Symbol!), 
            SvMemberAccess memAcc => ElaborateMemberAccess<T>(memAcc),
            SvArbitrarySymbol arbSym => ElaborateSymbolLookup<T>(arbSym.Symbol!),
            SvBinaryOp binOp => ElaborateBinaryOp<T>(binOp),
            SvUnaryOp unaryOp => ElaborateUnaryOp<T>(unaryOp),
            SvCall callAst => ElaborateSystemCall<T>(callAst),
            SvRangeSelect rs => new SliceReadExpr<T>(ResolveSignal(rs.Value!), EvaluateConstantInt(rs.Left!), EvaluateConstantInt(rs.Right!)),
            SvElementSelect es => new SliceReadExpr<T>(ResolveSignal(es.Value!), EvaluateConstantInt(es.Selector!), EvaluateConstantInt(es.Selector!)),
            _ => throw new NotImplementedException($"AST Node {astNode.GetType().Name} not supported yet.")
        };
    }
    
    private UnaryOpExpr<SimLogic<T>> ElaborateUnaryOp<T>(SvUnaryOp unaryOp) where T : IBinaryInteger<T>
    {
        var operand = ElaborateExpression<T>(unaryOp.Operand!);
        return unaryOp.Op switch
        {
            "BitwiseNot" => new UnaryOpExpr<SimLogic<T>>(operand, val => ~val),
            "Minus" => new UnaryOpExpr<SimLogic<T>>(operand, val => new SimLogic<T>(-val.Value, val.Unknown)),
            _ => throw new NotImplementedException($"Unary Operator {unaryOp.Op} not supported.")
        };
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
            default:
                throw new Exception($"Symbol {symbolId} type {obj.GetType().Name} is not supported in expressions.");
        }
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
        if (node is not SvMemberAccess ma)
            throw new Exception($"Could not resolve signal source for {node.GetType().Name}.");
        var container = ResolveRawObject(ma.Value!);
        if (container is HierarchicalScope scope && scope.Signals.TryGetValue(ma.Member!, out var s))
            return (ISimLogicSignal)s;

        throw new Exception($"Could not resolve signal source for {node.GetType().Name}.");
    }
    public static int EvaluateConstantInt(IKind node)
    {
        if (node is SvIntegerLiteral lit) return int.Parse(lit.Value!);
        throw new NotImplementedException("Dynamic indices not supported yet.");
    }
    private object ResolveRawObject(IKind node)
    {
        var addr = node switch {
            SvNamedValue nv => ExtractId(nv.Symbol),
            SvHierarchicalValue hv => ExtractId(hv.Symbol),
            _ => 0
        };
        
        var obj = GetSignal(addr);
        if (obj is not null) return obj;
        if (node is not SvMemberAccess ma) throw new Exception("Not a container.");
        var parent = ResolveRawObject(ma.Value!) as HierarchicalScope;
        if (parent!.Signals.TryGetValue(ma.Member!, out var sig)) return sig;
        return parent.Children.TryGetValue(ma.Member!, out var child) 
            ? child 
            : throw new Exception("Not a container.");
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
            "Add" => (l, r) => l + r,
            "Subtract" => (l, r) => l - r,
            "BinaryAnd" => (l, r) => l & r,
            "BinaryOr" => (l, r) => l | r,
            "BinaryXor" => (l, r) => l ^ r,
            "LogicalShiftLeft" => (l, r) => l << int.CreateTruncating(r.Value),
            "LogicalShiftRight" => (l, r) => l >> int.CreateTruncating(r.Value),
            "Equality" => (l, r) => new SimLogic<T>(l == r ? T.One : T.Zero, T.Zero),
            "CaseInequality" => (l, r) => new SimLogic<T>(l != r ? T.One : T.Zero, T.Zero),
            "GreaterThanEqual" => (l, r) => new SimLogic<T>(l.Value >= r.Value ? T.One : T.Zero, T.Zero),
            "LogicalOr" => (l, r) => new SimLogic<T>((l.Value != T.Zero || r.Value != T.Zero) ? T.One : T.Zero, T.Zero), 
            _ => throw new NotImplementedException($"Operator {binOp.Op} not supported.")
        };

        return new BinaryOpExpr<SimLogic<T>>(left, right, operation);
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