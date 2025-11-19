// ============================================================================
// ARQUIVO CORRIGIDO - FASE 2:
// src/Identity/Application/Services/PermissaoService.cs
// ============================================================================
//
// Este arquivo DEVE ficar no projeto Identity, camada Application.
// Ele implementa a lógica de agregação das permissões do usuário
// usando o repositório de permissões do legado.
//
// IMPORTANTE:
// - Este arquivo substitui o conteúdo anterior, que estava com um
//   controller dentro da pasta de Services (PermissoesController).
// - O controller correto já está em
//   src/API/Controllers/Identity/PermissoesController.cs
// ============================================================================

using Microsoft.Extensions.Logging;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Application.DTOs.Permissoes;
using RhSensoERP.Identity.Infrastructure.Repositories;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Implementação do serviço de permissões, responsável por
/// carregar funções e botões do usuário a partir das tabelas legadas.
/// </summary>
public sealed class PermissaoService : IPermissaoService
{
    private readonly IPermissaoRepository _permissaoRepository;
    private readonly ILogger<PermissaoService> _logger;

    public PermissaoService(
        IPermissaoRepository permissaoRepository,
        ILogger<PermissaoService> logger)
    {
        _permissaoRepository = permissaoRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<UserPermissionsDto> CarregarPermissoesAsync(
        string cdUsuario,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario))
            throw new ArgumentException("cdUsuario é obrigatório.", nameof(cdUsuario));

        _logger.LogInformation(
            "🔐 Carregando permissões para usuário {User} (Sistema: {Sistema})",
            cdUsuario,
            cdSistema ?? "TODOS");

        // Busca as funções + botões do usuário no legado
        List<FuncaoPermissaoDto> funcoes =
            await _permissaoRepository.GetPermissoesDoUsuarioAsync(cdUsuario, cdSistema, ct);

        var result = new UserPermissionsDto();

        // ==========================
        // Funções (telas/módulos)
        // ==========================
        result.Funcoes = funcoes
            .Select(f => new UserFuncaoDto
            {
                CdFuncao = f.CdFuncao,
                DcFuncao = f.DcFuncao,
                CdSistema = f.CdSistema,
                // Ações vêm do DTO do repositório (ex: "IAEC")
                CdAcoes = f.Acoes,
                // Restrição ainda não vem do legado → default neutro
                CdRestric = 'N'
            })
            .ToList();

        // ==========================
        // Botões por função
        // ==========================
        result.Botoes = funcoes
            .SelectMany(f => f.Botoes.Select(b => new UserBotaoDto
            {
                CdFuncao = f.CdFuncao,
                // Hoje o DTO de botão tem NmBotao, não CdBotao.
                // Usamos NmBotao como identificador lógico.
                CdBotao = b.NmBotao,
                DcBotao = b.DcBotao
            }))
            .ToList();

        // Grupos ainda não estão sendo carregados pelo repositório atual.
        // Quando as tabelas de grupos forem mapeadas no repositório,
        // basta preencher result.Grupos aqui.

        _logger.LogInformation(
            "✅ Permissões carregadas. Funções: {Funcoes}, Botões: {Botoes}",
            result.Funcoes.Count,
            result.Botoes.Count);

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> TemPermissaoAsync(
        string cdUsuario,
        string cdFuncao,
        char acao,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario))
            throw new ArgumentException("cdUsuario é obrigatório.", nameof(cdUsuario));

        if (string.IsNullOrWhiteSpace(cdFuncao))
            throw new ArgumentException("cdFuncao é obrigatório.", nameof(cdFuncao));

        var funcoes = await _permissaoRepository.GetPermissoesDoUsuarioAsync(cdUsuario, cdSistema, ct);

        var funcao = funcoes.FirstOrDefault(f =>
            f.CdFuncao == cdFuncao &&
            (cdSistema == null || f.CdSistema == cdSistema));

        if (funcao is null)
            return false;

        return !string.IsNullOrEmpty(funcao.Acoes) &&
               funcao.Acoes.Contains(acao);
    }

    /// <inheritdoc />
    public async Task<List<string>> ObterFuncoesPermitidasAsync(
        string cdUsuario,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        var funcoes = await _permissaoRepository.GetPermissoesDoUsuarioAsync(cdUsuario, cdSistema, ct);

        return funcoes
            .Select(f => f.CdFuncao)
            .Distinct()
            .OrderBy(f => f)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<List<string>> ObterBotoesPermitidosAsync(
        string cdUsuario,
        string cdFuncao,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        var funcoes = await _permissaoRepository.GetPermissoesDoUsuarioAsync(cdUsuario, cdSistema, ct);

        var funcao = funcoes.FirstOrDefault(f =>
            f.CdFuncao == cdFuncao &&
            (cdSistema == null || f.CdSistema == cdSistema));

        if (funcao is null)
            return new List<string>();

        return funcao.Botoes
            .Select(b => b.NmBotao)
            .Distinct()
            .OrderBy(b => b)
            .ToList();
    }
}
