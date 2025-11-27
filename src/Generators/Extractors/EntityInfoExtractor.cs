// =============================================================================
// RHSENSOERP GENERATOR v3.0 - ENTITY INFO EXTRACTOR
// =============================================================================
// Arquivo: src/Generators/Extractors/EntityInfoExtractor.cs
// Versão: 3.0 - Extração completa com módulos e permissões
// =============================================================================
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Extractors;

/// <summary>
/// Extrai informações de uma Entity marcada com [GenerateCrud].
/// </summary>
public static class EntityInfoExtractor
{
    /// <summary>
    /// Extrai todas as informações necessárias de uma Entity.
    /// </summary>
    public static EntityInfo? Extract(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
            return null;

        var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (symbol is not INamedTypeSymbol typeSymbol)
            return null;

        // Busca o atributo [GenerateCrud]
        var attribute = typeSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name is "GenerateCrudAttribute" or "GenerateCrud");

        if (attribute == null)
            return null;

        var info = new EntityInfo
        {
            EntityName = typeSymbol.Name,
            FullClassName = typeSymbol.ToDisplayString(),
            Namespace = typeSymbol.ContainingNamespace.ToDisplayString()
        };

        // Extrai informações do namespace para determinar o módulo
        ExtractModuleInfo(info);

        // Extrai valores do atributo
        ExtractAttributeValues(attribute, info);

        // Extrai propriedades da classe
        ExtractProperties(typeSymbol, info);

        // Determina a chave primária
        DeterminePrimaryKey(info);

        // Gera valores padrão para campos não preenchidos
        ApplyDefaults(info);

