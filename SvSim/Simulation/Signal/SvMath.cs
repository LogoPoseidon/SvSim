using System.Numerics;

namespace SvSim.Simulation.Signal;


public static class SvMath
{
    public static T GetMask<T>(int width) where T : IBinaryInteger<T>
    {
        var maxBits = T.AllBitsSet.GetByteCount() * 8;
        if (width >= maxBits) return T.AllBitsSet;
        return (T.One << width) - T.One;
    }
}