using System.Numerics;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class ReturnException(SimLogic<BigInteger>? returnValue) : Exception
{
    public SimLogic<BigInteger>? ReturnValue { get; } = returnValue;
}