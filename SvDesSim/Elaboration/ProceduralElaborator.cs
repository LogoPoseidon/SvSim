using System.Numerics;
using SvAstParser.AstTree;
using SvAstParser.AstTree.Expression;
using SvAstParser.AstTree.Expression.ValueExpressionBase;
using SvAstParser.AstTree.Statement;
using SvAstParser.AstTree.SvEnums;
using SvAstParser.AstTree.Symbol;
using SvAstParser.AstTree.TimingControl;
using SvAstParser.AstTree.Pattern;
using SvDesSim.Simulation.Engine;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Signal;
using SvDesSim.Simulation.Statements;
using SvConditional = SvAstParser.AstTree.Statement.SvConditional;
using SvInvalid = SvAstParser.AstTree.Statement.SvInvalid;

namespace SvDesSim.Elaboration;

public class RangeMatchExpr(IExpression<SimLogic<BigInteger>> min, IExpression<SimLogic<BigInteger>> max)
    : IExpression<SimLogic<BigInteger>>
{
    public SimLogic<BigInteger> Evaluate()
    {
        return new SimLogic<BigInteger>(0, 0); // Should not be used for direct equality
    }

    public bool IsMatch(SimLogic<BigInteger> value)
    {
        var minVal = min.Evaluate().Value;
        var maxVal = max.Evaluate().Value;
        return value.Value >= minVal && value.Value <= maxVal;
    }
}

