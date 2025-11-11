using RhSensoERP.Shared.Core.Primitives;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    /// <summary>
    /// Situações/tipos auxiliares por categoria (Taux1).
    /// </summary>
    public class Taux2 : BaseEntity
    {
        public string CdTpTabela { get; set; } = default!;  // PK (parte 1) FK -> Taux1
        public string CdSituacao { get; set; } = default!;  // PK (parte 2)
        public string DcSituacao { get; set; } = default!;  // varchar(60)
        public int? NoOrdem { get; set; }                   // int
        public string? FlAtivoAux { get; set; }             // char(1)

        // Navegação
        public virtual Taux1 TipoTabela { get; set; } = default!;
    }
}
