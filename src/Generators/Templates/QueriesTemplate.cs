// =============================================================================
// RHSENSOERP SOURCE GENERATOR - TEMPLATE DE QUERIES
// =============================================================================

using System.Text;
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

public static class QueriesTemplate
{
    public static string GenerateGetByIdQuery(EntityInfo entity)
    {
        var sb = new StringBuilder();
        var pk = entity.PrimaryKey;
        var pkName = pk?.Name ?? "Id";
        var pkType = pk?.TypeName ?? "int";

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - GetById Query");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using AutoMapper;");
        sb.AppendLine("using MediatR;");
        sb.AppendLine($"using {entity.DtoNamespace};");
        sb.AppendLine($"using {entity.RepositoryInterfaceNamespace};");
        sb.AppendLine("using RhSensoERP.Shared.Core.Common;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.QueriesNamespace};");
        sb.AppendLine();

        // Query Record
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Query para obter {entity.DisplayName} por código.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed record Get{entity.ClassName}ByIdQuery({pkType} {pkName}) : IRequest<Result<{entity.ClassName}Dto>>;");
        sb.AppendLine();

        // Handler
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Handler do Get{entity.ClassName}ByIdQuery.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class Get{entity.ClassName}ByIdQueryHandler : IRequestHandler<Get{entity.ClassName}ByIdQuery, Result<{entity.ClassName}Dto>>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly I{entity.ClassName}Repository _repository;");
        sb.AppendLine("    private readonly IMapper _mapper;");
        sb.AppendLine();
        sb.AppendLine($"    public Get{entity.ClassName}ByIdQueryHandler(I{entity.ClassName}Repository repository, IMapper mapper)");
        sb.AppendLine("    {");
        sb.AppendLine("        _repository = repository;");
        sb.AppendLine("        _mapper = mapper;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<Result<{entity.ClassName}Dto>> Handle(Get{entity.ClassName}ByIdQuery query, CancellationToken ct)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var entity = await _repository.GetBy{pkName}Async(query.{pkName}, ct);");
        sb.AppendLine();
        sb.AppendLine("        if (entity == null)");
        sb.AppendLine("        {");
        sb.AppendLine($"            return Result<{entity.ClassName}Dto>.Failure(\"NOT_FOUND\", \"{entity.DisplayName} não encontrado.\");");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine($"        var dto = _mapper.Map<{entity.ClassName}Dto>(entity);");
        sb.AppendLine();
        sb.AppendLine($"        return Result<{entity.ClassName}Dto>.Success(dto);");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public static string GenerateGetPagedQuery(EntityInfo entity)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - GetPaged Query");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using AutoMapper;");
        sb.AppendLine("using MediatR;");
        sb.AppendLine($"using {entity.DtoNamespace};");
        sb.AppendLine($"using {entity.RepositoryInterfaceNamespace};");
        sb.AppendLine("using RhSensoERP.Shared.Contracts.Common;");
        sb.AppendLine("using RhSensoERP.Shared.Core.Common;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.QueriesNamespace};");
        sb.AppendLine();

        // Query Record
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Query para listar {entity.PluralName} com paginação.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed record Get{entity.PluralName}PagedQuery(PagedRequest Request) : IRequest<Result<PagedResult<{entity.ClassName}Dto>>>;");
        sb.AppendLine();

        // Handler
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Handler do Get{entity.PluralName}PagedQuery.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class Get{entity.PluralName}PagedQueryHandler : IRequestHandler<Get{entity.PluralName}PagedQuery, Result<PagedResult<{entity.ClassName}Dto>>>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly I{entity.ClassName}Repository _repository;");
        sb.AppendLine("    private readonly IMapper _mapper;");
        sb.AppendLine();
        sb.AppendLine($"    public Get{entity.PluralName}PagedQueryHandler(I{entity.ClassName}Repository repository, IMapper mapper)");
        sb.AppendLine("    {");
        sb.AppendLine("        _repository = repository;");
        sb.AppendLine("        _mapper = mapper;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<Result<PagedResult<{entity.ClassName}Dto>>> Handle(Get{entity.PluralName}PagedQuery query, CancellationToken ct)");
        sb.AppendLine("    {");
        sb.AppendLine("        var req = query.Request;");
        sb.AppendLine();
        sb.AppendLine("        var (items, totalCount) = await _repository.GetPagedAsync(");
        sb.AppendLine("            req.Page,");
        sb.AppendLine("            req.PageSize,");
        sb.AppendLine("            req.Search,");
        sb.AppendLine("            req.SortBy,");
        sb.AppendLine("            req.Desc,");
        sb.AppendLine("            ct);");
        sb.AppendLine();
        sb.AppendLine($"        var dtos = _mapper.Map<IEnumerable<{entity.ClassName}Dto>>(items);");
        sb.AppendLine();
        sb.AppendLine($"        var result = new PagedResult<{entity.ClassName}Dto>(");
        sb.AppendLine("            dtos,");
        sb.AppendLine("            totalCount,");
        sb.AppendLine("            req.Page,");
        sb.AppendLine("            req.PageSize);");
        sb.AppendLine();
        sb.AppendLine($"        return Result<PagedResult<{entity.ClassName}Dto>>.Success(result);");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
