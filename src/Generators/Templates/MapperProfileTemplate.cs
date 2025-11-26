// =============================================================================
// RHSENSOERP SOURCE GENERATOR - TEMPLATE DE AUTOMAPPER PROFILE
// =============================================================================

using System.Text;
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

public static class MapperProfileTemplate
{
    public static string Generate(EntityInfo entity)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - AutoMapper Profile");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using AutoMapper;");
        sb.AppendLine($"using {entity.Namespace};");
        sb.AppendLine($"using {entity.DtoNamespace};");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.MapperNamespace};");
        sb.AppendLine();

        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Perfil de mapeamento para {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class {entity.ClassName}Profile : Profile");
        sb.AppendLine("{");
        sb.AppendLine($"    public {entity.ClassName}Profile()");
        sb.AppendLine("    {");
        sb.AppendLine($"        // Entity -> DTO");
        sb.AppendLine($"        CreateMap<{entity.ClassName}, {entity.ClassName}Dto>();");
        sb.AppendLine();
        sb.AppendLine($"        // Request -> Entity (Create)");
        sb.AppendLine($"        CreateMap<Create{entity.ClassName}Request, {entity.ClassName}>();");
        sb.AppendLine();
        sb.AppendLine($"        // Request -> Entity (Update)");
        sb.AppendLine($"        CreateMap<Update{entity.ClassName}Request, {entity.ClassName}>();");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
