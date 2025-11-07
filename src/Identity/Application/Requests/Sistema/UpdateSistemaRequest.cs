namespace RhSensoERP.Identity.Application.Requests.Sistema;

public sealed class UpdateSistemaRequest
{
    public string DcSistema { get; init; } = default!;
    public bool Ativo { get; init; }
}
