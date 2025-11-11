using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    /// <summary>
    /// Lançamentos por processo/conta (tabela sem chave primária definida).
    /// </summary>
    [Table("lanc3")]
    [Keyless]
    public class Lanc3 : BaseEntity
    {
        [Column("nomatric"), StringLength(8)]
        public string NoMatric { get; set; } = default!;          // Matrícula do colaborador

        [Column("cdempresa")]
        public int CdEmpresa { get; set; }                        // Código da empresa

        [Column("cdfilial")]
        public int CdFilial { get; set; }                         // Código da filial

        [Column("noprocesso"), StringLength(6)]
        public string NoProcesso { get; set; } = default!;        // Número do processo

        [Column("cdconta"), StringLength(4)]
        public string CdConta { get; set; } = default!;           // Código da conta contábil

        [Column("cdccusres"), StringLength(5)]
        public string? CdCcUsRes { get; set; }                    // Centro de custo responsável (opcional)

        [Column("qtconta")]
        public double? QtConta { get; set; }                      // Quantidade da conta (tipo REAL no SQL)

        [Column("cdusuario"), StringLength(20)]
        public string? CdUsuario { get; set; }                    // Usuário que realizou o lançamento
    }
}
