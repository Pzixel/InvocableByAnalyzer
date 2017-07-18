using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using InvocableByAnalyzer.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace InvocableByAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvocableByDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "InvocableByAnalyzer";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Invocation";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ObjectCreationExpression);
        }

        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var invocationSymbolInfo = context.SemanticModel.GetSymbolInfo(context.Node);
            var invocableByAttributeData = invocationSymbolInfo.Symbol.GetAttributes().FirstOrDefault(data => data.AttributeClass.Name == nameof(InvocableByAttribute));
            if (invocableByAttributeData == null)
                return;
            var parentBlock = GetContainingType(context.Node);
            var parentBlockSymbol = context.SemanticModel.GetDeclaredSymbol(parentBlock);
            var allowedTypes = invocableByAttributeData.ConstructorArguments.Single().Values;
            if (!allowedTypes.Any(type => type.Value == parentBlockSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), parentBlock.Identifier.ValueText));
            }
        }

        private static TypeDeclarationSyntax GetContainingType(SyntaxNode contextNode)
        {
            while (true)
            {
                if (contextNode is TypeDeclarationSyntax typeDeclarationSyntax)
                    return typeDeclarationSyntax;
                contextNode = contextNode.Parent;
            }
        }
    }
}
