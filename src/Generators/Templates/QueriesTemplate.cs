// =============================================================================
// RHSENSOERP GENERATOR v3.0 - QUERIES TEMPLATE
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Queries (CQRS).
/// </summary>
public static class QueriesTemplate
{
    /// <summary>
    /// Gera a Query GetById com Handler.
    /// </summary>
    public static string GenerateGetByIdQuery(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using {{info.DtoNamespace}};
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Core.Common;

namespace {{info.QueriesNamespace}};

/// <summary>
/// Query para buscar {{info.DisplayName}} por ID.
/// </summary>
public sealed record GetBy{{info.EntityName}}IdQuery({{pkType}} Id)
    : IRequest<Result<{{info.EntityName}}Dto>>;

/// <summary>
/// Handler da query de busca por ID.
/// </summary>
public sealed class GetBy{{info.EntityName}}IdHandler
    : IRequestHandler<GetBy{{info.EntityName}}IdQuery, Result<{{info.EntityName}}Dto>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetBy{{info.EntityName}}IdHandler> _logger;

    public GetBy{{info.EntityName}}IdHandler(
        I{{info.EntityName}}Repository repository,
        IMapper mapper,
        ILogger<GetBy{{info.EntityName}}IdHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<{{info.EntityName}}Dto>> Handle(
        GetBy{{info.EntityName}}IdQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Buscando {{info.DisplayName}} {Id}...", query.Id);

            var entity = await _repository.GetByIdAsync(query.Id, cancellationToken);

            if (entity == null)
            {
                _logger.LogWarning("{{info.DisplayName}} {Id} não encontrado", query.Id);
                return Result<{{info.EntityName}}Dto>.Failure(
                    Error.NotFound("{{info.EntityName}}.NotFound", "{{info.DisplayName}} não encontrado"));
            }

            var dto = _mapper.Map<{{info.EntityName}}Dto>(entity);

            return Result<{{info.EntityName}}Dto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {{info.DisplayName}} {Id}", query.Id);
            return Result<{{info.EntityName}}Dto>.Failure(
                Error.Failure("{{info.EntityName}}.Error", $"Erro ao buscar {{info.DisplayName}}: {ex.Message}"));
        }
    }
}
""";
    }

    /// <summary>
    /// Gera a Query GetPaged com Handler.
    /// </summary>
    public static string GenerateGetPagedQuery(EntityInfo info)
    {
        // Determina o campo de busca principal (primeiro campo string não-PK)
        var searchField = info.Properties
            .FirstOrDefault(p => p.IsString && !p.IsPrimaryKey)?.Name ?? info.PrimaryKeyProperty;

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using {{info.DtoNamespace}};
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Core.Common;

namespace {{info.QueriesNamespace}};

/// <summary>
/// Query para listagem paginada de {{info.DisplayName}}.
/// </summary>
public sealed record Get{{info.PluralName}}PagedQuery(PagedRequest Request)
    : IRequest<Result<PagedResult<{{info.EntityName}}Dto>>>;

/// <summary>
/// Handler da query de listagem paginada.
/// </summary>
public sealed class Get{{info.PluralName}}PagedHandler
    : IRequestHandler<Get{{info.PluralName}}PagedQuery, Result<PagedResult<{{info.EntityName}}Dto>>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<Get{{info.PluralName}}PagedHandler> _logger;

    public Get{{info.PluralName}}PagedHandler(
        I{{info.EntityName}}Repository repository,
        IMapper mapper,
        ILogger<Get{{info.PluralName}}PagedHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PagedResult<{{info.EntityName}}Dto>>> Handle(
        Get{{info.PluralName}}PagedQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = query.Request;
            _logger.LogDebug(
                "Buscando {{info.DisplayName}} - Página {Page}, Tamanho {Size}, Busca '{Search}'",
                request.Page,
                request.PageSize,
                request.Search);

            // Query base
            var queryable = _repository.Query();

            // Aplica filtro de busca
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                queryable = queryable.Where(e =>
                    EF.Functions.Like(e.{{searchField}}.ToLower(), $"%{search}%"));
            }

            // Conta total
            var totalCount = await queryable.CountAsync(cancellationToken);

            // Aplica paginação
            var items = await queryable
                .OrderBy(e => e.{{info.PrimaryKeyProperty}})
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Mapeia para DTO
            var dtos = _mapper.Map<List<{{info.EntityName}}Dto>>(items);

            // PagedResult usa construtor: (items, totalCount, pageNumber, pageSize)
            var result = new PagedResult<{{info.EntityName}}Dto>(
                dtos,
                totalCount,
                request.Page,
                request.PageSize);

            return Result<PagedResult<{{info.EntityName}}Dto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {{info.DisplayName}} paginado");
            return Result<PagedResult<{{info.EntityName}}Dto>>.Failure(
                Error.Failure("{{info.EntityName}}.Error", $"Erro ao buscar {{info.DisplayName}}: {ex.Message}"));
        }
    }
}
""";
    }
}
