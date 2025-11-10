using System;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

public class Tcbo
{
    // PK
    public string CdCbo { get; set; } = default!;
    public string DcCbo { get; set; } = default!;
    public string? Sinonimo { get; set; }
    public Guid Id { get; set; }
}
