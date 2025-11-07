namespace RhSensoERP.Identity.Application.DTOs.Sistema;

public sealed class SistemaDto
{
    public string CdSistema { get; init; } = default!;
    public string DcSistema { get; init; } = default!;
    public bool Ativo { get; init; }
}
