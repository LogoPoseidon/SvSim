using System.Reflection;
using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.AssertionExpr;
using SvSim.SlangAstParser.AstTree.BinsSelectExpr;
using SvSim.SlangAstParser.AstTree.Constraint;
using SvSim.SlangAstParser.AstTree.TimingControl;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.Pattern;
using SvSim.SlangAstParser.AstTree.Symbol;
using SvSim.SlangAstParser.AstTree.Statement;

namespace SvSim.SlangAstParser.Tests;

[TestFixture]
public class AstPolymorphicMappingTests
{
    [Test]
    public void Verify_TimingControl_Mappings_Are_Complete()
    {
        AssertPolymorphicMappingComplete<ISvTimingControl, SvTimingControlKind>();
    }
    [Test]
    public void Verify_AssertionExpr_Mappings_Are_Complete()
    {
        AssertPolymorphicMappingComplete<ISvAssertionExpr, SvAssertionExprKind>();
    }
    [Test]
    public void Verify_BinsSelectExpr_Mappings_Are_Complete()
    {
        AssertPolymorphicMappingComplete<ISvBinsSelectExpr, SvBinsSelectExprKind>();
    }
    [Test]
    public void Verify_Constraint_Mappings_Are_Complete()
    {
        AssertPolymorphicMappingComplete<ISvConstraint, SvConstraintKind>();
    }
    
    [Test]
    public void Verify_Expression_Mappings_Are_Complete()
    {
        AssertPolymorphicMappingComplete<ISvExpression, SvExpressionKind>();
    }
    [Test]
    public void Verify_Pattern_Mappings_Are_Complete()
    {
        AssertPolymorphicMappingComplete<ISvPattern, SvPatternKind>();
    }
    
    [Test]
    public void Verify_Symbol_Mappings_Are_Complete()
    {
        var ignoredKinds = new[] { SvSymbolKind.DeferredMember }; 
        AssertPolymorphicMappingComplete<ISvSymbol, SvSymbolKind>(ignoredKinds);
    }

    [Test]
    public void Verify_Statement_Mappings_Are_Complete()
    {
        AssertPolymorphicMappingComplete<ISvStatement, SvStatementKind>();
    }
    
    private static void AssertPolymorphicMappingComplete<TInterface, TEnum>(params TEnum[] ignoredKinds) 
        where TEnum : struct, Enum
    {
        var interfaceType = typeof(TInterface);
        var enumType = typeof(TEnum);

        var expectedDiscriminators = Enum.GetValues<TEnum>()
            .Where(e => !ignoredKinds.Contains(e))
            .Select(e => e.ToString())
            .ToHashSet();

        var derivedAttributes = interfaceType.GetCustomAttributes<JsonDerivedTypeAttribute>(inherit: false)
            .ToList();

        using (Assert.EnterMultipleScope())
        {
            foreach (var kindName in expectedDiscriminators)
            {
                var matchingAttribute = derivedAttributes.FirstOrDefault(attr => 
                    attr.TypeDiscriminator?.ToString() == kindName);

                Assert.That(matchingAttribute, Is.Not.Null,
                    $"Enum value '{kindName}' in {enumType.Name} is missing a corresponding " +
                    $"[JsonDerivedType(typeof(...), nameof({enumType.Name}.{kindName}))] mapping on {interfaceType.Name}.");

                if (matchingAttribute is null) continue;
                var concreteType = matchingAttribute.DerivedType;

                Assert.That(interfaceType.IsAssignableFrom(concreteType), Is.True,
                    $"The mapped type '{concreteType.Name}' registered for '{kindName}' " +
                    $"does not implement interface {interfaceType.Name}.");

                Assert.That(concreteType is { IsClass: true, IsAbstract: false }, Is.True,
                    $"The mapped type '{concreteType.Name}' registered for '{kindName}' " +
                    $"must be a concrete, non-abstract class or record.");
            }

            foreach (var attr in derivedAttributes)
            {
                var discriminatorStr = attr.TypeDiscriminator?.ToString();
                if (discriminatorStr != null)
                {
                    Assert.That(expectedDiscriminators, Does.Contain(discriminatorStr),
                        $"Interface {interfaceType.Name} has a mapping for '{discriminatorStr}' (Type: {attr.DerivedType.Name}), " +
                        $"but this value does not exist in the enum {enumType.Name}.");
                }
            }
        }
    }
}