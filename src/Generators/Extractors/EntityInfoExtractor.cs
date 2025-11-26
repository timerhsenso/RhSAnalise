// =============================================================================
// RHSENSOERP SOURCE GENERATOR - EXTRACTOR
// =============================================================================

using Microsoft.CodeAnalysis;
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Extractors;

public static class EntityInfoExtractor
{
    public static EntityInfo? Extract(INamedTypeSymbol classSymbol)
    {
        var config = ExtractConfig(classSymbol);
        if (config == null) return null;

        var className = classSymbol.Name;
        var ns = classSymbol.ContainingNamespace.ToDisplayString();

        var entityInfo = new EntityInfo
        {
            ClassName = className,
            Namespace = ns,
            DisplayName = config.DisplayName ?? className,
            PluralName = Pluralize(className),
            CamelCaseName = ToCamelCase(className),
            TableName = config.TableName ?? className.ToLowerInvariant(),
            Schema = config.Schema,
            Config = config
        };

        foreach (var member in classSymbol.GetMembers())
        {
            if (member is IPropertySymbol property && property.DeclaredAccessibility == Accessibility.Public)
            {
                var propInfo = ExtractProperty(property);
                if (propInfo != null)
                    entityInfo.Properties.Add(propInfo);
            }
        }

        return entityInfo;
    }

    private static GenerationConfig? ExtractConfig(INamedTypeSymbol classSymbol)
    {
        var attr = classSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name is "GenerateCrudAttribute" or "GenerateCrud");

        if (attr == null) return null;

        var config = new GenerationConfig();

        foreach (var arg in attr.NamedArguments)
        {
            var value = arg.Value.Value;
            switch (arg.Key)
            {
                case "TableName": config.TableName = value as string; break;
                case "Schema": config.Schema = value as string; break;
                case "DisplayName": config.DisplayName = value as string; break;
                case "BaseNamespace": config.BaseNamespace = value as string; break;
                case "DtoNamespace": config.DtoNamespace = value as string; break;
                case "CommandsNamespace": config.CommandsNamespace = value as string; break;
                case "QueriesNamespace": config.QueriesNamespace = value as string; break;
                case "ValidatorsNamespace": config.ValidatorsNamespace = value as string; break;
                case "RepositoryNamespace": config.RepositoryNamespace = value as string; break;
                case "ConfigurationNamespace": config.ConfigurationNamespace = value as string; break;

                case "GenerateDto": config.GenerateDto = (bool)(value ?? true); break;
                case "GenerateCreateDto": config.GenerateCreateDto = (bool)(value ?? true); break;
                case "GenerateUpdateDto": config.GenerateUpdateDto = (bool)(value ?? true); break;
                case "GenerateCreateCommand": config.GenerateCreateCommand = (bool)(value ?? true); break;
                case "GenerateUpdateCommand": config.GenerateUpdateCommand = (bool)(value ?? true); break;
                case "GenerateDeleteCommand": config.GenerateDeleteCommand = (bool)(value ?? true); break;
                case "GenerateDeleteBatchCommand": config.GenerateDeleteBatchCommand = (bool)(value ?? true); break;
                case "GenerateGetByIdQuery": config.GenerateGetByIdQuery = (bool)(value ?? true); break;
                case "GenerateGetPagedQuery": config.GenerateGetPagedQuery = (bool)(value ?? true); break;
                case "GenerateCreateValidator": config.GenerateCreateValidator = (bool)(value ?? true); break;
                case "GenerateUpdateValidator": config.GenerateUpdateValidator = (bool)(value ?? true); break;
                case "GenerateRepositoryInterface": config.GenerateRepositoryInterface = (bool)(value ?? true); break;
                case "GenerateRepositoryImplementation": config.GenerateRepositoryImplementation = (bool)(value ?? true); break;
                case "GenerateMapperProfile": config.GenerateMapperProfile = (bool)(value ?? true); break;
                case "GenerateEfConfiguration": config.GenerateEfConfiguration = (bool)(value ?? true); break;
            }
        }

