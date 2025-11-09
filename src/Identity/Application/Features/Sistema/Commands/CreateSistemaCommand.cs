using AutoMapper;
using FluentValidation;
using MediatR;
using RhSensoERP.Identity.Application.Requests.Sistema;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Sistema.Commands;

// Command
public sealed record CreateSistemaCommand(CreateSistemaRequest Payload) : IRequest<Result<string>>;

// Handler
public sealed class CreateSistemaCommandHandler : IRequestHandler<CreateSistemaCommand, Result<string>>
{
    private readonly ISistemaRepository _repository;
    private readonly IValidator<CreateSistemaRequest> _validator;
    private readonly IMapper _mapper;

    public CreateSistemaCommandHandler(
        ISistemaRepository repository,
        IValidator<CreateSistemaRequest> validator,
        IMapper mapper)
    {
        _repository = repository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<string>> Handle(CreateSistemaCommand request, CancellationToken ct)
    {
        // Validar
        var validationResult = await _validator.ValidateAsync(request.Payload, ct);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<string>.Failure("VALIDATION", errors);
        }

        // Verificar duplicado
        var exists = await _repository.ExistsAsync(request.Payload.CdSistema, ct);
        if (exists)
        {
            return Result<string>.Failure("DUPLICATE", "Sistema já existe");
        }

        // Mapear e salvar
        var entity = _mapper.Map<Domain.Entities.Sistema>(request.Payload);
        await _repository.AddAsync(entity, ct);
        await _repository.UnitOfWork.SaveChangesAsync(ct);

        return Result<string>.Success(entity.CdSistema);
    }
}