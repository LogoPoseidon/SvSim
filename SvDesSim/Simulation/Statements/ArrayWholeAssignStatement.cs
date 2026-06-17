using System.Numerics;
using SvDesSim.Simulation.Engine;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class ArrayWholeAssignStatement(object targetObj, IExpression<SimLogic<BigInteger>> rhsExpr, EventScheduler? scheduler = null) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var val = rhsExpr.Evaluate();
        if (scheduler != null) 
        {
            scheduler.Schedule(EventRegion.Nba, new NbaArrayUpdateEvent(targetObj, val));
        }
        else 
        {
            PerformAssign(targetObj, val);
        }
        yield break;
    }

    public static void PerformAssign(object targetObj, SimLogic<BigInteger> evaluated)
    {
        var tempVal = evaluated.Value;
        var tempUnk = evaluated.Unknown;

        switch (targetObj)
        {
            case QueueVar<ISimLogicSignal> q:
            {
                q.Delete();
                var dummy = q.Factory();
                var w = dummy.BitWidth;
                if (w <= 0) w = 1;

                var elements = new List<ISimLogicSignal>();
                var loopCount = 0;
                while ((tempVal > 0 || tempUnk > 0) && loopCount < 10000)
                {
                    var mask = (BigInteger.One << w) - 1;
                    var el = q.Factory();
                    el.AssignFromBigInteger(tempVal & mask, tempUnk & mask);
                    elements.Insert(0, el);
                    tempVal >>= w;
                    tempUnk >>= w;
                    loopCount++;
                }
                if (elements.Count == 0)
                {
                    var el = q.Factory();
                    el.AssignFromBigInteger(0, 0);
                    elements.Add(el);
                }
                foreach (var e in elements) q.PushBack(e);
                break;
            }
            case DynamicArrayVar<ISimLogicSignal> dyn:
            {
                var dummy = dyn.Factory();
                var w = dummy.BitWidth;
                if (w <= 0) w = 1;

                var elements = new List<ISimLogicSignal>();
                var loopCount = 0;
                while ((tempVal > 0 || tempUnk > 0) && loopCount < 10000)
                {
                    var mask = (BigInteger.One << w) - 1;
                    var el = dyn.Factory();
                    el.AssignFromBigInteger(tempVal & mask, tempUnk & mask);
                    elements.Insert(0, el);
                    tempVal >>= w;
                    tempUnk >>= w;
                    loopCount++;
                }
                if (elements.Count == 0)
                {
                    var el = dyn.Factory();
                    el.AssignFromBigInteger(0, 0);
                    elements.Add(el);
                }
            
                dyn.New(elements.Count);
                for(var i = 0; i < elements.Count; i++)
                {
                    dyn[i] = elements[i];
                }

                break;
            }
        }
    }
}

public class NbaArrayUpdateEvent(object targetObj, SimLogic<BigInteger> evaluated) : ISimEvent
{
    public void Execute() => ArrayWholeAssignStatement.PerformAssign(targetObj, evaluated);
    public void Trigger() { }
}
