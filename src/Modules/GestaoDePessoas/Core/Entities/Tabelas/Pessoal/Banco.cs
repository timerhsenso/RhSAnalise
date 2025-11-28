// src/Modules/GestaoDePessoas/Core/Entities/Tabelas/Pessoal/Banco.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

[GenerateCrud(
    TableName = "tbanco",
    DisplayName = "Banco",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_TBANCO",
    IsLegacyTable = true,
    GenerateApiController = true  // ← ADICIONAR ISSO!
)]
public class Banco
{
    [Key]  // ← Define CodigoBanco como chave primária
    [Required]
    [Column("codigobanco")]
    [StringLength(10)]
    [FieldDisplayName("Código do Banco")]
    public string CodigoBanco { get; set; } = string.Empty;

    [Required]
    [Column("descricaobanco")]
    [StringLength(150)]
    [FieldDisplayName("Descrição do Banco")]
    public string DescricaoBanco { get; set; } = string.Empty;

    // ═══════════════════════════════════════════════════════════════
    // NAVEGAÇÃO - Relacionamentos 1:N
    // ═══════════════════════════════════════════════════════════════

   // public virtual ICollection<Agencia> Agencias { get; set; } = new HashSet<Agencia>();
   // public virtual ICollection<Funcionario> Funcionarios { get; set; } = new HashSet<Funcionario>();
}