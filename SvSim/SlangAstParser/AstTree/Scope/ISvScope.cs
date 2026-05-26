using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.Symbol;
using SvSim.SlangAstParser.AstTree.Symbol.Type;
using SvSim.SlangAstParser.AstTree.Symbol.Type.IntegralType;


namespace SvSim.SlangAstParser.AstTree.Scope;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvAnonymousProgram), nameof(SvSymbolKind.AnonymousProgram))]
[JsonDerivedType(typeof(SvCheckerInstanceBody), nameof(SvSymbolKind.CheckerInstanceBody))]
[JsonDerivedType(typeof(SvChecker), nameof(SvSymbolKind.Checker))]
[JsonDerivedType(typeof(SvClassType), nameof(SvSymbolKind.ClassType))]
[JsonDerivedType(typeof(SvClockingBlock), nameof(SvSymbolKind.ClockingBlock))]
[JsonDerivedType(typeof(SvCompilationUnit), nameof(SvSymbolKind.CompilationUnit))]
[JsonDerivedType(typeof(SvConfigBlock), nameof(SvSymbolKind.ConfigBlock))]
[JsonDerivedType(typeof(SvConstraintBlock), nameof(SvSymbolKind.ConstraintBlock))]
[JsonDerivedType(typeof(SvCoverCrossBody), nameof(SvSymbolKind.CoverCrossBody))]
[JsonDerivedType(typeof(SvCoverCross), nameof(SvSymbolKind.CoverCross))]
[JsonDerivedType(typeof(SvCovergroupBody), nameof(SvSymbolKind.CovergroupBody))]
[JsonDerivedType(typeof(SvCovergroup), nameof(SvSymbolKind.CovergroupType))]
[JsonDerivedType(typeof(SvCoverpoint), nameof(SvSymbolKind.Coverpoint))]
[JsonDerivedType(typeof(SvEnum), nameof(SvSymbolKind.EnumType))]
[JsonDerivedType(typeof(SvGenerateBlockArray), nameof(SvSymbolKind.GenerateBlockArray))]
[JsonDerivedType(typeof(SvGenerateBlock), nameof(SvSymbolKind.GenerateBlock))]
[JsonDerivedType(typeof(SvInstanceArray), nameof(SvSymbolKind.InstanceArray))]
[JsonDerivedType(typeof(SvInstanceBody), nameof(SvSymbolKind.InstanceBody))]
[JsonDerivedType(typeof(SvLetDecl), nameof(SvSymbolKind.LetDecl))]
[JsonDerivedType(typeof(SvMethodPrototype), nameof(SvSymbolKind.MethodPrototype))]
[JsonDerivedType(typeof(SvModport), nameof(SvSymbolKind.Modport))]
[JsonDerivedType(typeof(SvPackage), nameof(SvSymbolKind.Package))]
[JsonDerivedType(typeof(SvPackedStruct), nameof(SvSymbolKind.PackedStructType))]
[JsonDerivedType(typeof(SvPackedUnion), nameof(SvSymbolKind.PackedUnionType))]
[JsonDerivedType(typeof(SvPrimitive), nameof(SvSymbolKind.Primitive))]
[JsonDerivedType(typeof(SvProperty), nameof(SvSymbolKind.Property))]
[JsonDerivedType(typeof(SvRandSeqProduction), nameof(SvSymbolKind.RandSeqProduction))]
[JsonDerivedType(typeof(SvRoot), nameof(SvSymbolKind.Root))]
[JsonDerivedType(typeof(SvSequence), nameof(SvSymbolKind.Sequence))]
[JsonDerivedType(typeof(SvSpecifyBlock), nameof(SvSymbolKind.SpecifyBlock))]
[JsonDerivedType(typeof(SvStatementBlock), nameof(SvSymbolKind.StatementBlock))]
[JsonDerivedType(typeof(SvSubroutine), nameof(SvSymbolKind.Subroutine))]
[JsonDerivedType(typeof(SvUnpackedStructType), nameof(SvSymbolKind.UnpackedStructType))]
[JsonDerivedType(typeof(SvUnpackedUnionType), nameof(SvSymbolKind.UnpackedUnionType))]
public interface ISvScope : ISvAstNode
{
    public ISvSymbol[]? Members { get; init; }

}