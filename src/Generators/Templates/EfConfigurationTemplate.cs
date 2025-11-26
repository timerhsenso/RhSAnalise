// =============================================================================
// RHSENSOERP SOURCE GENERATOR - TEMPLATE DE EF CONFIGURATION
// =============================================================================

using System.Text;
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

public static class EfConfigurationTemplate
{
    public static string Generate(EntityInfo entity)
    {
        var sb = new StringBuilder();
        var pk = entity.PrimaryKey;
        var pkName = pk?.Name ?? "Id";

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - EF Core Configuration");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
        sb.AppendLine($"using {entity.Namespace};");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.ConfigurationNamespace};");
        sb.AppendLine();

        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Configuração EF Core para {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class {entity.ClassName}Configuration : IEntityTypeConfiguration<{entity.ClassName}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public void Configure(EntityTypeBuilder<{entity.ClassName}> builder)");
        sb.AppendLine("    {");

        // Table
        if (!string.IsNullOrEmpty(entity.Schema))
        {
            sb.AppendLine($"        builder.ToTable(\"{entity.TableName}\", \"{entity.Schema}\");");
        }
        else
        {
            sb.AppendLine($"        builder.ToTable(\"{entity.TableName}\");");
        }
        sb.AppendLine();

        // Primary Key
        if (pk != null)
        {
            sb.AppendLine($"        // Chave primária");
            sb.AppendLine($"        builder.HasKey(e => e.{pkName});");
            sb.AppendLine();
        }

        // Properties
        sb.AppendLine("        // Propriedades");
        foreach (var prop in entity.ScalarProperties.Where(p => !p.IsNavigation))
        {
            sb.AppendLine();
            sb.Append($"        builder.Property(e => e.{prop.Name})");
            
            // Column name
            sb.AppendLine();
            sb.Append($"            .HasColumnName(\"{prop.ColumnName}\")");

            // Required
            if (prop.IsRequired && !prop.IsKey)
            {
                sb.AppendLine();
                sb.Append("            .IsRequired()");
            }

            // MaxLength for strings
            if (prop.MaxLength.HasValue && prop.IsString)
            {
                sb.AppendLine();
                sb.Append($"            .HasMaxLength({prop.MaxLength})");
            }

            // Unicode for strings
            if (prop.IsString)
            {
                sb.AppendLine();
                sb.Append("            .IsUnicode(true)");
            }

            // Precision for decimals
            if (prop.TypeName == "decimal")
            {
                sb.AppendLine();
                sb.Append("            .HasPrecision(18, 2)");
            }

            sb.AppendLine(";");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
