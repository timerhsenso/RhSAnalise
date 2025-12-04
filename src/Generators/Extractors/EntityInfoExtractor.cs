// =============================================================================
// RHSENSOERP GENERATOR v3.3 - ENTITY INFO EXTRACTOR
// =============================================================================
// Arquivo: src/Generators/Extractors/EntityInfoExtractor.cs
// Versão: 3.3 - Suporte a navegações/relacionamentos
// =============================================================================
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        // =====================================================================
        // NOVO v3.3: Extrai navegações (relacionamentos)
        // =====================================================================
        ExtractNavigations(classDeclaration, context.SemanticModel, info);

        // Determina a chave primária
        DeterminePrimaryKey(info);

        // Gera valores padrão para campos não preenchidos
        ApplyDefaults(info);

        return info;
    }

    /// <summary>
    /// Extrai informações do módulo baseado no namespace.
    /// </summary>
    private static void ExtractModuleInfo(EntityInfo info)
    {
        var parts = info.Namespace.Split('.');

        if (parts.Length < 2 || parts[0] != "RhSensoERP")
        {
            info.ModuleName = parts.Length > 1 ? parts[1] : "Core";
            info.ModuleNamespace = string.Join(".", parts.Take(2));
            info.IsModulesStructure = false;
            return;
        }

        if (parts.Length >= 3 && parts[1] == "Modules")
        {
            info.ModuleName = parts[2];
            info.ModuleNamespace = $"RhSensoERP.Modules.{parts[2]}";
            info.IsModulesStructure = true;
        }
        else
        {
            info.ModuleName = parts[1];
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
                case "TableName":
                    info.TableName = value.ToString()!;
                    break;
                case "Schema":
                    info.Schema = value.ToString()!;
                    break;
                case "DisplayName":
                    info.DisplayName = value.ToString()!;
                    break;
                case "CdSistema":
                    info.CdSistema = value.ToString()!;
                    break;
                case "CdFuncao":
                    info.CdFuncao = value.ToString()!;
                    break;
                case "ApiRoute":
                    info.ApiRoute = value.ToString()!;
                    break;
                case "ApiGroup":
                    info.ApiGroup = value.ToString()!;
                    break;
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
                case "GenerateMetadata":
                    info.GenerateMetadata = (bool)value;
                    break;
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
        // Pega propriedades da classe atual (exclui navegações)
        var properties = typeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public)
            .Where(p => !IsNavigationProperty(p));

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
                var attrName = attr.AttributeClass?.Name ?? "";

                switch (attrName)
                {
                    case "KeyAttribute":
                    case "Key":
                        propInfo.IsPrimaryKey = true;
                        propInfo.IsReadOnly = true;
                        break;

                    case "ColumnAttribute":
                    case "Column":
                        if (attr.ConstructorArguments.Length > 0)
                            propInfo.ColumnName = attr.ConstructorArguments[0].Value?.ToString() ?? "";
                        break;

                    case "RequiredAttribute":
                    case "Required":
                        propInfo.IsRequired = true;
                        break;

                    case "StringLengthAttribute":
                    case "StringLength":
                        if (attr.ConstructorArguments.Length > 0 && 
                            attr.ConstructorArguments[0].Value is int maxLen)
                            propInfo.MaxLength = maxLen;
                        break;

                    case "MaxLengthAttribute":
                    case "MaxLength":
                        if (attr.ConstructorArguments.Length > 0 && 
                            attr.ConstructorArguments[0].Value is int max)
                            propInfo.MaxLength = max;
                        break;

                    case "DatabaseGeneratedAttribute":
                    case "DatabaseGenerated":
                        // Verifica se é Identity
                        if (attr.ConstructorArguments.Length > 0)
                        {
                            var genOption = attr.ConstructorArguments[0].Value;
                            // DatabaseGeneratedOption.Identity = 1
                            if (genOption is int optionValue && optionValue == 1)
                            {
                                propInfo.IsIdentity = true;
                                propInfo.IsReadOnly = true;
                            }
                        }
                        break;

                    case "NotMappedAttribute":
                    case "NotMapped":
                        propInfo.ExcludeFromDto = true;
                        propInfo.IsReadOnly = true;
                        break;
                }
            }

            // Se não tem ColumnName, usa o nome da propriedade em lowercase
            if (string.IsNullOrEmpty(propInfo.ColumnName))
                propInfo.ColumnName = prop.Name.ToLowerInvariant();

            info.Properties.Add(propInfo);
        }

        // Adiciona propriedades base se não for tabela legada
        if (!info.IsLegacyTable)
        {
            AddBaseEntityPropertiesIfMissing(info);
        }
    }

    // =========================================================================
    // NOVO v3.3: EXTRAÇÃO DE NAVEGAÇÕES
    // =========================================================================

    /// <summary>
    /// Extrai informações de navegações (propriedades virtual que são outras entities).
    /// CORREÇÃO v3.3.3: 
    /// - Só adiciona navegação ManyToOne se encontrar a FK REAL
    /// - Cada FK só pode ser usada por UMA navegação (evita duplicatas)
    /// - Coleções são extraídas para gerar Ignore() no EF Config
    /// </summary>
    private static void ExtractNavigations(
        ClassDeclarationSyntax classDeclaration,
        SemanticModel semanticModel,
        EntityInfo info)
    {
        // Controle de FKs já usadas (cada FK só pode ter UMA navegação)
        var usedForeignKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        // Lista temporária de candidatas a navegação ManyToOne
        var navigationCandidates = new List<(PropertyDeclarationSyntax Property, string Name, string TargetEntity, string FkProperty, bool IsNullable, ITypeSymbol TypeSymbol)>();

        foreach (var member in classDeclaration.Members)
        {
            if (member is not PropertyDeclarationSyntax property)
                continue;

            // Verifica se é virtual
            var isVirtual = property.Modifiers.Any(m => m.IsKind(SyntaxKind.VirtualKeyword));
            if (!isVirtual)
                continue;

            var propertyName = property.Identifier.Text;
            var propertyType = property.Type;

            // Ignora propriedades com [NotMapped]
            var hasNotMapped = property.AttributeLists
                .SelectMany(al => al.Attributes)
                .Any(a => a.Name.ToString().Contains("NotMapped"));
            if (hasNotMapped)
                continue;

            // Obtém informações do tipo
            var typeInfo = semanticModel.GetTypeInfo(propertyType);
            var typeSymbol = typeInfo.Type;

            if (typeSymbol == null)
                continue;

            // Verifica se é uma coleção (ICollection<T>, IList<T>, List<T>)
            if (IsCollectionType(typeSymbol, out var elementType))
            {
                // OneToMany - Coleção 
                // Extraímos para gerar Ignore() - evita erro de ambiguidade
                if (elementType != null && IsEntityTypeSymbol(elementType))
                {
                    info.Navigations.Add(new NavigationInfo
                    {
                        Name = propertyName,
                        TargetEntity = elementType.Name,
                        TargetEntityFullName = elementType.ToDisplayString(),
                        RelationshipType = NavigationRelationshipType.OneToMany,
                        IsNullable = true
                    });
                }
            }
            else if (IsEntityTypeSymbol(typeSymbol))
            {
                // ManyToOne - Referência simples
                var targetEntity = typeSymbol.Name;
                var fkProperty = FindForeignKeyProperty(classDeclaration, property, targetEntity, info);
                
                // Verifica se a FK encontrada REALMENTE EXISTE nas propriedades
                var fkExists = info.Properties.Any(p => 
                    p.Name.Equals(fkProperty, StringComparison.OrdinalIgnoreCase));
                
                if (fkExists)
                {
                    // Adiciona à lista de candidatas
                    navigationCandidates.Add((
                        property, 
                        propertyName, 
                        targetEntity, 
                        fkProperty, 
                        IsNullableTypeSyntax(propertyType),
                        typeSymbol
                    ));
                }
                else
                {
                    // FK não existe - marca como OneToMany para gerar Ignore
                    info.Navigations.Add(new NavigationInfo
                    {
                        Name = propertyName,
                        TargetEntity = targetEntity,
                        TargetEntityFullName = typeSymbol.ToDisplayString(),
                        RelationshipType = NavigationRelationshipType.OneToMany, // Marca para Ignore
                        IsNullable = true
                    });
                }
            }
        }
        
        // ================================================================
        // CORREÇÃO v3.3.2: Processa candidatas, priorizando melhor match
        // ================================================================
        
        // Agrupa por FK
        var groupedByFk = navigationCandidates
            .GroupBy(c => c.FkProperty, StringComparer.OrdinalIgnoreCase);
        
        foreach (var fkGroup in groupedByFk)
        {
            var candidates = fkGroup.ToList();
            
            if (candidates.Count == 1)
            {
                // Só uma navegação usa esta FK - adiciona
                var nav = candidates[0];
                AddNavigationFromCandidate(nav, info);
            }
            else
            {
                // Múltiplas navegações tentam usar a mesma FK
                // Prioriza: navegação cujo nome é mais próximo da FK
                // Ex: FK "Idcargo" -> prioriza "Cargo" sobre "Cargo1", "Cargo2"
                
                var fkName = fkGroup.Key;
                
                // Tenta encontrar match exato (sem sufixo numérico)
                var bestMatch = candidates.FirstOrDefault(c => 
                    fkName.Equals($"Id{c.Name}", StringComparison.OrdinalIgnoreCase) ||
                    fkName.Equals($"{c.Name}Id", StringComparison.OrdinalIgnoreCase) ||
                    fkName.Equals($"id{c.Name.ToLower()}", StringComparison.OrdinalIgnoreCase));
                
                if (bestMatch.Property != null)
                {
                    AddNavigationFromCandidate(bestMatch, info);
                    
                    // Outras navegações da mesma FK viram Ignore
                    foreach (var other in candidates.Where(c => c.Name != bestMatch.Name))
                    {
                        info.Navigations.Add(new NavigationInfo
                        {
                            Name = other.Name,
                            TargetEntity = other.TargetEntity,
                            TargetEntityFullName = other.TypeSymbol.ToDisplayString(),
                            RelationshipType = NavigationRelationshipType.OneToMany, // Marca para Ignore
                            IsNullable = true
                        });
                    }
                }
                else
                {
                    // Usa a primeira (geralmente a que não tem sufixo numérico)
                    var first = candidates.OrderBy(c => c.Name.Length).First();
                    AddNavigationFromCandidate(first, info);
                    
                    // Outras navegações da mesma FK viram Ignore
                    foreach (var other in candidates.Where(c => c.Name != first.Name))
                    {
                        info.Navigations.Add(new NavigationInfo
                        {
                            Name = other.Name,
                            TargetEntity = other.TargetEntity,
                            TargetEntityFullName = other.TypeSymbol.ToDisplayString(),
                            RelationshipType = NavigationRelationshipType.OneToMany, // Marca para Ignore
                            IsNullable = true
                        });
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Adiciona uma navegação a partir de um candidato validado.
    /// </summary>
    private static void AddNavigationFromCandidate(
        (PropertyDeclarationSyntax Property, string Name, string TargetEntity, string FkProperty, bool IsNullable, ITypeSymbol TypeSymbol) candidate,
        EntityInfo info)
    {
        // Pega o nome REAL da FK (com o case correto)
        var realFkName = info.Properties
            .First(p => p.Name.Equals(candidate.FkProperty, StringComparison.OrdinalIgnoreCase))
            .Name;

        info.Navigations.Add(new NavigationInfo
        {
            Name = candidate.Name,
            TargetEntity = candidate.TargetEntity,
            TargetEntityFullName = candidate.TypeSymbol.ToDisplayString(),
            ForeignKeyProperty = realFkName,
            RelationshipType = NavigationRelationshipType.ManyToOne,
            IsNullable = candidate.IsNullable,
            OnDelete = NavigationDeleteBehavior.Restrict
        });
    }

    /// <summary>
    /// Verifica se o tipo é uma coleção e extrai o tipo do elemento.
    /// </summary>
    private static bool IsCollectionType(ITypeSymbol typeSymbol, out ITypeSymbol? elementType)
    {
        elementType = null;

        if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var typeName = namedType.OriginalDefinition.ToDisplayString();

            if (typeName.StartsWith("System.Collections.Generic.ICollection") ||
                typeName.StartsWith("System.Collections.Generic.IList") ||
                typeName.StartsWith("System.Collections.Generic.List") ||
                typeName.StartsWith("System.Collections.Generic.IEnumerable") ||
                typeName.StartsWith("System.Collections.Generic.HashSet"))
            {
                elementType = namedType.TypeArguments.FirstOrDefault();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Verifica se o tipo é uma Entity (não é primitivo, não é System.*).
    /// </summary>
    private static bool IsEntityTypeSymbol(ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null)
            return false;

        var fullName = typeSymbol.ToDisplayString();

        // Ignora tipos primitivos e do sistema
        if (fullName.StartsWith("System.") ||
            fullName.StartsWith("Microsoft.") ||
            typeSymbol.SpecialType != SpecialType.None)
        {
            return false;
        }

        // Verifica se é uma classe
        return typeSymbol.TypeKind == TypeKind.Class;
    }

    /// <summary>
    /// Verifica se o tipo é nullable (T?).
    /// </summary>
    private static bool IsNullableTypeSyntax(TypeSyntax typeSyntax)
    {
        return typeSyntax is NullableTypeSyntax;
    }

    /// <summary>
    /// Encontra a propriedade FK correspondente a uma navegação.
    /// </summary>
    private static string FindForeignKeyProperty(
        ClassDeclarationSyntax classDeclaration,
        PropertyDeclarationSyntax navigationProperty,
        string targetEntityName,
        EntityInfo info)
    {
        var navName = navigationProperty.Identifier.Text;

        // 1. Verifica se tem atributo [ForeignKey("...")] ou [ForeignKey(nameof(...))] NA NAVEGAÇÃO
        foreach (var attrList in navigationProperty.AttributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                var attrName = attr.Name.ToString();
                if (attrName.Contains("ForeignKey"))
                {
                    var arg = attr.ArgumentList?.Arguments.FirstOrDefault();
                    if (arg != null)
                    {
                        var value = arg.Expression.ToString();
                        
                        // Remove aspas se for string literal: "IdMotivoPai" -> IdMotivoPai
                        value = value.Trim('"');
                        
                        // Resolve nameof(): nameof(IdMotivoPai) -> IdMotivoPai
                        if (value.StartsWith("nameof(") && value.EndsWith(")"))
                        {
                            value = value.Substring(7, value.Length - 8); // Remove "nameof(" e ")"
                        }
                        
                        return value;
                    }
                }
            }
        }

        // 2. Procura propriedades com [ForeignKey] apontando para esta navegação
        foreach (var member in classDeclaration.Members)
        {
            if (member is not PropertyDeclarationSyntax prop)
                continue;

            var propName = prop.Identifier.Text;

            foreach (var attrList in prop.AttributeLists)
            {
                foreach (var attr in attrList.Attributes)
                {
                    var attrName = attr.Name.ToString();
                    if (attrName.Contains("ForeignKey"))
                    {
                        var arg = attr.ArgumentList?.Arguments.FirstOrDefault();
                        if (arg != null)
                        {
                            var value = arg.Expression.ToString();
                            
                            // Remove aspas se for string literal
                            value = value.Trim('"');
                            
                            // Resolve nameof(): nameof(Banco) -> Banco
                            if (value.StartsWith("nameof(") && value.EndsWith(")"))
                            {
                                value = value.Substring(7, value.Length - 8);
                            }
                            
                            if (value.Equals(navName, StringComparison.OrdinalIgnoreCase))
                            {
                                return propName;
                            }
                        }
                    }
                }
            }
        }

        // 3. Convenções de nome para FK
        var possibleNames = new[]
        {
            $"Id{navName}",               // IdBanco
            $"{navName}Id",               // BancoId
            $"id{navName.ToLower()}",     // idbanco (exato, não substring)
            $"{navName.ToLower()}id",     // bancoid
            $"Id{targetEntityName}",      // IdBanco (pelo nome da entity)
            $"{targetEntityName}Id",      // BancoId
            $"Fk{navName}",               // FkBanco
            $"{navName}Fk",               // BancoFk
        };

        // Procura nas propriedades já extraídas
        foreach (var propInfo in info.Properties)
        {
            if (possibleNames.Any(pn => pn.Equals(propInfo.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return propInfo.Name;
            }
        }

        // 4. Fallback: tenta Id + nome da navegação
        return $"Id{navName}";
    }

    /// <summary>
    /// Adiciona propriedades de BaseEntity se não existirem.
    /// </summary>
    private static void AddBaseEntityPropertiesIfMissing(EntityInfo info)
    {
        var baseProps = new[]
        {
            new Models.PropertyInfo
            {
                Name = "Id",
                Type = "Guid",
                IsPrimaryKey = true,
                IsReadOnly = true,
                IsIdentity = false,
                ColumnName = "id"
            },
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
        var keyProp = info.Properties.FirstOrDefault(p => p.IsPrimaryKey);

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
            info.PrimaryKeyIsGenerated = keyProp.IsIdentity || keyProp.IsGuid;
        }
    }

    /// <summary>
    /// Aplica valores padrão para campos não preenchidos.
    /// </summary>
    private static void ApplyDefaults(EntityInfo info)
    {
        if (string.IsNullOrEmpty(info.DisplayName))
            info.DisplayName = info.EntityName;

        if (string.IsNullOrEmpty(info.PluralName))
            info.PluralName = Pluralize(info.EntityName);

        if (string.IsNullOrEmpty(info.TableName))
            info.TableName = info.EntityName.ToLowerInvariant();

        if (string.IsNullOrEmpty(info.ApiRoute))
            info.ApiRoute = $"{info.ModuleName.ToLowerInvariant()}/{info.PluralName.ToLowerInvariant()}";

        if (string.IsNullOrEmpty(info.ApiGroup))
            info.ApiGroup = !string.IsNullOrEmpty(info.CdSistema)
                ? MapCdSistemaToApiGroup(info.CdSistema)
                : info.ModuleName;

        if (string.IsNullOrEmpty(info.CdSistema))
            info.CdSistema = MapModuleToCdSistema(info.ModuleName);

        if (string.IsNullOrEmpty(info.CdFuncao))
            info.CdFuncao = $"{info.CdSistema}_FM_T{info.EntityName.ToUpperInvariant()}";
    }

    /// <summary>
    /// Pluraliza um nome de entidade.
    /// </summary>
    private static string Pluralize(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        if (name.EndsWith("ao", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 2) + "oes";

        if (name.EndsWith("al", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 2) + "ais";

        if (name.EndsWith("el", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 2) + "eis";

        if (name.EndsWith("y", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 1) + "ies";

        if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            return name + "es";

        return name + "s";
    }

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

    /// <summary>
    /// Verifica se a propriedade é uma propriedade de navegação (relacionamento).
    /// </summary>
    private static bool IsNavigationProperty(IPropertySymbol property)
    {
        var typeName = property.Type.ToDisplayString();

        // Verifica se é uma coleção genérica
        if (typeName.Contains("System.Collections.Generic.ICollection<") ||
            typeName.Contains("System.Collections.Generic.IEnumerable<") ||
            typeName.Contains("System.Collections.Generic.IList<") ||
            typeName.Contains("System.Collections.Generic.List<") ||
            typeName.Contains("System.Collections.Generic.HashSet<"))
        {
            return true;
        }

        // Verifica se o tipo é uma entidade
        var typeSymbol = property.Type as INamedTypeSymbol;
        if (typeSymbol != null)
        {
            if (typeSymbol.TypeKind == TypeKind.Class &&
                !typeSymbol.SpecialType.ToString().StartsWith("System_") &&
                typeSymbol.SpecialType == SpecialType.None)
            {
                var ns = typeSymbol.ContainingNamespace?.ToDisplayString() ?? "";
                if (ns.Contains(".Entities") || ns.Contains(".Domain"))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
