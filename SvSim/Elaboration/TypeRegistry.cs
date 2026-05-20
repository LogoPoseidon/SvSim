using System.Collections.Generic;

namespace SvSim.Elaboration;

public class TypeDefinition
{
    public string Name { get; set; } = "";
    public bool IsStruct { get; set; }
    public bool IsUnion { get; set; }
    public bool IsPacked { get; set; }
    public Dictionary<string, (int Msb, int Lsb, string SubType)> Fields { get; set; } = new();
}

public static class TypeRegistry
{
    private static readonly Dictionary<string, TypeDefinition> Types = new();

    public static void Register(string typeName, TypeDefinition def)
    {
        Types[typeName] = def;
    }

    public static bool TryGetType(string typeName, out TypeDefinition def)
    {
        return Types.TryGetValue(typeName, out def!);
    }
}