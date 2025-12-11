using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record TypeAliasType : SvType
{
    public DeclaredType? TargetType;
    public Visibility? Visibility;
};