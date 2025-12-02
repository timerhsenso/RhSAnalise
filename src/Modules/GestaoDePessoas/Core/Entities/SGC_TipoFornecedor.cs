using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

[GenerateCrud(
    TableName = "sgc_tipofornecedor",
    DisplayName = "Tipo Fornecedor",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_TAUX1",
    IsLegacyTable = true,
    GenerateApiController = true
)]
public class SGC_TipoFornecedor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdTipoFornecedor { get; set; }

    public Guid? IdSaas { get; set; }

    [Required]
    [StringLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Descricao { get; set; } = string.Empty;

    [StringLength(50)]
    public string Icone { get; set; } = string.Empty;

    [StringLength(7)]
    public string CorHex { get; set; } = string.Empty;

    public int Ordem { get; set; }

    public bool Ativo { get; set; }

    public DateTime DataCadastro { get; set; }

    public DateTime? DataAtualizacao { get; set; }

    public Guid? IdUsuarioCadastro { get; set; }

    public Guid? IdUsuarioAtualizacao { get; set; }

}
