namespace SvSim.SlangAstParser.AstTree.SvEnums;

public enum SvConstraintKind
{
    Invalid,
    List,
    Expression,
    Implication,
    Conditional,
    Uniqueness,
    DisableSoft,
    SolveBefore,
    Foreach 
}