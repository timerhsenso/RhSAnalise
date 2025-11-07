namespace RhSensoERP.Identity.Application.Requests.Sistema;

public sealed class CreateSistemaRequest
{
    public string CdSistema { get; init; } = default!;
    public string DcSistema { get; init; } = default!;
    public bool Ativo { get; init; } = true;
}
