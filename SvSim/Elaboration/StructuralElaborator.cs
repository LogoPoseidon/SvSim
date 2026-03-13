using System.Numerics;
using SvSim.Simulation.Engine;
using SvSim.Simulation.Signal;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Statements;
using SvSim.SlangAstParser.AstTree;

namespace SvSim.Elaboration;

public class StructuralElaborator
{
    private readonly ExpressionElaborator _exprElaborator;
    private readonly ProceduralElaborator _procElaborator;
    private readonly EventScheduler _scheduler;
    
    public Dictionary<string, (IStatement body, List<ISimLogicSignal> args)> CompiledTasks { get; } = new();
    public StructuralElaborator(EventScheduler scheduler)
    {
        _scheduler = scheduler;
        _exprElaborator = new ExpressionElaborator(_scheduler);
        _procElaborator = new ProceduralElaborator(_exprElaborator, _scheduler, CompiledTasks);
    }

    public HierarchicalScope ElaborateDesign(SvDesign design)
    {
        var rootScope = new HierarchicalScope(design.Name, null);
        DiscoverAndProcess(design.Members, rootScope);
        return rootScope;
    }
    private void DiscoverAndProcess(IEnumerable<IKind> members, HierarchicalScope scope)
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
        var scope = new HierarchicalScope(instanceAst.Name ?? "unnamed", parent);
        _exprElaborator.RegisterSignal(instanceAst.Addr, scope);
        var body = instanceAst.Body as SvInstanceBody;
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
        
        ProcessBehavioral(body.Members, scope);
        
