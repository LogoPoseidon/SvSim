using System.Numerics;
using System.Text;
using SvSim.Elaboration;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Expressions;

public class EnumNameExpr<T>(ISimLogicSignal signal) : IExpression<SimLogic<T>> 
    where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate()
    {
        var val = signal.GetValueAsBigInt();
        var name = EnumRegistry.GetName(signal.EnumTypeId, val);
    
        var bytes = System.Text.Encoding.ASCII.GetBytes(name);
        var strVal = bytes.Aggregate<byte, BigInteger>(0, (current, b) => (current << 8) | b);

        return new SimLogic<T>(T.CreateTruncating(strVal), T.Zero);
    }
}