        return config;
    }

    private static Models.PropertyInfo? ExtractProperty(IPropertySymbol property)
    {
        if (IsNavigationProperty(property))
        {
            return new Models.PropertyInfo
            {
                Name = property.Name,
                TypeName = SimplifyTypeName(property.Type),
                IsNavigation = true
            };
        }

        var propInfo = new Models.PropertyInfo
        {
            Name = property.Name,
            TypeName = SimplifyTypeName(property.Type),
            IsNullable = property.Type.NullableAnnotation == NullableAnnotation.Annotated,
            ColumnName = property.Name.ToUpperInvariant(),
            DisplayName = property.Name
        };

        foreach (var attr in property.GetAttributes())
        {
            ProcessAttribute(propInfo, attr);
        }

        return propInfo;
    }

    private static void ProcessAttribute(Models.PropertyInfo propInfo, AttributeData attr)
    {
        var name = attr.AttributeClass?.Name;

        switch (name)
        {
            case "KeyAttribute" or "Key":
                propInfo.IsKey = true;
                propInfo.IsRequired = true;
                break;

            case "RequiredAttribute" or "Required":
                propInfo.IsRequired = true;
                var reqMsg = attr.NamedArguments.FirstOrDefault(a => a.Key == "ErrorMessage").Value.Value as string;
                if (!string.IsNullOrEmpty(reqMsg))
                    propInfo.Messages.RequiredMessage = reqMsg;
                break;

            case "StringLengthAttribute" or "StringLength":
                if (attr.ConstructorArguments.Length > 0)
                    propInfo.MaxLength = (int)attr.ConstructorArguments[0].Value!;
                foreach (var arg in attr.NamedArguments)
                {
                    if (arg.Key == "MinimumLength")
                        propInfo.MinLength = (int)arg.Value.Value!;
                    if (arg.Key == "ErrorMessage")
                        propInfo.Messages.LengthMessage = arg.Value.Value as string;
                }
                break;

            case "MaxLengthAttribute" or "MaxLength":
                if (attr.ConstructorArguments.Length > 0)
                    propInfo.MaxLength = (int)attr.ConstructorArguments[0].Value!;
                break;

            case "ColumnNameAttribute" or "ColumnName":
                if (attr.ConstructorArguments.Length > 0)
                    propInfo.ColumnName = attr.ConstructorArguments[0].Value as string ?? propInfo.ColumnName;
                break;

            case "FieldDisplayNameAttribute" or "FieldDisplayName":
                if (attr.ConstructorArguments.Length > 0)
                    propInfo.DisplayName = attr.ConstructorArguments[0].Value as string ?? propInfo.DisplayName;
                break;

            case "ReadOnlyFieldAttribute" or "ReadOnlyField":
                propInfo.IsReadOnly = true;
                break;

            case "IgnoreInDtoAttribute" or "IgnoreInDto":
                propInfo.IgnoreInAllDtos = true;
                break;
        }
    }

    private static bool IsNavigationProperty(IPropertySymbol property)
    {
        var typeName = property.Type.ToDisplayString();
        return typeName.Contains("ICollection") ||
               typeName.Contains("IEnumerable") ||
               typeName.Contains("IList") ||
               typeName.Contains("List<");
    }

    private static string SimplifyTypeName(ITypeSymbol type)
    {
        var fullName = type.ToDisplayString();
        var isNullable = fullName.EndsWith("?");
        var baseName = isNullable ? fullName.TrimEnd('?') : fullName;

        var simplified = baseName switch
        {
            "System.String" => "string",
            "System.Int32" => "int",
            "System.Int64" => "long",
            "System.Boolean" => "bool",
            "System.Decimal" => "decimal",
            "System.Double" => "double",
            "System.DateTime" => "DateTime",
            "System.Guid" => "Guid",
            _ => baseName.Contains(".") ? baseName.Split('.').Last() : baseName
        };

        return isNullable ? $"{simplified}?" : simplified;
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    private static string Pluralize(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        if (name.EndsWith("ao", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 2) + "oes";
        if (name.EndsWith("m", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 1) + "ns";
        if (name.EndsWith("r", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            return name + "es";
        if (name.EndsWith("l", StringComparison.OrdinalIgnoreCase))
            return name.Substring(0, name.Length - 1) + "is";

        return name + "s";
    }
}
