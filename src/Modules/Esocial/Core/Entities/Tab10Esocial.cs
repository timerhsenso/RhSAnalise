// src/Modules/Esocial/Core/Entities/Tab10Esocial.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Esocial.Core.Entities;

/// <summary>
/// Tabela 10 do eSocial - Tipos de Lotação Tributária.
/// </summary>
[GenerateCrud(
    TableName = "tab10_esocial",
    DisplayName = "Tabela 10 eSocial",
    CdSistema = "RHU",
    CdFuncao = "ESO_FM_TAB10",
    IsLegacyTable = true,
    GenerateApiController = true  // ← ADICIONAR ISSO!

)]
public class Tab10Esocial
{
    [Key]
    [Required]
    [Column("tab10_codigo", TypeName = "char(2)")]
    [StringLength(2)]
    [FieldDisplayName("Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [Column("tab10_descricao")]
    [StringLength(255)]
    [FieldDisplayName("Descrição")]
    public string Descricao { get; set; } = string.Empty;

    [Column("tab10_desc_doc_requisito")]
    [StringLength(255)]
    [FieldDisplayName("Descrição Doc. Requisito")]
    public string? DescDocRequisito { get; set; }

}