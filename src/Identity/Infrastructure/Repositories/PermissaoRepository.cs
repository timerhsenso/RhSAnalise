// src/Identity/Infrastructure/Repositories/PermissaoRepository.cs

using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Application.DTOs.Permissoes;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence;

namespace RhSensoERP.Identity.Infrastructure.Repositories;

public interface IPermissaoRepository
{
    Task<List<FuncaoPermissaoDto>> GetPermissoesDoUsuarioAsync(
        string cdUsuario,
        string? cdSistema,
        CancellationToken ct);
}

public sealed class PermissaoRepository : IPermissaoRepository
{
    private readonly IdentityDbContext _db;

    public PermissaoRepository(IdentityDbContext db) => _db = db;

    public async Task<List<FuncaoPermissaoDto>> GetPermissoesDoUsuarioAsync(
        string cdUsuario,
        string? cdSistema,
        CancellationToken ct)
    {
        // ================================================================
        // PASSO 1: Buscar grupos do usuário
        // ================================================================
        var grupos = _db.Set<UserGroup>().AsNoTracking()
            .Where(ug => ug.CdUsuario == cdUsuario);

        if (!string.IsNullOrWhiteSpace(cdSistema))
        {
            grupos = grupos.Where(ug => ug.CdSistema == cdSistema);
        }

        // ================================================================
        // PASSO 2: Join com permissões e funções (traduzível para SQL)
        // ================================================================
        var queryFuncoes =
            from ug in grupos
            join gf in _db.Set<GrupoFuncao>().AsNoTracking()
                on new { ug.CdSistema, ug.CdGrUser }
                equals new { CdSistema = gf.CdSistema!, gf.CdGrUser }
            join f in _db.Funcoes.AsNoTracking()
                on new { gf.CdSistema, gf.CdFuncao }
                equals new { f.CdSistema, f.CdFuncao }
            select new
            {
                gf.CdSistema,
                gf.CdFuncao,
                f.DcFuncao,
                f.DcModulo,
                gf.CdAcoes
            };

        var funcoes = await queryFuncoes
            .Distinct()
            .ToListAsync(ct);

        // ================================================================
        // PASSO 3: Buscar botões - CORREÇÃO DO ERRO LINQ
        // ================================================================

        // ✅ FIX: Extrair chaves em memória ANTES de usar no Contains
        var chaves = funcoes
            .Select(x => new { x.CdSistema, x.CdFuncao })
            .Distinct()
            .ToHashSet(); // ✅ HashSet para performance em Contains

        // ✅ FIX: Buscar TODOS os botões do sistema primeiro
        var todosBotoesDaQuery = cdSistema != null
            ? await _db.Set<BotaoFuncao>()
                .AsNoTracking()
                .Where(b => b.CdSistema == cdSistema)
                .ToListAsync(ct)
            : await _db.Set<BotaoFuncao>()
                .AsNoTracking()
                .ToListAsync(ct);

        // ✅ FIX: Filtrar EM MEMÓRIA usando HashSet.Contains
        var botoes = todosBotoesDaQuery
            .Where(b => chaves.Contains(new { b.CdSistema, b.CdFuncao }))
            .Select(b => new
            {
                b.CdSistema,
                b.CdFuncao,
                b.NmBotao,
                b.DcBotao,
                b.CdAcao
            })
            .ToList();

        // ================================================================
        // PASSO 4: Montar DTO final
        // ================================================================
        var result = funcoes
            .GroupBy(x => new
            {
                x.CdSistema,
                x.CdFuncao,
                x.DcFuncao,
                x.DcModulo
            })
            .Select(g => new FuncaoPermissaoDto
            {
                CdSistema = g.Key.CdSistema!,
                CdFuncao = g.Key.CdFuncao,
                DcFuncao = g.Key.DcFuncao,
                DcModulo = g.Key.DcModulo,

                // Mesclar CdAcoes de múltiplos grupos (distinct + ordenado)
                Acoes = string.Concat(
                    g.Select(z => z.CdAcoes)
                     .Where(s => !string.IsNullOrWhiteSpace(s))
                     .SelectMany(s => s!)
                     .Distinct()
                     .OrderBy(c => c)),

                // Botões filtrados
                Botoes = botoes
                    .Where(b => b.CdSistema == g.Key.CdSistema &&
                                b.CdFuncao == g.Key.CdFuncao)
                    .Select(b => new BotaoDto
                    {
                        NmBotao = b.NmBotao,
                        DcBotao = b.DcBotao,
                        CdAcao = b.CdAcao.ToString()
                    })
                    .OrderBy(b => b.NmBotao)
                    .ToList()
            })
            .OrderBy(r => r.CdSistema)
            .ThenBy(r => r.DcModulo)
            .ThenBy(r => r.CdFuncao)
            .ToList();

        return result;
    }
}