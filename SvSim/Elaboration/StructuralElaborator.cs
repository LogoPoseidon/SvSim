using System.Numerics;
using SvSim.Simulation.Engine;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;
using SvSim.Simulation.Statements;
using SvSim.SlangAstParser;
using SvSim.SlangAstParser.AstTree;
using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.Expression.ValueExpressionBase;
using SvSim.SlangAstParser.AstTree.Scope;
using SvSim.SlangAstParser.AstTree.Statement;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.Symbol;
using SvSim.SlangAstParser.AstTree.Symbol.InstanceSymbolBase;
using SvSim.SlangAstParser.AstTree.Symbol.Type;
using SvSim.SlangAstParser.AstTree.Symbol.Type.IntegralType;
using SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol;
using SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol;
using SvSim.SlangAstParser.AstTree.TimingControl;
using SvSim.SlangAstParser.Serializer;

namespace SvSim.Elaboration;

public class StructuralElaborator
{
    private readonly ExpressionElaborator _exprElaborator;
    private readonly ProceduralElaborator _procElaborator;
    private readonly EventScheduler _scheduler;

    private Dictionary<string, (IStatement body, List<ISimLogicSignal> args)> CompiledTasks { get; } = new();

    public StructuralElaborator(EventScheduler scheduler)
    {
        _scheduler = scheduler;
        _exprElaborator = new ExpressionElaborator(_scheduler);
        _procElaborator = new ProceduralElaborator(_exprElaborator, _scheduler, CompiledTasks);
    }

    public HierarchicalScope ElaborateDesign(TopLevel topLevel)
    {
        var rootScope = new HierarchicalScope(topLevel.Design.Kind ?? "root", null);
        if (topLevel.Design.Members != null)
            DiscoverAndProcess(topLevel.Design.Members, rootScope);
        DiscoverAndProcess(topLevel.Definitions, rootScope);
        return rootScope;
    }

    private void DiscoverAndProcess(IEnumerable<ISvSymbol> members, HierarchicalScope scope)
    {
        foreach (var member in members)
        {
            switch (member)
            {
                case SvCompilationUnit cu:
                    if (cu.Members != null) DiscoverAndProcess(cu.Members, scope);
                    break;
                case SvPackage pkg:
                    if (pkg.Members != null) ProcessMembers(pkg.Members, scope);
                    break;
                case SvInstance inst:
                    scope.AddChild(ElaborateInstance(inst, scope));
                    break;
            }
        }
    }

    private HierarchicalScope ElaborateInstance(SvInstance instanceAst, HierarchicalScope? parent)
    {
        var scope = new HierarchicalScope(instanceAst.Name, parent);
        _exprElaborator.RegisterSignal(instanceAst.Addr, scope);
        var body = instanceAst.Body;
        if (body?.Members == null) return scope;

        Console.WriteLine($"\n>>> Elaborating Instance: {scope.FullName} (Type: {body.Name})");

        ProcessMembers(body.Members, scope);

        if (instanceAst.Connections != null)
        {
            foreach (var conn in instanceAst.Connections)
            {
                ElaboratePortConnection(conn);
            }
        }

        ProcessBehavioral(body.Members);

        return scope;
    }

