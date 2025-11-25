// src/Modules/Identity/RhSensoERP.Identity.Application/Features/Sistema/Commands/DeleteSistemasCommand.cs
using MediatR;
using RhSensoERP.Identity.Application.DTOs.Common;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Sistema.Commands;

/// <summary>
/// Command para exclusao em massa de sistemas.
/// </summary>
public sealed record DeleteSistemasCommand(List<string> Codigos) : IRequest<Result<BatchDeleteResult>>;