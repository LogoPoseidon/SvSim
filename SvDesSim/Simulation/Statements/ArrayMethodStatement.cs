using System.Numerics;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class ArrayMethodStatement(object? targetObj, string methodName, List<IExpression<SimLogic<BigInteger>>> args) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        switch (targetObj)
        {
            case QueueVar<ISimLogicSignal> q:
                switch (methodName)
                {
                    case "push_back":
                    {
                        var newElement = q.Factory();
                        var val = args[0].Evaluate();
                        newElement.AssignFromBigInteger(val.Value, val.Unknown);
                        q.PushBack(newElement);
                        break;
                    }
                    case "push_front":
                    {
                        var newElement = q.Factory();
                        var val = args[0].Evaluate();
                        newElement.AssignFromBigInteger(val.Value, val.Unknown);
                        q.PushFront(newElement);
                        break;
                    }
                    case "pop_back": q.PopBack(); break;
                    case "pop_front": q.PopFront(); break;
                    case "delete": q.Delete(); break;
                }

                break;
            case DynamicArrayVar<ISimLogicSignal> dyn:
            {
                if (methodName == "delete") dyn.Delete();
                break;
            }
            case AssociativeArrayVar<BigInteger, ISimLogicSignal> aa:
            {
                if (methodName == "delete")
                {
                    if (args.Count > 0) aa.Delete(args[0].Evaluate().Value);
                    else aa.Clear();
                }

                break;
            }
        }

        yield break;
    }
}