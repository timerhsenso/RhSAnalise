// src/Modules/GestaoDePessoas/Core/Entities/GrauInstrucao.cs

using RhSensoERP.Shared.Core.Primitives;
using System;
using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    public class GrauInstrucao : BaseEntity
    {
        public string CodigoInstrucao { get; set; }
        public string DescricaoInstrucao { get; set; }
        public string CodigoRAIS { get; set; }
        public string CodigoCAGED { get; set; }
        public string CodigoESocial { get; set; }

        // Navegação
        public virtual ICollection<Funcionario> Funcionarios { get; set; }
        public virtual ICollection<Cargo> Cargos { get; set; }

        public GrauInstrucao()
        {
            Funcionarios = new HashSet<Funcionario>();
            Cargos = new HashSet<Cargo>();
        }
    }
}