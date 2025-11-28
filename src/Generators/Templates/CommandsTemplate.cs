// =============================================================================
// RHSENSOERP GENERATOR v3.1 - COMMANDS TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/CommandsTemplate.cs
// CORRIGIDO: BatchDeleteResult inline, sem dependências externas
// CORRIGIDO: Removido operador ?. de Guid (value type não nullable)
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Commands (CQRS).
/// </summary>
public static class CommandsTemplate
{
    /// <summary>
    /// Gera o CreateCommand com Handler.
    /// </summary>
    public static string GenerateCreateCommand(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var pkProp = info.PrimaryKeyProperty;
        var entityNs = info.Namespace;

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.1
// Entity: {{info.EntityName}}
// =============================================================================
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using {{entityNs}};
using {{info.DtoNamespace}};
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Core.Common;

namespace {{info.CommandsNamespace}};

/// <summary>
/// Command para criação de {{info.DisplayName}}.
/// </summary>
public sealed record Create{{info.EntityName}}Command(Create{{info.EntityName}}Request Request)
    : IRequest<Result<{{pkType}}>>;

/// <summary>
/// Handler do comando de criação.
/// </summary>
public sealed class Create{{info.EntityName}}Handler
    : IRequestHandler<Create{{info.EntityName}}Command, Result<{{pkType}}>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<Create{{info.EntityName}}Handler> _logger;

    public Create{{info.EntityName}}Handler(
        I{{info.EntityName}}Repository repository,
        IMapper mapper,
        ILogger<Create{{info.EntityName}}Handler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<{{pkType}}>> Handle(
        Create{{info.EntityName}}Command command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Criando {{info.DisplayName}}...");

            // Mapeia request para entity
            var entity = _mapper.Map<{{info.EntityName}}>(command.Request);

            // Persiste no banco
            await _repository.AddAsync(entity, cancellationToken);

            _logger.LogInformation("{{info.DisplayName}} criado com sucesso: {Id}", entity.{{pkProp}});

            return Result<{{pkType}}>.Success(entity.{{pkProp}});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar {{info.DisplayName}}");
            return Result<{{pkType}}>.Failure(
                Error.Failure("{{info.EntityName}}.CreateError", $"Erro ao criar {{info.DisplayName}}: {ex.Message}"));
        }
    }
}
""";
    }

    /// <summary>
    /// Gera o UpdateCommand com Handler.
    /// </summary>
    public static string GenerateUpdateCommand(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var pkProp = info.PrimaryKeyProperty;
        var entityNs = info.Namespace;

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.1
// Entity: {{info.EntityName}}
// =============================================================================
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using {{entityNs}};
using {{info.DtoNamespace}};
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Core.Common;

namespace {{info.CommandsNamespace}};

/// <summary>
/// Command para atualização de {{info.DisplayName}}.
/// </summary>
public sealed record Update{{info.EntityName}}Command({{pkType}} Id, Update{{info.EntityName}}Request Request)
    : IRequest<Result<bool>>;

/// <summary>
/// Handler do comando de atualização.
/// </summary>
public sealed class Update{{info.EntityName}}Handler
    : IRequestHandler<Update{{info.EntityName}}Command, Result<bool>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<Update{{info.EntityName}}Handler> _logger;

    public Update{{info.EntityName}}Handler(
        I{{info.EntityName}}Repository repository,
        IMapper mapper,
        ILogger<Update{{info.EntityName}}Handler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        Update{{info.EntityName}}Command command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Atualizando {{info.DisplayName}} {Id}...", command.Id);

            // Busca entidade existente
            var entity = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("{{info.DisplayName}} {Id} não encontrado", command.Id);
                return Result<bool>.Failure(
                    Error.NotFound("{{info.EntityName}}.NotFound", "{{info.DisplayName}} não encontrado"));
            }

            // Atualiza propriedades
            _mapper.Map(command.Request, entity);

            // Persiste alterações
            await _repository.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("{{info.DisplayName}} {Id} atualizado com sucesso", command.Id);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar {{info.DisplayName}} {Id}", command.Id);
            return Result<bool>.Failure(
                Error.Failure("{{info.EntityName}}.UpdateError", $"Erro ao atualizar {{info.DisplayName}}: {ex.Message}"));
        }
    }
}
""";
    }

    /// <summary>
    /// Gera o DeleteCommand com Handler.
    /// </summary>
    public static string GenerateDeleteCommand(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.1
// Entity: {{info.EntityName}}
// =============================================================================
using MediatR;
using Microsoft.Extensions.Logging;
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Core.Common;

namespace {{info.CommandsNamespace}};

/// <summary>
/// Command para exclusão de {{info.DisplayName}}.
/// </summary>
public sealed record Delete{{info.EntityName}}Command({{pkType}} Id)
    : IRequest<Result<bool>>;

/// <summary>
/// Handler do comando de exclusão.
/// </summary>
public sealed class Delete{{info.EntityName}}Handler
    : IRequestHandler<Delete{{info.EntityName}}Command, Result<bool>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly ILogger<Delete{{info.EntityName}}Handler> _logger;

    public Delete{{info.EntityName}}Handler(
        I{{info.EntityName}}Repository repository,
        ILogger<Delete{{info.EntityName}}Handler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        Delete{{info.EntityName}}Command command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Excluindo {{info.DisplayName}} {Id}...", command.Id);

            // Busca entidade
            var entity = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("{{info.DisplayName}} {Id} não encontrado", command.Id);
                return Result<bool>.Failure(
                    Error.NotFound("{{info.EntityName}}.NotFound", "{{info.DisplayName}} não encontrado"));
            }

            // Remove
            await _repository.DeleteAsync(entity, cancellationToken);

            _logger.LogInformation("{{info.DisplayName}} {Id} excluído com sucesso", command.Id);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir {{info.DisplayName}} {Id}", command.Id);
            return Result<bool>.Failure(
                Error.Failure("{{info.EntityName}}.DeleteError", $"Erro ao excluir {{info.DisplayName}}: {ex.Message}"));
        }
    }
}
""";
    }

    /// <summary>
    /// Gera o DeleteMultipleCommand (Batch) com Handler.
    /// Retorna Result com contagem simples em vez de BatchDeleteResult.
    /// </summary>
    public static string GenerateBatchDeleteCommand(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.1
// Entity: {{info.EntityName}}
// =============================================================================
using MediatR;
using Microsoft.Extensions.Logging;
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Core.Common;

namespace {{info.CommandsNamespace}};

/// <summary>
/// Resultado da exclusão em lote.
/// </summary>
public sealed class {{info.EntityName}}BatchDeleteResult
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<{{info.EntityName}}BatchDeleteError> Errors { get; set; } = new();
}

/// <summary>
/// Erro de exclusão individual.
/// </summary>
public sealed class {{info.EntityName}}BatchDeleteError
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public {{info.EntityName}}BatchDeleteError() { }

    public {{info.EntityName}}BatchDeleteError(string id, string message)
    {
        Id = id;
        Message = message;
    }
}

/// <summary>
/// Command para exclusão em lote de {{info.DisplayName}}.
/// </summary>
public sealed record Delete{{info.PluralName}}Command(List<{{pkType}}> Ids)
    : IRequest<Result<{{info.EntityName}}BatchDeleteResult>>;

/// <summary>
/// Handler do comando de exclusão em lote.
/// </summary>
public sealed class Delete{{info.PluralName}}Handler
    : IRequestHandler<Delete{{info.PluralName}}Command, Result<{{info.EntityName}}BatchDeleteResult>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly ILogger<Delete{{info.PluralName}}Handler> _logger;

    public Delete{{info.PluralName}}Handler(
        I{{info.EntityName}}Repository repository,
        ILogger<Delete{{info.PluralName}}Handler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<{{info.EntityName}}BatchDeleteResult>> Handle(
        Delete{{info.PluralName}}Command command,
        CancellationToken cancellationToken)
    {
        var errors = new List<{{info.EntityName}}BatchDeleteError>();
        var successCount = 0;

        _logger.LogInformation("Excluindo {Count} {{info.DisplayName}}(s) em lote...", command.Ids.Count);

        foreach (var id in command.Ids)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id, cancellationToken);
                if (entity == null)
                {
                    errors.Add(new {{info.EntityName}}BatchDeleteError(id.ToString(), "Registro não encontrado"));
                    continue;
                }

                await _repository.DeleteAsync(entity, cancellationToken);
                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir {{info.DisplayName}} {Id}", id);
                errors.Add(new {{info.EntityName}}BatchDeleteError(id.ToString(), ex.Message));
            }
        }

        var result = new {{info.EntityName}}BatchDeleteResult
        {
            SuccessCount = successCount,
            FailureCount = errors.Count,
            Errors = errors
        };

        _logger.LogInformation(
            "Exclusão em lote concluída: {Success} sucesso(s), {Failure} falha(s)",
            result.SuccessCount,
            result.FailureCount);

        return Result<{{info.EntityName}}BatchDeleteResult>.Success(result);
    }
}
""";
    }
}