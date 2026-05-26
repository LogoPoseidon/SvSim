using System.Collections;
using System.Reflection;
using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.BinsSelectExpr;
using SvSim.SlangAstParser.AstTree.Expression.ValueExpressionBase;
using SvSim.SlangAstParser.AstTree.RandSeqProductionProd;
using SvSim.SlangAstParser.AstTree.Statement;
using SvSim.SlangAstParser.AstTree.Symbol;
using SvSim.SlangAstParser.AstTree.Symbol.InstanceSymbolBase;
using SvSim.SlangAstParser.AstTree.Symbol.Type;
using SvSim.SlangAstParser.AstTree.Symbol.Type.IntegralType;
using SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol;
using SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol;
using SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol.TempVarSymbol;

namespace SvSim.SlangAstParser.Serializer;


public class SlangAstResolver
{
    private readonly Dictionary<long, ISvSymbol> _symbolRegistry = new();
    private readonly HashSet<object> _visited = new(ReferenceComparer.Instance);

    public static void Resolve(TopLevel topLevel)
    {
        var resolver = new SlangAstResolver();
        resolver.Run(topLevel);
    }

    private void Run(TopLevel topLevel)
    {
        _visited.Clear();
        Crawl(topLevel, RegisterSymbol);

        _visited.Clear();
        Crawl(topLevel, LinkReferences);
    }

    private void RegisterSymbol(object node)
    {
        if (node is ISvSymbol symbol && symbol.Addr != 0)
        {
            _symbolRegistry[symbol.Addr] = symbol;
        }
    }

