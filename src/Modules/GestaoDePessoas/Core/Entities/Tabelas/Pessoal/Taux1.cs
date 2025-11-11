using RhSensoERP.Shared.Core.Primitives;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal
{
    /// <summary>
    /// Tipos de tabelas auxiliares.
    /// </summary>
    public class Taux1 : BaseEntity
    {
        public string CdTpTabela { get; set; } = default!;  // PK varchar(2)
        public string DcTabela { get; set; } = default!;    // varchar(60)

        // Navegações
        public virtual ICollection<Taux2> Situacoes { get; set; } = new HashSet<Taux2>();
    }
}
