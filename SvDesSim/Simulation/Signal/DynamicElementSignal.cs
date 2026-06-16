using System.Numerics;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Signal;

public class DynamicElementSignal(object container, IExpression<SimLogic<BigInteger>> indexExpr)
    : IStructSignal
{
    private ISimLogicSignal GetTarget()
    {
        var idx = indexExpr.Evaluate().Value;
        return container switch
        {
            DynamicArrayVar<ISimLogicSignal> dyn => dyn[(int)idx],
            QueueVar<ISimLogicSignal> q => q[(int)idx],
            AssociativeArrayVar<BigInteger, ISimLogicSignal> aa => aa[idx],
            ISimLogicSignal sig => new SliceSignal(sig, (int)idx, (int)idx),
            _ => throw new InvalidOperationException($"Cannot dynamically index into {container.GetType().Name}")
        };
    }

    public string StructTypeName
    {
        get
        {
            return container switch
            {
                DynamicArrayVar<ISimLogicSignal> dyn => dyn.ElementTypeName,
                QueueVar<ISimLogicSignal> q => q.ElementTypeName,
                AssociativeArrayVar<BigInteger, ISimLogicSignal> aa => aa.ElementTypeName,
                _ => (GetTarget() as IStructSignal)?.StructTypeName ?? ""
            };
        }
        set
        {
            if (GetTarget() is IStructSignal s) s.StructTypeName = value;
        }
    }

    public int BitWidth => container is ISimLogicSignal ? 1 : GetTarget().BitWidth;

    public long EnumTypeId
    {
        get => GetTarget().EnumTypeId;
        set => GetTarget().EnumTypeId = value;
    }

    public SimLogic<TOut> ReadAsLogic<TOut>() where TOut : IBinaryInteger<TOut> => GetTarget().ReadAsLogic<TOut>();
    public SimLogic<BigInteger> ReadSlice(int msb, int lsb) => GetTarget().ReadSlice(msb, lsb);
    public void WriteSlice(int msb, int lsb, SimLogic<BigInteger> value) => GetTarget().WriteSlice(msb, lsb, value);

    public void AssignFromBigInteger(BigInteger value, BigInteger unknown = default) =>
        GetTarget().AssignFromBigInteger(value, unknown);

    public BigInteger GetValueAsBigInt() => GetTarget().GetValueAsBigInt();

    public void Subscribe(ISimEvent consumer)
    {
        if (container is ISimEventSource src) src.Subscribe(consumer);
    }

    public void Unsubscribe(ISimEvent consumer)
    {
        if (container is ISimEventSource src) src.Unsubscribe(consumer);
    }
}