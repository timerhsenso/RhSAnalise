// src/Modules/Identity/RhSensoERP.Identity.Application/Features/Sistema/Commands/DeleteSistemasCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using RhSensoERP.Identity.Application.DTOs.Common;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Sistema.Commands;

/// <summary>
/// Handler para exclusao em massa de sistemas.
/// </summary>
public sealed class DeleteSistemasCommandHandler
    : IRequestHandler<DeleteSistemasCommand, Result<BatchDeleteResult>>
{
    private readonly ISistemaRepository _repository;
    private readonly ILogger<DeleteSistemasCommandHandler> _logger;

    public DeleteSistemasCommandHandler(
        ISistemaRepository repository,
        ILogger<DeleteSistemasCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<BatchDeleteResult>> Handle(
        DeleteSistemasCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Codigos is null || request.Codigos.Count == 0)
        {
            return Result<BatchDeleteResult>.Failure(
                Error.Validation("DeleteSistemas.Empty", "Nenhum codigo informado para exclusao."));
        }

        var successCount = 0;
        var errors = new List<BatchDeleteError>();

        foreach (var codigo in request.Codigos.Distinct())
        {
            try
            {
                var sistema = await _repository.GetByIdAsync(codigo, cancellationToken);

                if (sistema is null)
                {
                    errors.Add(new BatchDeleteError(codigo, "Sistema nao encontrado."));
                    continue;
                }

                // Usa Delete (metodo especifico do ISistemaRepository)
                _repository.Delete(sistema);
                successCount++;

                _logger.LogInformation("Sistema {Codigo} marcado para exclusao.", codigo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir sistema {Codigo}.", codigo);
                errors.Add(new BatchDeleteError(codigo, ex.Message));
            }
        }

        // Salva as alteracoes
        try
        {
            await _repository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("{Count} sistemas excluidos com sucesso.", successCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar exclusoes em massa.");
            return Result<BatchDeleteResult>.Failure(
                Error.Failure("DeleteSistemas.SaveFailed", $"Erro ao salvar: {ex.Message}"));
        }

        var result = new BatchDeleteResult
        {
            SuccessCount = successCount,
            FailureCount = errors.Count,
            Errors = errors
        };

        return Result<BatchDeleteResult>.Success(result);
    }
}