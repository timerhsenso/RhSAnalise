// =============================================================================
// RHSENSOERP SOURCE GENERATOR - MAIN GENERATOR
// =============================================================================

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using RhSensoERP.Generators.Extractors;
using RhSensoERP.Generators.Templates;

namespace RhSensoERP.Generators;

[Generator]
public class CrudSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Registrar atributos embutidos
        context.RegisterPostInitializationOutput(RegisterAttributes);

        // Pipeline
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsCandidateClass,
                transform: GetClassToGenerate)
            .Where(static c => c is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, Execute);
    }

    private static void RegisterAttributes(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("GenerateCrudAttribute.g.cs", SourceText.From(GetGenerateCrudAttributeSource(), Encoding.UTF8));
        context.AddSource("PropertyAttributes.g.cs", SourceText.From(GetPropertyAttributesSource(), Encoding.UTF8));
    }

    private static bool IsCandidateClass(SyntaxNode node, CancellationToken _)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    private static ClassDeclarationSyntax? GetClassToGenerate(GeneratorSyntaxContext context, CancellationToken _)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var name = attribute.Name.ToString();
                if (name is "GenerateCrud" or "GenerateCrudAttribute")
                    return classDeclaration;
            }
        }

        return null;
    }

    private static void Execute(
        SourceProductionContext context,
        (Compilation Compilation, ImmutableArray<ClassDeclarationSyntax?> Classes) input)
    {
        var (compilation, classes) = input;

        if (classes.IsDefaultOrEmpty) return;

        foreach (var classDeclaration in classes.Distinct())
        {
            if (classDeclaration == null) continue;

            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

            if (classSymbol == null) continue;

            GenerateForClass(context, classSymbol);
        }
    }

    private static void GenerateForClass(SourceProductionContext context, INamedTypeSymbol classSymbol)
    {
        var entity = EntityInfoExtractor.Extract(classSymbol);
        if (entity == null) return;

        var config = entity.Config;
        var name = entity.ClassName;

        // DTOs/Requests
        if (config.GenerateDto)
            AddSource(context, $"{name}Dto.g.cs", DtoTemplate.GenerateReadDto(entity));

        if (config.GenerateCreateDto)
            AddSource(context, $"Create{name}Request.g.cs", DtoTemplate.GenerateCreateRequest(entity));

        if (config.GenerateUpdateDto)
            AddSource(context, $"Update{name}Request.g.cs", DtoTemplate.GenerateUpdateRequest(entity));

        // Commands
        if (config.GenerateCreateCommand)
            AddSource(context, $"Create{name}Command.g.cs", CommandsTemplate.GenerateCreateCommand(entity));

        if (config.GenerateUpdateCommand)
            AddSource(context, $"Update{name}Command.g.cs", CommandsTemplate.GenerateUpdateCommand(entity));

        if (config.GenerateDeleteCommand)
            AddSource(context, $"Delete{name}Command.g.cs", CommandsTemplate.GenerateDeleteCommand(entity));

        if (config.GenerateDeleteBatchCommand)
            AddSource(context, $"Delete{entity.PluralName}Command.g.cs", CommandsTemplate.GenerateDeleteBatchCommand(entity));

        // Queries
        if (config.GenerateGetByIdQuery)
            AddSource(context, $"Get{name}ByIdQuery.g.cs", QueriesTemplate.GenerateGetByIdQuery(entity));

        if (config.GenerateGetPagedQuery)
            AddSource(context, $"Get{entity.PluralName}PagedQuery.g.cs", QueriesTemplate.GenerateGetPagedQuery(entity));

        // Validators
        if (config.GenerateCreateValidator)
            AddSource(context, $"Create{name}RequestValidator.g.cs", ValidatorsTemplate.GenerateCreateValidator(entity));

        if (config.GenerateUpdateValidator)
            AddSource(context, $"Update{name}RequestValidator.g.cs", ValidatorsTemplate.GenerateUpdateValidator(entity));

        // Repository
        if (config.GenerateRepositoryInterface)
            AddSource(context, $"I{name}Repository.g.cs", RepositoryTemplate.GenerateInterface(entity));

        if (config.GenerateRepositoryImplementation)
            AddSource(context, $"{name}Repository.g.cs", RepositoryTemplate.GenerateImplementation(entity));

        // Mapper
        if (config.GenerateMapperProfile)
            AddSource(context, $"{name}Profile.g.cs", MapperProfileTemplate.Generate(entity));

        // EF Configuration
        if (config.GenerateEfConfiguration)
            AddSource(context, $"{name}Configuration.g.cs", EfConfigurationTemplate.Generate(entity));
    }

    private static void AddSource(SourceProductionContext context, string fileName, string source)
    {
        context.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
    }

    // =========================================================================
    // ATRIBUTOS EMBUTIDOS
    // =========================================================================

    private static string GetGenerateCrudAttributeSource() => @"// <auto-generated />