    private void ProcessMembers(IEnumerable<ISvSymbol> members, HierarchicalScope scope)
    {
        foreach (var member in members)
        {
            switch (member)
            {
                case SvPort port:
                    if (scope.Signals.TryGetValue(port.Name, out var portSig))
                    {
                        _exprElaborator.RegisterSignal(port.Addr, portSig);
                    }
                    break;
                case SvVariable varAst:
                    ElaborateVariable(varAst, scope);
                    break;
                case SvEnum et:
                    var mapping = new Dictionary<BigInteger, string>();
                    if (et.Members != null)
                    {
                        foreach (var m in et.Members.OfType<SvEnumValue>())
                        {
                            var val = ExpressionElaborator.ParseSlangIntToBigInt(m.Value);
                            mapping[val.Value] = m.Name;
                            _exprElaborator.RegisterSignal(m.Addr, val);
                        }
                    }

                    EnumRegistry.Register(et.Addr, mapping);

                    var enumWidth = ParseWidth(et.BaseType, et);
                    if (!string.IsNullOrEmpty(et.Name))
                    {
                        EnumRegistry.EnumWidths[et.Name] = enumWidth;
                    }
                    break;
                case SvInstance childInst:
                    scope.AddChild(ElaborateInstance(childInst, scope));
                    break;
                case SvSubroutine taskAst:
                    ElaborateTask(taskAst, scope);
                    break;
                case SvGenerateBlock gen:
                    if (gen.Members != null) ProcessMembers(gen.Members, scope);
                    break;
                case SvModportPort mp:
                    var actualSigAddr = mp.ResolvedInternalSymbol?.Addr ??
                                        ExpressionElaborator.ExtractId(mp.InternalSymbol);
                    var actualSignal = _exprElaborator.GetSignal(actualSigAddr);
                    if (actualSignal != null)
                    {
                        _exprElaborator.RegisterSignal(mp.Addr, actualSignal);
                    }
                    break;
                case SvStatementBlock stmtBlock:
                    if (stmtBlock.Members != null)
                    {
                        ProcessMembers(stmtBlock.Members, scope);
                    }
                    break;
                case SvModport modPort:
                    if (modPort.Members != null)
                    {
                        foreach (var mp in modPort.Members.OfType<SvModportPort>())
                        {
                            var targetAddr = mp.ResolvedInternalSymbol?.Addr ??
                                             ExpressionElaborator.ExtractId(mp.InternalSymbol);
                            var internalSig = _exprElaborator.GetSignal(targetAddr);
                            if (internalSig != null)
                                _exprElaborator.RegisterSignal(mp.Addr, internalSig);
                        }
                    }
                    break;
                case SvPackedStruct pst:
                    RegisterPackedStruct(pst);
                    break;

                case SvUnpackedStructType ust:
                    RegisterUnpackedStruct(ust);
                    break;

                case SvPackedUnion put:
                    RegisterPackedUnion(put);
                    break;

                case SvUnpackedUnionType uut:
                    RegisterUnpackedUnion(uut);
                    break;

                case SvQueueType:
                case SvDynamicArrayType:
                    break;

                default:
                    TryRegisterTypedefOrAlias(member);
                    break;
            }
        }
    }

