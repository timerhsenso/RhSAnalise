namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

public class Tab10Esocial
{
    // PK
    public string Codigo { get; set; } = default!;       // tab10_codigo char(2)
    public string Descricao { get; set; } = default!;    // tab10_descricao
    public string? DescDocRequisito { get; set; }        // tab10_desc_doc_requisito
}
