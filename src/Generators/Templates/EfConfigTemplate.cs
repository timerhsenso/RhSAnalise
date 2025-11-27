// =============================================================================
// RHSENSOERP GENERATOR v3.0 - EF CONFIG TEMPLATE
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Entity Framework Configuration.
/// </summary>
public static class EfConfigTemplate
{
    /// <summary>
    /// Gera a Configuration do Entity Framework.
    /// </summary>
    public static string GenerateConfig(EntityInfo info)
    {
        var entityNs = info.Namespace;
        var propertyConfigs = GeneratePropertyConfigurations(info);

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using {{entityNs}};

namespace {{info.EfConfigNamespace}};

/// <summary>
/// Configuração do Entity Framework para {{info.DisplayName}}.
/// </summary>
public sealed class {{info.EntityName}}Configuration : IEntityTypeConfiguration<{{info.EntityName}}>
{
    public void Configure(EntityTypeBuilder<{{info.EntityName}}> builder)
    {
        // Tabela
        builder.ToTable("{{info.TableName}}", "{{info.Schema}}");

        // Chave primária
        builder.HasKey(e => e.{{info.PrimaryKeyProperty}});

{{propertyConfigs}}
    }
}
""";
    }

    /// <summary>
    /// Gera as configurações de propriedades.
    /// </summary>
    private static string GeneratePropertyConfigurations(EntityInfo info)
    {
        var configs = new List<string>();

        foreach (var prop in info.Properties)
        {
            var propConfig = new List<string>();

            // Nome da coluna (se diferente)
            if (!string.IsNullOrEmpty(prop.ColumnName) &&
                !prop.ColumnName.Equals(prop.Name, StringComparison.OrdinalIgnoreCase))
            {
                propConfig.Add($".HasColumnName(\"{prop.ColumnName}\")");
            }

            // Required
            if (prop.IsRequired && !prop.IsNullable)
            {
                propConfig.Add(".IsRequired()");
            }

            // MaxLength para strings
            if (prop.IsString && prop.MaxLength.HasValue)
            {
                propConfig.Add($".HasMaxLength({prop.MaxLength.Value})");
            }

            // Só adiciona se tem configuração
            if (propConfig.Count > 0)
            {
                var config = $"        builder.Property(e => e.{prop.Name})\n            {string.Join("\n            ", propConfig)};";
                configs.Add(config);
            }
        }

        return configs.Count > 0
            ? string.Join("\n\n", configs)
            : "        // Configurações de propriedades usam convenções padrão";
    }
}
