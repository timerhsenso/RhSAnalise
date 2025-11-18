// ============================================================================
// ARQUIVO NOVO - FASE 2: src/Identity/Application/Services/PermissaoService.cs
// ============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Infrastructure.Persistence;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Implementação do serviço de permissões legadas.
/// Carrega grupos, funções e botões do usuário conforme arquitetura legada.
/// </summary>
public sealed class PermissaoService : IPermissaoService
{
    private readonly IdentityDbContext _db;
    private readonly ILogger<PermissaoService> _logger;

    public PermissaoService(
        IdentityDbContext db,
        ILogger<PermissaoService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Carrega todas as permissões do usuário.
    /// Implementa a Etapa 9 do fluxo de login conforme documento de lógica de negócio.
    /// </summary>
    public async Task<UserPermissionsDto> CarregarPermissoesAsync(
        string cdUsuario,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        _logger.LogInformation("🔑 Carregando permissões para usuário: {CdUsuario}, Sistema: {CdSistema}",
            cdUsuario, cdSistema ?? "TODOS");

        var result = new UserPermissionsDto();

        try
        {
            // ================================================================
            // 1. CARREGAR GRUPOS DO USUÁRIO (usrh1 + gurh1)
            // ================================================================
            var gruposQuery = _db.Set<Domain.Entities.UserGroup>()
                .AsNoTracking()
                .Include(ug => ug.GrupoDeUsuario)
                .Where(ug => ug.CdUsuario == cdUsuario);

            if (!string.IsNullOrWhiteSpace(cdSistema))
            {
                gruposQuery = gruposQuery.Where(ug => ug.CdSistema == cdSistema);
            }

            var grupos = await gruposQuery.ToListAsync(ct);

            result.Grupos = grupos.Select(g => new UserGroupDto
            {
                CdGrUser = g.CdGrUser,
                DcGrUser = g.GrupoDeUsuario?.DcGrUser,
                CdSistema = g.CdSistema
            }).ToList();

            _logger.LogInformation("✅ Grupos carregados: {Count}", result.Grupos.Count);

            if (result.Grupos.Count == 0)
            {
                _logger.LogWarning("⚠️ Usuário {CdUsuario} não possui grupos vinculados", cdUsuario);
                return result;
            }

            // ================================================================
            // 2. CARREGAR FUNÇÕES E AÇÕES (hbrh1 + fucn1)
            // ================================================================
            var gruposCodigos = result.Grupos.Select(g => g.CdGrUser).Distinct().ToList();

            var funcoesQuery = _db.Set<Domain.Entities.GrupoFuncao>()
                .AsNoTracking()
                .Include(gf => gf.Funcao)
                .Where(gf => gruposCodigos.Contains(gf.CdGrUser));

            if (!string.IsNullOrWhiteSpace(cdSistema))
            {
                funcoesQuery = funcoesQuery.Where(gf => gf.CdSistema == cdSistema);
            }

            var funcoes = await funcoesQuery.ToListAsync(ct);

            result.Funcoes = funcoes.Select(f => new UserFuncaoDto
            {
                CdFuncao = f.CdFuncao,
                DcFuncao = f.Funcao?.DcFuncao,
                CdSistema = f.CdSistema ?? string.Empty,
                CdAcoes = f.CdAcoes,
                CdRestric = f.CdRestric
            }).ToList();

            _logger.LogInformation("✅ Funções carregadas: {Count}", result.Funcoes.Count);

            // ================================================================
            // 3. CARREGAR BOTÕES (btfuncao)
            // ================================================================
            var funcoesCodigos = result.Funcoes.Select(f => f.CdFuncao).Distinct().ToList();

            if (funcoesCodigos.Any())
            {
                var botoes = await _db.Set<Domain.Entities.BotaoFuncao>()
                    .AsNoTracking()
                    .Where(bf => funcoesCodigos.Contains(bf.CdFuncao))
                    .ToListAsync(ct);

                result.Botoes = botoes.Select(b => new UserBotaoDto
                {
                    CdFuncao = b.CdFuncao,
                    CdBotao = b.NmBotao,
                    DcBotao = b.DcBotao,
                    // FlAtivo não existe em BotaoFuncao
                }).ToList();

                _logger.LogInformation("✅ Botões carregados: {Count}", result.Botoes.Count);
            }

            // ================================================================
            // 4. GERAR PERMISSÕES PARA CLAIMS (formato compacto)
            // ================================================================
            result.PermissionsForClaims = result.Funcoes
                .Select(f => $"{f.CdFuncao}:{f.CdAcoes}")
                .ToList();

            _logger.LogInformation(
                "✅ Permissões carregadas com sucesso - Grupos: {Grupos}, Funções: {Funcoes}, Botões: {Botoes}",
                result.Grupos.Count,
                result.Funcoes.Count,
                result.Botoes.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao carregar permissões para usuário: {CdUsuario}", cdUsuario);
            throw;
        }
    }

    /// <summary>
    /// Verifica se o usuário tem permissão para uma ação específica.
    /// </summary>
    public async Task<bool> TemPermissaoAsync(
        string cdUsuario,
        string cdFuncao,
        char acao,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        try
        {
            // Buscar grupos do usuário
            var gruposQuery = _db.Set<Domain.Entities.UserGroup>()
                .AsNoTracking()
                .Where(ug => ug.CdUsuario == cdUsuario);

            if (!string.IsNullOrWhiteSpace(cdSistema))
            {
                gruposQuery = gruposQuery.Where(ug => ug.CdSistema == cdSistema);
            }

            var gruposCodigos = await gruposQuery
                .Select(ug => ug.CdGrUser)
                .Distinct()
                .ToListAsync(ct);

            if (!gruposCodigos.Any())
            {
                return false;
            }

            // Verificar se algum grupo tem a permissão
            var temPermissao = await _db.Set<Domain.Entities.GrupoFuncao>()
                .AsNoTracking()
                .Where(gf => gruposCodigos.Contains(gf.CdGrUser))
                .Where(gf => gf.CdFuncao == cdFuncao)
                .Where(gf => gf.CdAcoes.Contains(acao))
                .AnyAsync(ct);

            return temPermissao;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ Erro ao verificar permissão: Usuário={CdUsuario}, Função={CdFuncao}, Ação={Acao}",
                cdUsuario, cdFuncao, acao);
            return false;
        }
    }

    /// <summary>
    /// Obtém lista de botões permitidos para o usuário em uma função.
    /// </summary>
    public async Task<List<string>> ObterBotoesPermitidosAsync(
        string cdUsuario,
        string cdFuncao,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        try
        {
            // Verificar se o usuário tem acesso à função
            var gruposQuery = _db.Set<Domain.Entities.UserGroup>()
                .AsNoTracking()
                .Where(ug => ug.CdUsuario == cdUsuario);

            if (!string.IsNullOrWhiteSpace(cdSistema))
            {
                gruposQuery = gruposQuery.Where(ug => ug.CdSistema == cdSistema);
            }

            var gruposCodigos = await gruposQuery
                .Select(ug => ug.CdGrUser)
                .Distinct()
                .ToListAsync(ct);

            if (!gruposCodigos.Any())
            {
                return new List<string>();
            }

            // Verificar se tem acesso à função
            var temAcesso = await _db.Set<Domain.Entities.GrupoFuncao>()
                .AsNoTracking()
                .Where(gf => gruposCodigos.Contains(gf.CdGrUser))
                .Where(gf => gf.CdFuncao == cdFuncao)
                .AnyAsync(ct);

            if (!temAcesso)
            {
                return new List<string>();
            }

            // Retornar botões da função
            var botoes = await _db.Set<Domain.Entities.BotaoFuncao>()
                .AsNoTracking()
                .Where(bf => bf.CdFuncao == cdFuncao)
                .Select(bf => bf.CdBotao)
                .ToListAsync(ct);

            return botoes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ Erro ao obter botões: Usuário={CdUsuario}, Função={CdFuncao}",
                cdUsuario, cdFuncao);
            return new List<string>();
        }
    }
}