public class ProceduralElaborator(
    ExpressionElaborator exprElab,
    EventScheduler scheduler,
    Dictionary<string, (IStatement body, List<ISimLogicSignal> args)> compiledTasks)
{
    public IStatement ElaborateStatement(ISvAstNode ast)
    {
        return ast switch
        {
            SvBlock block => ElaborateBlock(block),
            SvStatementBlock stmtBlock => ElaborateStatementBlock(stmtBlock),
            SvForeverLoop foreverAst => new ForeverStatement(ElaborateStatement(foreverAst.Body)),
            SvVariableDeclaration varDecl => ElaborateVariableDeclaration(varDecl),
            SvList list => ElaborateList(list),
            SvExpressionStatement exprStmt => ElaborateExpressionStmt(exprStmt),
            SvAssignment assign => ElaborateAssignment(assign),
            SvCase caseStmt => ElaborateCase(caseStmt),
            SvPatternCase pCase => ElaboratePatternCase(pCase),
            SvTimed timedStmt => ElaborateTimed(timedStmt),
            SvConditional condAst => ElaborateConditional(condAst),
            SvForLoop forAst => ElaborateForLoop(forAst),
            SvForeachLoop foreachAst => ElaborateForeachLoop(foreachAst),
            SvUnaryOp unary => ElaborateUnaryOpStmt(unary),
            SvRepeatLoop repeatAst => ElaborateRepeatLoop(repeatAst),
            SvCall call => ElaborateCall(call),
            SvBreak => new BreakStatement(),
            SvContinue => new ContinueStatement(),
            SvEmpty => new BlockStatement([]),
            
            SvImmediateAssertion immAssert => ElaborateImmediateAssertion(immAssert),
            SvProceduralAssign pAssign => pAssign.Assignment != null ? ElaborateAssignment((SvAssignment)pAssign.Assignment) : new BlockStatement([]),
            ISvStatement pDeassign when pDeassign.GetType().Name == "SvProceduralDeassign" => new BlockStatement([]),
            SvInvalid => new BlockStatement([]), 
            SvReturn retAst => ElaborateReturn(retAst),
            
            _ => throw new NotImplementedException($"{ast.GetType()} not implemented as procedural statement.")
        };
    }
    
    private ReturnStatement ElaborateReturn(SvReturn retAst)
    {
        if (retAst.Expr == null) return new ReturnStatement(null);
        var expr = exprElab.ElaborateExpression<BigInteger>(retAst.Expr);
        return new ReturnStatement(expr);
    }
    
    private IfStatement ElaborateImmediateAssertion(SvImmediateAssertion ast)
    {
        var condExpr = exprElab.ElaborateExpression<uint>(ast.Cond);
        var ifTrue = ast.IfTrue != null ? ElaborateStatement(ast.IfTrue) : new BlockStatement([]);
        var ifFalse = ast.IfFalse != null ? ElaborateStatement(ast.IfFalse) : new BlockStatement([]);
        return new IfStatement(condExpr, ifTrue, ifFalse);
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
                var delayExpr = exprElab.ElaborateExpression<BigInteger>(delay.Expr);
                var delayVal = (ulong)delayExpr.Evaluate().Value;
                var innerStmt = ElaborateStatement(timedAst.Stmt);
                return new BlockStatement([new DelayStatement(delayVal), innerStmt]);
            }
            case SvSignalEvent eventControl:
            {
                var sig = exprElab.ResolveSignal(eventControl.Expr);
                var waitStmt = new WaitEventStatement(sig, eventControl.Edge ?? SvEdgeKind.None);
                var innerStmt = ElaborateStatement(timedAst.Stmt);
                return new BlockStatement([waitStmt, innerStmt]);
            }
            case SvEventList eventList:
            {
                var triggers = StructuralElaborator.ResolveTriggers(eventList, exprElab);
                var waitStmt = new WaitEventListStatement(triggers);
                var innerStmt = ElaborateStatement(timedAst.Stmt);
                return new BlockStatement([waitStmt, innerStmt]);
            }
            case SvImplicitEvent:
                return new BlockStatement([ElaborateStatement(timedAst.Stmt)]);
            default:
                throw new NotImplementedException(
                    $"Unsupported timing control type: {timedAst.Timing.GetType().Name}");
        }
    }

    private static BlockStatement ElaborateVariableDeclaration(SvVariableDeclaration _)
    {
        return new BlockStatement([]);
    }

    private IfStatement ElaborateConditional(SvConditional condAst)
    {
        foreach (var condItem in condAst.Conditions)
        {
            if (condItem.Pattern is SvVariable varPattern)
            {
                var addr = varPattern.Variable.Addr;
                var width = StructuralElaborator.ParseWidth(varPattern.Variable.Type);
                var sig = new LogicVar<BigInteger>(width, new SimLogic<BigInteger>(0, 0));
                exprElab.RegisterSignal(addr, sig);
            }
        }

        var condExpr = exprElab.ElaborateExpression<uint>(condAst.Conditions[0].Expr);
        var ifTrue = ElaborateStatement(condAst.IfTrue);
        var ifFalse = condAst.IfFalse != null ? ElaborateStatement(condAst.IfFalse) : null;

        return new IfStatement(condExpr, ifTrue, ifFalse);
    }

    private ForStatement ElaborateForLoop(SvForLoop forAst)
    {
        var inits = new List<IStatement>();
        if (forAst.Initializers != null) inits.AddRange(forAst.Initializers.Select(ElaborateStatement));

        var stopExpr = forAst.StopExpr != null 
            ? exprElab.ElaborateExpression<uint>(forAst.StopExpr) 
            : new LiteralExpr<SimLogic<uint>>(new SimLogic<uint>(1u, 0u));

        var steps = new List<IStatement>();
        if (forAst.Steps != null) steps.AddRange(forAst.Steps.Select(ElaborateStatement));

        var body = forAst.Body != null ? ElaborateStatement(forAst.Body) : new BlockStatement([]);

        return new ForStatement(new BlockStatement(inits), stopExpr, new BlockStatement(steps), body);
    }

    private RepeatStatement ElaborateRepeatLoop(SvRepeatLoop repeatAst)
    {
        var countExpr = exprElab.ElaborateExpression<BigInteger>(repeatAst.Count);
        var body = ElaborateStatement(repeatAst.Body);
        return new RepeatStatement(countExpr, body);
    }

    private ForeachStatement ElaborateForeachLoop(SvForeachLoop foreachAst)
    {
        var arrayObj = exprElab.ResolveRawObject(foreachAst.ArrayRef);
        ISimLogicSignal? indexSig = null;

        if (foreachAst.LoopDims is { Length: > 0 })
        {
            var loopVar = foreachAst.LoopDims[0].Var;
            
            indexSig = new LogicVar<uint>(32, new SimLogic<uint>(0, 0));
            exprElab.RegisterSignal(loopVar.Addr, indexSig);
        }

        var body = ElaborateStatement(foreachAst.Body);
        return new ForeachStatement(arrayObj, indexSig, body);
    }

    private IStatement ElaborateBlock(SvBlock block) => ElaborateStatement(block.Body);

    private BlockStatement ElaborateList(SvList list)
    {
        var stmts = new List<IStatement>();
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
        if (assignAst.Right is not SvNewArray newArr)
            return assignAst.Left switch
            {
                SvNamedValue namedVal => BuildStandardAssign(namedVal, assignAst),
                SvHierarchicalValue hv => BuildStandardAssign(hv, assignAst),
                SvMemberAccess memAcc => BuildMemberAssign(memAcc, assignAst),
                SvRangeSelect range => BuildSliceAssign(range, assignAst),
                SvElementSelect bit => BuildBitAssign(bit, assignAst),
                SvConcatenation concatAst => BuildConcatAssign(concatAst, assignAst),
                { } sc when sc.GetType().Name == "SvStreamingConcatenation" => BuildStreamingConcatAssign(sc, assignAst),
                _ => throw new NotImplementedException($"Assignment to {assignAst.Left.GetType().Name} not supported.")
            };
        var targetObj = exprElab.ResolveRawObject(assignAst.Left);
        var sizeExpr = exprElab.ElaborateExpression<uint>(newArr.SizeExpr);

        return new NewArrayStatement(targetObj, sizeExpr);
    }

    private IStatement BuildStandardAssign(ISvValueExpressionBase valNode, SvAssignment assignAst)
    {
        var addr = valNode.ResolvedSymbol?.Addr ?? ExpressionElaborator.ExtractId(valNode.Symbol);
        var lhsObj = exprElab.GetSignal(addr);

        return lhsObj switch
        {
            null => throw new InvalidOperationException(
                $"LHS Symbol '{valNode.Symbol}' (ID: {addr}) was not found in the symbol table."),
            ISimLogicSignal sig => BuildAssignInternal(sig, assignAst),
            QueueVar<ISimLogicSignal> q => new ArrayWholeAssignStatement(q, exprElab.ElaborateRhsExpression<BigInteger>(assignAst.Right, assignAst.Left), assignAst.IsNonBlocking ? scheduler : null),
            DynamicArrayVar<ISimLogicSignal> dyn => new ArrayWholeAssignStatement(dyn, exprElab.ElaborateRhsExpression<BigInteger>(assignAst.Right, assignAst.Left), assignAst.IsNonBlocking ? scheduler : null),
            _ => throw new InvalidOperationException(
                $"LHS '{valNode.Symbol}' is a {lhsObj.GetType().Name}, which is not an assignable logic variable.")
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

        var rhsExpr = exprElab.ElaborateRhsExpression<BigInteger>(assignAst.Right, assignAst.Left);
        if (assignAst.IsNonBlocking)
            return new NbaGeneralAssignStatement(sig, rhsExpr, scheduler);

        return new GeneralAssignStatement(sig, rhsExpr);
    }

    private IStatement BuildSliceAssign(SvRangeSelect rangeAst, SvAssignment assignAst)
    {
        var sig = exprElab.ResolveSignal(rangeAst.Value);

        var msb = ExpressionElaborator.EvaluateConstantInt(rangeAst.Left);
        var lsb = ExpressionElaborator.EvaluateConstantInt(rangeAst.Right);

        var rhsExpr = exprElab.ElaborateRhsExpression<BigInteger>(assignAst.Right, assignAst.Left);

        if (assignAst.IsNonBlocking)
            return new NbaSliceAssignStatement(sig, msb, lsb, rhsExpr, scheduler);

        return new SliceAssignStatement(sig, msb, lsb, rhsExpr);
    }

    private IStatement BuildBitAssign(SvElementSelect bitAst, SvAssignment assignAst)
    {
        var targetObj = exprElab.ResolveRawObject(bitAst.Value);
        var indexExpr = exprElab.ElaborateExpression<BigInteger>(bitAst.Selector);
        var rhsExpr = exprElab.ElaborateRhsExpression<BigInteger>(assignAst.Right, assignAst.Left);

        switch (targetObj)
        {
            case AssociativeArrayVar<BigInteger, ISimLogicSignal> aa:
                return new AssociativeArrayAssignStatement(aa, indexExpr, rhsExpr);
            case DynamicArrayVar<ISimLogicSignal> dyn:
                return new DynamicArrayAssignStatement(dyn, indexExpr, rhsExpr);
            case QueueVar<ISimLogicSignal> queue:
                return new QueueAssignStatement(queue, indexExpr, rhsExpr);
        }

        var sig = exprElab.ResolveSignal(bitAst.Value);
        if (assignAst.IsNonBlocking)
            return new NbaBitAssignStatement(sig, indexExpr, rhsExpr, scheduler);

        return new BitAssignStatement(sig, indexExpr, rhsExpr);
    }

    private IStatement BuildAssign<T>(LogicVar<T> lhs, SvAssignment assignAst) where T : IBinaryInteger<T>
    {
        var rhsExpr = exprElab.ElaborateRhsExpression<T>(assignAst.Right, assignAst.Left);

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
            var addr = nv.ResolvedSymbol?.Addr ?? ExpressionElaborator.ExtractId(nv.Symbol);
            var sig = exprElab.GetSignal(addr);
            if (sig is ISimLogicSignal ls) targetSignals.Add(ls);
        }

        var rhsExpr = exprElab.ElaborateRhsExpression<BigInteger>(assignAst.Right, assignAst.Left);

        if (assignAst.IsNonBlocking)
            return new NbaConcatAssignStatement(targetSignals.ToArray(), rhsExpr, scheduler);

        return new ConcatAssignStatement(targetSignals.ToArray(), rhsExpr);
    }

    private IStatement BuildStreamingConcatAssign(ISvExpression concatAst, SvAssignment assignAst)
    {
        var targetSignals = new List<ISimLogicSignal>();
        var type = concatAst.GetType();
        var operandsProp = type.GetProperty("Operands") ?? type.GetProperty("Elements") ?? type.GetProperty("Exprs");
        if (operandsProp?.GetValue(concatAst) is System.Collections.IEnumerable operands)
        {
            foreach (var operand in operands)
            {
                if (operand is not ISvExpression op) continue;
                if (op is SvNamedValue nv)
                {
                    var addr = nv.ResolvedSymbol?.Addr ?? ExpressionElaborator.ExtractId(nv.Symbol);
                    var sig = exprElab.GetSignal(addr);
                    if (sig is ISimLogicSignal ls) targetSignals.Add(ls);
                }
            }
        }

        var rhsExpr = exprElab.ElaborateRhsExpression<BigInteger>(assignAst.Right, assignAst.Left);

        if (assignAst.IsNonBlocking)
            return new NbaConcatAssignStatement(targetSignals.ToArray(), rhsExpr, scheduler);

        return new ConcatAssignStatement(targetSignals.ToArray(), rhsExpr);
    }

    private IStatement ElaborateCall(SvCall callAst)
    {
        var taskName = ExpressionElaborator.ExtractName(callAst.Subroutine);
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
            return new SystemCallStatement(string.IsNullOrEmpty(taskName) ? "unknown" : taskName, formatStr, displayArgs, scheduler);
        }

        var isFatal = taskName == "$fatal";
        var isFinish = taskName == "$finish";

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

        return new SystemCallStatement(string.IsNullOrEmpty(taskName) ? "unknown" : taskName, formatStr, displayArgs, scheduler);
    }

    private IStatement ElaboratePostIncrementHelper(SvUnaryOp unaryOp)
    {
        if (unaryOp.Operand is not SvNamedValue namedVal) return new BlockStatement([]);

        var addr = namedVal.ResolvedSymbol?.Addr ?? ExpressionElaborator.ExtractId(namedVal.Symbol);
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
        var condition = exprElab.ElaborateExpression<BigInteger>(caseAst.Expr);
        var items = new List<(IExpression<SimLogic<BigInteger>>[], IStatement)>();

        foreach (var item in caseAst.Items)
        {
            var matches = new List<IExpression<SimLogic<BigInteger>>>();
            foreach (var expr in item.Expressions)
            {
                if (expr is SvValueRange range)
                {
                    var min = exprElab.ElaborateExpression<BigInteger>(range.Left!);
                    var max = exprElab.ElaborateExpression<BigInteger>(range.Right!);
                    matches.Add(new RangeMatchExpr(min, max));
                }
                else
                {
                    matches.Add(exprElab.ElaborateExpression<BigInteger>(expr));
                }
            }

            items.Add((matches.ToArray(), ElaborateStatement(item.Stmt)));
        }

        var defaultCase = caseAst.DefaultCase != null ? ElaborateStatement(caseAst.DefaultCase) : null;
        return new CaseStatement<BigInteger>(condition, items, defaultCase);
    }

    private IStatement ElaboratePatternCase(SvPatternCase pCase)
    {
        var condition = exprElab.ElaborateExpression<BigInteger>(pCase.Expr);
        var items = new List<(IExpression<SimLogic<BigInteger>>[], IStatement)>();

        foreach (var item in pCase.Items)
        {
            if (item.Pattern is SvVariable varPattern)
            {
                var addr = varPattern.Variable.Addr;
                var width = StructuralElaborator.ParseWidth(varPattern.Variable.Type);
                var sig = new LogicVar<BigInteger>(width, new SimLogic<BigInteger>(0, 0));
                exprElab.RegisterSignal(addr, sig);
            }

            var matches = new List<IExpression<SimLogic<BigInteger>>>();
            if (item.Pattern is SvConstant constantPattern)
            {
                matches.Add(exprElab.ElaborateExpression<BigInteger>(constantPattern.Expr));
            }
            else
            {
                // For other patterns, we'll need a way to represent the match.
                // For now, if it's a variable pattern, it matches everything (like default or a wildcard).
                // But in SV, pattern matching is more complex.
                // If it's a variable pattern, it's basically a "match and bind".
                // As a simplified version, we can use a Literal 1 to match everything.
                matches.Add(new LiteralExpr<SimLogic<BigInteger>>(new SimLogic<BigInteger>(1, 0)));
            }

            items.Add((matches.ToArray(), ElaborateStatement(item.Stmt)));
        }

        var defaultCase = pCase.DefaultCase != null ? ElaborateStatement(pCase.DefaultCase) : null;
        return new CaseStatement<BigInteger>(condition, items, defaultCase);
    }
}
