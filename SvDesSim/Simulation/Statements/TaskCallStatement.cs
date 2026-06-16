using System.Numerics;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class TaskCallStatement(
    IStatement taskBody, 
    List<ISimLogicSignal> targetArguments, 
    List<IExpression<SimLogic<BigInteger>>> callerArguments) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        for (var i = 0; i < callerArguments.Count; i++)
        {
            var evalResult = callerArguments[i].Evaluate();
            
            targetArguments[i].AssignFromBigInteger(evalResult.Value, evalResult.Unknown);
        }
        
        foreach (var instruction in taskBody.Execute())
        {
            yield return instruction;
        }
    }
}