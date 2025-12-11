using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Symbols.ValueSymbols;

public record ModportPortSymbol : ValueSymbol
{
    public ArgumentDirection? Direction;
    public SvSymbol? InternalSymbol;
    public SvExpression? ExplicitConnection;
};