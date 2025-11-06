namespace RhSensoERP.Identity.Application.DTOs.Permissoes;

public sealed class BotaoDto
{
    public string CdBotao { get; init; } = string.Empty;
    public string DcBotao { get; init; } = string.Empty;
    public string? Icone { get; init; }
    public string? Acao { get; init; }
    public int Ordem { get; init; }
}