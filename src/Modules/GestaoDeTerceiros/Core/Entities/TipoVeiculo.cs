using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(
    TableName = "SGC_TipoVeiculo",
    DisplayName = "Tipo de Veículo",
    CdSistema = "GTR",
    CdFuncao = "GTR_AUX_TIPOVEICULO",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SGC_TipoVeiculo")]
public class TipoVeiculo
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("Codigo")]
    [StringLength(20)]
    [Required]
    public string Codigo { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(100)]
    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Column("ExigeNR20")]
    public bool ExigeNR20 { get; set; }

    [Column("ExigePesagem")]
    public bool ExigePesagem { get; set; }

    [Column("ExigeAgendamento")]
    public bool ExigeAgendamento { get; set; }

    [Column("ExigeFISPQ")]
    public bool ExigeFISPQ { get; set; }

    [Column("Ordem")]
    public int Ordem { get; set; }

    [Column("Ativo")]
    public bool Ativo { get; set; } = true;

    [Column("Aud_CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("Aud_UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("Aud_IdUsuarioCadastro")]
    public Guid? CreatedBy { get; set; }

    [Column("Aud_IdUsuarioAtualizacao")]
    public Guid? UpdatedBy { get; set; }

    [InverseProperty(nameof(Veiculo.TipoVeiculo))]
    public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}
