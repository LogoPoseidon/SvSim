using System.Numerics;
using System.Runtime.CompilerServices;

namespace SvSim.Simulation.Signal;

public static class SimLogicExtensions
{
    public static string ToVcdString<T>(this SimLogic<T> logic, int width) 
        where T : IBinaryInteger<T>
    {
        if (width == 1) return GetBitChar(logic, 0).ToString();
        
        Span<char> chars = stackalloc char[width + 1];
        chars[0] = 'b';
        for (var i = 0; i < width; i++)
        {
            chars[width - i] = GetBitChar(logic, i);
        }
        return new string(chars);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char GetBitChar<T>(SimLogic<T> logic, int bitIndex)
        where T : IBinaryInteger<T>
    {
        var mask = T.One << bitIndex;
        var v = (logic.Value & mask) != T.Zero;
        var u = (logic.Unknown & mask) != T.Zero;

        if (!u) return v ? '1' : '0';
        return v ? 'x' : 'z';
    }
}