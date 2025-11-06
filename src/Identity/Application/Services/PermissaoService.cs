using RhSensoERP.Identity.Application.DTOs.Permissoes;
using RhSensoERP.Identity.Infrastructure.Repositories;

namespace RhSensoERP.Identity.Application.Services;

public interface IPermissaoService
{
    Task<List<FuncaoPermissaoDto>> GetPermissoesAsync(string cdUsuario, string? cdSistema, CancellationToken ct);
}

public sealed class PermissaoService : IPermissaoService
{
    private readonly IPermissaoRepository _repo;
    public PermissaoService(IPermissaoRepository repo) => _repo = repo;

    public Task<List<FuncaoPermissaoDto>> GetPermissoesAsync(string cdUsuario, string? cdSistema, CancellationToken ct) =>
        _repo.GetPermissoesDoUsuarioAsync(cdUsuario, cdSistema, ct);
}
