using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Application.Services;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Auth.Commands;

/// <summary>
/// Command para login de usuário.
/// </summary>
public sealed record LoginCommand(LoginRequest Request, string IpAddress, string? UserAgent)
    : IRequest<Result<AuthResponse>>;

/// <summary>
/// Handler do LoginCommand.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginRequest> _validator;

    public LoginCommandHandler(IAuthService authService, IValidator<LoginRequest> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(request.Request, ct);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<AuthResponse>.Failure("VALIDATION_ERROR", errors);
        }

        return await _authService.LoginAsync(request.Request, request.IpAddress, request.UserAgent, ct);
    }
}