        return info;
    }

    /// <summary>
    /// Extrai informações do módulo baseado no namespace.
    /// Exemplos de namespaces:
    /// - RhSensoERP.Identity.Domain.Entities
    /// - RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal
    /// - RhSensoERP.Modules.ControleDePonto.Core.Entities
    /// </summary>
    private static void ExtractModuleInfo(EntityInfo info)
    {
        var parts = info.Namespace.Split('.');

        if (parts.Length < 2 || parts[0] != "RhSensoERP")
        {
            // Fallback para namespaces não esperados
            info.ModuleName = parts.Length > 1 ? parts[1] : "Core";
            info.ModuleNamespace = string.Join(".", parts.Take(2));
            info.IsModulesStructure = false;
            return;
        }

        // Verifica se é estrutura de Modules (RhSensoERP.Modules.{Nome})
        if (parts.Length >= 3 && parts[1] == "Modules")
        {
            // Exemplo: RhSensoERP.Modules.GestaoDePessoas.Core.Entities...
            info.ModuleName = parts[2]; // GestaoDePessoas, ControleDePonto, etc.
            info.ModuleNamespace = $"RhSensoERP.Modules.{parts[2]}";
            info.IsModulesStructure = true;
        }
        else
        {
            // Módulos raiz: Identity, Shared, etc.
            // Exemplo: RhSensoERP.Identity.Domain.Entities
            info.ModuleName = parts[1]; // Identity, Shared, etc.
            info.ModuleNamespace = $"RhSensoERP.{parts[1]}";
            info.IsModulesStructure = false;
        }
    }

    /// <summary>
    /// Extrai os valores do atributo [GenerateCrud].
    /// </summary>
    private static void ExtractAttributeValues(AttributeData attribute, EntityInfo info)
    {
        foreach (var namedArg in attribute.NamedArguments)
        {
            var value = namedArg.Value.Value;
            if (value == null) continue;

            switch (namedArg.Key)
            {
                // Configurações básicas
                case "TableName":
                    info.TableName = value.ToString()!;
                    break;
                case "Schema":
                    info.Schema = value.ToString()!;
                    break;
                case "DisplayName":
                    info.DisplayName = value.ToString()!;
                    break;

                // Módulo e permissões
                case "CdSistema":
                    info.CdSistema = value.ToString()!;
                    break;
                case "CdFuncao":
                    info.CdFuncao = value.ToString()!;
                    break;

                // Rotas e API
                case "ApiRoute":
                    info.ApiRoute = value.ToString()!;
                    break;
                case "ApiGroup":
                    info.ApiGroup = value.ToString()!;
                    break;

                // Flags de geração - Backend
                case "GenerateDto":
                    info.GenerateDto = (bool)value;
                    break;
                case "GenerateRequests":
                    info.GenerateRequests = (bool)value;
                    break;
                case "GenerateCommands":
                    info.GenerateCommands = (bool)value;
                    break;
                case "GenerateQueries":
                    info.GenerateQueries = (bool)value;
                    break;
                case "GenerateValidators":
                    info.GenerateValidators = (bool)value;
                    break;
                case "GenerateRepository":
                    info.GenerateRepository = (bool)value;
                    break;
                case "GenerateMapper":
                    info.GenerateMapper = (bool)value;
                    break;
                case "GenerateEfConfig":
                    info.GenerateEfConfig = (bool)value;
                    break;

                // Flags de geração - API e Web
                case "GenerateApiController":
                    info.GenerateApiController = (bool)value;
                    break;
                case "GenerateWebController":
                    info.GenerateWebController = (bool)value;
                    break;
                case "GenerateWebModels":
                    info.GenerateWebModels = (bool)value;
                    break;
                case "GenerateWebServices":
                    info.GenerateWebServices = (bool)value;
                    break;
                case "ApiRequiresAuth":
                    info.ApiRequiresAuth = (bool)value;
                    break;

                // Funcionalidades
                case "SupportsBatchDelete":
                    info.SupportsBatchDelete = (bool)value;
                    break;
                case "IsLegacyTable":
                    info.IsLegacyTable = (bool)value;
                    break;
                case "PrimaryKeyProperty":
                    info.PrimaryKeyProperty = value.ToString()!;
                    break;
                case "PrimaryKeyType":
                    info.PrimaryKeyType = value.ToString()!;
                    break;
            }
        }
    }

    /// <summary>
    /// Extrai as propriedades da classe.
    /// </summary>
    private static void ExtractProperties(INamedTypeSymbol typeSymbol, EntityInfo info)
    {
        // Pega propriedades da classe atual
        var properties = typeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public)
            .Where(p => !p.IsStatic)
            .Where(p => p.GetMethod != null && p.SetMethod != null);

        foreach (var prop in properties)
        {
            var propInfo = new Models.PropertyInfo
            {
                Name = prop.Name,
                Type = prop.Type.ToDisplayString(),
                IsNullable = prop.Type.NullableAnnotation == NullableAnnotation.Annotated ||
                             prop.Type.ToDisplayString().EndsWith("?")
            };

            // Extrai atributos da propriedade
            foreach (var attr in prop.GetAttributes())
            {
                var attrName = attr.AttributeClass?.Name;

                switch (attrName)
                {
                    case "KeyAttribute" or "Key":
                        propInfo.IsPrimaryKey = true;
                        break;

                    case "RequiredAttribute" or "Required":
                        propInfo.IsRequired = true;
                        break;

                    case "ColumnAttribute" or "Column":
                        var columnName = attr.ConstructorArguments.FirstOrDefault().Value?.ToString();
                        if (!string.IsNullOrEmpty(columnName))
                            propInfo.ColumnName = columnName;
                        break;

                    case "StringLengthAttribute" or "StringLength":
                        if (attr.ConstructorArguments.Length > 0)
                            propInfo.MaxLength = (int)attr.ConstructorArguments[0].Value!;
                        // Verifica MinimumLength como named argument
                        var minLengthArg = attr.NamedArguments
                            .FirstOrDefault(a => a.Key == "MinimumLength");
                        if (minLengthArg.Value.Value != null)
                            propInfo.MinLength = (int)minLengthArg.Value.Value;
                        break;

                    case "MaxLengthAttribute" or "MaxLength":
                        if (attr.ConstructorArguments.Length > 0)
                            propInfo.MaxLength = (int)attr.ConstructorArguments[0].Value!;
                        break;

                    case "MinLengthAttribute" or "MinLength":
                        if (attr.ConstructorArguments.Length > 0)
                            propInfo.MinLength = (int)attr.ConstructorArguments[0].Value!;
                        break;

                    case "FieldDisplayNameAttribute" or "FieldDisplayName":
                        var displayName = attr.ConstructorArguments.FirstOrDefault().Value?.ToString();
                        if (!string.IsNullOrEmpty(displayName))
                            propInfo.DisplayName = displayName;
                        break;

                    case "ExcludeFromDtoAttribute" or "ExcludeFromDto":
                        propInfo.ExcludeFromDto = true;
                        break;

                    case "ReadOnlyFieldAttribute" or "ReadOnlyField":
                        propInfo.IsReadOnly = true;
                        break;

                    case "RequiredOnCreateAttribute" or "RequiredOnCreate":
                        propInfo.RequiredOnCreate = true;
                        break;

                    case "DatabaseGeneratedAttribute" or "DatabaseGenerated":
                        // Verifica se é Identity
                        var option = attr.ConstructorArguments.FirstOrDefault().Value;
                        if (option != null && (int)option == 1) // Identity = 1
                        {
                            propInfo.IsReadOnly = true;
                        }
                        break;
                }
            }

            // Se não tem ColumnName, usa o nome da propriedade em lowercase
            if (string.IsNullOrEmpty(propInfo.ColumnName))
                propInfo.ColumnName = prop.Name.ToLowerInvariant();

            // Se não tem DisplayName, usa o nome da propriedade
            if (string.IsNullOrEmpty(propInfo.DisplayName))
                propInfo.DisplayName = prop.Name;

            info.Properties.Add(propInfo);
        }

        // Se herda de BaseEntity, adiciona propriedades de auditoria (a menos que seja legado)
        if (!info.IsLegacyTable)
        {
            var baseType = typeSymbol.BaseType;
            while (baseType != null && baseType.Name != "Object")
            {
                if (baseType.Name == "BaseEntity")
                {
                    // Adiciona propriedades de BaseEntity se não existirem
                    AddBaseEntityPropertiesIfMissing(info);
                    break;
                }
                baseType = baseType.BaseType;
            }
        }
    }

    /// <summary>
    /// Adiciona propriedades de BaseEntity se não existirem.
    /// </summary>
    private static void AddBaseEntityPropertiesIfMissing(EntityInfo info)
    {
        var baseProps = new[]
        {
            new Models.PropertyInfo { Name = "Id", Type = "Guid", IsPrimaryKey = true, IsReadOnly = true, ColumnName = "id" },
            new Models.PropertyInfo { Name = "CreatedAt", Type = "DateTime", IsReadOnly = true, ColumnName = "createdat" },
            new Models.PropertyInfo { Name = "CreatedBy", Type = "string", IsReadOnly = true, ColumnName = "createdby", MaxLength = 100 },
            new Models.PropertyInfo { Name = "UpdatedAt", Type = "DateTime?", IsReadOnly = true, IsNullable = true, ColumnName = "updatedat" },
            new Models.PropertyInfo { Name = "UpdatedBy", Type = "string?", IsReadOnly = true, IsNullable = true, ColumnName = "updatedby", MaxLength = 100 }
        };

        foreach (var baseProp in baseProps)
        {
            if (!info.Properties.Any(p => p.Name == baseProp.Name))
            {
                info.Properties.Insert(0, baseProp);
            }
        }
    }

    /// <summary>
    /// Determina qual propriedade é a chave primária.
    /// </summary>
    private static void DeterminePrimaryKey(EntityInfo info)
    {
        // Primeiro tenta encontrar uma propriedade marcada com [Key]
        var keyProp = info.Properties.FirstOrDefault(p => p.IsPrimaryKey);

        // Se não encontrou, procura por "Id" ou "{EntityName}Id"
        if (keyProp == null)
        {
            keyProp = info.Properties.FirstOrDefault(p =>
                p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals($"{info.EntityName}Id", StringComparison.OrdinalIgnoreCase));

            if (keyProp != null)
                keyProp.IsPrimaryKey = true;
        }

        if (keyProp != null)
        {
            info.PrimaryKeyProperty = keyProp.Name;
            info.PrimaryKeyColumn = keyProp.ColumnName;
            info.PrimaryKeyType = keyProp.BaseType;
        }
    }

    /// <summary>
    /// Aplica valores padrão para campos não preenchidos.
    /// </summary>
    private static void ApplyDefaults(EntityInfo info)
    {
        // DisplayName padrão = EntityName
        if (string.IsNullOrEmpty(info.DisplayName))
            info.DisplayName = info.EntityName;

        // PluralName padrão
        if (string.IsNullOrEmpty(info.PluralName))
            info.PluralName = Pluralize(info.EntityName);

        // TableName padrão = EntityName em lowercase
        if (string.IsNullOrEmpty(info.TableName))
            info.TableName = info.EntityName.ToLowerInvariant();

        // ApiRoute padrão = module/entities
        if (string.IsNullOrEmpty(info.ApiRoute))
            info.ApiRoute = $"{info.ModuleName.ToLowerInvariant()}/{info.PluralName.ToLowerInvariant()}";

        // ApiGroup padrão = ModuleName
        if (string.IsNullOrEmpty(info.ApiGroup))
            info.ApiGroup = !string.IsNullOrEmpty(info.CdSistema)
                ? MapCdSistemaToApiGroup(info.CdSistema)
                : info.ModuleName;

        // CdSistema padrão baseado no módulo
        if (string.IsNullOrEmpty(info.CdSistema))
            info.CdSistema = MapModuleToCdSistema(info.ModuleName);

        // CdFuncao padrão = {CdSistema}_FM_T{ENTITYNAME}
        if (string.IsNullOrEmpty(info.CdFuncao))
            info.CdFuncao = $"{info.CdSistema}_FM_T{info.EntityName.ToUpperInvariant()}";
    }

    /// <summary>
    /// Pluraliza um nome de entidade.
    /// </summary>
    private static string Pluralize(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        // Regras simples de pluralização para português/inglês
        if (name.EndsWith("ao", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 2) + "oes"; // funcao -> funcoes

        if (name.EndsWith("al", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 2) + "ais"; // animal -> animais

        if (name.EndsWith("el", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 2) + "eis"; // papel -> papeis

        if (name.EndsWith("y", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 1) + "ies"; // category -> categories

        if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            return name + "es";

        return name + "s";
    }

    /// <summary>
    /// Mapeia nome do módulo para código do sistema.
    /// </summary>
    private static string MapModuleToCdSistema(string moduleName)
    {
        return moduleName.ToUpperInvariant() switch
        {
            "IDENTITY" => "SEG",
            "GESTAODEPESSOAS" => "RHU",
            "CONTROLEDEPONTO" => "CPT",
            "TREINAMENTOS" => "TRE",
            "SAUDEOCUPACIONAL" => "SOC",
            "AVALIACOES" => "AVA",
            "FINANCEIRO" => "FIN",
            "SHARED" => "SHR",
            _ => moduleName.Length >= 3 ? moduleName.Substring(0, 3).ToUpperInvariant() : moduleName.ToUpperInvariant()
        };
    }

    /// <summary>
    /// Mapeia código do sistema para nome do grupo da API.
    /// </summary>
    private static string MapCdSistemaToApiGroup(string cdSistema)
    {
        return cdSistema.ToUpperInvariant() switch
        {
            "SEG" => "Identity",
            "RHU" => "GestaoDePessoas",
            "CPT" => "ControleDePonto",
            "TRE" => "Treinamentos",
            "SOC" => "SaudeOcupacional",
            "AVA" => "Avaliacoes",
            "FIN" => "Financeiro",
            "SHR" => "Shared",
            _ => cdSistema
        };
    }
}