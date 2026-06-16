using System.Numerics;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class ForeachStatement(
    object arrayObj, 
    ISimLogicSignal? indexSignal, 
    IStatement body) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var size = arrayObj switch
        {
            DynamicArrayVar<ISimLogicSignal> dyn => dyn.Size,
            QueueVar<ISimLogicSignal> q => q.Size,
            AssociativeArrayVar<BigInteger, ISimLogicSignal> aa => aa.Size,
            _ => 0
        };

        for (var i = 0; i < size; i++)
        {
            indexSignal?.AssignFromBigInteger(i, 0);

            foreach (var inst in body.Execute())
            {
                yield return inst;
            }
        }
    }
}