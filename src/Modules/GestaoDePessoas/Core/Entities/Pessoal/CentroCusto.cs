// src/Modules/GestaoDePessoas/Core/Entities/CentroCusto.cs

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Pessoal
{
    public class CentroCusto : BaseEntity
    {
        public string CodigoCentroCusto { get; set; }
        public string DescricaoCentroCusto { get; set; }
        public string SiglaCentroCusto { get; set; }
        public string NumeroCentroCusto { get; set; }
        public int? FlagAtivo { get; set; }
        public string DescricaoAreaCracha { get; set; }
        public DateTime? DataBloqueio { get; set; }
        public string CodigoCentroCustoPai { get; set; }
        public string CodigoCentroCustoResponsavel { get; set; }
        public string FlagCentroCusto { get; set; }

        // Navegação
        public virtual ICollection<Funcionario> Funcionarios { get; set; }

        public CentroCusto()
        {
            Funcionarios = new HashSet<Funcionario>();
            FlagAtivo = 1;
        }
    }
}