    private void ProcessBehavioral(IEnumerable<ISvSymbol> members)
    {
        foreach (var member in members)
        {
            switch (member)
            {
                case SvContinuousAssign assignAst:
                    ElaborateContinuousAssign(assignAst);
                    break;

                case SvProceduralBlock block:
                    switch (block.ProcedureKind)
                    {
                        case SvProceduralBlockKind.Initial:
                            var initBody = _procElaborator.ElaborateStatement(block.Body!);
                            new SvProcess(initBody.Execute().GetEnumerator(), _scheduler).Start();
                            break;

                        case SvProceduralBlockKind.Always:
                            var alwaysBody = _procElaborator.ElaborateStatement(block.Body!);
                            var foreverAlways = new ForeverStatement(alwaysBody);
                            new SvProcess(foreverAlways.Execute().GetEnumerator(), _scheduler).Start();
                            break;

                        case SvProceduralBlockKind.AlwaysComb:
                        case SvProceduralBlockKind.AlwaysLatch:
                            _exprElaborator.ClearDependencies();
                            var combBody = _procElaborator.ElaborateStatement(block.Body!);
                            _ = new AlwaysCombProcess(combBody, _exprElaborator.Dependencies, _scheduler);
                            break;

                        case SvProceduralBlockKind.AlwaysFF:
                            if (block.Body is SvTimed timed)
                            {
                                var triggers = ResolveTriggers(timed.Timing);
                                if (triggers.Count == 0)
                                {
                                    Console.WriteLine($"[Warning] No triggers found for AlwaysFF at {block.Addr}");
                                    break;
                                }

                                var ffBody = _procElaborator.ElaborateStatement(timed.Stmt);

                                foreach (var foreverFf
                                         in from t
                                             in triggers
                                         select new WaitEventStatement(t.sig, t.edge)
                                         into wait
                                         select new BlockStatement([wait, ffBody])
                                         into loopBody
                                         select new ForeverStatement(loopBody))
                                {
                                    new SvProcess(foreverFf.Execute().GetEnumerator(), _scheduler).Start();
                                }
                            }
                            break;

                        case SvProceduralBlockKind.Final:
                        case null:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                $"{nameof(block)} type not supported {block.GetType()}");
                    }
                    break;

                case SvGenerateBlock gen:
                    if (gen.Members != null) ProcessBehavioral(gen.Members);
                    break;

                case SvVariable:
                case SvParameter:
                case SvPort:
                case SvEnum:
                case SvInstance:
                case SvModport:
                case SvSubroutine:
                    break;
            }
        }
    }

    private List<(ISimLogicSignal sig, SvEdgeKind edge)> ResolveTriggers(ISvAstNode? timing)
    {
        var list = new List<(ISimLogicSignal sig, SvEdgeKind edge)>();
        if (timing is null) return list;

        switch (timing)
        {
            case SvSignalEvent ev:
                var sig = _exprElaborator.ResolveSignal(ev.Expr);
                list.Add((sig, ev.Edge ?? SvEdgeKind.None));
                break;

            case SvList nested:
                foreach (var item in nested.List)
                    list.AddRange(ResolveTriggers(item));
                break;

            case SvBinary binOp:
                list.AddRange(ResolveTriggers(binOp.Left));
                list.AddRange(ResolveTriggers(binOp.Right));
                break;
            case SvEventList eventList:
                if (eventList.Events != null)
                {
                    foreach (var item in eventList.Events)
                    {
                        list.AddRange(ResolveTriggers(item));
                    }
                }
                break;
            case ISvExpression expr:
                var rawSig = _exprElaborator.ResolveSignal(expr);
                list.Add((rawSig, SvEdgeKind.None));
                break;
        }

        return list;
    }

    private static ISimLogicSignal CreateSignalForType(string? typeStr, int width, ISvType? resolvedType = null)
    {
        var cleanType = resolvedType?.Name ?? ExpressionElaborator.ExtractCleanTypeName(typeStr);
        if (!TypeRegistry.TryGetType(cleanType, out var def))
        {
            if (resolvedType?.GetCanonicalType() is SvEnum ev)
            {
                width = (int)ev.BitWidth;
            }

            return width switch
            {
                <= 8 => new LogicVar<byte>(width, new SimLogic<byte>(0, 0)),
                <= 16 => new LogicVar<ushort>(width, new SimLogic<ushort>(0, 0)),
                <= 32 => new LogicVar<uint>(width, new SimLogic<uint>(0, 0)),
                <= 64 => new LogicVar<ulong>(width, new SimLogic<ulong>(0, 0)),
                <= 128 => new LogicVar<UInt128>(width, new SimLogic<UInt128>(0, 0)),
                _ => new LogicVar<BigInteger>(width, new SimLogic<BigInteger>(0, 0)),
            };
        }

        if (def.IsUnion)
        {
            var memberWidths = def.Fields.ToDictionary(k => k.Key, v => v.Value.Msb - v.Value.Lsb + 1);
            return new PackedUnionVar(width, memberWidths, new SimLogic<BigInteger>(0, 0))
                { StructTypeName = cleanType };
        }

        var layout = def.Fields.ToDictionary(k => k.Key, v => (v.Value.Msb, v.Value.Lsb));
        return new PackedStructVar(width, layout, new SimLogic<BigInteger>(0, 0)) { StructTypeName = cleanType };
    }

    private static ISimLogicSignal CreateSignalForType(ISvType type, int width)
    {
        return CreateSignalForType(type.Name, width, type);
    }

    private void BuildContinuousAssign<T>(LogicVar<T> lhs, ISvExpression rhsAst) where T : IBinaryInteger<T>
    {
        _exprElaborator.ClearDependencies();
        var rhsExpr = _exprElaborator.ElaborateExpression<T>(rhsAst);
        _ = new ContinuousAssignProcess<SimLogic<T>>(lhs, rhsExpr, _exprElaborator.Dependencies, _scheduler);
    }

    private ISimEventSource ElaborateVariable(SvVariable ast, HierarchicalScope scope)
    {
        ISimEventSource simVar;

        switch (ast.ResolvedType)
        {
            case SvQueueType queueType:
            {
                var elemType = queueType.ElementType;
                simVar = new QueueVar<ISimLogicSignal>(() => CreateSignalForType(elemType, ParseWidth(elemType)))
                {
                    ElementTypeName = ExpressionElaborator.ExtractCleanTypeName(elemType.Name)
                };
                break;
            }
            case SvDynamicArrayType dynType:
            {
                var elemType = dynType.ElementType;
                simVar = new DynamicArrayVar<ISimLogicSignal>(() => CreateSignalForType(elemType, ParseWidth(elemType)))
                {
                    ElementTypeName = ExpressionElaborator.ExtractCleanTypeName(elemType.Name)
                };
                break;
            }
            case SvAssociativeArrayType assocType:
            {
                var elemType = assocType.ElementType;
                simVar = new AssociativeArrayVar<BigInteger, ISimLogicSignal>(() =>
                    CreateSignalForType(elemType, ParseWidth(elemType)))
                {
                    ElementTypeName = ExpressionElaborator.ExtractCleanTypeName(elemType.Name)
                };
                break;
            }
            default:
            {
                var typeStr = ast.Type ?? "";
                if (typeStr.Contains("$[$]"))
                {
                    var cleanType = typeStr.Replace("$[$]", "");
                    simVar = new QueueVar<ISimLogicSignal>(() => CreateSignalForType(cleanType, ParseWidth(cleanType)))
                    {
                        ElementTypeName = ExpressionElaborator.ExtractCleanTypeName(cleanType)
                    };
                }
                else if (typeStr.Contains("$[]"))
                {
                    var cleanType = typeStr.Replace("$[]", "");
                    simVar = new DynamicArrayVar<ISimLogicSignal>(() => CreateSignalForType(cleanType, ParseWidth(cleanType)))
                    {
                        ElementTypeName = ExpressionElaborator.ExtractCleanTypeName(cleanType)
                    };
                }
                else if (typeStr.Contains("$["))
                {
                    var cleanType = typeStr[..typeStr.IndexOf("$[", StringComparison.Ordinal)];
                    simVar = new AssociativeArrayVar<BigInteger, ISimLogicSignal>(() =>
                        CreateSignalForType(cleanType, ParseWidth(cleanType)))
                    {
                        ElementTypeName = ExpressionElaborator.ExtractCleanTypeName(cleanType)
                    };
                }
                else
                {
                    var width = ParseWidth(typeStr, ast.ResolvedType);
                    var sig = CreateSignalForType(typeStr, width, ast.ResolvedType);

                    if (ast.Initializer != null)
                    {
                        _exprElaborator.ClearDependencies();
                        var initVal = _exprElaborator.ElaborateExpression<BigInteger>(ast.Initializer).Evaluate();
                        sig.AssignFromBigInteger(initVal.Value, initVal.Unknown);
                    }

                    simVar = sig;
                }

                break;
            }
        }

        if (ast.ResolvedType != null)
        {
            if (simVar is ISimLogicSignal logicSig)
                logicSig.EnumTypeId = ast.ResolvedType.Addr;
        }
        else if (!string.IsNullOrEmpty(ast.Type))
        {
            var typeParts = ast.Type.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (typeParts.Length > 0 && long.TryParse(typeParts[0], out var typeId))
            {
                if (simVar is ISimLogicSignal logicSig)
                    logicSig.EnumTypeId = typeId;
            }
        }

        scope.AddSignal(ast.Name, simVar);
        _exprElaborator.RegisterSignal(ast.Addr, simVar);
        return simVar;
    }

    private void ElaborateTask(SvSubroutine ast, HierarchicalScope scope)
    {
        var taskScope = new HierarchicalScope(ast.Name, scope);
        var taskArgs = new List<ISimLogicSignal>();

        if (ast.Members != null)
        {
            ProcessMembers(ast.Members, taskScope);
        }

        if (ast.Arguments != null)
        {
            foreach (var kind in ast.Arguments)
            {
                var arg = (SvFormalArgument)kind;
                var width = ParseWidth(arg.Type, arg.ResolvedType);

                var simVar = CreateSignalForType(arg.Type, width, arg.ResolvedType);

                taskScope.AddSignal(arg.Name, simVar);
                _exprElaborator.RegisterSignal(arg.Addr, simVar);
                taskArgs.Add(simVar);
            }
        }

        var taskBody = _procElaborator.ElaborateStatement(ast.Body!);

        CompiledTasks[ast.Name] = (taskBody, taskArgs);
    }

    private void ElaboratePortConnection(InstanceConnection conn)
    {
        switch (conn.Port)
        {
            case SvPort portAst:
                var internalObj = _exprElaborator.GetSignal(portAst.Addr);
                if (internalObj is ISimLogicSignal sig)
                    InvokeBuildPortConnection(sig, conn,
                        portAst.Direction ?? throw new InvalidOperationException("Port direction cannot be null"));
                break;

            case SvInterfacePort ifPort:
                var externalIfaceId = conn.ResolvedIfaceInstance?.Addr ??
                                      ExpressionElaborator.ExtractId(conn.IfaceInstance);

                if (_exprElaborator.GetSignal(externalIfaceId) is HierarchicalScope actualIfaceScope)
                {
                    _exprElaborator.RegisterSignal(ifPort.Addr, actualIfaceScope);
                    Console.WriteLine($"[Interface] Bound Port {ifPort.Name} to {actualIfaceScope.FullName}");
                }
                break;
        }
    }

    private void InvokeBuildPortConnection(ISimLogicSignal internalSignal, InstanceConnection conn,
        SvArgumentDirection direction)
    {
        var type = internalSignal.GetType();
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(LogicVar<>)) return;
        var T = type.GetGenericArguments()[0];
        var method = typeof(StructuralElaborator)
            .GetMethod(nameof(BuildPortConnection),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .MakeGenericMethod(T);
        method.Invoke(this, [internalSignal, conn, direction]);
    }

    private void BuildPortConnection<T>(LogicVar<T> internalSignal, InstanceConnection conn,
        SvArgumentDirection direction)
        where T : IBinaryInteger<T>
    {
        switch (direction)
        {
            case SvArgumentDirection.In:
                {
                    _exprElaborator.ClearDependencies();
                    var externalExpr = _exprElaborator.ElaborateExpression<T>(conn.Expr!);
                    _ = new ContinuousAssignProcess<SimLogic<T>>(internalSignal, externalExpr, _exprElaborator.Dependencies,
                        _scheduler);
                    break;
                }
            case SvArgumentDirection.Out:
                {
                    if (conn.Expr is not SvAssignment { Left: SvNamedValue externalNet }) return;

                    var addr = externalNet.ResolvedSymbol?.Addr ?? ExpressionElaborator.ExtractId(externalNet.Symbol);
                    var externalObj = _exprElaborator.GetSignal(addr);

                    if (externalObj is LogicVar<T> externalSignal)
                    {
                        var internalExpr = new SignalCastReadExpr<T>(internalSignal);
                        _ = new ContinuousAssignProcess<SimLogic<T>>(externalSignal, internalExpr, [internalSignal],
                            _scheduler);
                    }
                    break;
                }
            case SvArgumentDirection.InOut:
                break;
            case SvArgumentDirection.Ref:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    private void ElaborateContinuousAssign(SvContinuousAssign ast)
    {
        if (ast.Assignment is not SvAssignment { Left: SvNamedValue lhsVal } assignAst) return;

        var addr = lhsVal.ResolvedSymbol?.Addr ?? ExpressionElaborator.ExtractId(lhsVal.Symbol);
        var lhsObj = _exprElaborator.GetSignal(addr);

        switch (lhsObj)
        {
            case LogicVar<byte> sig8:
                BuildContinuousAssign(sig8, assignAst.Right);
                break;
            case LogicVar<ushort> sig16:
                BuildContinuousAssign(sig16, assignAst.Right);
                break;
            case LogicVar<uint> sig32:
                BuildContinuousAssign(sig32, assignAst.Right);
                break;
            case LogicVar<ulong> sig64:
                BuildContinuousAssign(sig64, assignAst.Right);
                break;
            case LogicVar<UInt128> sig128:
                BuildContinuousAssign(sig128, assignAst.Right);
                break;
            case LogicVar<BigInteger> sigBig:
                BuildContinuousAssign(sigBig, assignAst.Right);
                break;
        }
    }

    private static void TryRegisterTypedefOrAlias(ISvSymbol member)
    {
        if (member is not SvTypeAlias alias) return;

        switch (alias.ResolvedTarget)
        {
            case SvPackedStruct pst:
                RegisterPackedStructWithAlias(alias.Name, pst);
                return;
            case SvPackedUnion put:
                RegisterPackedUnionWithAlias(alias.Name, put);
                return;
            case SvUnpackedStructType ust:
                RegisterUnpackedStructWithAlias(alias.Name, ust);
                return;
            case SvUnpackedUnionType uut:
                RegisterUnpackedUnionWithAlias(alias.Name, uut);
                return;
        }

        var name = alias.Name;
        var target = alias.Target;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(target)) return;

        ParseAndRegisterTypeString(name, target);
    }

    private static void ParseAndRegisterTypeString(string typeName, string target)
    {
        var isUnion = target.StartsWith("union");
        var isStruct = target.StartsWith("struct");
        if (!isUnion && !isStruct) return;

        var openBrace = target.IndexOf('{');
        var closeBrace = target.LastIndexOf('}');
        if (openBrace == -1 || closeBrace == -1) return;

        var inner = target.Substring(openBrace + 1, closeBrace - openBrace - 1);
        var fields = ParseFieldsFromInnerString(inner);

        var finalFields = new Dictionary<string, (int Msb, int Lsb, string SubType)>();
        var currentBitOffset = 0;

        for (var i = fields.Count - 1; i >= 0; i--)
        {
            var f = fields[i];
            var fWidth = ParseWidth(f.Type);

            if (isUnion)
            {
                finalFields[f.Name] = (fWidth - 1, 0, f.SubTypeName);
            }
            else
            {
                finalFields[f.Name] = (currentBitOffset + fWidth - 1, currentBitOffset, f.SubTypeName);
                currentBitOffset += fWidth;
            }
        }

        TypeRegistry.Register(typeName, new TypeDefinition
        {
            Name = typeName,
            IsStruct = isStruct,
            IsUnion = isUnion,
            IsPacked = true,
            Fields = finalFields
        });
    }

    private class ParsedField
    {
        public string Type { get; init; } = "";
        public string Name { get; init; } = "";
        public string SubTypeName { get; init; } = "";
    }

    private static List<ParsedField> ParseFieldsFromInnerString(string inner)
    {
        var fields = new List<ParsedField>();
        var braceDepth = 0;
        var lastStart = 0;

        for (var i = 0; i < inner.Length; i++)
        {
            var c = inner[i];
            switch (c)
            {
                case '{':
                    braceDepth++;
                    break;
                case '}':
                    braceDepth--;
                    break;
                case ';' when braceDepth == 0:
                    {
                        var fieldStr = inner.Substring(lastStart, i - lastStart).Trim();
                        if (!string.IsNullOrEmpty(fieldStr))
                        {
                            fields.Add(ParseSingleFieldString(fieldStr));
                        }

                        lastStart = i + 1;
                        break;
                    }
            }
        }

        if (lastStart >= inner.Length) return fields;
        {
            var fieldStr = inner[lastStart..].Trim();
            if (!string.IsNullOrEmpty(fieldStr))
            {
                fields.Add(ParseSingleFieldString(fieldStr));
            }
        }

        return fields;
    }

    private static ParsedField ParseSingleFieldString(string fieldStr)
    {
        fieldStr = fieldStr.Trim();
        var lastSpace = fieldStr.LastIndexOf(' ');
        if (lastSpace == -1) return new ParsedField { Type = fieldStr, Name = fieldStr, SubTypeName = fieldStr };

        var typePart = fieldStr[..lastSpace].Trim();
        var namePart = fieldStr[(lastSpace + 1)..].Trim();

        var subTypeName = typePart;
        if (!typePart.StartsWith("struct") && !typePart.StartsWith("union"))
            return new ParsedField
            {
                Type = typePart,
                Name = namePart,
                SubTypeName = subTypeName
            };
        subTypeName = namePart + "_t";
        ParseAndRegisterTypeString(subTypeName, typePart);

        return new ParsedField
        {
            Type = typePart,
            Name = namePart,
            SubTypeName = subTypeName
        };
    }

    private static void RegisterPackedStruct(SvPackedStruct pst) => RegisterPackedStructWithAlias(pst.Name, pst);

    private static void RegisterPackedStructWithAlias(string aliasName, SvPackedStruct pst)
    {
        var fields = new Dictionary<string, (int Msb, int Lsb, string SubType)>();
        var currentBitOffset = 0;

        var members = GetMembersOfRecord(pst);
        if (members == null) return;

        for (var i = members.Count - 1; i >= 0; i--)
        {
            var m = members[i];
            if (m is not SvVariable fieldVar) continue;
            var fWidth = ParseWidth(fieldVar.Type, fieldVar.ResolvedType);
            var cleanSubType = ExpressionElaborator.ExtractCleanTypeName(fieldVar.Type);

            fields[fieldVar.Name] = (currentBitOffset + fWidth - 1, currentBitOffset, cleanSubType);
            currentBitOffset += fWidth;
        }

        TypeRegistry.Register(aliasName, new TypeDefinition
        {
            Name = aliasName,
            IsStruct = true,
            IsUnion = false,
            IsPacked = true,
            Fields = fields
        });
    }

    private static void RegisterPackedUnion(SvPackedUnion put) => RegisterPackedUnionWithAlias(put.Name, put);

    private static void RegisterPackedUnionWithAlias(string aliasName, SvPackedUnion put)
    {
        var fields = new Dictionary<string, (int Msb, int Lsb, string SubType)>();
        var members = GetMembersOfRecord(put);
        if (members == null) return;

        foreach (var m in members)
        {
            if (m is not SvVariable fieldVar) continue;
            var fWidth = ParseWidth(fieldVar.Type, fieldVar.ResolvedType);
            var cleanSubType = ExpressionElaborator.ExtractCleanTypeName(fieldVar.Type);

            fields[fieldVar.Name] = (fWidth - 1, 0, cleanSubType);
        }

        TypeRegistry.Register(aliasName, new TypeDefinition
        {
            Name = aliasName,
            IsStruct = false,
            IsUnion = true,
            IsPacked = true,
            Fields = fields
        });
    }

    private static void RegisterUnpackedStruct(SvUnpackedStructType ust) =>
        RegisterUnpackedStructWithAlias(ust.Name, ust);

    private static void RegisterUnpackedStructWithAlias(string aliasName, SvUnpackedStructType ust)
    {
        var fields = new Dictionary<string, (int Msb, int Lsb, string SubType)>();
        var members = GetMembersOfRecord(ust);
        if (members == null) return;

        foreach (var m in members)
        {
            if (m is not SvVariable fieldVar) continue;
            var cleanSubType = ExpressionElaborator.ExtractCleanTypeName(fieldVar.Type);
            fields[fieldVar.Name] = (0, 0, cleanSubType);
        }

        TypeRegistry.Register(aliasName, new TypeDefinition
        {
            Name = aliasName,
            IsStruct = true,
            IsUnion = false,
            IsPacked = false,
            Fields = fields
        });
    }

    private static void RegisterUnpackedUnion(SvUnpackedUnionType uut) =>
        RegisterUnpackedUnionWithAlias(uut.Name, uut);

    private static void RegisterUnpackedUnionWithAlias(string aliasName, SvUnpackedUnionType uut)
    {
        var fields = new Dictionary<string, (int Msb, int Lsb, string SubType)>();
        var members = GetMembersOfRecord(uut);
        if (members == null) return;

        foreach (var m in members)
        {
            if (m is not SvVariable fieldVar) continue;
            var cleanSubType = ExpressionElaborator.ExtractCleanTypeName(fieldVar.Type);
            fields[fieldVar.Name] = (0, 0, cleanSubType);
        }

        TypeRegistry.Register(aliasName, new TypeDefinition
        {
            Name = aliasName,
            IsStruct = false,
            IsUnion = true,
            IsPacked = false,
            Fields = fields
        });
    }

    private static List<ISvSymbol>? GetMembersOfRecord(ISvSymbol recordNode)
    {
        if (recordNode is ISvScope scope)
        {
            return scope.Members?.ToList();
        }

        return null;
    }

    private static int ParseWidth(string? typeStr, ISvType? resolvedType = null)
    {
        if (resolvedType != null)
        {
            var canonical = resolvedType.GetCanonicalType();
            switch (canonical)
            {
                case ISvIntegralType integral:
                    return (int)integral.BitWidth;
                case SvFixedSizeUnpackedArrayType unpacked:
                    return (int)unpacked.BitstreamWidth;
            }
        }

        if (string.IsNullOrEmpty(typeStr)) return 32;

        var cleanType = ExpressionElaborator.ExtractCleanTypeName(typeStr);

        if (TypeRegistry.TryGetType(cleanType, out var def))
        {
            return def.Fields.Values.Max(x => x.Msb) + 1;
        }

        if (EnumRegistry.EnumWidths.TryGetValue(cleanType, out var enumWidth))
        {
            return enumWidth;
        }

        switch (cleanType)
        {
            case "int":
                return 32;
            case "logic":
            case "bit":
                return 1;
            case "string":
                return 2048;
        }

        var start = typeStr.IndexOf('[');
        if (start == -1) return 32;

        var colon = typeStr.IndexOf(':', start);
        if (colon == -1)
        {
            var end = typeStr.IndexOf(']', start);
            if (end != -1 && int.TryParse(typeStr.AsSpan(start + 1, end - start - 1), out var singleDim))
            {
                return singleDim;
            }

            return 32;
        }

        var msbStr = typeStr.Substring(start + 1, colon - start - 1).Trim();
        return int.TryParse(msbStr, out var msb) ? msb + 1 : 32;
    }

    private static int ParseWidth(ISvType type)
    {
        return ParseWidth(type.Name, type);
    }
}