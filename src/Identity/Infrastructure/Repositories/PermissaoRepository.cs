using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Application.DTOs.Permissoes;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence;

namespace RhSensoERP.Identity.Infrastructure.Repositories;

public interface IPermissaoRepository
{
    Task<List<FuncaoPermissaoDto>> GetPermissoesDoUsuarioAsync(string cdUsuario, string? cdSistema, CancellationToken ct);
}

public sealed class PermissaoRepository : IPermissaoRepository
{
    private readonly IdentityDbContext _db;
    public PermissaoRepository(IdentityDbContext db) => _db = db;

    public async Task<List<FuncaoPermissaoDto>> GetPermissoesDoUsuarioAsync(string cdUsuario, string? cdSistema, CancellationToken ct)
    {
        // Grupos do usuário (por sistema)
        var grupos = _db.UserGroups.AsNoTracking()
            .Where(ug => ug.CdUsuario == cdUsuario);

        if (!string.IsNullOrWhiteSpace(cdSistema))
            grupos = grupos.Where(ug => ug.CdSistema == cdSistema);

        // Join com permissões por função (hbrh1) e com a função (fucn1)
        var query =
            from ug in grupos
            join gf in _db.Set<GrupoFuncao>().AsNoTracking()
                on new { ug.CdSistema, ug.CdGrUser } equals new { gf.CdSistema!, gf.CdGrUser }
            join f in _db.Funcoes.AsNoTracking()
                on new { gf.CdSistema, gf.CdFuncao } equals new { f.CdSistema, f.CdFuncao }
            select new
            {
                gf.CdSistema,
                gf.CdFuncao,
                f.DcFuncao,
                f.DcModulo,
                gf.CdAcoes
            };

        var funcoes = await query
            .Distinct()
            .ToListAsync(ct);

        // Carregar botões por função/sistema
        var chaves = funcoes.Select(x => new { x.CdSistema, x.CdFuncao }).Distinct().ToList();

        var botoes = await _db.Set<BotaoFuncao>().AsNoTracking()
            .Where(b => chaves.Contains(new { b.CdSistema, b.CdFuncao }))
            .Select(b => new { b.CdSistema, b.CdFuncao, b.NmBotao, b.DcBotao, b.CdAcao })
            .ToListAsync(ct);

        // Montar DTO
        var result = funcoes
            .GroupBy(x => new { x.CdSistema, x.CdFuncao, x.DcFuncao, x.DcModulo })
            .Select(g => new FuncaoPermissaoDto
            {
                CdSistema = g.Key.CdSistema!,
                CdFuncao  = g.Key.CdFuncao,
                DcFuncao  = g.Key.DcFuncao,
                DcModulo  = g.Key.DcModulo,
                // Mesclar CdAcoes de múltiplos grupos (distinct + ordenado)
                Acoes = string.Concat(g.Select(z => z.CdAcoes).Where(s => !string.IsNullOrWhiteSpace(s))
                                       .SelectMany(s => s!).Distinct().OrderBy(c => c)),
                Botoes = botoes
                    .Where(b => b.CdSistema == g.Key.CdSistema && b.CdFuncao == g.Key.CdFuncao)
                    .Select(b => new BotaoDto { NmBotao = b.NmBotao, DcBotao = b.DcBotao, CdAcao = b.CdAcao })
                    .OrderBy(b => b.NmBotao)
                    .ToList()
            })
            .OrderBy(r => r.CdSistema).ThenBy(r => r.DcModulo).ThenBy(r => r.CdFuncao)
            .ToList();

        return result;
    }
}