        return scope;
    }
    
    private void ProcessMembers(IEnumerable<IKind> members, HierarchicalScope scope)
    {
        foreach (var member in members)
        {
            switch (member)
            {
                case SvPort port:
                    if (scope.Signals.TryGetValue(port.Name!, out var portSig))
                    {
                        _exprElaborator.RegisterSignal(port.Addr, portSig);
                    }
                    break;
                case SvVariable varAst:
                    ElaborateVariable(varAst, scope);
                    break;
                case SvEnumType et:
                    if (et.Members != null)
                    {
                        foreach (var m in et.Members.OfType<SvEnumValue>())
                        {
                            var val = ExpressionElaborator.ParseSlangIntToBigInt(m.Value);
                            _exprElaborator.RegisterSignal(m.Addr, val);
                        }
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
                    var actualSigAddr = ExpressionElaborator.ExtractId(mp.InternalSymbol);
                    var actualSignal = _exprElaborator.GetSignal(actualSigAddr);
                    if (actualSignal != null)
                    {
                        _exprElaborator.RegisterSignal(mp.Addr, actualSignal);
                    }
                    break;
                case SvModport modport:
                    if (modport.Members != null)
                    {
                        foreach (var mp in modport.Members.OfType<SvModportPort>())
                        {
                            var internalSig = _exprElaborator.GetSignal(ExpressionElaborator.ExtractId(mp.InternalSymbol));
                            if (internalSig != null)
                                _exprElaborator.RegisterSignal(mp.Addr, internalSig);
                        }
                    }
                    break;
            }
        }
    }
    
    private void ProcessBehavioral(IEnumerable<IKind> members, HierarchicalScope scope)
    {
        foreach (var member in members)
        {
            switch (member)
            {
                case SvContinuousAssign assignAst:
                    ElaborateContinuousAssign(assignAst);
                    break;
                case SvProceduralBlock { ProcedureKind: "AlwaysComb" } procBlock:
                    _exprElaborator.ClearDependencies();
                    var stmtTree = _procElaborator.ElaborateStatement(procBlock.Body!);
                    _ = new AlwaysCombProcess(stmtTree, _exprElaborator.Dependencies, _scheduler);
                    break;
                case SvProceduralBlock { ProcedureKind: "Initial" } initBlock:
                    _ = new InitialProcess(_procElaborator.ElaborateStatement(initBlock.Body), _scheduler);
                    break;
                case SvGenerateBlock gen:
                    if (gen.Members != null) ProcessBehavioral(gen.Members, scope);
                    break;
            }
        }
    }

    
    
    private void BuildContinuousAssign<T>(LogicVar<T> lhs, IKind rhsAst) where T : IBinaryInteger<T>
    {
        _exprElaborator.ClearDependencies();
        var rhsExpr = _exprElaborator.ElaborateExpression<T>(rhsAst);
        _ = new ContinuousAssignProcess<SimLogic<T>>(lhs, rhsExpr, _exprElaborator.Dependencies, _scheduler);
    }

    private void ElaborateVariable(SvVariable ast, HierarchicalScope scope)
    {
        var width = ParseWidth(ast.Type);
    
        ISimLogicSignal simVar = width switch
        {
            <= 8   => new LogicVar<byte>(width, new SimLogic<byte>(0, 0)),
            <= 16  => new LogicVar<ushort>(width, new SimLogic<ushort>(0, 0)),
            <= 32  => new LogicVar<uint>(width, new SimLogic<uint>(0, 0)),
            <= 64  => new LogicVar<ulong>(width, new SimLogic<ulong>(0, 0)),
            <= 128 => new LogicVar<UInt128>(width, new SimLogic<UInt128>(0, 0)),
            _ => new LogicVar<BigInteger>(width, new SimLogic<BigInteger>(0, 0)),
        };

        scope.AddSignal(ast.Name!, simVar);
        _exprElaborator.RegisterSignal(ast.Addr, simVar);
    }
    
    private void ElaborateTask(SvSubroutine ast, HierarchicalScope scope)
    {
        var taskArgs = new List<ISimLogicSignal>();

        if (ast.Arguments != null)
        {
            foreach (var kind in ast.Arguments)
            {
                var arg = (SvFormalArgument)kind;
                var width = ParseWidth(arg.Type);
            
                ISimLogicSignal simVar = width switch
                {
                    <= 8   => new LogicVar<byte>(width, new SimLogic<byte>(0, 0)),
                    <= 16  => new LogicVar<ushort>(width, new SimLogic<ushort>(0, 0)),
                    <= 32  => new LogicVar<uint>(width, new SimLogic<uint>(0, 0)),
                    <= 64  => new LogicVar<ulong>(width, new SimLogic<ulong>(0, 0)),
                    <= 128 => new LogicVar<UInt128>(width, new SimLogic<UInt128>(0, 0)),
                    _      => new LogicVar<BigInteger>(width, new SimLogic<BigInteger>(0, 0)),
                };
            
                scope.AddSignal(arg.Name!, simVar);
                _exprElaborator.RegisterSignal(arg.Addr, simVar);
                taskArgs.Add(simVar);
            }
        }

        var taskBody = _procElaborator.ElaborateStatement(ast.Body!);

        CompiledTasks[ast.Name!] = (taskBody, taskArgs);
    }

    private void ElaboratePortConnection(InstanceConnection conn)
    {
        switch (conn.Port)
        {
            case SvPort portAst:
                var internalObj = _exprElaborator.GetSignal(portAst.Addr);
                if (internalObj is ISimLogicSignal sig)
                    InvokeBuildPortConnection(sig, conn, portAst.Direction!);
                break;

            case SvInterfacePort ifPort:
                var externalIfaceId = ExpressionElaborator.ExtractId(conn.IfaceInstance);

                if (_exprElaborator.GetSignal(externalIfaceId) is HierarchicalScope actualIfaceScope)
                {
                    _exprElaborator.RegisterSignal(ifPort.Addr, actualIfaceScope);
                    Console.WriteLine($"[Interface] Bound Port {ifPort.Name} to {actualIfaceScope.FullName}");
                }
                break;
        }
    }

    private void InvokeBuildPortConnection(ISimLogicSignal internalSignal, InstanceConnection conn, string direction)
    {
        var type = internalSignal.GetType();
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(LogicVar<>)) return;
        var T = type.GetGenericArguments()[0];
        var method = typeof(StructuralElaborator)
            .GetMethod(nameof(BuildPortConnection), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .MakeGenericMethod(T);
        method.Invoke(this, [internalSignal, conn, direction]);
    }
    private void BuildPortConnection<T>(LogicVar<T> internalSignal, InstanceConnection conn, string direction) 
        where T : IBinaryInteger<T>
    {
        switch (direction)
        {
            case "In":
            {
                _exprElaborator.ClearDependencies();
                var externalExpr = _exprElaborator.ElaborateExpression<T>(conn.Expr!);
                _ = new ContinuousAssignProcess<SimLogic<T>>(internalSignal, externalExpr, _exprElaborator.Dependencies, _scheduler);
                break;
            }
            case "Out":
            {
                if (conn.Expr is not SvAssignment { Left: SvNamedValue externalNet }) return;
                
                var addr = ExpressionElaborator.ExtractId(externalNet.Symbol);
                var externalObj = _exprElaborator.GetSignal(addr);
                
                if (externalObj is LogicVar<T> externalSignal)
                {
                    var internalExpr = new SignalCastReadExpr<T>(internalSignal);
                    _ = new ContinuousAssignProcess<SimLogic<T>>(externalSignal, internalExpr, [internalSignal], _scheduler);
                }
                break;
            }
        }
    }
    private void ElaborateContinuousAssign(SvContinuousAssign ast)
    {
        if (ast.Assignment is not SvAssignment { Left: SvNamedValue lhsVal } assignAst) return;
        
        var addr = ExpressionElaborator.ExtractId(lhsVal.Symbol);
        var lhsObj = _exprElaborator.GetSignal(addr);

        switch (lhsObj)
        {
            case LogicVar<byte> sig8:
                BuildContinuousAssign(sig8, assignAst.Right!);
                break;
            case LogicVar<ushort> sig16:
                BuildContinuousAssign(sig16, assignAst.Right!);
                break;
            case LogicVar<uint> sig32:
                BuildContinuousAssign(sig32, assignAst.Right!);
                break;
            case LogicVar<ulong> sig64:
                BuildContinuousAssign(sig64, assignAst.Right!);
                break;
            case LogicVar<UInt128> sig128:
                BuildContinuousAssign(sig128, assignAst.Right!);
                break;
            case LogicVar<BigInteger> sigBig:
                BuildContinuousAssign(sigBig, assignAst.Right!);
                break;
        }
    }


    private static int ParseWidth(string? typeStr)
    {
        if (string.IsNullOrEmpty(typeStr)) return 32;
        switch (typeStr)
        {
            case "int":
                return 32;
            case "logic":
            case "bit":
                return 1;
        }

        var start = typeStr.IndexOf('[');
        var colon = typeStr.IndexOf(':');
        if (start == -1 || colon == -1) return 32;
        var msb = int.Parse(typeStr.Substring(start + 1, colon - start - 1));
        return msb + 1;
    }
}