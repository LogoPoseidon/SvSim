using System.Numerics;

namespace SvSim.Elaboration;

public static class EnumRegistry
{
    private static readonly Dictionary<long, Dictionary<BigInteger, string>> EnumMappings = new();

    public static readonly Dictionary<string, int> EnumWidths = new();
    public static void Register(long signalAddr, Dictionary<BigInteger, string> mapping)
    {
        EnumMappings[signalAddr] = mapping;
    }

    public static string GetName(long signalAddr, BigInteger value)
    {
        if (EnumMappings.TryGetValue(signalAddr, out var map) && map.TryGetValue(value, out var name))
            return name;
        
        foreach (var fallbackMap in EnumMappings.Values)
        {
            if (fallbackMap.TryGetValue(value, out var fallbackName))
                return fallbackName;
        }

        return value.ToString();
    }
}