    private void LinkReferences(object node)
    {
        switch (node)
        {
            // Symbols and Ports
            case SvInterfacePort ip:
                ip.ResolvedInterfaceDef = ResolveRef<SvDefinition>(ip.InterfaceDef);
                ip.ResolvedModport = ResolveRef<SvModport>(ip.Modport);
                break;
            case SvModportClocking mc:
                mc.ResolvedTarget = ResolveRef<SvClockingBlock>(mc.Target);
                break;
            case SvExplicitImport ei:
                ei.ResolvedPackage = ResolveRef<SvPackage>(ei.Package);
                break;
            case SvWildcardImport wi:
                wi.ResolvedPackage = ResolveRef<SvPackage>(wi.Package);
                break;
            case SvUninstantiatedDef ud:
                ud.ResolvedDefinition = ResolveRef<SvDefinition>(ud.DefinitionName);
                break;
            case MultiPortConnection mpc:
                mpc.ResolvedInternalSymbol = ResolveRef<ISvSymbol>(mpc.InternalSymbol);
                break;
            case SvPort port:
                port.ResolvedType = ResolveRef<ISvType>(port.Type);
                port.ResolvedInternalSymbol = ResolveRef<ISvSymbol>(port.InternalSymbol);
                break;
            case SvInstanceBody body:
                body.ResolvedDefinition = ResolveRef<SvDefinition>(body.Definition);
                break;
            case SvNetType nt:
                nt.ResolvedType = ResolveRef<ISvType>(nt.Type);
                nt.ResolvedResolutionFunction = ResolveRef<SvSubroutine>(nt.ResolutionFunction);
                break;
            case SvDefParam dp:
                dp.ResolvedTarget = ResolveRef<ISvSymbol>(dp.Target);
                break;
            case SvTypeParameter tp:
                tp.ResolvedType = ResolveRef<ISvType>(tp.Type);
                break;
            case CoverCrossTarget cct:
                cct.ResolvedCoverpoint = ResolveRef<SvCoverpoint>(cct.Coverpoint);
                break;

            // Value Symbols
            case SvModportPort mpp:
                mpp.ResolvedType = ResolveRef<ISvType>(mpp.Type);
                mpp.ResolvedInternalSymbol = ResolveRef<ISvSymbol>(mpp.InternalSymbol);
                break;
            case SvNet net:
                net.ResolvedType = ResolveRef<ISvType>(net.Type);
                break;
            case SvParameter param:
                param.ResolvedType = ResolveRef<ISvType>(param.Type);
                break;
            case SvPrimitivePort primPort:
                primPort.ResolvedType = ResolveRef<ISvType>(primPort.Type);
                break;
            case SvSpecparam spec:
                spec.ResolvedType = ResolveRef<ISvType>(spec.Type);
                break;

            // Variable Symbols
            case SvVariable v:
                v.ResolvedType = ResolveRef<ISvType>(v.Type);
                break;
            case SvClassProperty cp:
                cp.ResolvedType = ResolveRef<ISvType>(cp.Type);
                break;
            case SvClockVar cv:
                cv.ResolvedType = ResolveRef<ISvType>(cv.Type);
                break;
            case SvField f:
                f.ResolvedType = ResolveRef<ISvType>(f.Type);
                break;
            case SvFormalArgument fa:
                fa.ResolvedType = ResolveRef<ISvType>(fa.Type);
                break;

            // Temporary Variables
            case SvIterator it:
                it.ResolvedType = ResolveRef<ISvType>(it.Type);
                it.ResolvedNextTmp = ResolveRef<ISvTempVarSymbol>(it.NextTmp);
                break;
            case SvPatternVar pv:
                pv.ResolvedType = ResolveRef<ISvType>(pv.Type);
                pv.ResolvedNextTmp = ResolveRef<ISvTempVarSymbol>(pv.NextTmp);
                break;

            // Types
            case SvTypeAlias ta:
                ta.ResolvedTarget = ResolveRef<ISvType>(ta.Target);
                break;
            case SvClassType ct:
                ct.ResolvedBaseClass = ResolveRef<SvClassType>(ct.BaseClass);
                ct.ResolvedGenericClass = ResolveRef<SvGenericClassDef>(ct.GenericClass);
                if (ct.Implements != null)
                {
                    ct.ResolvedImplements = ct.Implements.Select(ResolveRef<SvClassType>).OfType<SvClassType>().ToArray();
                }
                break;
            case SvVirtualInterfaceType vit:
                vit.ResolvedModport = ResolveRef<SvModport>(vit.Modport);
                break;
            case SvEnum ev:
                ev.ResolvedBaseType = ResolveRef<ISvType>(ev.BaseType);
                break;
            
            case InstanceConnection conn:
                conn.ResolvedIfaceInstance = ResolveRef<SvInstance>(conn.IfaceInstance);
                conn.ResolvedModport = ResolveRef<SvModport>(conn.Modport);
                break;
            case SvPrimitiveInstance pi:
                pi.ResolvedPrimitiveType = ResolveRef<SvPrimitive>(pi.PrimitiveType);
                break;

            case ISvValueExpressionBase valExpr:
                valExpr.ResolvedSymbol = ResolveRef<ISvSymbol>(valExpr.Symbol);
                break;
            case SvVariableDeclaration vd:
                vd.ResolvedSymbol = ResolveRef<ISvSymbol>(vd.Symbol);
                break;
            case SvRandSequence rseq:
                rseq.ResolvedFirstProduction = ResolveRef<SvRandSeqProduction>(rseq.FirstProduction);
                break;
            case RandSeqItemDetails rsid:
                rsid.ResolvedTarget = ResolveRef<SvRandSeqProduction>(rsid.Target);
                break;

            case SvCrossId cid:
                cid.ResolvedCross = ResolveRef<SvCoverCross>(cid.Cross);
                if (cid.Id != null)
                {
                    cid.ResolvedId = cid.Id.Select(ResolveRef<SvCoverpoint>).OfType<SvCoverpoint>().ToArray();
                }
                break;
            case SvCondition cond:
                cond.ResolvedTarget = ResolveRef<ISvSymbol>(cond.Target);
                break;
        }
    }
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    private static PropertyInfo[] GetCachedProperties(Type type)
    {
        return PropertyCache.GetOrAdd(type, t =>
        {
            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return props
                .Where(prop => prop.GetIndexParameters().Length <= 0)
                .Where(prop => !prop.IsDefined(typeof(JsonIgnoreAttribute), true))
                .ToArray();
        });
    }

    private T? ResolveRef<T>(string? refString) where T : class
    {
        var addr = TryExtractAddress(refString);
        return addr.HasValue ? ResolveRef<T>(addr.Value) : null;
    }

    private T? ResolveRef<T>(long addr) where T : class
    {
        if (_symbolRegistry.TryGetValue(addr, out var symbol))
        {
            return symbol as T;
        }
        return null;
    }

    private static long? TryExtractAddress(string? referenceString)
    {
        if (string.IsNullOrWhiteSpace(referenceString)) return null;

        var span = referenceString.AsSpan().TrimStart();
        var firstSpace = span.IndexOf(' ');
        var token = firstSpace == -1 ? span : span[..firstSpace];

        if (long.TryParse(token, out var addr))
        {
            return addr;
        }
        return null;
    }

    private void Crawl(object? obj, Action<object> action)
    {
        if (obj == null) return;
        if (!_visited.Add(obj)) return;

        action(obj);

        var type = obj.GetType();
        if (type.IsPrimitive || obj is string) return;

        if (obj is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                Crawl(item, action);
            }
            return;
        }

        var properties = GetCachedProperties(type);
        foreach (var prop in properties)
        {
            var val = prop.GetValue(obj);
            if (val != null)
            {
                Crawl(val, action);
            }
        }
    }

    private class ReferenceComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceComparer Instance = new();
        bool IEqualityComparer<object>.Equals(object? x, object? y) => ReferenceEquals(x, y);
        public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
    }
}