using System.Numerics;
using SvSim.Simulation.Engine;
using SvSim.SlangAstParser.AstTree;
using SvSim.Simulation.Statements;
using SvSim.Simulation.Signal;
using SvSim.Simulation.Expressions;
using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.Expressions;
using SvSim.SlangAstParser.AstTree.Statements;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.TimingControls;

namespace SvSim.Elaboration;

public class ProceduralElaborator(
    ExpressionElaborator exprElab,
    EventScheduler scheduler,
    Dictionary<string, (IStatement body, List<ISimLogicSignal> args)> compiledTasks)
{
    public IStatement ElaborateStatement(IKind ast)
    {
        return ast switch
        {
            SvBlock block => ElaborateBlock(block),
            SvStatementBlock stmtBlock => ElaborateStatementBlock(stmtBlock),
            SvForeverLoop foreverAst => new ForeverStatement(ElaborateStatement(foreverAst.Body!)),
            SvVariableDeclaration varDecl => ElaborateVariableDeclaration(varDecl),
            SvList list => ElaborateList(list),
            SvExpressionStatement exprStmt => ElaborateExpressionStmt(exprStmt),
            SvAssignment assign => ElaborateAssignment(assign),
            SvCase caseStmt => ElaborateCase(caseStmt),
            SvTimed timedStmt => ElaborateTimed(timedStmt),
            SvConditional condAst => ElaborateConditional(condAst),
            SvForLoop forAst => ElaborateForLoop(forAst),
            SvUnaryOp unary => ElaborateUnaryOpStmt(unary),
            SvCall call => ElaborateCall(call),
            SvBreak => new BreakStatement(),
            SvEmpty => new BlockStatement([]),
            _ => throw new NotImplementedException($"{ast.GetType()} not implemented")
        };
    }

    private BlockStatement ElaborateStatementBlock(SvStatementBlock block)
    {
        var stmts = new List<IStatement>();
        if (block.Members != null) stmts.AddRange(block.Members.Select(ElaborateStatement));
        return new BlockStatement(stmts);
    }

    private BlockStatement ElaborateTimed(SvTimed timedAst)
    {
        switch (timedAst.Timing)
        {
            case SvDelay delay:
            {
                if (delay.Expr is not SvIntegerLiteral intLit)
                    throw new NotImplementedException(
                        $"Unsupported timing control type: {timedAst.Timing?.GetType().Name}");
                var delayVal = ulong.Parse(intLit.Value!);
                var innerStmt = ElaborateStatement(timedAst.Stmt!);
                return new BlockStatement([new DelayStatement(delayVal), innerStmt]);
            }
            case SvSignalEvent eventControl:
            {
                var sig = exprElab.ResolveSignal(eventControl.Expr!);
                var waitStmt = new WaitEventStatement(sig, eventControl.Edge ?? SvEdgeKind.None);
                var innerStmt = ElaborateStatement(timedAst.Stmt!);
                return new BlockStatement([waitStmt, innerStmt]);
            }
            default:
                throw new NotImplementedException(
                    $"Unsupported timing control type: {timedAst.Timing?.GetType().Name}");
        }
    }

    private BlockStatement ElaborateVariableDeclaration(SvVariableDeclaration decl)
    {
        return new BlockStatement([]); // Handled structurally by ProcessMembers via scope
    }

    private IfStatement ElaborateConditional(SvConditional condAst)
    {
        var condExpr = exprElab.ElaborateExpression<uint>(condAst.Conditions![0].Expr!);
        var ifTrue = condAst.IfTrue != null ? ElaborateStatement(condAst.IfTrue) : new BlockStatement([]);
        var ifFalse = condAst.IfFalse != null ? ElaborateStatement(condAst.IfFalse) : null;

        return new IfStatement(condExpr, ifTrue, ifFalse);
    }

    private ForStatement ElaborateForLoop(SvForLoop forAst)
    {
        var inits = new List<IStatement>();
        if (forAst.Initializers != null) inits.AddRange(forAst.Initializers.Select(ElaborateStatement));

        var stopExpr = exprElab.ElaborateExpression<uint>(forAst.StopExpr!);

        var steps = new List<IStatement>();
        if (forAst.Steps != null) steps.AddRange(forAst.Steps.Select(ElaborateStatement));

        var body = ElaborateStatement(forAst.Body!);

        return new ForStatement(new BlockStatement(inits), stopExpr, new BlockStatement(steps), body);
    }

    private IStatement ElaborateBlock(SvBlock block) => ElaborateStatement(block.Body!);

    private BlockStatement ElaborateList(SvList list)
    {
        var stmts = new List<IStatement>();
        if (list.List == null) return new BlockStatement(stmts);
        stmts.AddRange(list.List.Select(ElaborateStatement));
        return new BlockStatement(stmts);
    }

    private IStatement ElaborateExpressionStmt(SvExpressionStatement exprStmt)
    {
        return exprStmt.Expr switch
        {
            SvAssignment assign => ElaborateAssignment(assign),
            SvCall call => ElaborateCall(call),
            SvUnaryOp { Op: SvUnaryOperator.Preincrement } unary => ElaboratePostIncrementHelper(unary),
            SvExpressionStatement => ElaborateStatement(exprStmt.Expr!),
            _ => new BlockStatement([])
        };
    }

    private IStatement ElaborateUnaryOpStmt(SvUnaryOp unary)
    {
        return unary.Op switch
        {
            SvUnaryOperator.Postincrement or SvUnaryOperator.Preincrement => ElaboratePostIncrementHelper(unary),
            _ => new BlockStatement([])
        };
    }

    private IStatement ElaborateAssignment(SvAssignment assignAst)
    {
        if (assignAst.Right is SvNewArray newArr)
        {
            var targetObj = exprElab.ResolveRawObject(assignAst.Left!);
            var sizeExpr = exprElab.ElaborateExpression<uint>(newArr.SizeExpr!);

            return new NewArrayStatement(targetObj, sizeExpr);
        }

        return assignAst.Left switch
        {
            SvNamedValue namedVal => BuildStandardAssign(namedVal.Symbol!, assignAst),
            SvHierarchicalValue hv => BuildStandardAssign(hv.Symbol!, assignAst),
            SvMemberAccess memAcc => BuildMemberAssign(memAcc, assignAst),
            SvRangeSelect range => BuildSliceAssign(range, assignAst),
            SvElementSelect bit => BuildBitAssign(bit, assignAst),
            SvConcatenation concatAst => BuildConcatAssign(concatAst, assignAst),
            _ => throw new NotImplementedException($"Assignment to {assignAst.Left?.GetType().Name} not supported.")
        };
    }

    private IStatement BuildStandardAssign(string symbolId, SvAssignment assignAst)
    {
        var addr = ExpressionElaborator.ExtractId(symbolId);
        var lhsObj = exprElab.GetSignal(addr);

        return lhsObj switch
        {
            null => throw new InvalidOperationException(
                $"LHS Symbol '{symbolId}' (ID: {addr}) was not found in the symbol table. " +
                "This usually means the signal/port was not registered during structural elaboration."),
            ISimLogicSignal sig => BuildAssignInternal(sig, assignAst),
            _ => throw new InvalidOperationException(
                $"LHS '{symbolId}' is a {lhsObj.GetType().Name}, which is not an assignable logic variable.")
        };
    }

    private IStatement BuildMemberAssign(SvMemberAccess memAcc, SvAssignment assignAst)
    {
        var sig = exprElab.ResolveSignal(memAcc);
        return BuildAssignInternal(sig, assignAst);
    }

    private IStatement BuildAssignInternal(ISimLogicSignal sig, SvAssignment assignAst)
    {
        var type = sig.GetType();
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LogicVar<>))
        {
            return sig.BitWidth switch
            {
                <= 8 => BuildAssign((LogicVar<byte>)sig, assignAst),
                <= 16 => BuildAssign((LogicVar<ushort>)sig, assignAst),
                <= 32 => BuildAssign((LogicVar<uint>)sig, assignAst),
                <= 64 => BuildAssign((LogicVar<ulong>)sig, assignAst),
                <= 128 => BuildAssign((LogicVar<UInt128>)sig, assignAst),
                _ => BuildAssign((LogicVar<BigInteger>)sig, assignAst)
            };
        }

        // Catch-all for SliceSignal, PackedStructs, and Unions mapped as generic ISimLogicSignals
        var rhsExpr = exprElab.ElaborateExpression<BigInteger>(assignAst.Right!);
        if (assignAst.IsNonBlocking)
            return new NbaGeneralAssignStatement(sig, rhsExpr, scheduler);

        return new GeneralAssignStatement(sig, rhsExpr);
    }

    private IStatement BuildSliceAssign(SvRangeSelect rangeAst, SvAssignment assignAst)
    {
        var sig = exprElab.ResolveSignal(rangeAst.Value!);

        var msb = ExpressionElaborator.EvaluateConstantInt(rangeAst.Left!);
        var lsb = ExpressionElaborator.EvaluateConstantInt(rangeAst.Right!);

        var rhsExpr = exprElab.ElaborateExpression<BigInteger>(assignAst.Right!);

        if (assignAst.IsNonBlocking)
            return new NbaSliceAssignStatement(sig, msb, lsb, rhsExpr, scheduler);

        return new SliceAssignStatement(sig, msb, lsb, rhsExpr);
    }

    private IStatement BuildBitAssign(SvElementSelect bitAst, SvAssignment assignAst)
    {
        var targetObj = exprElab.ResolveRawObject(bitAst.Value!);
        var indexExpr = exprElab.ElaborateExpression<BigInteger>(bitAst.Selector!);
        var rhsExpr = exprElab.ElaborateExpression<BigInteger>(assignAst.Right!);

        if (targetObj is AssociativeArrayVar<BigInteger, ISimLogicSignal> aa)
        {
            return new AssociativeArrayAssignStatement(aa, indexExpr, rhsExpr);
        }

        if (targetObj is DynamicArrayVar<ISimLogicSignal> dyn)
        {
            return new DynamicArrayAssignStatement(dyn, indexExpr, rhsExpr);
        }

        if (targetObj is QueueVar<ISimLogicSignal> queue)
        {
            return new QueueAssignStatement(queue, indexExpr, rhsExpr);
        }

        var sig = exprElab.ResolveSignal(bitAst.Value!);
        if (assignAst.IsNonBlocking)
            return new NbaBitAssignStatement(sig, indexExpr, rhsExpr, scheduler);

        return new BitAssignStatement(sig, indexExpr, rhsExpr);
    }

    private IStatement BuildAssign<T>(LogicVar<T> lhs, SvAssignment assignAst) where T : IBinaryInteger<T>
    {
        var rhsExpr = exprElab.ElaborateExpression<T>(assignAst.Right!);

        if (assignAst.IsNonBlocking)
            return new NbaAssignStatement<SimLogic<T>>(lhs, rhsExpr, scheduler);

        return new AssignStatement<SimLogic<T>>(lhs, rhsExpr);
    }

    private IStatement BuildConcatAssign(SvConcatenation concatAst, SvAssignment assignAst)
    {
        var targetSignals = new List<ISimLogicSignal>();
        foreach (var operand in concatAst.Operands!)
        {
            if (operand is not SvNamedValue nv) continue;
            var addr = ExpressionElaborator.ExtractId(nv.Symbol);
            var sig = exprElab.GetSignal(addr);
            if (sig is ISimLogicSignal ls) targetSignals.Add(ls);
        }

        var rhsExpr = exprElab.ElaborateExpression<BigInteger>(assignAst.Right!);

        if (assignAst.IsNonBlocking)
            return new NbaConcatAssignStatement(targetSignals.ToArray(), rhsExpr, scheduler);

        return new ConcatAssignStatement(targetSignals.ToArray(), rhsExpr);
    }

    private IStatement ElaborateCall(SvCall callAst)
    {
        var taskName = callAst.Subroutine;
        if (!string.IsNullOrEmpty(taskName))
        {
            var key = compiledTasks.Keys.FirstOrDefault(taskName.EndsWith);
            if (key != null && compiledTasks.TryGetValue(key, out var compiledTask))
            {
                var callerArgs = new List<IExpression<SimLogic<BigInteger>>>();
                if (callAst.Arguments != null)
                    callerArgs.AddRange(callAst.Arguments.Select(exprElab.ElaborateExpression<BigInteger>));

                return new TaskCallStatement(compiledTask.body, compiledTask.args, callerArgs);
            }

            if (taskName is "push_back" or "push_front" or "pop_back" or "pop_front" or "delete")
            {
                var targetObj = exprElab.ResolveRawObject(callAst.Arguments![0]);

                var args = new List<IExpression<SimLogic<BigInteger>>>();
                for (var i = 1; i < callAst.Arguments.Length; i++)
                {
                    args.Add(exprElab.ElaborateExpression<BigInteger>(callAst.Arguments[i]));
                }

                return new ArrayMethodStatement(targetObj, taskName, args);
            }
        }

        var formatStr = "";
        var displayArgs = new List<IExpression<SimLogic<BigInteger>>>();

        if (callAst.Arguments == null || callAst.Arguments.Length == 0)
        {
            return new SystemCallStatement(callAst.Subroutine ?? "unknown", formatStr, displayArgs, scheduler);
        }

        var isFatal = callAst.Subroutine == "$fatal";
        var isFinish = callAst.Subroutine == "$finish";

        var startIdx = 0;
        if (isFatal)
        {
            displayArgs.Add(exprElab.ElaborateExpression<BigInteger>(callAst.Arguments[0]));
            if (callAst.Arguments.Length > 1 && callAst.Arguments[1] is SvStringLiteral strLit)
            {
                formatStr = strLit.Literal ?? "";
                startIdx = 2;
            }
        }
        else if (isFinish)
        {
            displayArgs.Add(exprElab.ElaborateExpression<BigInteger>(callAst.Arguments[0]));
            startIdx = callAst.Arguments.Length;
        }
        else
        {
            if (callAst.Arguments[0] is SvStringLiteral strLit)
            {
                formatStr = strLit.Literal ?? "";
                startIdx = 1;
            }
            else
            {
                formatStr = string.Join(" ", callAst.Arguments.Select(_ => "%d"));
                startIdx = 0;
            }
        }

        for (var i = startIdx; i < callAst.Arguments.Length; i++)
            displayArgs.Add(exprElab.ElaborateExpression<BigInteger>(callAst.Arguments[i]));

        return new SystemCallStatement(callAst.Subroutine ?? "unknown", formatStr, displayArgs, scheduler);
    }

    private IStatement ElaboratePostIncrementHelper(SvUnaryOp unaryOp)
    {
        if (unaryOp.Operand is not SvNamedValue namedVal) return new BlockStatement([]);

        var addr = ExpressionElaborator.ExtractId(namedVal.Symbol);
        var lhsObj = exprElab.GetSignal(addr);

        return lhsObj switch
        {
            LogicVar<byte> sig8 => BuildPostIncrement(sig8),
            LogicVar<ushort> sig16 => BuildPostIncrement(sig16),
            LogicVar<uint> sig32 => BuildPostIncrement(sig32),
            LogicVar<ulong> sig64 => BuildPostIncrement(sig64),
            LogicVar<UInt128> sig128 => BuildPostIncrement(sig128),
            LogicVar<BigInteger> sigBig => BuildPostIncrement(sigBig),
            _ => throw new InvalidOperationException($"LHS ID {addr} is not a valid logic variable.")
        };
    }

    private static AssignStatement<SimLogic<T>> BuildPostIncrement<T>(LogicVar<T> lhs)
        where T : IBinaryInteger<T>
    {
        var readExpr = new SignalCastReadExpr<T>(lhs);
        var literalOne = new LiteralExpr<SimLogic<T>>(new SimLogic<T>(T.One, T.Zero));
        var addOneExpr = new BinaryOpExpr<SimLogic<T>>(readExpr, literalOne, (l, r) => l + r);

        return new AssignStatement<SimLogic<T>>(lhs, addOneExpr);
    }

    private CaseStatement<BigInteger> ElaborateCase(SvCase caseAst)
    {
        var condition = exprElab.ElaborateExpression<BigInteger>(caseAst.Expr!);
        var items = new List<(IExpression<SimLogic<BigInteger>>[], IStatement)>();

        if (caseAst.Items != null)
        {
            foreach (var item in caseAst.Items)
            {
                var matches = new List<IExpression<SimLogic<BigInteger>>>();
                if (item.Expressions != null)
                    matches.AddRange(item.Expressions.Select(exprElab.ElaborateExpression<BigInteger>));

                items.Add((matches.ToArray(), ElaborateStatement(item.Stmt!)));
            }
        }

        var defaultCase = caseAst.DefaultCase != null ? ElaborateStatement(caseAst.DefaultCase) : null;
        return new CaseStatement<BigInteger>(condition, items, defaultCase);
    }
}