namespace RhSensoERP.Generators;

[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateCrudAttribute : System.Attribute
{
    // DTOs/Requests
    public bool GenerateDto { get; set; } = true;
    public bool GenerateCreateDto { get; set; } = true;
    public bool GenerateUpdateDto { get; set; } = true;

    // Commands
    public bool GenerateCreateCommand { get; set; } = true;
    public bool GenerateUpdateCommand { get; set; } = true;
    public bool GenerateDeleteCommand { get; set; } = true;
    public bool GenerateDeleteBatchCommand { get; set; } = true;

    // Queries
    public bool GenerateGetByIdQuery { get; set; } = true;
    public bool GenerateGetPagedQuery { get; set; } = true;

    // Validators
    public bool GenerateCreateValidator { get; set; } = true;
    public bool GenerateUpdateValidator { get; set; } = true;

    // Repository
    public bool GenerateRepositoryInterface { get; set; } = true;
    public bool GenerateRepositoryImplementation { get; set; } = true;

    // Mapper & EF
    public bool GenerateMapperProfile { get; set; } = true;
    public bool GenerateEfConfiguration { get; set; } = true;

    // Banco
    public string? TableName { get; set; }
    public string? Schema { get; set; }

    // Display
    public string? DisplayName { get; set; }

    // Namespaces
    public string? BaseNamespace { get; set; }
    public string? DtoNamespace { get; set; }
    public string? CommandsNamespace { get; set; }
    public string? QueriesNamespace { get; set; }
    public string? ValidatorsNamespace { get; set; }
    public string? RepositoryNamespace { get; set; }
    public string? ConfigurationNamespace { get; set; }

    // Helpers
    public bool GenerateAll { set { 
        GenerateDto = value; GenerateCreateDto = value; GenerateUpdateDto = value;
        GenerateCreateCommand = value; GenerateUpdateCommand = value; 
        GenerateDeleteCommand = value; GenerateDeleteBatchCommand = value;
        GenerateGetByIdQuery = value; GenerateGetPagedQuery = value;
        GenerateCreateValidator = value; GenerateUpdateValidator = value;
        GenerateRepositoryInterface = value; GenerateRepositoryImplementation = value;
        GenerateMapperProfile = value; GenerateEfConfiguration = value;
    }}
}
";

    private static string GetPropertyAttributesSource() => @"// <auto-generated />
namespace RhSensoERP.Generators;

[System.AttributeUsage(System.AttributeTargets.Property)]
public sealed class ColumnNameAttribute : System.Attribute
{
    public string Name { get; }
    public ColumnNameAttribute(string name) => Name = name;
}

[System.AttributeUsage(System.AttributeTargets.Property)]
public sealed class FieldDisplayNameAttribute : System.Attribute
{
    public string DisplayName { get; }
    public FieldDisplayNameAttribute(string displayName) => DisplayName = displayName;
}

[System.AttributeUsage(System.AttributeTargets.Property)]
public sealed class ReadOnlyFieldAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Property)]
public sealed class IgnoreInDtoAttribute : System.Attribute { }
";
}
