// =============================================================================
// RHSENSOERP GENERATOR v3.0 - MAPPER TEMPLATE
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de AutoMapper Profile.
/// </summary>
public static class MapperTemplate
{
    /// <summary>
    /// Gera o Profile do AutoMapper.
    /// </summary>
    public static string GenerateProfile(EntityInfo info)
    {
        var entityNs = info.Namespace;

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================
using AutoMapper;
using {{entityNs}};
using {{info.DtoNamespace}};

namespace {{info.MapperNamespace}};

/// <summary>
/// Perfil de mapeamento do AutoMapper para {{info.DisplayName}}.
/// </summary>
public sealed class {{info.EntityName}}Profile : Profile
{
    public {{info.EntityName}}Profile()
    {
        // Entity → DTO
        CreateMap<{{info.EntityName}}, {{info.EntityName}}Dto>();

        // CreateRequest → Entity
        CreateMap<Create{{info.EntityName}}Request, {{info.EntityName}}>();

        // UpdateRequest → Entity (para atualizações parciais)
        CreateMap<Update{{info.EntityName}}Request, {{info.EntityName}}>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
""";
    }
}
