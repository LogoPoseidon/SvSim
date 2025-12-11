namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record QueueType : SvType
{
    public SvType? ElementType;
    public uint? MaxBound;
};