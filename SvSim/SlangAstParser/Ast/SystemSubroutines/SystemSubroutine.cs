using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.SystemSubroutines;

public abstract record SystemSubroutine : AstNode
{
    public string? Name;
    public SubroutineKind? Kind;
    public string? KnownNameId;
    public bool? HasOutputArgs;
    public bool? NeverReturns;
    public WithClauseMode? WithClauseMode;
};
public enum WithClauseMode
{
    None,
    Iterator,
    Randomize
}