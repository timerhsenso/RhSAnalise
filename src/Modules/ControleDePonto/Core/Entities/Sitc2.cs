// src/Modules/ControleDePonto/Core/Entities/Sitc2.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Situação de fechamento/processamento de frequência por colaborador/dia.
/// Tabela: sitc2
/// </summary>
[GenerateCrud(
    TableName = "sitc2",
    DisplayName = "Situação de Frequência",
    CdSistema = "FRE",
    CdFuncao = "CPT_FM_SITC2",
    IsLegacyTable = true
)]
public class Sitc2
{
    [Key]
    [Column("id")]
    [FieldDisplayName("Id")]
    public Guid Id { get; set; }

    [Required]
    [Column("cdempresa")]
    [FieldDisplayName("Código Empresa")]
    public int CdEmpresa { get; set; }

    [Required]
    [Column("cdfilial")]
    [FieldDisplayName("Código Filial")]
    public int CdFilial { get; set; }

    [Required]
    [Column("nomatric", TypeName = "char(8)")]
    [StringLength(8)]
    [FieldDisplayName("Matrícula")]
    public string NoMatric { get; set; } = string.Empty;

    [Required]
    [Column("dtfrequen")]
    [FieldDisplayName("Data Frequência")]
    public DateTime DtFrequen { get; set; }

    [Required]
    [Column("flsituacao")]
    [FieldDisplayName("Situação")]
    public int FlSituacao { get; set; }

    [Column("cdusuario")]
    [StringLength(20)]
    [FieldDisplayName("Código Usuário")]
    public string? CdUsuario { get; set; }

    [Required]
    [Column("dtultmov")]
    [FieldDisplayName("Data Última Movimentação")]
    public DateTime DtUltMov { get; set; }

    [Required]
    [Column("FLPROCESSADO")]
    [FieldDisplayName("Processado")]
    public int FlProcessado { get; set; }

    [Required]
    [Column("FLIMPORTADO")]
    [FieldDisplayName("Importado")]
    public int FlImportado { get; set; }

    [Column("DTIMPORTACAO")]
    [FieldDisplayName("Data Importação")]
    public DateTime? DtImportacao { get; set; }

    [Column("DTPROCESSAMENTO")]
    [FieldDisplayName("Data Processamento")]
    public DateTime? DtProcessamento { get; set; }

    [Column("idfuncionario")]
    [FieldDisplayName("Id Funcionário")]
    public Guid? IdFuncionario { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // NAVEGAÇÃO
    // ═══════════════════════════════════════════════════════════════

    // Se tiver a entidade Funcionario no módulo, descomente:
    // [ForeignKey(nameof(IdFuncionario))]
    // public virtual Funcionario? Funcionario { get; set; }
}