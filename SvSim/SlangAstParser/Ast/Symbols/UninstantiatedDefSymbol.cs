using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Symbols;

public record UninstantiatedDefSymbol : SvSymbol
{
    public string? DefinitionName;
    public SvExpression[] ParamExpressions = [];
};